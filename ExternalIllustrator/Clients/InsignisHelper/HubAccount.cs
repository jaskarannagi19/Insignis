using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class HubAccountHelper
    {
        public static Institution GetHubAccountInstitutionForBranch(Branch pBranch, CoreAbstraction pCoreAbstraction, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
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

        public static decimal SumCredits(int pHubAccountID, FinancialAbstraction pFinancialAbstraction)
        {
            decimal totalCredits = 0;
            try
            {
                string SQL = "SELECT SUM(CreditValue)";
                SQL += " FROM[SchFinancial].[AccountTransaction]";
                SQL += " WHERE AccountID = " + pHubAccountID;
                BaseString[] recordSet = pFinancialAbstraction.CustomQuery(SQL);
                totalCredits = Convert.ToDecimal(recordSet[0].Value);
            }
            catch
            {
            }
            return totalCredits;
        }

        public static Client GetClientForHubAccount(Account pHubAccount, FinancialAbstraction pFinancialAbstraction, int pLanguageID)
        {
            Client client = new Client();
            try
            {
                BaseString[] recordSet = pFinancialAbstraction.CustomQuery("SELECT ClientID FROM [SchFinancial].[Account] WHERE AccountID=" + pHubAccount.AccountID);
                if (recordSet.Length > 0)
                {
                    if (recordSet[0].ErrorsDetected == false)
                        client = pFinancialAbstraction.GetClient(Convert.ToInt32(recordSet[0].Value), pLanguageID);
                }
            }
            catch (Exception exc)
            {
                client.ErrorsDetected = true;
                client.StackTrace = exc.StackTrace;
                client.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
            }
            return client;
        }
    }
}