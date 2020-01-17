using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;

namespace Insignis.Asset.Management.Reports.Custom.Report
{
    public class NewDepositsWithinPeriodReport
    {
        private DateTime from = DateTime.Now.AddDays(-7);
        private DateTime to = DateTime.Now;

        private string SQL = "";

        public NewDepositsWithinPeriodReport()
        {
            //from = DateTime.ParseExact("2017-05-01", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            SQL = "SELECT c.clientID, a.AccountID, SUM(t.CreditValue) AS TotalDeposits";
            SQL += " FROM [SchFinancial].[AccountTransaction] t";
            SQL += " INNER JOIN [SchFinancial].[Account] a ON a.AccountID = t.AccountID";
            SQL += " INNER JOIN [SchFinancial].[AccountType] ty ON ty.AccountTypeID = a.AccountTypeID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] tr ON tr.TranslationID = ty.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[Client] c ON c.ClientID = a.ClientID";
            SQL += " WHERE tr.Alias LIKE '%HUBACCOUNT'";
            SQL += " AND (TransactionDetails LIKE '%Opening%' OR TransactionDetails LIKE '%Deposit%')";
            SQL += " AND (TransactionDate >= '" + from.ToString("yyyy-MM-dd") + " 00:00:00' AND TransactionDate <= '" + to.ToString("yyyy-MM-dd") + " 23:59:59')";
            SQL += " GROUP BY c.ClientID, a.AccountID";
        }

        public GenericReport Generate(FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            GenericReport genericReport = new GenericReport("New Deposits");
            genericReport.Elements.Add(new SubHeading("New Deposits within Period " + from.ToString("yyyy-MMM-dd") + " to " + to.ToString("yyyy-MMM-dd")));

            BaseString[] recordset = pFinancialAbstraction.CustomQuery(SQL);
            if (recordset.Length > 0)
            {
                string pipeSeparator = "|";
                foreach (BaseString record in recordset)
                {
                    if (record.ErrorsDetected == false)
                    {
                        try
                        {
                            string[] fields = record.Value.Split(pipeSeparator.ToCharArray());
                            decimal totalDepositsWithinPeriod = Convert.ToDecimal(fields[2].Replace("'", ""));

                            Client client = pFinancialAbstraction.GetClient(Convert.ToInt32(fields[0].Replace("'", "")), pLanguageID);
                            if (client.ErrorsDetected == false)
                            {
                                client.Category = pFinancialAbstraction.GetClientCategory((int)client.Category.ClientCategoryID, pLanguageID);
                                if (client.Category.Detail.Alias.StartsWith("DEMO_") == false)
                                {
                                    HubAccount hubAccount = new HubAccount(Convert.ToInt32(fields[1].Replace("'", "")), pFinancialAbstraction, pLanguageID);
                                    if (hubAccount.ErrorsDetected == false)
                                    {
                                        Institution hubAccountInstitution = hubAccount.GetInstitution();
                                        if (hubAccountInstitution.ErrorsDetected == false && hubAccountInstitution.PartyID.HasValue)
                                        {
                                            if (hubAccountInstitution.Name.StartsWith("Barclays"))
                                            {
                                                Table table = new Table();
                                                TransparentRow headerRow = new TransparentRow();
                                                headerRow.Cells.Add(new TextCell(hubAccount.GetAccountType()));
                                                headerRow.Cells.Add(new TextCell(""));
                                                table.Rows.Add(headerRow);

                                                TransparentRow depositsRow = new TransparentRow();
                                                depositsRow.Cells.Add(new TextCell("Deposits within Period"));
                                                depositsRow.Cells.Add(new RightAlignedCell(totalDepositsWithinPeriod.ToString("0.00")));
                                                table.Rows.Add(depositsRow);

                                                TransparentRow availableToInvestRow = new TransparentRow();
                                                availableToInvestRow.Cells.Add(new TextCell("Amount available to deposit"));
                                                availableToInvestRow.Cells.Add(new RightAlignedCell(hubAccount.CalculateAmountAvailableToInvest().ToString("0.00")));
                                                table.Rows.Add(availableToInvestRow);

                                                TransparentRow portfolioValueRow = new TransparentRow();
                                                portfolioValueRow.Cells.Add(new TextCell("Total value of Portfolio"));
                                                Clients.Helper.FeeAccount feeAccount = new Clients.Helper.FeeAccount(client, hubAccount.GetAccount(), pFinancialAbstraction, pLanguageID);
                                                decimal amountMinimumHubBalance = feeAccount.account.Balance.Value;
                                                portfolioValueRow.Cells.Add(new RightAlignedCell(hubAccount.CalculatePortfolioValue(amountMinimumHubBalance).ToString("0.00")));
                                                table.Rows.Add(portfolioValueRow);

                                                genericReport.Elements.Add(new SubHeading(client.ClientReference));
                                                genericReport.Elements.Add(table);
                                            }
                                        }
                                    }
                                    else
                                        genericReport.Elements.Add(new Paragraph("Unable to read hub account : " + hubAccount.ErrorDetails[0].ErrorMessage));
                                }
                            }
                            else
                                genericReport.Elements.Add(new Paragraph("Unable to read client : " + client.ErrorDetails[0].ErrorMessage));

                        }
                        catch (Exception exc)
                        {
                            genericReport.Elements.Add(new Paragraph("Caught Error : " + exc.Message));
                        }
                    }
                    else
                    {
                        genericReport.Elements.Add(new Paragraph("There was an error : " + recordset[0].ErrorDetails[0].ErrorMessage));
                        break;
                    }
                }
            }
            else
                genericReport.Elements.Add(new Paragraph("There are no deposits recorded within the period"));

            return genericReport;
        }
    }
}
