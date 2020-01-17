using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;

namespace Insignis.Asset.Management.Reports.Custom.Report
{
    public class TotalFundsReport
    {
        public TotalFundsReport()
        {
        }

        public GenericReport Generate(FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            GenericReport genericReport = new GenericReport("Total Funds");

            genericReport.Elements.Add(new SubHeading("Total Funds Invested by Client and Currency"));

            decimal totalFundsInvestedGBP = 0;
            decimal totalFundsInvestedUSD = 0;
            decimal totalFundsInvestedEUR = 0;

            Table table = new Table();
            TransparentRow headerRow = new TransparentRow();
            headerRow.Cells.Add(new TextCell("Client Reference"));
            headerRow.Cells.Add(new TextCell("Total Portfolio Value"));
            headerRow.Cells.Add(new TextCell(""));
            headerRow.Cells.Add(new TextCell(""));
            table.Rows.Add(headerRow);

            TransparentRow currencyRow = new TransparentRow();
            currencyRow.Cells.Add(new TextCell(""));
            currencyRow.Cells.Add(new RightAlignedCell("GBP"));
            currencyRow.Cells.Add(new RightAlignedCell("USD"));
            currencyRow.Cells.Add(new RightAlignedCell("EUR"));
            table.Rows.Add(currencyRow);

            genericReport.Elements.Add(table);

            Client[] clients = pFinancialAbstraction.ListClients();
            foreach(Client client in clients)
            {
                if (client.ErrorsDetected == false)
                {
                    client.Category = pFinancialAbstraction.GetClientCategory((int)client.Category.ClientCategoryID, pLanguageID);
                    if (client.Category.Detail.Alias.StartsWith("DEMO_") == false)
                    {
                        string SQL = "";
                        SQL += "SELECT AccountID FROM [SchFinancial].[Account] a";
                        SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = a.AccountTypeID";
                        SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = act.TranslationID";
                        SQL += " INNER JOIN [SchFinancial].[Branch] b ON b.BranchID = a.BranchID";
                        SQL += " WHERE ClientID = " + client.PartyID;
                        SQL += " AND t.Alias LIKE '%HUBACCOUNT'";
                        BaseString[] hubAccountIDs = pFinancialAbstraction.CustomQuery(SQL);
                        foreach (BaseString hubAccountID in hubAccountIDs)
                        {
                            if (hubAccountID.ErrorsDetected == false)
                            {
                                HubAccount hubAccount = new HubAccount(Convert.ToInt32(hubAccountID.Value), pFinancialAbstraction, pLanguageID);
                                if (hubAccount.ErrorsDetected == false)
                                {
                                    Institution hubAccountInstitution = hubAccount.GetInstitution();
                                    if (hubAccountInstitution.ErrorsDetected == false)
                                    {
                                        if (hubAccountInstitution.Name.ToUpper().Contains("BARCLAYS"))
                                        {
                                            TransparentRow portfolioValueRow = new TransparentRow();
                                            portfolioValueRow.Cells.Add(new TextCell(client.ClientReference));

                                            Clients.Helper.FeeAccount feeAccount = new Clients.Helper.FeeAccount(client, hubAccount.GetAccount(), pFinancialAbstraction, pLanguageID);
                                            decimal amountMinimumHubBalance = feeAccount.account.Balance.Value;

                                            decimal portfolioValue = hubAccount.CalculatePortfolioValue(amountMinimumHubBalance);

                                            if (hubAccount.GetCurrency().CurrencyCode.CompareTo("GBP") == 0)
                                            {
                                                portfolioValueRow.Cells.Add(new RightAlignedCell(portfolioValue.ToString("0.00")));
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                totalFundsInvestedGBP += portfolioValue;
                                            }
                                            else if (hubAccount.GetCurrency().CurrencyCode.CompareTo("USD") == 0)
                                            {
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                portfolioValueRow.Cells.Add(new RightAlignedCell(portfolioValue.ToString("0.00")));
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                totalFundsInvestedUSD += portfolioValue;
                                            }
                                            else if (hubAccount.GetCurrency().CurrencyCode.CompareTo("EUR") == 0)
                                            {
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                portfolioValueRow.Cells.Add(new TextCell(""));
                                                portfolioValueRow.Cells.Add(new RightAlignedCell(portfolioValue.ToString("0.00")));
                                                totalFundsInvestedEUR += portfolioValue;
                                            }
                                            table.Rows.Add(portfolioValueRow);
                                        }
                                    }
                                    else
                                        genericReport.Elements.Add(new Paragraph("Unable to read institution : " + hubAccountInstitution.ErrorDetails[0].ErrorMessage));
                                }
                                else
                                    genericReport.Elements.Add(new Paragraph("Unable to read hub account : " + hubAccount.ErrorDetails[0].ErrorMessage));
                            }
                        }
                    }
                }
                else
                {
                    genericReport.Elements.Add(new Paragraph("Unable to read client : " + client.ErrorDetails[0].ErrorMessage));
                    break;
                }
            }
            TransparentRow totalRow = new TransparentRow();
            totalRow.Cells.Add(new RightAlignedCell("Total"));
            totalRow.Cells.Add(new RightAlignedCell(totalFundsInvestedGBP.ToString("0.00")));
            totalRow.Cells.Add(new RightAlignedCell(totalFundsInvestedUSD.ToString("0.00")));
            totalRow.Cells.Add(new RightAlignedCell(totalFundsInvestedEUR.ToString("0.00")));
            table.Rows.Add(totalRow);
            return genericReport;
        }
    }
}
