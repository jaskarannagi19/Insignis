using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class FeeAccount
    {
        public Account account = null;
        public AccountFee accountFee = null;

        private Account hubAccount = null;
        private FinancialAbstraction financialAbstraction = null;
        private int languageID;

        public FeeAccount(Client pClient, Account pHubAccount, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            try
            {
                hubAccount = pHubAccount;
                financialAbstraction = pFinancialAbstraction;
                languageID = pLanguageID;

                // get the AccountType entity for the FEEACCOUNT
                AccountType feeAccountType = pFinancialAbstraction.GetAccountTypeByAlias("ACT_FEEACCOUNT", pLanguageID);
                if (feeAccountType.ErrorsDetected == false && feeAccountType.AccountTypeID.HasValue)
                {
                    // list all fee accounts - we are only expecting one
                    Account[] feeAccounts = pFinancialAbstraction.ListAccounts((int)pClient.PartyID, (int)feeAccountType.AccountTypeID);
                    if (feeAccounts.Length > 0)
                    {
                        foreach (Account feeAccount in feeAccounts)
                        {
                            // if we are all good
                            if (feeAccount.ErrorsDetected == false)
                            {
                                // make sure we use the right fee account for the hub account currency
                                if (feeAccount.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                                {
                                    // set the account object to the the feeAccount
                                    account = feeAccount;
                                    break;
                                }
                            }
                        }
                    }
                    feeAccounts = null;
                }
                if (account == null)
                {
                    account = new Account();
                    account.Balance = 0;
                    account.BalanceDate = DateTime.Now;
                    account.Currency = hubAccount.Currency;
                    account.DateOpened = DateTime.Now;
                    account.IBAN = "";
                    account.IsOffShore = false;
                    account.Name = "";
                    account.Number = "";
                    account.Reference = "";
                    account.Status = financialAbstraction.GetAccountStatusByAlias("ACT_OPEN", languageID);
                    account.SWIFTBIC = "";
                    account.Type = feeAccountType;
                    account = financialAbstraction.InsertAccount(account, (int)pClient.PartyID, null);
                }

                // now load the accountFee entity from the DB which contains the fee percentage and any shared percentage (i.e. Introducer fees)
                accountFee = pFinancialAbstraction.GetAccountFee((int)pHubAccount.AccountID);
            }
            catch
            {
            }
        }
        public FeeAccount(Client pClient, Account pFeeAccount, Currency pCurrency, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            try
            {
                account = pFeeAccount;
                financialAbstraction = pFinancialAbstraction;
                languageID = pLanguageID;

                Account[] clientHubAccounts = financialAbstraction.ListAccounts((int)pClient.PartyID, "%HUBACCOUNT");
                foreach (Account clientHubAccount in clientHubAccounts)
                {
                    if (clientHubAccount.ErrorsDetected == false)
                    {
                        // we are only interested in hubs which have the same currency as the fee account
                        if (clientHubAccount.Currency.CurrencyID == pCurrency.CurrencyID)
                        {
                            // we are only interested in open accounts
                            if (clientHubAccount.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                            {
                                // we are only interested in BARCLAYS accounts
                                Institution hubAccountInstitution = GetHubAccountInstitutionForBranch(clientHubAccount.Branch, new CoreAbstraction(financialAbstraction.ConnectionString, financialAbstraction.DBType, financialAbstraction.ErrorLogFile), financialAbstraction, languageID);
                                if (hubAccountInstitution.ErrorsDetected == false && hubAccountInstitution.PartyID.HasValue)
                                {
                                    if (hubAccountInstitution.Name.ToUpper().StartsWith("BARCLAY"))
                                    {
                                        hubAccount = clientHubAccount;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (hubAccount != null && hubAccount.AccountID.HasValue)
                {
                    // now load the accountFee entity from the DB which contains the fee percentage and any shared percentage (i.e. Introducer fees)
                    accountFee = pFinancialAbstraction.GetAccountFee((int)hubAccount.AccountID);
                }
            }
            catch
            {
            }
        }

        public BaseBoolean Allocate(decimal pFeeAllocation)
        {
            return Allocate(pFeeAllocation, DateTime.Now, "Transfer from Hub Account", "Fee Allocation");
        }
        public BaseBoolean Allocate(decimal pFeeAllocation, DateTime pTransactionDate, string pCreditDetails, string pDebitDetails)
        {
            BaseBoolean result = new BaseBoolean(false);
            try
            {
                if (account != null)
                {
                    if (hubAccount != null)
                    {
                        if (pFeeAllocation.ToString("0.00").CompareTo("0.00") != 0)
                        {
                            // insert into fee account a credit transaction relating to this allocation
                            AccountTransaction transaction = new AccountTransaction();
                            transaction.CreditValue = pFeeAllocation;
                            transaction.Date = pTransactionDate;
                            transaction.DebitValue = null;
                            transaction.Details = pCreditDetails;
                            transaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_HIDDENFEECREDIT", languageID);
                            transaction.Status = financialAbstraction.GetTransactionStatusByAlias("TS_RECONCILED", languageID);
                            transaction = financialAbstraction.InsertAccountTransaction(transaction, (int)account.AccountID);
                            if (transaction.ErrorsDetected == false && transaction.AccountTransactionID.HasValue)
                            {
                                int relatedTransactionID = transaction.AccountTransactionID.Value;
                                // update the account balance
                                account.Balance += pFeeAllocation;
                                account.BalanceDate = DateTime.Now;
                                account = financialAbstraction.UpdateAccount(account);
                                if (account.ErrorsDetected == false)
                                {
                                    // insert a hub account debit transaction relating to this allocation
                                    transaction.CreditValue = null;
                                    transaction.DebitValue = pFeeAllocation;
                                    transaction.Details = pDebitDetails;
                                    transaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_HIDDENFEEDEBIT", languageID);
                                    transaction.RelatedAccountID = account.AccountID;
                                    transaction.RelatedTransactionID = relatedTransactionID;
                                    transaction = financialAbstraction.InsertAccountTransaction(transaction, (int)hubAccount.AccountID);
                                    if (transaction.ErrorsDetected == false)
                                    {
                                        financialAbstraction.UpdateAccountTransaction(relatedTransactionID, hubAccount.AccountID, transaction.AccountTransactionID);

                                        // update the account balance
                                        hubAccount.Balance -= pFeeAllocation;
                                        hubAccount.BalanceDate = DateTime.Now;
                                        hubAccount = financialAbstraction.UpdateAccount(hubAccount);
                                        if (hubAccount.ErrorsDetected == false)
                                        {
                                        }
                                        else
                                        {
                                            result.ErrorsDetected = true;
                                            result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, hubAccount.ErrorDetails[0].ErrorMessage));
                                        }
                                    }
                                    else
                                    {
                                        result.ErrorsDetected = true;
                                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, transaction.ErrorDetails[0].ErrorMessage));
                                    }
                                }
                                else
                                {
                                    result.ErrorsDetected = true;
                                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, account.ErrorDetails[0].ErrorMessage));
                                }
                            }
                            else
                            {
                                result.ErrorsDetected = true;
                                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, transaction.ErrorDetails[0].ErrorMessage));
                            }
                        }
                        else
                        {
                            // we don't want zero value transactions in the fee account
                        }
                    }
                    else
                    {
                        result.ErrorsDetected = true;
                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No hub account initialised"));
                    }
                }
                else
                {
                    result.ErrorsDetected = true;
                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No fee account initialised"));
                }
            }
            catch(Exception exc)
            {
                result.ErrorsDetected = true;
                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Caught Error: " + exc.Message));
            }
            return result;
        }

        public BaseBoolean Reduce(decimal pFeeReduction)
        {
            return Reduce(pFeeReduction, DateTime.Now, "Transfer from Fee Account", "Fee Reduction");
        }
        public BaseBoolean Reduce(decimal pFeeReduction, DateTime pTransactionDate, string pCreditDetails, string pDebitDetails)
        {
            BaseBoolean result = new BaseBoolean(false);
            try
            {
                if (account != null)
                {
                    if (hubAccount != null)
                    {
                        if (pFeeReduction.ToString("0.00").CompareTo("0.00") != 0)
                        {
                            // insert into fee account a credit transaction relating to this allocation
                            AccountTransaction transaction = new AccountTransaction();
                            transaction.CreditValue = null;
                            transaction.Date = pTransactionDate;
                            transaction.DebitValue = pFeeReduction;
                            transaction.Details = pDebitDetails;
                            transaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_HIDDENFEEDEBIT", languageID);
                            transaction.Status = financialAbstraction.GetTransactionStatusByAlias("TS_RECONCILED", languageID);
                            transaction = financialAbstraction.InsertAccountTransaction(transaction, (int)account.AccountID);
                            if (transaction.ErrorsDetected == false && transaction.AccountTransactionID.HasValue)
                            {
                                int relatedTransactionID = transaction.AccountTransactionID.Value;
                                // update the account balance
                                account.Balance -= pFeeReduction;
                                account.BalanceDate = DateTime.Now;
                                account = financialAbstraction.UpdateAccount(account);
                                if (account.ErrorsDetected == false)
                                {
                                    // insert a hub account debit transaction relating to this allocation
                                    transaction.CreditValue = pFeeReduction;
                                    transaction.DebitValue = null;
                                    transaction.Details = pCreditDetails;
                                    transaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_HIDDENFEECREDIT", languageID);
                                    transaction.RelatedAccountID = account.AccountID;
                                    transaction.RelatedTransactionID = relatedTransactionID;
                                    transaction = financialAbstraction.InsertAccountTransaction(transaction, (int)hubAccount.AccountID);
                                    if (transaction.ErrorsDetected == false)
                                    {
                                        financialAbstraction.UpdateAccountTransaction(relatedTransactionID, hubAccount.AccountID, transaction.AccountTransactionID);

                                        // update the account balance
                                        hubAccount.Balance += pFeeReduction;
                                        hubAccount.BalanceDate = DateTime.Now;
                                        hubAccount = financialAbstraction.UpdateAccount(hubAccount);
                                        if (hubAccount.ErrorsDetected == false)
                                        {
                                        }
                                        else
                                        {
                                            result.ErrorsDetected = true;
                                            result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, hubAccount.ErrorDetails[0].ErrorMessage));
                                        }
                                    }
                                    else
                                    {
                                        result.ErrorsDetected = true;
                                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, transaction.ErrorDetails[0].ErrorMessage));
                                    }
                                }
                                else
                                {
                                    result.ErrorsDetected = true;
                                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, account.ErrorDetails[0].ErrorMessage));
                                }
                            }
                            else
                            {
                                result.ErrorsDetected = true;
                                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, transaction.ErrorDetails[0].ErrorMessage));
                            }
                        }
                        else
                        {
                            // we don't want zero value transactions in the fee account
                        }
                    }
                    else
                    {
                        result.ErrorsDetected = true;
                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No hub account initialised"));
                    }
                }
                else
                {
                    result.ErrorsDetected = true;
                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No fee account initialised"));
                }
            }
            catch (Exception exc)
            {
                result.ErrorsDetected = true;
                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Caught Error: " + exc.Message));
            }
            return result;
        }

        public BaseBoolean Withdrawal(decimal pFeeWithdrawalAmount)
        {
            return Withdrawal(pFeeWithdrawalAmount, DateTime.Now, "Fee Withdrawal", "Fee Withdrawal");
        }
        public BaseBoolean Withdrawal(decimal pFeeWithdrawalAmount, DateTime pTransactionDate, string pCreditDetails, string pDebitDetails)
        {
            BaseBoolean result = new BaseBoolean(false);
            try
            {
                if (account != null)
                {
                    if (hubAccount != null)
                    {
                        if (pFeeWithdrawalAmount.ToString("0.00").CompareTo("0.00") != 0)
                        {
                            AccountType feeAccountType = financialAbstraction.GetAccountTypeByAlias("ACT_FEEACCOUNT", languageID);
                            if (feeAccountType.ErrorsDetected == false && feeAccountType.AccountTypeID.HasValue)
                            {
                                Client insignis = financialAbstraction.GetClientByReference("INSIGNIS", languageID);
                                if (insignis.ErrorsDetected == false && insignis.PartyID.HasValue)
                                {
                                    insignis.Accounts = financialAbstraction.ListAccounts((int)insignis.PartyID, (int)feeAccountType.AccountTypeID);
                                    if (insignis.Accounts.Length > 0 && insignis.Accounts[0].ErrorsDetected == false)
                                    {
                                        Account insignisFeeAccount = null;
                                        foreach (Account iFeeAccount in insignis.Accounts)
                                        {
                                            if (iFeeAccount.Currency.CurrencyID == account.Currency.CurrencyID)
                                            {
                                                insignisFeeAccount = iFeeAccount;
                                                break;
                                            }
                                        }

                                        if (insignisFeeAccount != null)
                                        {
                                            if (pFeeWithdrawalAmount > 0)
                                            {
                                                if (pFeeWithdrawalAmount <= account.Balance)
                                                {
                                                    AccountTransaction accountTransaction = new AccountTransaction();
                                                    accountTransaction.DebitValue = pFeeWithdrawalAmount;

                                                    accountTransaction.Date = pTransactionDate;
                                                    accountTransaction.CreditValue = null;

                                                    accountTransaction.Details = "Fee Withdrawal";
                                                    accountTransaction.Details = pDebitDetails;

                                                    accountTransaction.Status = financialAbstraction.GetTransactionStatusByAlias("TS_ACTIONED", languageID);
                                                    accountTransaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_FEEDEBIT", languageID);
                                                    accountTransaction = financialAbstraction.InsertAccountTransaction(accountTransaction, (int)account.AccountID);
                                                    if (accountTransaction.ErrorsDetected == false)
                                                    {
                                                        int relatedTransactionID = accountTransaction.AccountTransactionID.Value;

                                                        account.Balance -= pFeeWithdrawalAmount;
                                                        account.BalanceDate = DateTime.Now;
                                                        financialAbstraction.UpdateAccount(account);

                                                        accountTransaction.CreditValue = accountTransaction.DebitValue;
                                                        accountTransaction.DebitValue = null;
                                                        accountTransaction.Details = pCreditDetails;
                                                        accountTransaction.Type = financialAbstraction.GetTransactionTypeByAlias("TT_FEECREDIT", languageID);

                                                        accountTransaction.RelatedAccountID = account.AccountID;
                                                        accountTransaction.RelatedTransactionID = relatedTransactionID;

                                                        accountTransaction = financialAbstraction.InsertAccountTransaction(accountTransaction, (int)insignisFeeAccount.AccountID);
                                                        if (accountTransaction.ErrorsDetected == false)
                                                        {
                                                            if (insignisFeeAccount.Balance.HasValue)
                                                                insignisFeeAccount.Balance += pFeeWithdrawalAmount;
                                                            else
                                                                insignisFeeAccount.Balance = pFeeWithdrawalAmount;
                                                            insignisFeeAccount.BalanceDate = DateTime.Now;

                                                            financialAbstraction.UpdateAccount(insignisFeeAccount);

                                                            financialAbstraction.UpdateAccountTransaction(relatedTransactionID, insignisFeeAccount.AccountID, accountTransaction.AccountTransactionID);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result.ErrorsDetected = true;
                                                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unable to insert transaction : " + accountTransaction.ErrorDetails[0].ErrorMessage));
                                                    }
                                                }
                                                else
                                                {
                                                    result.ErrorsDetected = true;
                                                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "The withdrawal amount must be less than or equal to the amount reserved for fees."));
                                                }
                                            }
                                            else
                                            {
                                                result.ErrorsDetected = true;
                                                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "The withdrawal amount must be greater than zero."));
                                            }
                                        }
                                        else
                                        {
                                            result.ErrorsDetected = true;
                                            result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unable to load fee account."));
                                        }
                                    }
                                    else
                                    {
                                        result.ErrorsDetected = true;
                                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unable to load Insignis Fee Account"));
                                    }
                                }
                                else
                                {
                                    result.ErrorsDetected = true;
                                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unable to load Insignis"));
                                }
                            }
                            else
                            {
                                result.ErrorsDetected = true;
                                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unable to load Fee Account Type"));
                            }
                        }
                        else
                        {
                            // we don't want zero value transactions in the fee account
                        }
                    }
                    else
                    {
                        result.ErrorsDetected = true;
                        result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No hub account initialised"));
                    }
                }
                else
                {
                    result.ErrorsDetected = true;
                    result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No fee account initialised"));
                }
            }
            catch (Exception exc)
            {
                result.ErrorsDetected = true;
                result.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Caught Error: " + exc.Message));
            }
            return result;
        }

        private Institution GetHubAccountInstitutionForBranch(Branch pBranch, CoreAbstraction pCoreAbstraction, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            Institution hubAccountInstitution = new Institution();

            if (pBranch != null && pBranch.ErrorsDetected == false && pBranch.PartyID.HasValue)
            {
                string sql = "";
                sql += "SELECT org.OrganisationID FROM [SchFinancial].[Branch] bra";
                sql += " INNER JOIN [SchCore].[PartyRole] prot ON prot.PartyID = bra.BranchID";
                sql += " INNER JOIN [SchCore].[PartyRelationship] pr ON pr.ToPartyRoleID = prot.PartyRoleID";
                sql += " INNER JOIN [SchCore].[PartyRole] prof ON prof.PartyRoleID = pr.FromPartyRoleID";
                sql += " INNER JOIN [SchCore].[Organisation] org ON org.OrganisationID = prof.PartyID";
                sql += " WHERE BranchID = " + pBranch.PartyID;
                Octavo.Gate.Nabu.Entities.BaseString[] records = pCoreAbstraction.CustomQuery(sql);
                if (records.Length > 0)
                {
                    foreach (Octavo.Gate.Nabu.Entities.BaseString record in records)
                    {
                        if (record.ErrorsDetected == false)
                        {
                            hubAccountInstitution = pFinancialAbstraction.GetInstitution(Convert.ToInt32(record.Value), pLanguageID);
                        }
                        else
                            break;
                    }
                }
            }
            return hubAccountInstitution;
        }
    }
}
