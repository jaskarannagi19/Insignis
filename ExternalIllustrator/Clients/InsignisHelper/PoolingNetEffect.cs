using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Encryption;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class PoolingNetEffect
    {
        public Currency poolCurrency = null;
        public List<KeyValuePair<int, string>> Clients = new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, decimal>> WithdrawalRequestsPrincipal = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> WithdrawalRequestsInterest = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> WithdrawalRequestedPrincipal = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> WithdrawalRequestedInterest = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> NoticeGivenRequestsPrincipal = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> NoticeGivenRequestsInterest = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> MaturedRequestsPrincipal = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> MaturedRequestsInterest = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> Deposits = new List<KeyValuePair<int, decimal>>();
        public List<KeyValuePair<int, decimal>> TopUps = new List<KeyValuePair<int, decimal>>();
        public List<int> DepositSpokeAccountIDs = new List<int>();
        public List<int> NoticeGivenTransactionIDs = new List<int>();
        public List<int> MaturedRequestsTransactionIDs = new List<int>();
        public List<int> WithdrawalTransactionIDs = new List<int>();
        public List<int> WithdrawalRequestedTransactionIDs = new List<int>();
        public List<int> TopUpTransactionIDs = new List<int>();
        public List<int> AccountIDsToClose = new List<int>();

        private Pooling poolingHelper = null;
        public PoolingNetEffect()
        {
        }
        public PoolingNetEffect(int pPoolingAccountID, int pInstitutionID, string pAccountType, BaseAbstraction pBaseAbstraction, Octavo.Gate.Nabu.Extensions.Calendar.Helper.CalendarAbstraction pCalendarAbstraction, int pLanguageID)
        {
            poolingHelper = new Pooling();
            poolingHelper.financialAbstraction = new FinancialAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
            poolingHelper.operationsAbstraction = new OperationsAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);

            // look for new deposits (Spoke Accounts) to append to this pool
            PoolingRequest[] poolingRequests = poolingHelper.ListPendingSpokesForInstantOrNoticePools(pPoolingAccountID, pLanguageID);
            if (poolingRequests.Length > 0)
            {
                foreach (PoolingRequest poolingRequest in poolingRequests)
                {
                    if (poolingRequest.institution.PartyID == pInstitutionID)
                    {
                        EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
                        foreach (ProductPoolingRequest productRequest in poolingRequest.productPoolingRequests)
                        {
                            foreach (Client spokeClient in productRequest.clients)
                            {
                                foreach (Account spokePoolAccount in spokeClient.Accounts)
                                {
                                    AccountProductKey productKey = null;
                                    string productName = "";
                                    spokePoolAccount.Attributes = poolingHelper.financialAbstraction.ListAccountAttributes((int)spokePoolAccount.AccountID, pLanguageID);
                                    foreach (AccountAttribute accountAttribute in spokePoolAccount.Attributes)
                                    {
                                        if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                                            productKey = new AccountProductKey(accountAttribute.Value);
                                        else if (accountAttribute.Detail.Alias.EndsWith("PRODUCTNAME"))
                                            productName = accountAttribute.Value;
                                    }
                                    bool showProduct = false;
                                    if (productKey != null)
                                    {
                                        if (productKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount && pAccountType.CompareTo("Instant") == 0)
                                            showProduct = true;
                                        else if (productKey.investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount && pAccountType.CompareTo("Notice") == 0)
                                            showProduct = true;
                                        if (showProduct)
                                        {
                                            //Client spokeClient = poolingHelper.GetClientForAccount((int)relatedClientSpokedAccount.AccountID, pLanguageID);
                                            RegisterClient(spokeClient);
                                            RegisterDeposit(spokeClient.PartyID.Value, spokePoolAccount.Balance.Value);
                                            DepositSpokeAccountIDs.Add(spokePoolAccount.AccountID.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // now look for withdrawls, notice given and notice matured transactions from within the pool
            TransactionStatus[] transactionStatuses = poolingHelper.financialAbstraction.ListTransactionStatuses(pLanguageID);
            TransactionType[] transactionTypes = poolingHelper.financialAbstraction.ListTransactionTypes(pLanguageID);

            // now iterate through each transaction for that account to see if there are pending transactions requiring action
            Account insignisPoolAccount = poolingHelper.financialAbstraction.GetAccount(pPoolingAccountID);
            if(insignisPoolAccount.ErrorsDetected == false && insignisPoolAccount.AccountID.HasValue)
            {
                insignisPoolAccount.Currency = poolingHelper.financialAbstraction.GetCurrency((int)insignisPoolAccount.Currency.CurrencyID);

                string innerSQL = "SELECT act.TransactionID, cli.ClientID, cli.ClientReference, acc.AccountID FROM [SchFinancial].[AccountTransaction] act";
                innerSQL += " LEFT OUTER JOIN [SchFinancial].[Account] acc ON acc.AccountID = act.RelatedAccountID";
                innerSQL += " LEFT OUTER JOIN [SchFinancial].[Client] cli ON cli.ClientID = acc.ClientID";
                innerSQL += " INNER JOIN [SchFinancial].[TransactionType] tt ON tt.TransactionTypeID = act.TransactionTypeID";
                innerSQL += " INNER JOIN [SchFinancial].[TransactionStatus] ts ON ts.TransactionStatusID = act.TransactionStatusID";
                innerSQL += " INNER JOIN [SchGlobalisation].[Translation] ttt ON ttt.TranslationID = tt.TranslationID";
                innerSQL += " INNER JOIN [SchGlobalisation].[Translation] tst ON tst.TranslationID = ts.TranslationID";
                innerSQL += " WHERE act.AccountID = " + insignisPoolAccount.AccountID;
                innerSQL += " AND tst.Alias = 'TS_CONFIRMED';";
                // iterate through each transaction
                Octavo.Gate.Nabu.Entities.BaseString[] innerRecordSet = poolingHelper.financialAbstraction.CustomQuery(innerSQL);
                foreach (Octavo.Gate.Nabu.Entities.BaseString innerRecord in innerRecordSet)
                {
                    if (innerRecord.ErrorsDetected == false)
                    {
                        // extract the account number
                        string[] innerFields = innerRecord.GetFields();
                        AccountTransaction pendingTransaction = poolingHelper.financialAbstraction.GetAccountTransaction(Convert.ToInt32(innerFields[0].Replace("'", "")));
                        if (pendingTransaction.ErrorsDetected == false && pendingTransaction.AccountTransactionID.HasValue)
                        {
                            pendingTransaction.SetStatus(transactionStatuses);
                            pendingTransaction.SetType(transactionTypes);

                            Client spokeClient = new Client(Convert.ToInt32(innerFields[1].Replace("'", "")));
                            spokeClient.ClientReference = innerFields[2].Replace("'", "");
                            RegisterClient(spokeClient);

                            if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_WITHDRAWALREQUEST") == 0)
                            {
                                Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                if (spokeAccount.ErrorsDetected == false)
                                {
                                    if (spokeAccount.Balance.HasValue && spokeAccount.Balance.Value == 0)
                                        RegisterWithdrawalRequestedOfInterest(spokeClient.PartyID.Value, ForecastInterest(false, spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                    RegisterWithdrawalOfPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                }
                                WithdrawalTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                            }
                            else if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_WITHDRAWALREQUESTANDCLOSE") == 0)
                            {
                                Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                if (spokeAccount.ErrorsDetected == false)
                                {
                                    // this means the withdrawal action will empty the count and ultimately close it
                                    if (spokeAccount.Balance.HasValue && spokeAccount.Balance.Value == 0)
                                        RegisterWithdrawalRequestedOfInterest(spokeClient.PartyID.Value, ForecastInterest(false, spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                    RegisterWithdrawalOfPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                }
                                WithdrawalTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                            }
                            if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_WITHDRAWALREQUESTED") == 0)
                            {
                                Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                if (spokeAccount.ErrorsDetected == false)
                                {
                                    // this means the withdrawal action will empty the count and ultimately close it
                                    RegisterWithdrawalRequestedOfInterest(spokeClient.PartyID.Value, ForecastInterest(false, spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                    RegisterWithdrawalRequestedOfPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                }
                                WithdrawalRequestedTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                            }
                            else if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_WITHDRAWALREQUESTEDANDCLOSE") == 0)
                            {
                                Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                if (spokeAccount.ErrorsDetected == false)
                                {
                                    // this means the withdrawal action will empty the count and ultimately close it
                                    if (spokeAccount.Balance.HasValue && spokeAccount.Balance.Value == 0)
                                        RegisterWithdrawalRequestedOfInterest(spokeClient.PartyID.Value, ForecastInterest(false, spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                    RegisterWithdrawalRequestedOfPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                    AccountIDsToClose.Add(spokeAccount.AccountID.Value);
                                }
                                WithdrawalRequestedTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                            }
                            else if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_NOTICEGIVEN") == 0 || pendingTransaction.Type.Detail.Alias.CompareTo("TT_NOTICEGIVENANDCLOSE") == 0)
                            {
                                AccountProductKey accountProductKey = poolingHelper.GetProductKeyForAccount((int)insignisPoolAccount.AccountID, pLanguageID);
                                if (accountProductKey != null)
                                {
                                    DateTime availableDate = pendingTransaction.Date.Value.AddDays(accountProductKey.investmentTerm.NoticeDays.Value);
                                    double daysUntilAvailable = DateConversion.GetDifferenceBetweenDatesIgnoringTime(availableDate, DateTime.Now);
                                    if (daysUntilAvailable > 0)
                                    {
                                        // so if this is a CONFIRMED transaction, but the countdown is still off in the distance, record it as on notice
                                        RegisterNoticeGivenPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                        if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_NOTICEGIVENANDCLOSE") == 0)
                                        {
                                            Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                            if (spokeAccount.ErrorsDetected == false)
                                                RegisterNoticeGivenInterest(spokeClient.PartyID.Value, ForecastInterest(true,spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                        }
                                        NoticeGivenTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                                    }
                                    else
                                    {
                                        RegisterMaturedRequestPrincipal(spokeClient.PartyID.Value, pendingTransaction.DebitValue.Value);
                                        Account spokeAccount = poolingHelper.financialAbstraction.GetAccount(Convert.ToInt32(innerFields[3].Replace("'", "")));
                                        if (spokeAccount.ErrorsDetected == false)
                                            RegisterMaturedRequestInterest(spokeClient.PartyID.Value, ForecastInterest(false, spokeAccount, pendingTransaction, pCalendarAbstraction, pLanguageID));
                                        MaturedRequestsTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                                    }
                                }
                            }
                            else if (pendingTransaction.Type.Detail.Alias.CompareTo("TT_TOPUPREQUEST") == 0)
                            {
                                RegisterTopUp(spokeClient.PartyID.Value, pendingTransaction.CreditValue.Value);
                                TopUpTransactionIDs.Add(pendingTransaction.AccountTransactionID.Value);
                            }
                        }
                    }
                }
            }
        }

        public void RegisterClient(Client pClient)
        {
            bool found = false;
            foreach (KeyValuePair<int, string> client in Clients)
            {
                if (client.Key == pClient.PartyID.Value)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
                Clients.Add(new KeyValuePair<int, string>(pClient.PartyID.Value, pClient.ClientReference));
        }
        public void RegisterNoticeGivenPrincipal(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> noticeGiven in NoticeGivenRequestsPrincipal)
            {
                if (noticeGiven.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                NoticeGivenRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = NoticeGivenRequestsPrincipal[removeAtIndex].Value;
                updatedAmount += pWithdrawalAmount;
                NoticeGivenRequestsPrincipal.RemoveAt(removeAtIndex);
                NoticeGivenRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterNoticeGivenInterest(int pClientID, decimal pAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> noticeGiven in NoticeGivenRequestsInterest)
            {
                if (noticeGiven.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                NoticeGivenRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, pAmount));
            else
            {
                decimal updatedAmount = NoticeGivenRequestsPrincipal[removeAtIndex].Value;
                updatedAmount += pAmount;
                NoticeGivenRequestsInterest.RemoveAt(removeAtIndex);
                NoticeGivenRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterMaturedRequestPrincipal(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> maturedRequest in MaturedRequestsPrincipal)
            {
                if (maturedRequest.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                MaturedRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = MaturedRequestsPrincipal[removeAtIndex].Value;
                updatedAmount += pWithdrawalAmount;
                MaturedRequestsPrincipal.RemoveAt(removeAtIndex);
                MaturedRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterMaturedRequestInterest(int pClientID, decimal pAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> maturedRequest in MaturedRequestsInterest)
            {
                if (maturedRequest.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                MaturedRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, pAmount));
            else
            {
                decimal updatedAmount = MaturedRequestsInterest[removeAtIndex].Value;
                updatedAmount += pAmount;
                MaturedRequestsInterest.RemoveAt(removeAtIndex);
                MaturedRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterWithdrawalOfPrincipal(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> withdrawalRequestPrincipal in WithdrawalRequestsPrincipal)
            {
                if (withdrawalRequestPrincipal.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                WithdrawalRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = WithdrawalRequestsPrincipal[removeAtIndex].Value;
                updatedAmount -= pWithdrawalAmount;
                WithdrawalRequestsPrincipal.RemoveAt(removeAtIndex);
                WithdrawalRequestsPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterWithdrawalOfInterest(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> withdrawalRequestInterest in WithdrawalRequestsInterest)
            {
                if (withdrawalRequestInterest.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                WithdrawalRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = WithdrawalRequestsInterest[removeAtIndex].Value;
                updatedAmount -= pWithdrawalAmount;
                WithdrawalRequestsInterest.RemoveAt(removeAtIndex);
                WithdrawalRequestsInterest.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterWithdrawalRequestedOfPrincipal(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> withdrawalRequestedPrincipal in WithdrawalRequestedPrincipal)
            {
                if (withdrawalRequestedPrincipal.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                WithdrawalRequestedPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = WithdrawalRequestedPrincipal[removeAtIndex].Value;
                updatedAmount -= pWithdrawalAmount;
                WithdrawalRequestedPrincipal.RemoveAt(removeAtIndex);
                WithdrawalRequestedPrincipal.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterWithdrawalRequestedOfInterest(int pClientID, decimal pWithdrawalAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> withdrawalRequestedInterest in WithdrawalRequestedInterest)
            {
                if (withdrawalRequestedInterest.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                WithdrawalRequestedInterest.Add(new KeyValuePair<int, decimal>(pClientID, pWithdrawalAmount));
            else
            {
                decimal updatedAmount = WithdrawalRequestedInterest[removeAtIndex].Value;
                updatedAmount -= pWithdrawalAmount;
                WithdrawalRequestedInterest.RemoveAt(removeAtIndex);
                WithdrawalRequestedInterest.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterDeposit(int pClientID, decimal pDepositAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> deposit in Deposits)
            {
                if (deposit.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                Deposits.Add(new KeyValuePair<int, decimal>(pClientID, pDepositAmount));
            else
            {
                decimal updatedAmount = Deposits[removeAtIndex].Value;
                updatedAmount += pDepositAmount;
                Deposits.RemoveAt(removeAtIndex);
                Deposits.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public void RegisterTopUp(int pClientID, decimal pDepositAmount)
        {
            bool found = false;
            int index = 0;
            int removeAtIndex = -1;
            foreach (KeyValuePair<int, decimal> deposit in TopUps)
            {
                if (deposit.Key == pClientID)
                {
                    found = true;
                    removeAtIndex = index;
                    break;
                }
                index++;
            }
            if (found == false)
                TopUps.Add(new KeyValuePair<int, decimal>(pClientID, pDepositAmount));
            else
            {
                decimal updatedAmount = TopUps[removeAtIndex].Value;
                updatedAmount += pDepositAmount;
                TopUps.RemoveAt(removeAtIndex);
                TopUps.Add(new KeyValuePair<int, decimal>(pClientID, updatedAmount));
            }
        }
        public decimal CalculateNetTotal()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> deposit in Deposits)
                    netTotal += deposit.Value;
                foreach (KeyValuePair<int, decimal> deposit in TopUps)
                    netTotal += deposit.Value;
                foreach (KeyValuePair<int, decimal> interestWithdrawal in WithdrawalRequestsInterest)
                    netTotal -= interestWithdrawal.Value;
                foreach (KeyValuePair<int, decimal> principalWithdrawal in WithdrawalRequestsPrincipal)
                    netTotal -= principalWithdrawal.Value;
                foreach (KeyValuePair<int, decimal> interestWithdrawal in MaturedRequestsInterest)
                    netTotal -= interestWithdrawal.Value;
                foreach (KeyValuePair<int, decimal> principalWithdrawal in MaturedRequestsPrincipal)
                    netTotal -= principalWithdrawal.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal CalculateNetNotice()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> interestWithdrawal in NoticeGivenRequestsInterest)
                    netTotal -= interestWithdrawal.Value;
                foreach (KeyValuePair<int, decimal> principalWithdrawal in NoticeGivenRequestsPrincipal)
                    netTotal -= principalWithdrawal.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal CalculateNetRequested()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> withdrawalRequested in WithdrawalRequestedInterest)
                    netTotal -= withdrawalRequested.Value;
                foreach (KeyValuePair<int, decimal> principalWithdrawal in WithdrawalRequestedPrincipal)
                    netTotal -= principalWithdrawal.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal CalculateNetMaturedWithdrawals()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> interestRequested in MaturedRequestsInterest)
                    netTotal -= interestRequested.Value;
                foreach (KeyValuePair<int, decimal> principalWithdrawal in MaturedRequestsPrincipal)
                    netTotal -= principalWithdrawal.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal CalculateNetDeposits()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> deposits in Deposits)
                    netTotal += deposits.Value;
                foreach (KeyValuePair<int, decimal> topup in TopUps)
                    netTotal += topup.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal CalculateNetWithdrawals()
        {
            decimal netTotal = 0;
            try
            {
                foreach (KeyValuePair<int, decimal> interest in WithdrawalRequestedInterest)
                    netTotal -= interest.Value;
                foreach (KeyValuePair<int, decimal> principal in WithdrawalRequestedPrincipal)
                    netTotal -= principal.Value;

                foreach (KeyValuePair<int, decimal> interest in WithdrawalRequestsInterest)
                    netTotal -= interest.Value;
                foreach (KeyValuePair<int, decimal> principal in WithdrawalRequestsPrincipal)
                    netTotal -= principal.Value;

                foreach (KeyValuePair<int, decimal> interest in MaturedRequestsInterest)
                    netTotal -= interest.Value;
                foreach (KeyValuePair<int, decimal> principal in MaturedRequestsPrincipal)
                    netTotal -= principal.Value;
            }
            catch
            {
            }
            return netTotal;
        }
        public decimal ForecastInterest(bool pIncludeForecastPowerCalculation, Account pSpokeAccount, AccountTransaction pTriggeringTransaction, Octavo.Gate.Nabu.Extensions.Calendar.Helper.CalendarAbstraction pCalendarAbstraction, int pLanguageID)
        {
            decimal forecastInterest = 0;
            try
            {
                decimal A = SumOfInterestAfterTriggerTransaction(pSpokeAccount, pTriggeringTransaction);
                if (pIncludeForecastPowerCalculation == true)
                {
                    AccountProductKey accountProductKey = poolingHelper.GetProductKeyForAccount((int)pSpokeAccount.AccountID, pLanguageID);
                    if (accountProductKey != null)
                    {
                        DateTime endOfNoticeDate = DateTime.Now;// pTriggeringTransaction.Date.Value;
                        decimal daysUntilAvailable = 0;
                        if (accountProductKey.investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                        {
                            // we assume the end of the notice is the same date as requested on Instant Access accounts - we later factor in working day
                        }
                        else
                        {
                            // only other alternative is a Notice Account
                            try
                            {
                                endOfNoticeDate = endOfNoticeDate.AddDays(accountProductKey.investmentTerm.NoticeDays.Value);
                            }
                            catch
                            {
                            }
                        }
                        Octavo.Gate.Nabu.Entities.BaseDateTime nextWorkingDate = pCalendarAbstraction.GetNextWorkingDay(endOfNoticeDate);
                        if (nextWorkingDate.ErrorsDetected == false)
                            daysUntilAvailable = Convert.ToDecimal(Helper.DateConversion.GetDifferenceBetweenDatesIgnoringTime(nextWorkingDate.Value, DateTime.Now));

                        if (daysUntilAvailable <= 0)
                            daysUntilAvailable = 1;
                        decimal N = daysUntilAvailable;
                        decimal D = (accountProductKey.InterestAccruedRate.Value / 100);
                        decimal T = pTriggeringTransaction.DebitValue.Value + A;
                        // 10,000 * (  ( (1 + (0.0036/100) )^1) -1) = £0.36
                        decimal D1 = D + 1;
                        decimal Power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(D1), Convert.ToDouble(N)));
                        decimal dForecastInterest = T * (Power - 1);
                        forecastInterest = A + NumberFormatter.RoundDownPositive(Convert.ToDecimal(dForecastInterest), 2);
                    }
                }
                else
                    forecastInterest = A;
            }
            catch
            {
            }
            return forecastInterest;
        }
        public decimal SumOfInterestAfterTriggerTransaction(Account pSpokeAccount, AccountTransaction pTriggeringTransaction)
        {
            decimal interest = 0;
            try
            {
                string SQL = "";
                SQL += "SELECT SUM(CreditValue)";
                SQL += " FROM [SchFinancial].[AccountTransaction] act";
                SQL += " INNER JOIN [SchFinancial].[TransactionType] tt ON tt.TransactionTypeID = act.TransactionTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] ttt ON ttt.TranslationID = tt.TranslationID";
                SQL += " WHERE act.AccountID = " + pSpokeAccount.AccountID;
                SQL += " AND ttt.Alias = 'TT_INTERESTCREDIT'";
                SQL += " AND TransactionDate > '" + pTriggeringTransaction.Date.Value.ToString("yyyy-MM-dd") + " 00:00:00.000'";
                Octavo.Gate.Nabu.Entities.BaseDecimal value = poolingHelper.financialAbstraction.CustomQueryAsDecimal(SQL);
                if (value.ErrorsDetected == false)
                    interest = value.Value;
            }
            catch
            {
            }
            return interest;
        }
    }
}
