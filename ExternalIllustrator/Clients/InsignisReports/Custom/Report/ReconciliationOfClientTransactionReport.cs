using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using Octavo.Gate.Nabu.Entities.Globalisation;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Custom.Report
{
    public class ReconciliationOfClientTransactionReport
    {
        private List<ReconciliationOfClientTransactionRow> reportRows = new List<ReconciliationOfClientTransactionRow>();

        public GenericReport Render(DateTime pFromDate, DateTime pToDate, int pSelectedClientID, string selectedAccountTypeAliasContains, Currency pCurrency, int pSelectedInstitutionID, string pDescriptionFilter, FinancialAbstraction pFinancialAbstraction, Language pLanguage)
        {
            GenericReport genericReport = new GenericReport("Client Transactions");

            genericReport.Elements.Add(new SubHeading("Client Transactions Report"));
            try
            {
                TransactionStatus actioned = pFinancialAbstraction.GetTransactionStatusByAlias("TS_ACTIONED", (int)pLanguage.LanguageID);
                ClientStatus[] clientStatuses = pFinancialAbstraction.ListClientStatuses((int)pLanguage.LanguageID);
                ClientCategory[] clientCategories = pFinancialAbstraction.ListClientCategories((int)pLanguage.LanguageID);
                AccountType[] accountTypes = pFinancialAbstraction.ListAccountTypes((int)pLanguage.LanguageID);
                TransactionType[] transactionTypes = pFinancialAbstraction.ListTransactionTypes((int)pLanguage.LanguageID);

                Table tableReport = new Table();

                Row rowHeader = new Row();
                rowHeader.Cells.Add(new TextCell("Date"));
                rowHeader.Cells.Add(new TextCell("Client Reference"));
                if(pSelectedInstitutionID == -1)
                    rowHeader.Cells.Add(new TextCell("Institution"));
                rowHeader.Cells.Add(new TextCell("Account Type"));
                rowHeader.Cells.Add(new TextCell("Description"));
                rowHeader.Cells.Add(new RightAlignedCell("Credit"));
                rowHeader.Cells.Add(new RightAlignedCell("Debit"));
                tableReport.Rows.Add(rowHeader);

                decimal totalCredits = 0;
                decimal totalDebits = 0;

                Client[] clients = null;

                if (pSelectedClientID != -1)
                {
                    List<Client> tmpClients = new List<Client>();
                    tmpClients.Add(pFinancialAbstraction.GetClient(pSelectedClientID, (int)pLanguage.LanguageID));
                    clients = tmpClients.ToArray();
                    tmpClients = null;
                }
                else
                    clients = pFinancialAbstraction.ListClients();

                foreach (Client client in clients)
                {
                    if (client.ErrorsDetected == false)
                    {
                        client.SetStatus(clientStatuses);
                        client.SetCategory(clientCategories);
                        // we only want to show active clients
                        if (client.Status.Detail.Alias.CompareTo("ACTIVE") == 0)
                        {
                            // we want to exclude demo clients
                            if (client.Category.Detail.Alias.StartsWith("DEMO") == false && client.Category.Detail.Alias.StartsWith("HIDDEN") == false)
                            {
                                client.Accounts = pFinancialAbstraction.ListAccounts((int)client.PartyID, "%" + selectedAccountTypeAliasContains + "%");
                                foreach (Account loopHubAccount in client.Accounts)
                                {
                                    if (loopHubAccount.ErrorsDetected == false)
                                    {
                                        // make sure this hub account is for the reporting currency
                                        if (loopHubAccount.Currency.CurrencyID == pCurrency.CurrencyID)
                                        {
                                            try
                                            {
                                                Institution accountHeldAt = null;
                                                if (selectedAccountTypeAliasContains.CompareTo("FEEACCOUNT") == 0)
                                                {
                                                    accountHeldAt = new Institution(0);
                                                    accountHeldAt.Name = "";
                                                }
                                                else
                                                {
                                                    if (loopHubAccount.Branch != null && loopHubAccount.Branch.PartyID.HasValue)
                                                    {
                                                        accountHeldAt = Reports.Custom.Report.HubAccount.GetInstitutionForBranch((int)loopHubAccount.Branch.PartyID, pFinancialAbstraction, (int)pLanguage.LanguageID);
                                                        if (pSelectedInstitutionID == -1 || accountHeldAt.PartyID == pSelectedInstitutionID)
                                                        {
                                                        }
                                                        else
                                                            accountHeldAt = new Institution(-1);
                                                    }
                                                }
                                                if(accountHeldAt.PartyID != -1)
                                                {
                                                    loopHubAccount.Transactions = pFinancialAbstraction.ListAccountTransactions((int)loopHubAccount.AccountID, pFromDate, pToDate);

                                                    foreach (AccountTransaction transaction in loopHubAccount.Transactions)
                                                    {
                                                        if (transaction.ErrorsDetected == false)
                                                        {
                                                            transaction.SetType(transactionTypes);
                                                            bool showRow = false;
                                                            if (transaction.Type.Detail.Alias.Contains("HIDDEN") == false)
                                                                showRow = true;
                                                            else
                                                            {
                                                                if (transaction.Details.Contains("Transfer to Mini-Hub") || transaction.Details.Contains("Transfer from Mini-Hub"))
                                                                    showRow = true;
                                                            }
                                                            if(showRow)
                                                            {
                                                                if (pDescriptionFilter == null || pDescriptionFilter.Trim().Length == 0 || transaction.Details.ToLower().Contains(pDescriptionFilter.ToLower()))
                                                                {
                                                                    if (reportRows.Count == 0)
                                                                    {
                                                                        if (accountHeldAt.Name.Trim().Length > 0)
                                                                        {
                                                                            SubHeading subHeading = genericReport.Elements[0] as SubHeading;
                                                                            subHeading.Title += " (" + accountHeldAt.Name + ")";
                                                                        }
                                                                    }
                                                                    ReconciliationOfClientTransactionRow row = new ReconciliationOfClientTransactionRow();
                                                                    row.Date = transaction.Date.Value;
                                                                    loopHubAccount.SetType(accountTypes);
                                                                    string accountType = loopHubAccount.Type.Detail.Name;
                                                                    row.AccountType = accountType + "(" + pCurrency.CurrencyCode + ")";
                                                                    row.ClientReference = client.ClientReference;
                                                                    row.Details = transaction.Details;
                                                                    row.CreditValue = transaction.CreditValue;
                                                                    row.DebitValue = transaction.DebitValue;
                                                                    row.Institution = accountHeldAt.Name.Replace("&amp;", "&");
                                                                    reportRows.Add(row);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                reportRows.Sort((x, y) => x.Date.CompareTo(y.Date));

                foreach (ReconciliationOfClientTransactionRow reportRow in reportRows)
                {
                    Row row = new Row();
                    row.Cells.Add(new TextCell(reportRow.Date.ToString("dd-MMM-yyyy")));
                    row.Cells.Add(new TextCell(reportRow.ClientReference));
                    if(pSelectedInstitutionID == -1)
                        row.Cells.Add(new TextCell(reportRow.Institution));
                    row.Cells.Add(new TextCell(reportRow.AccountType));

                    row.Cells.Add(new TextCell(reportRow.Details));

                    if (reportRow.CreditValue.HasValue)
                    {
                        row.Cells.Add(new RightAlignedCell(DisplayValue(reportRow.CreditValue)));
                        totalCredits += reportRow.CreditValue.Value;
                    }
                    else
                        row.Cells.Add(new TextCell(""));
                    if (reportRow.DebitValue.HasValue)
                    {
                        row.Cells.Add(new RightAlignedCell(DisplayValue(reportRow.DebitValue)));
                        totalDebits += reportRow.DebitValue.Value;
                    }
                    else
                        row.Cells.Add(new TextCell(""));
                    tableReport.Rows.Add(row);
                }
                Row footer = new Row();
                footer.Cells.Add(new TextCell(""));
                footer.Cells.Add(new TextCell(""));
                footer.Cells.Add(new TextCell(""));
                footer.Cells.Add(new TextCell(""));
                footer.Cells.Add(new RightAlignedCell(DisplayValue(totalCredits)));
                footer.Cells.Add(new RightAlignedCell(DisplayValue(totalDebits)));
                tableReport.Rows.Add(footer);

                genericReport.Elements.Add(tableReport);
            }
            catch (Exception exc)
            {
                genericReport.Elements.Add(new Paragraph("Caught Error : " + exc.Message));
            }
            return genericReport;
        }

        private string DisplayValue(decimal? pValue)
        {
            string value = "";
            if (pValue.HasValue)
                value = pValue.Value.ToString("0.00");
            return value;
        }
    }

    public class ReconciliationOfClientTransactionRow
    {
        public DateTime Date;
        public string ClientReference;
        public string Institution;
        public string AccountType;
        public string Details;
        public decimal? CreditValue;
        public decimal? DebitValue;
    }
}
