using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Financial;
using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class Calculations
    {
        public static decimal AmountAvailableToInvest(Account pHubAccount, Account pMiniHubAccount, FinancialAbstraction pFinancialAbstraction)
        {
            decimal amountAvailableToInvest = 0;
            if (pHubAccount != null)
            {
                if (pHubAccount.Fee == null)
                    pHubAccount.Fee = pFinancialAbstraction.GetAccountFee((int)pHubAccount.AccountID);
            }

            decimal sum = 0;
            if (pHubAccount != null)
            {
                if (pHubAccount.Balance.HasValue)
                    sum += pHubAccount.Balance.Value;
            }

            if (pHubAccount != null)
            {
                if (pHubAccount.Children == null)
                    pHubAccount.Children = pFinancialAbstraction.ListAccountChildren((int)pHubAccount.AccountID);
            }

            if (pMiniHubAccount != null)
            {
                if (pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    if (pMiniHubAccount.Balance.HasValue)
                        sum += pMiniHubAccount.Balance.Value;

                    if (pMiniHubAccount.Children == null)
                        pMiniHubAccount.Children = pFinancialAbstraction.ListAccountChildren((int)pMiniHubAccount.AccountID);

                    foreach (Account child in pMiniHubAccount.Children)
                    {
                        if (child.ErrorsDetected == false)
                        {
                            bool calculate = false;
                            if (pHubAccount != null)
                            {
                                if (child.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                                {
                                    calculate = true;
                                }
                            }
                            else
                                calculate = true;
                            if(calculate)
                            {
                                if (child.Status.Detail.Alias.Contains("PENDING") || child.Status.Detail.Alias.Contains("WAITING_ON_CONFIRMATION"))     // PRE-PENDING AND PENDING ARE PROVISIONALLY DEDUCTED FROM THE BALANCE
                                {
                                    if (child.Type.Detail.Alias.EndsWith("SAPPHIRE") || child.Type.Detail.Alias.EndsWith("EMERALD") || child.Type.Detail.Alias.EndsWith("RUBY") || child.Type.Detail.Alias.EndsWith("AMETHYST"))
                                    {
                                        // do not factor in GEM summary account
                                    }
                                    else
                                    {
                                        if (child.Balance.HasValue)
                                            sum -= child.Balance.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (pHubAccount != null)
            {
                foreach (Account child in pHubAccount.Children)
                {
                    if (child.ErrorsDetected == false)
                    {
                        if (child.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                        {
                            if (child.Status.Detail.Alias.Contains("PENDING") || child.Status.Detail.Alias.Contains("WAITING_ON_CONFIRMATION"))     // PRE-PENDING AND PENDING ARE PROVISIONALLY DEDUCTED FROM THE BALANCE
                            {
                                if (child.Type.Detail.Alias.EndsWith("SAPPHIRE") || child.Type.Detail.Alias.EndsWith("EMERALD") || child.Type.Detail.Alias.EndsWith("RUBY") || child.Type.Detail.Alias.EndsWith("AMETHYST"))
                                {
                                    // do not factor in GEM summary account
                                }
                                else
                                {
                                    if (child.Balance.HasValue)
                                        sum -= child.Balance.Value;
                                }
                            }
                        }
                    }
                }
            }

            amountAvailableToInvest = sum;
            if (pHubAccount != null)
            {
                if (pHubAccount.Fee != null && pHubAccount.Fee.FeeValue.HasValue)
                    amountAvailableToInvest -= pHubAccount.Fee.FeeValue.Value;
            }
            return amountAvailableToInvest;
        }

        public static decimal CalculateConfirmedWeightedAverageRate(Account pHubAccount, Account pMiniHubAccount)
        {
            decimal totalAmount = 0;
            decimal totalYield = 0;
            try
            {
                foreach (Account account in pHubAccount.Children)
                {
                    if (account.ParentAccountID == pHubAccount.AccountID)
                    {
                        if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                        {
                            bool calculate = false;
                            if (account.Status.Detail.Alias.ToUpper().CompareTo("ACT_OPEN") == 0)
                                calculate = true;
                            if (calculate)
                            {
                                if (account.Balance.HasValue && account.Rate.HasValue)
                                {
                                    if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                                    {
                                        // do not factor in GEM summary account
                                    }
                                    else
                                    {
                                        decimal Amount = (account.Balance.Value / 100);
                                        totalAmount += account.Balance.Value;
                                        decimal Rate = account.Rate.Value;
                                        decimal Yield = (Amount * Rate);
                                        totalYield += Yield;
                                    }
                                }
                            }
                        }
                    }
                }
                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    foreach (Account account in pMiniHubAccount.Children)
                    {
                        if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                        {
                            if (account.ParentAccountID == pMiniHubAccount.AccountID)
                            {
                                bool calculate = false;
                                if (account.Status.Detail.Alias.ToUpper().CompareTo("ACT_OPEN") == 0)
                                    calculate = true;
                                if (calculate)
                                {
                                    if (account.Balance.HasValue && account.Rate.HasValue)
                                    {
                                        if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                                        {
                                            // do not factor in GEM summary account
                                        }
                                        else
                                        {
                                            decimal Amount = (account.Balance.Value / 100);
                                            totalAmount += account.Balance.Value;
                                            decimal Rate = account.Rate.Value;
                                            decimal Yield = (Amount * Rate);
                                            totalYield += Yield;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            decimal averageYield = 0;
            if (totalAmount > 0)
            {
                if (totalYield > 0)
                {
                    averageYield = totalYield / totalAmount;
                    averageYield *= 100;
                }
            }
            return averageYield;
        }

        public static decimal CalculatePredictedWeightedAverageRate(string pBankAccountStatusAlias, Account[] pClientAccounts, Account pHubAccount, Account pMiniHubAccount, decimal pAmountAvailableToInvest)
        {
            decimal totalAmount = 0;
            decimal totalYield = 0;
            try
            {
                totalAmount += pAmountAvailableToInvest;

                foreach (Account account in pClientAccounts)
                {
                    if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                    {
                        if (account.Type.ParentID == pHubAccount.Type.AccountTypeID)
                        {
                            bool calculate = false;
                            if (pBankAccountStatusAlias.Contains("|"))
                            {
                                string separator = "|";
                                string[] statusTypes = pBankAccountStatusAlias.Split(separator.ToCharArray());
                                foreach (string statusType in statusTypes)
                                {
                                    if (account.Status.Detail.Alias.ToUpper().CompareTo(statusType.ToUpper()) == 0)
                                    {
                                        calculate = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (account.Status.Detail.Alias.ToUpper().CompareTo(pBankAccountStatusAlias.ToUpper()) == 0)
                                    calculate = true;
                            }
                            if (calculate)
                            {
                                if (account.Balance.HasValue && account.Rate.HasValue)
                                {
                                    if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                                    {
                                        // do not factor in GEM summary account
                                    }
                                    else
                                    {
                                        decimal Amount = (account.Balance.Value / 100);
                                        totalAmount += account.Balance.Value;
                                        decimal Rate = account.Rate.Value;
                                        decimal Yield = (Amount * Rate);
                                        totalYield += Yield;
                                    }
                                }
                            }
                        }
                    }
                }
                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    foreach (Account account in pClientAccounts)
                    {
                        if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                        {
                            if (account.Type.ParentID == pMiniHubAccount.Type.AccountTypeID)
                            {
                                bool calculate = false;
                                if (pBankAccountStatusAlias.Contains("|"))
                                {
                                    string separator = "|";
                                    string[] statusTypes = pBankAccountStatusAlias.Split(separator.ToCharArray());
                                    foreach (string statusType in statusTypes)
                                    {
                                        if (account.Status.Detail.Alias.ToUpper().CompareTo(statusType.ToUpper()) == 0)
                                        {
                                            calculate = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (account.Status.Detail.Alias.ToUpper().CompareTo(pBankAccountStatusAlias.ToUpper()) == 0)
                                        calculate = true;
                                }
                                if (calculate)
                                {
                                    if (account.Balance.HasValue && account.Rate.HasValue)
                                    {
                                        if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                                        {
                                            // do not factor in GEM summary account
                                        }
                                        else
                                        {
                                            decimal Amount = (account.Balance.Value / 100);
                                            totalAmount += account.Balance.Value;
                                            decimal Rate = account.Rate.Value;
                                            decimal Yield = (Amount * Rate);
                                            totalYield += Yield;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            decimal averageYield = 0;
            if (totalAmount > 0)
            {
                if (totalYield > 0)
                {
                    averageYield = totalYield / totalAmount;
                    averageYield *= 100;
                }
            }
            return averageYield;
        }

        public static decimal CalculateFundsInvested(Account[] pClientAccounts, Account pHubAccount, Account pMiniHubAccount)
        {
            decimal fundsInvested = 0;
            foreach (Account account in pHubAccount.Children)
            {
                if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                {
                    if (account.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                    {
                        if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                        {
                            // we don't want to double count those investments summarised in gem accounts
                        }
                        else
                        {
                            if (account.Balance.HasValue)
                                fundsInvested += account.Balance.Value;
                        }
                    }
                }
            }
            if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
            {
                foreach (Account account in pMiniHubAccount.Children)
                {
                    if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                    {
                        if (account.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                        {
                            if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                            {
                                // we don't want to double count those investments summarised in gem accounts
                            }
                            else
                            {
                                if (account.Balance.HasValue)
                                    fundsInvested += account.Balance.Value;
                            }
                        }
                    }
                }
            }
            return fundsInvested;
        }

        public static decimal CalculatePendingInvestments(Account pHubAccount, Account pMiniHubAccount)
        {
            decimal pendingInvestments = 0;
            foreach (Account account in pHubAccount.Children)
            {
                if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                {
                    if (account.Status.Detail.Alias.CompareTo("ACT_PENDING") == 0 || account.Status.Detail.Alias.CompareTo("ACT_WAITING_ON_CONFIRMATION") == 0)
                    {
                        if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                        {
                            if (account.Balance.HasValue)
                                pendingInvestments += account.Balance.Value;
                        }
                        else
                        {
                            if (account.Balance.HasValue)
                                pendingInvestments += account.Balance.Value;
                        }
                    }
                }
            }
            if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
            {
                foreach (Account account in pMiniHubAccount.Children)
                {
                    if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                    {
                        if (account.Status.Detail.Alias.CompareTo("ACT_PENDING") == 0 || account.Status.Detail.Alias.CompareTo("ACT_WAITING_ON_CONFIRMATION") == 0)
                        {
                            if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                            {
                                if (account.Balance.HasValue)
                                    pendingInvestments += account.Balance.Value;
                            }
                            else
                            {
                                if (account.Balance.HasValue)
                                    pendingInvestments += account.Balance.Value;
                            }
                        }
                    }
                }
            }
            return pendingInvestments;
        }

        //public static decimal CalculateMinimumHubBalance(Account pHubAccount)
        //{
        //    decimal minimumHubBalance = 0;
        //    if (pHubAccount.Fee != null && pHubAccount.Fee.FeeValue.HasValue)
        //        minimumHubBalance = pHubAccount.Fee.FeeValue.Value;
        //    return minimumHubBalance;
        //}

        public static decimal CalculateAmountAvailableForWithdrawal(Account[] pClientAccounts, Account pHubAccount, Account pMiniHubAccount)
        {
            decimal availableToWithdraw = 0;

            if (pHubAccount.Balance.HasValue)
            {
                availableToWithdraw = pHubAccount.Balance.Value;

                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    if (pMiniHubAccount.Balance.HasValue)
                        availableToWithdraw += pMiniHubAccount.Balance.Value;
                }

                if (pHubAccount.Fee != null && pHubAccount.Fee.FeeValue.HasValue)
                    availableToWithdraw -= pHubAccount.Fee.FeeValue.Value;

                foreach (Account account in pClientAccounts)
                {
                    if (account.Type.ParentID == pHubAccount.Type.AccountTypeID)
                    {
                        if (account.Status.Detail.Alias.Contains("PENDING") || account.Status.Detail.Alias.Contains("WAITING_ON_CONFIRMATION"))
                        {
                            if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                            {
                                if (account.ParentAccountID == pHubAccount.AccountID)
                                {
                                    if (account.Balance.HasValue)
                                        availableToWithdraw -= account.Balance.Value;
                                }
                            }
                        }
                    }
                }
                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    foreach (Account account in pClientAccounts)
                    {
                        if (account.Type.ParentID == pMiniHubAccount.Type.AccountTypeID)
                        {
                            if (account.Status.Detail.Alias.Contains("PENDING") || account.Status.Detail.Alias.Contains("WAITING_ON_CONFIRMATION"))
                            {
                                if (account.Currency.CurrencyID == pMiniHubAccount.Currency.CurrencyID)
                                {
                                    if (account.ParentAccountID == pMiniHubAccount.AccountID)
                                    {
                                        if (account.Balance.HasValue)
                                            availableToWithdraw -= account.Balance.Value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (availableToWithdraw < 0)
                availableToWithdraw = 0;
            return availableToWithdraw;
        }

        public static decimal CalculateTotal(Account[] pClientAccounts, Account pHubAccount, Account pMiniHubAccount, decimal pMinimumHubBalance, decimal? pTotalWithdrawalRequestsPending)
        {
            decimal total = 0;

            if (pHubAccount.Balance.HasValue)
            {
                total = pHubAccount.Balance.Value;
                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    if (pMiniHubAccount.Balance.HasValue)
                        total += pMiniHubAccount.Balance.Value;
                }

                if (pHubAccount.Fee != null && pHubAccount.Fee.FeeValue.HasValue)
                    total -= pHubAccount.Fee.FeeValue.Value;

                foreach (Account account in pHubAccount.Children)
                {
                    if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                    {
                        // we don't want to double count those investments summarised in gem accounts
                    }
                    else
                    {
                        if (account.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                        {
                            if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                            {
                                if (account.Balance.HasValue)
                                    total += account.Balance.Value;
                            }
                        }
                    }
                }
                if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
                {
                    foreach (Account account in pMiniHubAccount.Children)
                    {
                        if (account.Type.Detail.Alias.EndsWith("SAPPHIRE") || account.Type.Detail.Alias.EndsWith("EMERALD") || account.Type.Detail.Alias.EndsWith("RUBY") || account.Type.Detail.Alias.EndsWith("AMETHYST"))
                        {
                            // we don't want to double count those investments summarised in gem accounts
                        }
                        else
                        {
                            if (account.Status.Detail.Alias.CompareTo("ACT_OPEN") == 0)
                            {
                                if (account.Currency.CurrencyID == pHubAccount.Currency.CurrencyID)
                                {
                                    if (account.Balance.HasValue)
                                        total += account.Balance.Value;
                                }
                            }
                        }
                    }
                }
                total += pMinimumHubBalance;
            }
            if (pTotalWithdrawalRequestsPending.HasValue)
                total += pTotalWithdrawalRequestsPending.Value;
            return total;
        }

        public static decimal? CalculatePendingWithdrawalsRequestedAgainstHubAccount(Account pHubAccount, Account pMiniHubAccount, FinancialAbstraction pFinancialAbstraction)
        {
            decimal? totalWithdrawalRequestsPending = null;
            string sqlPendingWithdrawals = "";
            sqlPendingWithdrawals += "SELECT DebitValue";
            sqlPendingWithdrawals += " FROM [SchFinancial].[AccountTransaction] act";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[TransactionType] trt ON trt.TransactionTypeID = act.TransactionTypeID";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[TransactionStatus] trs ON trs.TransactionStatusID = act.TransactionStatusID";
            sqlPendingWithdrawals += " INNER JOIN[SchGlobalisation].[Translation] ttt ON ttt.TranslationID = trt.TranslationID";
            sqlPendingWithdrawals += " INNER JOIN[SchGlobalisation].[Translation] tts ON tts.TranslationID = trs.TranslationID";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[Account] a ON a.AccountID = act.AccountID";
            string hubAccountIDs = "";
            hubAccountIDs += pHubAccount.AccountID;
            if (pMiniHubAccount != null && pMiniHubAccount.ErrorsDetected == false && pMiniHubAccount.AccountID.HasValue)
            {
                hubAccountIDs += ",";
                hubAccountIDs += pMiniHubAccount.AccountID;
            }
            sqlPendingWithdrawals += " WHERE (a.AccountID IN (" + hubAccountIDs + ") OR a.ParentAccountID IN (" + hubAccountIDs + "))";
            sqlPendingWithdrawals += " AND ttt.Alias IN ('TT_WITHDRAWALREQUEST','TT_WITHDRAWALREQUESTANDCLOSE', 'TT_NOTICEGIVEN', 'TT_NOTICEGIVENANDCLOSE')";
            sqlPendingWithdrawals += " AND tts.Alias NOT IN ('TS_CANCELLED')";
            Octavo.Gate.Nabu.Entities.BaseString[] pendingAccountWithdrawals = pFinancialAbstraction.CustomQuery(sqlPendingWithdrawals);
            if (pendingAccountWithdrawals != null && pendingAccountWithdrawals.Length > 0)
            {
                foreach (Octavo.Gate.Nabu.Entities.BaseString pendingAccountWithdrawal in pendingAccountWithdrawals)
                {
                    if (pendingAccountWithdrawal.ErrorsDetected == false)
                    {
                        if (totalWithdrawalRequestsPending.HasValue == false)
                            totalWithdrawalRequestsPending = Convert.ToDecimal(pendingAccountWithdrawal.Value);
                        else
                            totalWithdrawalRequestsPending += Convert.ToDecimal(pendingAccountWithdrawal.Value);
                    }
                    else
                        break;
                }
            }
            return totalWithdrawalRequestsPending;
        }

        public static decimal? CalculatePendingWithdrawalsRequestedAgainstSpokeAccount(int pSpokeAccountID, FinancialAbstraction pFinancialAbstraction)
        {
            decimal? totalWithdrawalRequestsPending = null;
            string sqlPendingWithdrawals = "";
            sqlPendingWithdrawals += "SELECT DebitValue";
            sqlPendingWithdrawals += " FROM [SchFinancial].[AccountTransaction] act";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[TransactionType] trt ON trt.TransactionTypeID = act.TransactionTypeID";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[TransactionStatus] trs ON trs.TransactionStatusID = act.TransactionStatusID";
            sqlPendingWithdrawals += " INNER JOIN[SchGlobalisation].[Translation] ttt ON ttt.TranslationID = trt.TranslationID";
            sqlPendingWithdrawals += " INNER JOIN[SchGlobalisation].[Translation] tts ON tts.TranslationID = trs.TranslationID";
            sqlPendingWithdrawals += " INNER JOIN[SchFinancial].[Account] a ON a.AccountID = act.AccountID";
            sqlPendingWithdrawals += " WHERE a.AccountID=" + pSpokeAccountID;
            sqlPendingWithdrawals += " AND ttt.Alias IN ('TT_WITHDRAWALREQUEST','TT_WITHDRAWALREQUESTANDCLOSE', 'TT_NOTICEGIVEN', 'TT_NOTICEGIVENANDCLOSE')";
            sqlPendingWithdrawals += " AND tts.Alias NOT IN ('TS_CANCELLED')";
            Octavo.Gate.Nabu.Entities.BaseString[] pendingAccountWithdrawals = pFinancialAbstraction.CustomQuery(sqlPendingWithdrawals);
            if (pendingAccountWithdrawals != null && pendingAccountWithdrawals.Length > 0)
            {
                foreach (Octavo.Gate.Nabu.Entities.BaseString pendingAccountWithdrawal in pendingAccountWithdrawals)
                {
                    if (pendingAccountWithdrawal.ErrorsDetected == false)
                    {
                        if (totalWithdrawalRequestsPending.HasValue == false)
                            totalWithdrawalRequestsPending = Convert.ToDecimal(pendingAccountWithdrawal.Value);
                        else
                            totalWithdrawalRequestsPending += Convert.ToDecimal(pendingAccountWithdrawal.Value);
                    }
                    else
                        break;
                }
            }
            return totalWithdrawalRequestsPending;
        }

        public static decimal CalculatePortfolioValue(Account pHubAccount, FinancialAbstraction pFinancialAbstraction)
        {
            decimal portfolioValue = 0;

            if (pHubAccount != null)
            {
                if (pHubAccount.ErrorsDetected == false)
                {
                    if (pHubAccount.AccountID.HasValue)
                    {
                        if (pHubAccount.Balance.HasValue)
                            portfolioValue += pHubAccount.Balance.Value;

                        string sqlSumSpokeAccounts = "SELECT SUM(Balance)";
                        sqlSumSpokeAccounts += " FROM [SchFinancial].[Account] a";
                        sqlSumSpokeAccounts += " INNER JOIN[SchFinancial].[AccountStatus] st ON st.AccountStatusID = a.AccountStatusID";
                        sqlSumSpokeAccounts += " INNER JOIN[SchGlobalisation].[Translation] t ON t.TranslationID = st.TranslationID";
                        sqlSumSpokeAccounts += " WHERE a.ParentAccountID = " + pHubAccount.AccountID;
                        sqlSumSpokeAccounts += " AND t.Alias = 'ACT_OPEN'";

                        Octavo.Gate.Nabu.Entities.BaseString[] recordSet = pFinancialAbstraction.CustomQuery(sqlSumSpokeAccounts);
                        if (recordSet != null && recordSet.Length > 0)
                        {
                            if (recordSet[0].ErrorsDetected == false)
                            {
                                try
                                {
                                    decimal sumOfSpokeAccountBalances = Convert.ToDecimal(recordSet[0].Value);
                                    portfolioValue += sumOfSpokeAccountBalances;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }

            return portfolioValue;
        }

        public static decimal CalculateFeeAllocation(decimal pAmountDeposited, decimal pFeePercentage)
        {
            decimal feeAllocation = 0;

            try
            {
                if (pAmountDeposited > 0)
                {
                    decimal percentage = (pFeePercentage / 100);
                    decimal feeValue = (pAmountDeposited * percentage);
                    feeAllocation = Math.Round(feeValue, 2);
                }
            }
            catch
            {
            }

            return feeAllocation;
        }
        public static decimal oldCalculateFeeAllocation(decimal pHubAccountBalance, decimal pPortfolioValue, decimal pFeePercentage)
        {
            decimal feeAllocation = 0;

            if (pHubAccountBalance > 0)
            {
                if (pPortfolioValue > 0)
                {
                    try
                    {
                        decimal percentage = (pFeePercentage / 100);
                        decimal feeValue = (pPortfolioValue * percentage);
                        feeValue = Math.Round(feeValue, 2);
                        if (feeValue < pHubAccountBalance)
                            feeAllocation = feeValue;
                        else
                            feeAllocation = pHubAccountBalance;
                    }
                    catch
                    {
                    }
                }
            }

            return feeAllocation;
        }
    }
}