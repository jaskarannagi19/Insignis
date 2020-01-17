using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class Pooling
    {
        public FinancialAbstraction financialAbstraction = null;
        public OperationsAbstraction operationsAbstraction = null;
        /**********************************************************************
         * CONSTRUCTOR
         *********************************************************************/

        /**********************************************************************
         * POOLING REQUESTS
         *********************************************************************/
        public PoolingRequest[] ListPoolingRequests(string pPartCode, int pLanguageID)
        {
            List<PoolingRequest> poolingRequests = new List<PoolingRequest>();

            string SQL = "SELECT acc.AccountID, aa.Value, c.ClientID, c.ClientReference";
            SQL += " FROM [SchFinancial].[Account] acc";
            SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[AccountAttribute] aa ON aa.AccountID = acc.AccountID";
            SQL += " INNER JOIN [SchFinancial].[AccountAttributeType] aat ON aat.AccountAttributeTypeID = aa.AccountAttributeTypeID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] aatt ON aatt.TranslationID = aat.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[Client] c ON c.ClientID = acc.ClientID";
            SQL += " WHERE acst.Alias = 'ACT_PENDING'";
            SQL += " AND aatt.Alias LIKE '%PRODUCTKEY'";
            SQL += " AND actt.Alias NOT IN ('ACT_BANKPOOLACCOUNT');";

            BaseString[] pendingAccounts = financialAbstraction.CustomQuery(SQL, false, "~");
            foreach (BaseString pendingAccount in pendingAccounts)
            {
                if (pendingAccount.ErrorsDetected == false)
                {
                    string[] fields = pendingAccount.GetFields("~");
                    if (fields[3].Replace("'", "").CompareTo("INSIGNIS") != 0)
                    {
                        AccountProductKey productKey = new AccountProductKey(fields[1].Replace("'", ""));
                        if (productKey.IsPooledProduct.HasValue && productKey.IsPooledProduct.Value == true)
                        {
                            bool includeProduct = false;
                            if (pPartCode != null && pPartCode.Trim().Length > 0)
                            {
                                if (productKey.PartCode.CompareTo(pPartCode) == 0)
                                    includeProduct = true;
                            }
                            else
                                includeProduct = true;

                            if (includeProduct)
                            {
                                Account clientSpokeAccount = financialAbstraction.GetAccount(Convert.ToInt32(fields[0].Replace("'", "")));
                                if (clientSpokeAccount.ErrorsDetected == false && clientSpokeAccount.AccountID.HasValue)
                                {
                                    if (clientSpokeAccount.LinkedAccountID.HasValue == false)
                                    {
                                        bool foundInstitution = false;
                                        foreach (PoolingRequest poolingRequest in poolingRequests)
                                        {
                                            if (poolingRequest.institution.PartyID == productKey.InstitutionID)
                                            {
                                                bool foundProduct = false;
                                                foreach (ProductPoolingRequest productPoolingRequest in poolingRequest.productPoolingRequests)
                                                {
                                                    if (productPoolingRequest != null && productPoolingRequest.product != null && productPoolingRequest.product.ErrorsDetected == false && productPoolingRequest.product.PartCode != null && productPoolingRequest.product.PartCode.CompareTo(productKey.PartCode) == 0)
                                                    {
                                                        bool foundClient = false;
                                                        foreach (Client client in productPoolingRequest.clients)
                                                        {
                                                            if (client.PartyID == Convert.ToInt32(fields[2].Replace("'", "")))
                                                            {
                                                                List<Account> accounts = new List<Account>();
                                                                foreach (Account account in client.Accounts)
                                                                    accounts.Add(account);
                                                                accounts.Add(clientSpokeAccount);
                                                                client.Accounts = accounts.ToArray();

                                                                foundClient = true;
                                                                break;
                                                            }
                                                        }
                                                        if (foundClient == false)
                                                        {
                                                            Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                                            List<Account> accounts = new List<Account>();
                                                            accounts.Add(clientSpokeAccount);
                                                            client.Accounts = accounts.ToArray();
                                                            productPoolingRequest.clients.Add(client);
                                                        }
                                                        foundProduct = true;
                                                        break;
                                                    }
                                                }
                                                if (foundProduct == false)
                                                {
                                                    ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();
                                                    productPoolingRequest.product = operationsAbstraction.GetPartByCode(productKey.PartCode, pLanguageID);

                                                    Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                                    List<Account> accounts = new List<Account>();
                                                    accounts.Add(clientSpokeAccount);
                                                    client.Accounts = accounts.ToArray();
                                                    productPoolingRequest.clients.Add(client);
                                                    poolingRequest.productPoolingRequests.Add(productPoolingRequest);
                                                }
                                                foundInstitution = true;
                                                break;
                                            }
                                        }
                                        if (foundInstitution == false)
                                        {
                                            PoolingRequest request = new PoolingRequest();
                                            request.institution = financialAbstraction.GetInstitution(productKey.InstitutionID.Value, pLanguageID);
                                            ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();
                                            productPoolingRequest.product = operationsAbstraction.GetPartByCode(productKey.PartCode, pLanguageID);

                                            Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                            List<Account> accounts = new List<Account>();
                                            accounts.Add(clientSpokeAccount);
                                            client.Accounts = accounts.ToArray();
                                            productPoolingRequest.clients.Add(client);
                                            request.productPoolingRequests.Add(productPoolingRequest);

                                            poolingRequests.Add(request);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return poolingRequests.ToArray();
        }

        public PoolingRequest[] ListPendingPoolAccounts(int pSelectedAccountID, int pLanguageID)
        {
            List<PoolingRequest> pendingPoolAccounts = new List<PoolingRequest>();

            string SQL = "SELECT acc.AccountID, aa.Value, c.ClientID, c.ClientReference";
            SQL += " FROM [SchFinancial].[Account] acc";
            SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[AccountAttribute] aa ON aa.AccountID = acc.AccountID";
            SQL += " INNER JOIN [SchFinancial].[AccountAttributeType] aat ON aat.AccountAttributeTypeID = aa.AccountAttributeTypeID";
            SQL += " INNER JOIN [SchGlobalisation].[Translation] aatt ON aatt.TranslationID = aat.TranslationID";
            SQL += " INNER JOIN [SchFinancial].[Client] c ON c.ClientID = acc.ClientID";
            SQL += " WHERE acst.Alias = 'ACT_PENDING'";
            SQL += " AND aatt.Alias LIKE '%PRODUCTKEY'";
            SQL += " AND c.ClientReference = 'INSIGNIS'";
            BaseString[] pendingAccounts = financialAbstraction.CustomQuery(SQL, false, "~");
            foreach (BaseString pendingAccount in pendingAccounts)
            {
                if (pendingAccount.ErrorsDetected == false)
                {
                    string[] fields = pendingAccount.GetFields("~");
                    AccountProductKey productKey = new AccountProductKey(fields[1].Replace("'", ""));
                    if (productKey.IsPooledProduct.HasValue && productKey.IsPooledProduct.Value == true)
                    {
                        bool includeProduct = false;
                        if (pSelectedAccountID != -1)
                        {
                            if (pSelectedAccountID == Convert.ToInt32(fields[0].Replace("'", "")))
                                includeProduct = true;
                        }
                        else
                            includeProduct = true;

                        if (includeProduct)
                        {
                            Account insignisPooledAccount = financialAbstraction.GetAccount(Convert.ToInt32(fields[0].Replace("'", "")));
                            if (insignisPooledAccount.ErrorsDetected == false && insignisPooledAccount.AccountID.HasValue)
                            {
                                bool foundInstitution = false;
                                foreach (PoolingRequest pendingPoolAccount in pendingPoolAccounts)
                                {
                                    if (pendingPoolAccount.institution.PartyID == productKey.InstitutionID)
                                    {
                                        bool foundProduct = false;
                                        foreach (ProductPoolingRequest productPoolingRequest in pendingPoolAccount.productPoolingRequests)
                                        {
                                            if (productPoolingRequest.product != null && productPoolingRequest.product.PartCode != null && productKey.PartCode != null)
                                            {
                                                if (productPoolingRequest.product.PartCode.CompareTo(productKey.PartCode) == 0)
                                                {
                                                    bool foundClient = false;
                                                    foreach (Client client in productPoolingRequest.clients)
                                                    {
                                                        if (client.PartyID == Convert.ToInt32(fields[2].Replace("'", "")))
                                                        {
                                                            List<Account> accounts = new List<Account>();
                                                            foreach (Account account in client.Accounts)
                                                                accounts.Add(account);
                                                            accounts.Add(insignisPooledAccount);
                                                            client.Accounts = accounts.ToArray();

                                                            foundClient = true;
                                                            break;
                                                        }
                                                    }
                                                    if (foundClient == false)
                                                    {
                                                        Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                                        List<Account> accounts = new List<Account>();
                                                        accounts.Add(insignisPooledAccount);
                                                        client.Accounts = accounts.ToArray();
                                                        productPoolingRequest.clients.Add(client);
                                                    }
                                                    foundProduct = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (foundProduct == false)
                                        {
                                            ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();
                                            Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                            List<Account> accounts = new List<Account>();
                                            accounts.Add(insignisPooledAccount);
                                            client.Accounts = accounts.ToArray();
                                            productPoolingRequest.clients.Add(client);
                                            pendingPoolAccount.productPoolingRequests.Add(productPoolingRequest);
                                        }
                                        foundInstitution = true;
                                        break;
                                    }
                                }
                                if (foundInstitution == false)
                                {
                                    PoolingRequest request = new PoolingRequest();
                                    request.institution = financialAbstraction.GetInstitution(productKey.InstitutionID.Value, pLanguageID);
                                    ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();

                                    Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[2].Replace("'", "")), pLanguageID);
                                    List<Account> accounts = new List<Account>();
                                    accounts.Add(insignisPooledAccount);
                                    client.Accounts = accounts.ToArray();
                                    productPoolingRequest.clients.Add(client);
                                    request.productPoolingRequests.Add(productPoolingRequest);

                                    pendingPoolAccounts.Add(request);
                                }
                            }
                        }
                    }
                }
            }
            return pendingPoolAccounts.ToArray();
        }

        public PoolingRequest[] ListPendingSpokesForInstantOrNoticePools(int pSelectedAccountID, int pLanguageID)
        {
            List<PoolingRequest> pendingPoolAccounts = new List<PoolingRequest>();

            if (pSelectedAccountID == -1)
            {
                // we have no specific account selected so we need to find all the unique LinkedAccountIDs which represent the individual pools,
                string SQL = "SELECT DISTINCT(LinkedAccountID) FROM [SchFinancial].[Account] a";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = a.AccountTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                SQL += " WHERE LinkedAccountID IS NOT NULL";
                SQL += " AND acst.Alias IN ('ACT_PENDING')";
                SQL += " AND actt.Alias NOT IN ('ACT_BANKPOOLACCOUNT');";
                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] field = record.GetFields();
                        Account poolAccount = financialAbstraction.GetAccount(Convert.ToInt32(field[0].Replace("'", "")));
                        if (poolAccount.ErrorsDetected == false && poolAccount.AccountID.HasValue)
                        {
                            poolAccount.Attributes = financialAbstraction.ListAccountAttributes((int)poolAccount.AccountID, pLanguageID);
                            AccountProductKey productKey = null;
                            foreach (AccountAttribute accountAttribute in poolAccount.Attributes)
                            {
                                if (accountAttribute.ErrorsDetected == false)
                                {
                                    if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                                    {
                                        productKey = new AccountProductKey(accountAttribute.Value);
                                        PoolingRequest poolingRequest = new PoolingRequest();
                                        poolingRequest.institution = financialAbstraction.GetInstitution((int)productKey.InstitutionID, pLanguageID);
                                        poolingRequest.optionalID = poolAccount.AccountID;

                                        ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();
                                        productPoolingRequest.product = operationsAbstraction.GetPartByCode(productKey.PartCode, pLanguageID);
                                        poolingRequest.productPoolingRequests.Add(productPoolingRequest);

                                        string subSQL = "SELECT a.ClientID, a.AccountID FROM [SchFinancial].[Account] a";
                                        subSQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                                        subSQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                                        subSQL += " WHERE LinkedAccountID = " + poolAccount.AccountID;
                                        subSQL += " AND acst.Alias = 'ACT_PENDING';";
                                        BaseString[] subRecordSet = financialAbstraction.CustomQuery(subSQL);
                                        foreach (BaseString subRecord in subRecordSet)
                                        {
                                            if (subRecord.ErrorsDetected == false)
                                            {
                                                string[] subFields = subRecord.GetFields();
                                                Client client = financialAbstraction.GetClient(Convert.ToInt32(subFields[0].Replace("'", "")), pLanguageID);
                                                Account spokeAccount = financialAbstraction.GetAccount(Convert.ToInt32(subFields[1].Replace("'", "")));
                                                List<Account> clientSpokeAccounts = new List<Account>();
                                                clientSpokeAccounts.Add(spokeAccount);
                                                client.Accounts = clientSpokeAccounts.ToArray();
                                                productPoolingRequest.clients.Add(client);
                                            }
                                        }
                                        pendingPoolAccounts.Add(poolingRequest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Account poolAccount = financialAbstraction.GetAccount(pSelectedAccountID);
                if (poolAccount.ErrorsDetected == false && poolAccount.AccountID.HasValue)
                {
                    poolAccount.Attributes = financialAbstraction.ListAccountAttributes((int)poolAccount.AccountID, pLanguageID);
                    AccountProductKey productKey = null;
                    foreach (AccountAttribute accountAttribute in poolAccount.Attributes)
                    {
                        if (accountAttribute.ErrorsDetected == false)
                        {
                            if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                            {
                                productKey = new AccountProductKey(accountAttribute.Value);
                                PoolingRequest poolingRequest = new PoolingRequest();
                                poolingRequest.institution = financialAbstraction.GetInstitution((int)productKey.InstitutionID, pLanguageID);
                                poolingRequest.optionalID = poolAccount.AccountID;

                                ProductPoolingRequest productPoolingRequest = new ProductPoolingRequest();
                                productPoolingRequest.product = operationsAbstraction.GetPartByCode(productKey.PartCode, pLanguageID);
                                poolingRequest.productPoolingRequests.Add(productPoolingRequest);
                                // we have a specific account selected, so just look for the spoke accounts which are in a pending state,
                                // this would indicate that we are adding to an existing INSTANT or NOTICE product pool
                                string SQL = "SELECT a.ClientID, a.AccountID FROM [SchFinancial].[Account] a";
                                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                                SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                                SQL += " WHERE LinkedAccountID = " + pSelectedAccountID;
                                SQL += " AND acst.Alias = 'ACT_PENDING'";
                                SQL += " AND a.ClientID <> " + poolingRequest.institution.PartyID;
                                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL);
                                foreach (BaseString record in recordSet)
                                {
                                    if (record.ErrorsDetected == false)
                                    {
                                        string[] fields = record.GetFields();
                                        Client client = financialAbstraction.GetClient(Convert.ToInt32(fields[0].Replace("'", "")), pLanguageID);
                                        Account spokeAccount = financialAbstraction.GetAccount(Convert.ToInt32(fields[1].Replace("'", "")));
                                        List<Account> clientSpokeAccounts = new List<Account>();
                                        clientSpokeAccounts.Add(spokeAccount);
                                        client.Accounts = clientSpokeAccounts.ToArray();
                                        productPoolingRequest.clients.Add(client);
                                    }
                                }
                                pendingPoolAccounts.Add(poolingRequest);
                            }
                        }
                    }
                }
            }
            return pendingPoolAccounts.ToArray();
        }
        /**********************************************************************
         * Account Helper
         *********************************************************************/
        public Account GetClientHubAccountForSpoke(int pClientSpokeAccountID, int pLanguageID)
        {
            Account clientHubAccount = new Account();
            try
            {
                string SQL = "SELECT acc.ParentAccountID";
                SQL += " FROM [SchFinancial].[Account] acc";
                SQL += " WHERE acc.AccountID=" + pClientSpokeAccountID;
                BaseString[] clientAccountSpokeIDs = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString clientAccountSpokeID in clientAccountSpokeIDs)
                {
                    if (clientAccountSpokeID.ErrorsDetected == false)
                    {
                        string[] fields = clientAccountSpokeID.GetFields();
                        clientHubAccount = financialAbstraction.GetAccount(Convert.ToInt32(fields[0].Replace("'", "")));
                        break;
                    }
                }
            }
            catch
            {
            }
            return clientHubAccount;
        }

        public List<Account> ListClientSpokesWithinPool(int pInsignisPoolAccountID, int pLanguageID)
        {
            List<Account> clientSpokeAccounts = new List<Account>();
            try
            {
                string SQL = "SELECT acc.AccountID";
                SQL += " FROM [SchFinancial].[Account] acc";
                SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                SQL += " WHERE acc.LinkedAccountID=" + pInsignisPoolAccountID;
                SQL += " AND actt.Alias NOT IN ('ACT_BANKPOOLACCOUNT');";
                BaseString[] clientAccountSpokeIDs = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString clientAccountSpokeID in clientAccountSpokeIDs)
                {
                    if (clientAccountSpokeID.ErrorsDetected == false)
                    {
                        string[] fields = clientAccountSpokeID.GetFields();
                        clientSpokeAccounts.Add(financialAbstraction.GetAccount(Convert.ToInt32(fields[0].Replace("'", ""))));
                    }
                }
            }
            catch
            {
            }
            return clientSpokeAccounts;
        }

        public List<Account> ListClientSpokesWithinPool(int pInsignisPoolAccountID, int pLanguageID, string pAccountStatusAlias)
        {
            List<Account> clientSpokeAccounts = new List<Account>();
            try
            {
                string SQL = "SELECT acc.AccountID";
                SQL += " FROM [SchFinancial].[Account] acc";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                SQL += " WHERE acc.LinkedAccountID=" + pInsignisPoolAccountID;
                SQL += " AND acst.Alias='" + pAccountStatusAlias + "'";
                SQL += " AND actt.Alias NOT IN ('ACT_BANKPOOLACCOUNT');";
                BaseString[] clientAccountSpokeIDs = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString clientAccountSpokeID in clientAccountSpokeIDs)
                {
                    if (clientAccountSpokeID.ErrorsDetected == false)
                    {
                        string[] fields = clientAccountSpokeID.GetFields();
                        clientSpokeAccounts.Add(financialAbstraction.GetAccount(Convert.ToInt32(fields[0].Replace("'", ""))));
                    }
                }
            }
            catch
            {
            }
            return clientSpokeAccounts;
        }

        public List<Account> ListOpenPoolAccounts(int pSelectedAccountID, int pLanguageID)
        {
            List<Account> insignisPoolAccounts = new List<Account>();
            try
            {
                if (pSelectedAccountID == -1)
                {
                    string SQL = "SELECT";
                    SQL += " AccountID";
                    SQL += " FROM [SchFinancial].[Account] acc";
                    SQL += " INNER JOIN [SchFinancial].[Client] cli ON cli.ClientID = acc.ClientID";
                    SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                    SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                    SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
                    SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                    SQL += " WHERE cli.ClientReference = 'INSIGNIS'";
                    SQL += " AND actt.Alias LIKE '%POOLED%'";
                    SQL += " AND acst.Alias = 'ACT_OPEN';";
                    BaseString[] insignisPoolAccountIDs = financialAbstraction.CustomQuery(SQL);
                    foreach (BaseString insignisPoolAccountID in insignisPoolAccountIDs)
                    {
                        if (insignisPoolAccountID.ErrorsDetected == false)
                            insignisPoolAccounts.Add(financialAbstraction.GetAccount(Convert.ToInt32(insignisPoolAccountID.Value)));
                    }
                }
                else
                    insignisPoolAccounts.Add(financialAbstraction.GetAccount(pSelectedAccountID));
            }
            catch
            {
            }
            return insignisPoolAccounts;
        }

        public List<Account> ListClosedPoolAccounts(int pSelectedAccountID, int pLanguageID)
        {
            List<Account> insignisPoolAccounts = new List<Account>();
            try
            {
                if (pSelectedAccountID == -1)
                {
                    string SQL = "SELECT";
                    SQL += " AccountID";
                    SQL += " FROM [SchFinancial].[Account] acc";
                    SQL += " INNER JOIN [SchFinancial].[Client] cli ON cli.ClientID = acc.ClientID";
                    SQL += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                    SQL += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                    SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
                    SQL += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                    SQL += " WHERE cli.ClientReference = 'INSIGNIS'";
                    SQL += " AND actt.Alias LIKE '%POOLED%'";
                    SQL += " AND acst.Alias = 'ACT_CLOSED';";
                    BaseString[] insignisPoolAccountIDs = financialAbstraction.CustomQuery(SQL);
                    foreach (BaseString insignisPoolAccountID in insignisPoolAccountIDs)
                    {
                        if (insignisPoolAccountID.ErrorsDetected == false)
                            insignisPoolAccounts.Add(financialAbstraction.GetAccount(Convert.ToInt32(insignisPoolAccountID.Value)));
                    }
                }
                else
                    insignisPoolAccounts.Add(financialAbstraction.GetAccount(pSelectedAccountID));
            }
            catch
            {
            }
            return insignisPoolAccounts;
        }
        /**********************************************************************
         * Client Helper
         *********************************************************************/
        public Client GetClientForAccount(int pAccountID, int pLanguageID)
        {
            Client client = new Client();
            try
            {
                string SQL = "SELECT ClientID";
                SQL += " FROM [SchFinancial].[Account]";
                SQL += " WHERE AccountID=" + pAccountID;
                BaseString[] clientIDs = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString clientID in clientIDs)
                {
                    if (clientID.ErrorsDetected == false)
                    {
                        string[] fields = clientID.GetFields();
                        client = financialAbstraction.GetClient(Convert.ToInt32(fields[0].Replace("'", "")), pLanguageID);
                        break;
                    }
                }
            }
            catch (Exception exc)
            {
                client.ErrorsDetected = true;
                client.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
                client.StackTrace = exc.StackTrace;
            }
            return client;
        }

        /**********************************************************************
         * Client Helper
         *********************************************************************/
        public List<Institution> ListInstitutionsOfferingPooledProductLine(int pLanguageID)
        {
            List<Institution> institutions = new List<Institution>();
            try
            {
                string SQL = "SELECT";
                SQL += " InstitutionID";
                SQL += " FROM [SchPublicity].[ProductLine] pl";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = pl.TranslationID";
                SQL += " INNER JOIN [SchFinancial].[Institution] i ON i.InstitutionID = pl.OrganisationID";
                SQL += " WHERE t.Alias LIKE 'POOLEDPRODUCTS%'";
                BaseString[] institutionIDs = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString institutionID in institutionIDs)
                {
                    if (institutionID.ErrorsDetected == false)
                        institutions.Add(financialAbstraction.GetInstitution(Convert.ToInt32(institutionID.Value), pLanguageID));
                }
            }
            catch
            {
            }
            return institutions;
        }

        /**********************************************************************
         * Bank Pool Account
         *********************************************************************/
        public Account GetBankPoolAccount(int pInstitutionID, int pInsignisPoolAccountID)
        {
            Account bankPoolAccount = new Account();
            try
            {
                string SQL = "SELECT AccountID FROM [SchFinancial].[Account]";
                SQL += " WHERE ClientID=" + pInstitutionID;
                SQL += " AND LinkedAccountID = " + pInsignisPoolAccountID;

                BaseInteger accountID = financialAbstraction.CustomQueryAsInteger(SQL);
                if (accountID.ErrorsDetected == false)
                    bankPoolAccount = financialAbstraction.GetAccount(accountID.Value);
            }
            catch (Exception exc)
            {
                bankPoolAccount.ErrorsDetected = true;
                bankPoolAccount.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
                bankPoolAccount.StackTrace = exc.StackTrace;
            }
            return bankPoolAccount;
        }

        /**********************************************************************
         * HELPER METHODS
         *********************************************************************/
        public AccountProductKey GetProductKeyForAccount(int pAccountID, int pLanguageID)
        {
            AccountProductKey accountProductKey = null;
            try
            {
                AccountAttribute[] accountAttributes = financialAbstraction.ListAccountAttributes(pAccountID, pLanguageID);
                if (accountAttributes != null && accountAttributes.Length > 0)
                {
                    foreach (AccountAttribute accountAttribute in accountAttributes)
                    {
                        if (accountAttribute.Detail.Alias.EndsWith("PRODUCTKEY"))
                            accountProductKey = new AccountProductKey(accountAttribute.Value);
                    }
                }
            }
            catch
            {
            }
            return accountProductKey;
        }
        public string GetProductNameForAccount(int pAccountID, int pLanguageID)
        {
            string accountProductName = "";
            try
            {
                AccountAttribute[] accountAttributes = financialAbstraction.ListAccountAttributes(pAccountID, pLanguageID);
                if (accountAttributes != null && accountAttributes.Length > 0)
                {
                    foreach (AccountAttribute accountAttribute in accountAttributes)
                    {
                        if (accountAttribute.Detail.Alias.EndsWith("PRODUCTNAME"))
                            accountProductName = accountAttribute.Value;
                    }
                }
            }
            catch
            {
            }
            return accountProductName;
        }
        public bool GetPayInterestIntoThisAccount(int pAccountID, int pLanguageID)
        {
            bool isInterestToBePaidIn = false;
            try
            {
                AccountAttribute[] accountAttributes = financialAbstraction.ListAccountAttributes(pAccountID, pLanguageID);
                if (accountAttributes != null && accountAttributes.Length > 0)
                {
                    foreach (AccountAttribute accountAttribute in accountAttributes)
                    {
                        if (accountAttribute.ErrorsDetected == false)
                        {
                            if (accountAttribute.Detail.Alias.Contains("PAY_INTEREST_INTO_THIS_ACCOUNT"))
                            {
                                if (accountAttribute.Value.ToLower().CompareTo("true") == 0)
                                    isInterestToBePaidIn = true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return isInterestToBePaidIn;
        }

        /**********************************************************************
         * INTEREST TRANSACTION METHODS
         *********************************************************************/
        public AccountTransaction[] GetClientPoolingAccountInterestTransactionsGroupedByMonth(int pClientID, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            return GetClientPoolingAccountInterestTransactionsGroupedByMonth(pClientID, financialAbstraction, pLanguageID, null, null);
        }
        public AccountTransaction[] GetClientPoolingAccountInterestTransactionsGroupedByMonth(int pClientID, FinancialAbstraction pFinancialAbstraction, int pLanguageID, DateTime? pTransactionDateFrom, DateTime? pTransactionDateTo)
        {
            List<AccountTransaction> accountTransactions = new List<AccountTransaction>();
            try
            {
                TransactionType transactionType = financialAbstraction.GetTransactionTypeByAlias("TT_INTERESTCREDIT", pLanguageID);
                TransactionStatus transactionStatus = financialAbstraction.GetTransactionStatusByAlias("TS_ACTIONED", pLanguageID);

                string SQL = "";
                SQL += "SELECT FORMAT(t.TransactionDate, 'yyyyMM') AS InterestPaidInMonth, a.LinkedAccountID, SUM(t.CreditValue)";
                SQL += " FROM [SchFinancial].[Account] a";
                SQL += " INNER JOIN [SchFinancial].[AccountTransaction] t ON t.AccountID = a.AccountID";
                SQL += " INNER JOIN [SchFinancial].[TransactionStatus] ts ON ts.TransactionStatusID = t.TransactionStatusID";
                SQL += " INNER JOIN [SchFinancial].[TransactionType] tt ON tt.TransactionTypeID = t.TransactionTypeID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] tst ON tst.TranslationID = ts.TranslationID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] ttt ON ttt.TranslationID = tt.TranslationID";
                SQL += " WHERE a.ClientID = " + pClientID;
                SQL += " AND a.ParentAccountID IS NOT NULL";
                SQL += " AND a.LinkedAccountID IS NOT NULL";
                if (pTransactionDateFrom.HasValue && pTransactionDateTo.HasValue)
                {
                    SQL += " AND t.TransactionDate >= '" + pTransactionDateFrom.Value.ToString("yyyy-MM-dd") + "'";
                    SQL += " AND t.TransactionDate <= '" + pTransactionDateTo.Value.ToString("yyyy-MM-dd") + "'";
                }
                SQL += " AND ttt.Alias = 'TT_INTERESTCREDIT'";
                SQL += " AND tst.Alias = 'TS_ACTIONED'";
                SQL += " GROUP BY FORMAT(t.TransactionDate, 'yyyyMM'), a.LinkedAccountID";
                SQL += " ORDER BY InterestPaidInMonth ASC";

                Octavo.Gate.Nabu.Entities.BaseString[] recordSet = financialAbstraction.CustomQuery(SQL);
                foreach (Octavo.Gate.Nabu.Entities.BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields();

                        AccountTransaction transaction = new AccountTransaction();
                        transaction.AccountTransactionID = -1;
                        int linkedAccountID = Convert.ToInt32(fields[1].Replace("'", ""));
                        transaction.CreditValue = Convert.ToDecimal(fields[2].Replace("'", ""));
                        // take year and month and assume a date as the first of the month
                        transaction.Date = DateTime.ParseExact(fields[0].Replace("'", "") + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                        // now using that determine the last day of the month
                        transaction.Date = DateTime.ParseExact(DateTime.DaysInMonth(transaction.Date.Value.Year, transaction.Date.Value.Month).ToString("00") + "-" + transaction.Date.Value.Month.ToString("00") + "-" + transaction.Date.Value.Year.ToString("0000"), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        transaction.DebitValue = null;

                        transaction.Details = "Interest Payment for " + transaction.Date.Value.ToString("MMM yyyy") + " (" + linkedAccountID + ")";
                        AccountProductKey accountProductKey = GetProductKeyForAccount(linkedAccountID, pLanguageID);
                        if (accountProductKey != null)
                        {
                            if (accountProductKey.InstitutionID.HasValue)
                            {
                                transaction.Details = "Interest Payment for " + transaction.Date.Value.ToString("MMM yyyy") + " (" + financialAbstraction.GetInstitution(accountProductKey.InstitutionID.Value, pLanguageID).Name;
                                string productName = GetProductNameForAccount(linkedAccountID, pLanguageID);
                                if (productName != null && productName.Trim().Length > 0)
                                    transaction.Details += " - " + productName;
                                transaction.Details += ")";
                            }
                            else
                                transaction.Details = "Interest Payment for " + transaction.Date.Value.ToString("MMM yyyy") + " (" + linkedAccountID + ")";
                        }
                        transaction.RelatedAccountID = null;
                        transaction.RelatedTransactionID = null;
                        transaction.RunningBalance = null;
                        transaction.Status = transactionStatus;
                        transaction.Type = transactionType;

                        accountTransactions.Add(transaction);
                    }
                }
            }
            catch
            {
            }
            return accountTransactions.ToArray();
        }

        /**********************************************************************
         * STATIC METHODS
         *********************************************************************/
        public static bool IsPooledProduct(AccountProductKey pAccountProductKey)
        {
            bool isPooledProduct = false;
            if (pAccountProductKey.IsPooledProduct.HasValue)
                isPooledProduct = pAccountProductKey.IsPooledProduct.Value;
            return isPooledProduct;
        }
    }
}
