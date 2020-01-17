using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Custom.Report
{
    public class HubAccount : BaseType
    {
        private int _languageID = -1;
        private Client _client = null;
        private Account _hubAccount = null;
        private Account _miniHubAccount = null;
        private Account _feeAccount = null;
        private FinancialAbstraction _financialAbstraction = null;

        public HubAccount(int pAccountID, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            _financialAbstraction = pFinancialAbstraction;
            _languageID = pLanguageID;

            _hubAccount = _financialAbstraction.GetAccount(pAccountID);
            if (_hubAccount.ErrorsDetected == false && _hubAccount.AccountID.HasValue)
            {
                BaseString[] recordSet = _financialAbstraction.CustomQuery("SELECT ClientID FROM [SchFinancial].[Account] WHERE AccountID = " + _hubAccount.AccountID);
                _client = pFinancialAbstraction.GetClient(Convert.ToInt32(recordSet[0].Value),_languageID);
                _client.Accounts = _financialAbstraction.ListAccounts((int)_client.PartyID);

                foreach (Account account in _client.Accounts)
                {
                    if (account.ErrorsDetected == false)
                    {
                        if (account.Currency.CurrencyID == _hubAccount.Currency.CurrencyID)
                        {
                            if (account.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                            {
                                if (account.Type.Detail.Alias.CompareTo("ACT_FEEACCOUNT") == 0)
                                {
                                    _feeAccount = account;
                                    break;
                                }
                            }
                        }
                    }
                }

                _hubAccount.Fee = pFinancialAbstraction.GetAccountFee((int)_hubAccount.AccountID);
                _hubAccount.Currency = pFinancialAbstraction.GetCurrency((int)_hubAccount.Currency.CurrencyID);
                _hubAccount.Type = pFinancialAbstraction.GetAccountType((int)_hubAccount.Type.AccountTypeID, pLanguageID);
                _hubAccount.Status = pFinancialAbstraction.GetAccountStatus((int)_hubAccount.Status.AccountStatusID, pLanguageID);

                _hubAccount.Children = pFinancialAbstraction.ListAccountChildren((int)_hubAccount.AccountID);

                foreach (Account relatedAccount in _client.Accounts)
                {
                    if (relatedAccount.ErrorsDetected == false)
                    {
                        if (relatedAccount.AccountID != _hubAccount.AccountID && relatedAccount.Type.AccountTypeID == _hubAccount.Type.AccountTypeID)
                        {
                            if (relatedAccount.Status.AccountStatusID == _hubAccount.Status.AccountStatusID)
                            {
                                if (relatedAccount.Currency.CurrencyID == _hubAccount.Currency.CurrencyID)
                                {
                                    _miniHubAccount = relatedAccount;
                                    _miniHubAccount.Children = _financialAbstraction.ListAccountChildren((int)_miniHubAccount.AccountID);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.ErrorsDetected = true;
                this.ErrorDetails.Add(_hubAccount.ErrorDetails[0]);
            }
        }

        public Institution GetInstitution()
        {
            Institution hubAccountInstitution = new Institution();

            if (_hubAccount.Branch != null && _hubAccount.Branch.ErrorsDetected == false && _hubAccount.Branch.PartyID.HasValue)
            {
                CoreAbstraction coreAbstraction = new CoreAbstraction(_financialAbstraction.ConnectionString, _financialAbstraction.DBType, _financialAbstraction.ErrorLogFile);
                string sql = "";
                sql += "SELECT org.OrganisationID FROM [SchFinancial].[Branch] bra";
                sql += " INNER JOIN [SchCore].[PartyRole] prot ON prot.PartyID = bra.BranchID";
                sql += " INNER JOIN [SchCore].[PartyRelationship] pr ON pr.ToPartyRoleID = prot.PartyRoleID";
                sql += " INNER JOIN [SchCore].[PartyRole] prof ON prof.PartyRoleID = pr.FromPartyRoleID";
                sql += " INNER JOIN [SchCore].[Organisation] org ON org.OrganisationID = prof.PartyID";
                sql += " WHERE BranchID = " + _hubAccount.Branch.PartyID;
                Octavo.Gate.Nabu.Entities.BaseString[] records = coreAbstraction.CustomQuery(sql);
                if (records.Length > 0)
                {
                    foreach (Octavo.Gate.Nabu.Entities.BaseString record in records)
                    {
                        if (record.ErrorsDetected == false)
                        {
                            hubAccountInstitution = _financialAbstraction.GetInstitution(Convert.ToInt32(record.Value), _languageID);
                        }
                        else
                            break;
                    }
                }
            }
            return hubAccountInstitution;
        }

        public static Institution GetInstitutionForBranch(int pBranchID,FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            Institution institution = new Institution();

            string sql = "";
            sql += "SELECT org.OrganisationID FROM [SchFinancial].[Branch] bra";
            sql += " INNER JOIN [SchCore].[PartyRole] prot ON prot.PartyID = bra.BranchID";
            sql += " INNER JOIN [SchCore].[PartyRelationship] pr ON pr.ToPartyRoleID = prot.PartyRoleID";
            sql += " INNER JOIN [SchCore].[PartyRole] prof ON prof.PartyRoleID = pr.FromPartyRoleID";
            sql += " INNER JOIN [SchCore].[Organisation] org ON org.OrganisationID = prof.PartyID";
            sql += " WHERE BranchID = " + pBranchID;
            Octavo.Gate.Nabu.Entities.BaseString[] records = pFinancialAbstraction.CustomQuery(sql);
            if (records.Length > 0)
            {
                foreach (Octavo.Gate.Nabu.Entities.BaseString record in records)
                {
                    if (record.ErrorsDetected == false)
                        institution = pFinancialAbstraction.GetInstitution(Convert.ToInt32(record.Value), pLanguageID);
                    else
                        break;
                }
            }

            return institution;
        }

        public string GetAccountType()
        {
            return _hubAccount.Type.Detail.Name + "(" + _hubAccount.Currency.CurrencyCode + ")";
        }

        public Currency GetCurrency()
        {
            return _hubAccount.Currency;
        }

        public Client GetClient()
        {
            return _client;
        }

        public Account GetAccount()
        {
            return _hubAccount;
        }

        public Account GetMiniHubAccount()
        {
            return _miniHubAccount;
        }

        public decimal CalculatePortfolioValue(decimal pAmountMinimumHubBalance)
        {
            decimal portfolioValue = 0;
            try
            {
                decimal? totalWithdrawalRequestsPending = Clients.Helper.Calculations.CalculatePendingWithdrawalsRequestedAgainstHubAccount(_hubAccount, _miniHubAccount, _financialAbstraction);
                portfolioValue = Clients.Helper.Calculations.CalculateTotal(_client.Accounts, _hubAccount, _miniHubAccount, pAmountMinimumHubBalance, totalWithdrawalRequestsPending);
            }
            catch
            {
            }
            return portfolioValue;
        }

        public decimal MainAmountAvailableToInvest()
        {
            decimal amountAvailableToInvest = 0;
            try
            {
                amountAvailableToInvest = Clients.Helper.Calculations.AmountAvailableToInvest(_hubAccount, null, _financialAbstraction);
            }
            catch
            {
            }
            return amountAvailableToInvest;
        }

        public decimal MiniAmountAvailableToInvest()
        {
            decimal amountAvailableToInvest = 0;
            try
            {
                amountAvailableToInvest = Clients.Helper.Calculations.AmountAvailableToInvest(null, _miniHubAccount, _financialAbstraction);
            }
            catch
            {
            }
            return amountAvailableToInvest;
        }

        public decimal CalculateAmountAvailableToInvest()
        {
            decimal amountAvailableToInvest = 0;
            try
            {
                amountAvailableToInvest = Clients.Helper.Calculations.AmountAvailableToInvest(_hubAccount, _miniHubAccount, _financialAbstraction);
            }
            catch
            {
            }
            return amountAvailableToInvest;
        }

        public decimal CalculateWeightedAverageRate()
        {
            decimal amountWeightedAverageRate = 0;
            try
            {
                amountWeightedAverageRate = Clients.Helper.Calculations.CalculateConfirmedWeightedAverageRate(_hubAccount, _miniHubAccount);
            }
            catch
            {
            }
            return amountWeightedAverageRate;
        }

        public decimal? CalculateWithdrawalRequestsPending()
        {
            decimal? totalWithdrawalRequestsPending = 0;
            try
            {
                totalWithdrawalRequestsPending = Clients.Helper.Calculations.CalculatePendingWithdrawalsRequestedAgainstHubAccount(_hubAccount, _miniHubAccount, _financialAbstraction);
            }
            catch
            {
            }
            return totalWithdrawalRequestsPending;
        }

        public decimal CalculatePendingInvestments()
        {
            decimal pendingInvestments = 0;
            try
            {
                pendingInvestments = Clients.Helper.Calculations.CalculatePendingInvestments(_hubAccount, _miniHubAccount);
            }
            catch
            {
            }
            return pendingInvestments;
        }

        public decimal CalculateFundsInvested()
        {
            decimal amountFundsInvested = 0;
            try
            {
                amountFundsInvested = Clients.Helper.Calculations.CalculateFundsInvested(_client.Accounts, _hubAccount, _miniHubAccount);
            }
            catch
            {
            }
            return amountFundsInvested;
        }

        public decimal CalculateAmountAvailableForWithdrawal()
        {
            decimal amountAmountAvailableForWithdrawal = 0;
            try
            {
                amountAmountAvailableForWithdrawal = Clients.Helper.Calculations.CalculateAmountAvailableForWithdrawal(_client.Accounts, _hubAccount, _miniHubAccount);
            }
            catch
            {
            }
            return amountAmountAvailableForWithdrawal;
        }

        public decimal CalculateDepositsWithinPeriod(DateTime pFromDate, DateTime pToDate)
        {
            decimal totalDepositsWithinPeriod = 0;
            try
            {
                string SQL = "";
                SQL = "SELECT SUM(t.CreditValue) AS TotalDeposits";
                SQL += " FROM [SchFinancial].[AccountTransaction] t";
                SQL += " WHERE t.AccountID = " + _hubAccount.AccountID;
                SQL += " AND (TransactionDetails LIKE '%Opening%' OR TransactionDetails LIKE '%Deposit%')";
                SQL += " AND (TransactionDate >= '" + pFromDate.ToString("yyyy-MM-dd") + " 00:00:00' AND TransactionDate <= '" + pToDate.ToString("yyyy-MM-dd") + " 23:59:59')";

                BaseString[] recordSet = _financialAbstraction.CustomQuery(SQL);
                if (recordSet.Length > 0)
                {
                    foreach (BaseString record in recordSet)
                    {
                        if (record.ErrorsDetected == false)
                            totalDepositsWithinPeriod += Convert.ToDecimal(record.Value);
                    }
                }
            }
            catch
            {
            }
            return totalDepositsWithinPeriod;
        }

        public bool HasMiniHub()
        {
            if (_miniHubAccount != null && _miniHubAccount.ErrorsDetected == false && _miniHubAccount.AccountID.HasValue)
                return true;
            else
                return false;
        }

        public AccountTransaction[] CombineTransactions(AccountTransaction[] pHubAccountTransactions, AccountTransaction[] pMiniHubAccountTransactions)
        {
            List<AccountTransaction> combinedTransactions = new List<AccountTransaction>();
            foreach (AccountTransaction hubAccountTransaction in pHubAccountTransactions)
            {
                if (hubAccountTransaction.ErrorsDetected == false)
                    combinedTransactions.Add(hubAccountTransaction);
            }
            foreach (AccountTransaction minihubAccountTransaction in pMiniHubAccountTransactions)
            {
                if (minihubAccountTransaction.ErrorsDetected == false)
                    combinedTransactions.Add(minihubAccountTransaction);
            }
            combinedTransactions.Sort((x, y) => x.Date.Value.CompareTo(y.Date.Value));
            combinedTransactions.Reverse();

            return combinedTransactions.ToArray();
        }

        public decimal GetFeeReserve()
        {
            decimal feeReserve = 0;
            try
            {
                if (_feeAccount != null && _feeAccount.ErrorsDetected == false && _feeAccount.AccountID.HasValue)
                {
                    if (_feeAccount.Balance.HasValue)
                        feeReserve = _feeAccount.Balance.Value;
                }
                else
                {
                    if (_hubAccount.Fee != null)
                    {
                        if (_hubAccount.Fee.ErrorsDetected == false)
                        {
                            if (_hubAccount.Fee.FeeValue.HasValue)
                                feeReserve = _hubAccount.Fee.FeeValue.Value;
                        }
                    }
                }
            }
            catch
            {
            }
            return feeReserve;
        }

        public Account[] ListCombinedSpokeAccounts()
        {
            List<Account> combinedSpokeAccounts = new List<Account>();
            if (_hubAccount.Children.Length > 0)
            {
                foreach (Account child in _hubAccount.Children)
                {
                    if (child.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                        combinedSpokeAccounts.Add(child);
                }
            }
            if (_miniHubAccount != null && _miniHubAccount.Children != null)
            {
                if (_miniHubAccount.Children.Length > 0)
                {
                    foreach (Account child in _miniHubAccount.Children)
                    {
                        if (child.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                            combinedSpokeAccounts.Add(child);
                    }
                }
            }
            return combinedSpokeAccounts.ToArray();
        }
    }
}
