using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Core;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class RelatedAccounts
    {
        public static List<Account> ListRelatedHubAccounts(int pToPartyID, string pRelationshipTypeAlias, CoreAbstraction pCoreAbstraction, int pLanguageID)
        {
            List<Account> relatedAccounts = new List<Account>();

            try
            {
                PartyRelationshipType relationshipType = pCoreAbstraction.GetPartyRelationshipTypeByAlias(pRelationshipTypeAlias, pLanguageID);
                if (relationshipType != null && relationshipType.ErrorsDetected == false && relationshipType.PartyRelationshipTypeID.HasValue)
                {
                    List<int> partyIDs = new List<int>();
                    PartyRelationship[] relationships = pCoreAbstraction.ListPartyRelationshipsTo((int)pToPartyID, pLanguageID);
                    if (relationships != null && relationships.Length > 0)
                    {
                        if (pRelationshipTypeAlias.CompareTo("SPOUSEORPARTNER") == 0)
                        {
                            foreach (PartyRelationship relationship in relationships)
                            {
                                if (relationship.ErrorsDetected == false)
                                {
                                    if (relationship.PartyRelationshipType.PartyRelationshipTypeID == relationshipType.PartyRelationshipTypeID)
                                    {
                                        if (relationship.FromPartyID == pToPartyID)
                                            partyIDs.Add(relationship.ToPartyID);
                                        else
                                            partyIDs.Add(relationship.FromPartyID);
                                        break;
                                    }
                                }
                            }
                            if (partyIDs.Count == 0)
                            {
                                relationships = pCoreAbstraction.ListPartyRelationshipsFrom((int)pToPartyID, pLanguageID);
                                foreach (PartyRelationship relationship in relationships)
                                {
                                    if (relationship.ErrorsDetected == false)
                                    {
                                        if (relationship.PartyRelationshipType.PartyRelationshipTypeID == relationshipType.PartyRelationshipTypeID)
                                        {
                                            if (relationship.FromPartyID == pToPartyID)
                                                partyIDs.Add(relationship.ToPartyID);
                                            else
                                                partyIDs.Add(relationship.FromPartyID);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (PartyRelationship relationship in relationships)
                            {
                                if (relationship.ErrorsDetected == false)
                                {
                                    if (relationship.PartyRelationshipType.PartyRelationshipTypeID == relationshipType.PartyRelationshipTypeID)
                                        partyIDs.Add(relationship.FromPartyID);
                                }
                            }
                        }
                    }
                    if(partyIDs.Count > 0 && partyIDs[0] != pToPartyID)
                    {
                        foreach (int partyID in partyIDs)
                        {
                            string sql = "";
                            sql += "SELECT acc.AccountID,";
                            sql += "acc.CurrencyID,";
                            sql += "acc.AccountTypeID,";
                            sql += "acc.AccountNumber,";
                            sql += "acc.AccountName,";
                            sql += "acc.AccountReference,";
                            sql += "acc.AccountStatusID,";
                            sql += "acc.IsOffShore,";
                            sql += "acc.IBAN,";
                            sql += "acc.SWIFTBIC,";
                            sql += "acc.Balance,";
                            sql += "acc.BalanceDate,";
                            sql += "acc.BranchID";
                            sql += " FROM [SchFinancial].[Account] acc";
                            sql += " INNER JOIN [SchFinancial].[AccountType] act ON act.AccountTypeID = acc.AccountTypeID";
                            sql += " INNER JOIN [SchGlobalisation].[Translation] actt ON actt.TranslationID = act.TranslationID";
                            sql += " INNER JOIN [SchFinancial].[AccountStatus] acs ON acs.AccountStatusID = acc.AccountStatusID";
                            sql += " INNER JOIN [SchGlobalisation].[Translation] acst ON acst.TranslationID = acs.TranslationID";
                            sql += " WHERE ClientID= " + partyID;
                            if (pRelationshipTypeAlias.CompareTo("SPOUSEORPARTNER") == 0)
                                sql += " AND actt.Alias LIKE '%JOINTHUBACCOUNT'";
                            else
                                sql += " AND actt.Alias LIKE '%HUBACCOUNT'";
                            sql += " AND acst.Alias = 'ACT_OPEN'";

                            BaseString[] hubAccountRecords = pCoreAbstraction.CustomQuery(sql);
                            if (hubAccountRecords != null && hubAccountRecords.Length > 0)
                            {
                                string pipeSeparator = "|";
                                foreach (BaseString hubAccountRecord in hubAccountRecords)
                                {
                                    if (hubAccountRecord.ErrorsDetected == false)
                                    {
                                        string[] fields = hubAccountRecord.Value.Split(pipeSeparator.ToCharArray());
                                        Account relatedHubAccount = new Account(Convert.ToInt32(fields[0]));
                                        relatedHubAccount.Currency = new Currency(Convert.ToInt32(fields[1]));
                                        relatedHubAccount.Type = new AccountType(Convert.ToInt32(fields[2]));
                                        relatedHubAccount.Number = fields[3].Substring(1, fields[3].Length - 2);
                                        relatedHubAccount.Name = fields[4].Substring(1, fields[4].Length - 2);
                                        relatedHubAccount.Reference = fields[5].Substring(1, fields[5].Length - 2);
                                        relatedHubAccount.Status = new AccountStatus(Convert.ToInt32(fields[6]));
                                        relatedHubAccount.IsOffShore = ((fields[7].Replace("'", "").ToLower().CompareTo("true") == 0) ? true : false);
                                        relatedHubAccount.IBAN = fields[8].Substring(1, fields[8].Length - 2);
                                        relatedHubAccount.SWIFTBIC = fields[9].Substring(1, fields[9].Length - 2);

                                        relatedHubAccount.Balance = 0;
                                        try
                                        {
                                            relatedHubAccount.Balance = Convert.ToDecimal(fields[10]);
                                        }
                                        catch
                                        {
                                        }
                                        if (fields[11].Replace("'", "").Length > 0)
                                        {
                                            relatedHubAccount.BalanceDate = DateTime.Now;
                                            try
                                            {
                                                relatedHubAccount.BalanceDate = DateTime.ParseExact(fields[11].Replace("'", ""), "dd-MMM-yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        if (fields[12].Replace("'", "").Length > 0)
                                        {
                                            try
                                            {
                                                relatedHubAccount.Branch = new Branch(Convert.ToInt32(fields[12].Replace("'", "")));
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        relatedAccounts.Add(relatedHubAccount);
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
            return relatedAccounts;
        }
    }
}