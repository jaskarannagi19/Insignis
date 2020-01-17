using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class TopUpManager
    {
        public FinancialAbstraction financialAbstraction = null;

        public TopUpManager(FinancialAbstraction pFinancialAbstraction)
        {
            financialAbstraction = pFinancialAbstraction;
        }

        public List<string> ListLiveProductCodes(int pInstitutionID)
        {
            List<string> productCodes = new List<string>();
            try
            {
                string SQL = "SELECT DISTINCT(aa.Value) FROM [SchFinancial].[AccountAttribute] aa";
                SQL += " INNER JOIN [SchFinancial].[Account] a ON a.AccountID = aa.AccountID";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = acs.TranslationID";
                SQL += " WHERE t.Alias = 'ACT_OPEN'";
                SQL += " AND (aa.Value LIKE '" + pInstitutionID + "|%Instant%' OR aa.Value LIKE '" + pInstitutionID + "|%Notice%');";
                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL);
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        AccountProductKey productKey = new AccountProductKey(record.Value.Replace("'",""));

                        bool found = false;
                        foreach (string productCode in productCodes)
                        {
                            if (productCode.CompareTo(productKey.PartCode) == 0)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                        {
                            productCodes.Add(productKey.PartCode);
                        }
                    }
                }
                productCodes.Sort();
            }
            catch
            {
            }
            return productCodes;
        }

        public List<KeyValuePair<string,bool>> ListClientsWithProduct(int pInstitutionID, string pProductCode)
        {
            List<KeyValuePair<string, bool>> clientReferencePairs = new List<KeyValuePair<string, bool>>();
            try
            {
                string SQL = "SELECT c.ClientReference, aa.Value FROM [SchFinancial].[AccountAttribute] aa";
                SQL += " INNER JOIN [SchFinancial].[Account] a ON a.AccountID = aa.AccountID";
                SQL += " INNER JOIN [SchFinancial].[Client] c ON c.ClientID = a.ClientID";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = acs.TranslationID";
                SQL += " WHERE t.Alias = 'ACT_OPEN'";
                SQL += " AND aa.Value LIKE '" + pInstitutionID + "|" + pProductCode + "|%';";
                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL,false,"~");
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields("~");
                        string clientReference = fields[0].Replace("'", "");
                        AccountProductKey accountProductKey = new AccountProductKey(fields[1].Replace("'", ""));
                        clientReferencePairs.Add(new KeyValuePair<string, bool>(clientReference,accountProductKey.AccountPermitsTopUps));
                    }
                }
                clientReferencePairs.Sort();
            }
            catch
            {
            }
            return clientReferencePairs;
        }

        public BaseBoolean DisallowTopUpForProduct(int pInstitutionID, string pProductCode)
        {
            BaseBoolean result = new BaseBoolean(false);
            try
            {
                string SQL = "SELECT aa.AccountID, aa.AccountAttributeTypeID, aa.Value FROM [SchFinancial].[AccountAttribute] aa";
                SQL += " INNER JOIN [SchFinancial].[Account] a ON a.AccountID = aa.AccountID";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = acs.TranslationID";
                SQL += " WHERE t.Alias = 'ACT_OPEN'";
                SQL += " AND aa.Value LIKE '" + pInstitutionID + "|" + pProductCode + "|%';";
                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL, false, "~");
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields("~");
                        int accountID = Convert.ToInt32(fields[0].Replace("'", ""));
                        int accountAttributeTypeID = Convert.ToInt32(fields[1].Replace("'", ""));
                        AccountProductKey accountProductKey = new AccountProductKey(fields[2].Replace("'", ""));
                        if (accountProductKey.AccountPermitsTopUps)
                        {
                            accountProductKey.AccountPermitsTopUps = false;

                            string updateQuery = "UPDATE [SchFinancial].[AccountAttribute]";
                            updateQuery += " SET Value = '" + accountProductKey.ConvertToPSV() + "'";
                            updateQuery += " WHERE AccountID = " + accountID;
                            updateQuery += " AND AccountAttributeTypeID = " + accountAttributeTypeID;
                            updateQuery += ";";
                            result = financialAbstraction.CustomNonQuery(updateQuery);
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }
        public BaseBoolean AllowTopUpForProduct(int pInstitutionID, string pProductCode)
        {
            BaseBoolean result = new BaseBoolean(false);
            try
            {
                string SQL = "SELECT aa.AccountID, aa.AccountAttributeTypeID, aa.Value FROM [SchFinancial].[AccountAttribute] aa";
                SQL += " INNER JOIN [SchFinancial].[Account] a ON a.AccountID = aa.AccountID";
                SQL += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = a.AccountStatusID";
                SQL += " INNER JOIN [SchGlobalisation].[Translation] t ON t.TranslationID = acs.TranslationID";
                SQL += " WHERE t.Alias = 'ACT_OPEN'";
                SQL += " AND aa.Value LIKE '" + pInstitutionID + "|" + pProductCode + "|%';";
                BaseString[] recordSet = financialAbstraction.CustomQuery(SQL, false, "~");
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        string[] fields = record.GetFields("~");
                        int accountID = Convert.ToInt32(fields[0].Replace("'", ""));
                        int accountAttributeTypeID = Convert.ToInt32(fields[1].Replace("'", ""));
                        AccountProductKey accountProductKey = new AccountProductKey(fields[2].Replace("'", ""));
                        if (accountProductKey.AccountPermitsTopUps==false)
                        {
                            accountProductKey.AccountPermitsTopUps = true;

                            string updateQuery = "UPDATE [SchFinancial].[AccountAttribute]";
                            updateQuery += " SET Value = '" + accountProductKey.ConvertToPSV() + "'";
                            updateQuery += " WHERE AccountID = " + accountID;
                            updateQuery += " AND AccountAttributeTypeID = " + accountAttributeTypeID;
                            updateQuery += ";";
                            result = financialAbstraction.CustomNonQuery(updateQuery);
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }
    }
}
