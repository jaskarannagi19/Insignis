using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class HubAccountDeposit
    {
        public List<HubAccountBranch> hubAccountBranches = new List<HubAccountBranch>();

        public HubAccountDeposit()
        {
        }

        public HubAccountDeposit(string pConfigFile)
        {
            Read(pConfigFile);
        }

        private bool Read(string pConfigFile)
        {
            bool result = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pConfigFile);
                foreach (XmlNode hubAccountDepositsNode in doc.ChildNodes)
                {
                    if (hubAccountDepositsNode.Name.ToString().CompareTo("hubAccountDeposits") == 0)
                    {
                        foreach (XmlElement hubAccountBranchNode in hubAccountDepositsNode.ChildNodes)
                        {
                            if (hubAccountBranchNode.Name.ToString().CompareTo("hubAccountBranch") == 0)
                            {
                                HubAccountBranch branch = new HubAccountBranch();
                                foreach (XmlAttribute attribute in hubAccountBranchNode.Attributes)
                                {
                                    if (attribute.Name.CompareTo("id") == 0)
                                        branch.ID = Convert.ToInt32(attribute.Value);
                                    else if (attribute.Name.CompareTo("bankName") == 0)
                                        branch.bankName = attribute.Value;
                                }

                                if (hubAccountBranchNode.HasChildNodes)
                                {
                                    foreach (XmlElement hubAccountTypeNode in hubAccountBranchNode.ChildNodes)
                                    {
                                        if (hubAccountTypeNode.Name.ToString().CompareTo("hubAccountType") == 0)
                                        {
                                            HubAccountType hubAccountType = new HubAccountType();
                                            foreach (XmlAttribute attribute in hubAccountTypeNode.Attributes)
                                            {
                                                if (attribute.Name.CompareTo("alias") == 0)
                                                    hubAccountType.alias = attribute.Value;
                                                else if (attribute.Name.CompareTo("note") == 0)
                                                    hubAccountType.note = attribute.Value;
                                            }
                                            if (hubAccountTypeNode.HasChildNodes)
                                            {
                                                foreach (XmlElement hubAccountNode in hubAccountTypeNode.ChildNodes)
                                                {
                                                    if (hubAccountNode.Name.ToString().CompareTo("hubAccount") == 0)
                                                    {
                                                        HubAccount hubAccount = new HubAccount();
                                                        foreach (XmlAttribute accountAttribute in hubAccountNode.Attributes)
                                                        {
                                                            if (accountAttribute.Name.CompareTo("currencyCode") == 0)
                                                                hubAccount.CurrencyCode = accountAttribute.Value;
                                                            else if (accountAttribute.Name.CompareTo("number") == 0)
                                                                hubAccount.Number = accountAttribute.Value;
                                                            else if (accountAttribute.Name.CompareTo("iban") == 0)
                                                                hubAccount.IBAN = accountAttribute.Value;
                                                            else if (accountAttribute.Name.CompareTo("swift") == 0)
                                                                hubAccount.SWIFT = accountAttribute.Value;
                                                        }
                                                        hubAccountType.hubAccounts.Add(hubAccount);
                                                    }
                                                }
                                            }
                                            branch.hubAccountTypes.Add(hubAccountType);
                                        }
                                    }
                                }
                                hubAccountBranches.Add(branch);
                            }
                        }
                    }
                }
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public bool Write(string pConfigFile)
        {
            bool result = false;
            try
            {
                if (System.IO.File.Exists(pConfigFile))
                    System.IO.File.Delete(pConfigFile);

                using (StreamWriter sw = new StreamWriter(pConfigFile, false))
                {
                    sw.Write(ToXML());
                    sw.Close();
                }
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            xml += "<hubAccountDeposits>";
            if (hubAccountBranches.Count > 0)
            {
                foreach (HubAccountBranch branch in hubAccountBranches)
                    xml += branch.ToXML();
            }
            xml += "</hubAccountDeposits>";
            return xml;
        }

        public string GetBankName(int pBranchID)
        {
            string bankName = "";

            foreach (HubAccountBranch branch in hubAccountBranches)
            {
                if (branch.ID == pBranchID)
                {
                    bankName = branch.bankName;
                    break;
                }
            }

            return bankName;
        }

        public string GetAccountNumber(int pBranchID, string pHubAccountTypeAlias, string pCurrencyCode)
        {
            string accountNumber = "";

            foreach (HubAccountBranch branch in hubAccountBranches)
            {
                if (branch.ID == pBranchID)
                {
                    foreach (HubAccountType hubAccountType in branch.hubAccountTypes)
                    {
                        if (hubAccountType.alias.CompareTo(pHubAccountTypeAlias) == 0)
                        {
                            foreach (HubAccount account in hubAccountType.hubAccounts)
                            {
                                if (account.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                                {
                                    accountNumber = account.Number;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return accountNumber;
        }

        public string GetIBAN(int pBranchID, string pHubAccountTypeAlias, string pCurrencyCode)
        {
            string iban = "";

            foreach (HubAccountBranch branch in hubAccountBranches)
            {
                if (branch.ID == pBranchID)
                {
                    foreach (HubAccountType hubAccountType in branch.hubAccountTypes)
                    {
                        if (hubAccountType.alias.CompareTo(pHubAccountTypeAlias) == 0)
                        {
                            foreach (HubAccount account in hubAccountType.hubAccounts)
                            {
                                if (account.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                                {
                                    iban = account.IBAN;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return iban;
        }

        public string GetSWIFT(int pBranchID, string pHubAccountTypeAlias, string pCurrencyCode)
        {
            string swift = "";

            foreach (HubAccountBranch branch in hubAccountBranches)
            {
                if (branch.ID == pBranchID)
                {
                    foreach (HubAccountType hubAccountType in branch.hubAccountTypes)
                    {
                        if (hubAccountType.alias.CompareTo(pHubAccountTypeAlias) == 0)
                        {
                            foreach (HubAccount account in hubAccountType.hubAccounts)
                            {
                                if (account.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                                {
                                    swift = account.SWIFT;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return swift;
        }
    }

    public class HubAccountBranch
    {
        public int? ID = null;
        public List<HubAccountType> hubAccountTypes = new List<HubAccountType>();
        public string bankName = "";

        public string ToXML()
        {
            string xml = "";
            xml += "<hubAccountBranch id=\"" + ID + "\" bankName=\"" + bankName + "\">";
            if (hubAccountTypes.Count > 0)
            {
                foreach (HubAccountType hubAccountType in hubAccountTypes)
                    xml += hubAccountType.ToXML();
            }
            xml += "</hubAccountBranch>";
            return xml;
        }
    }

    public class HubAccountType
    {
        public string alias = "";
        public string note = "";

        public List<HubAccount> hubAccounts = new List<HubAccount>();

        public string ToXML()
        {
            string xml = "";
            xml += "<hubAccountType alias=\"" + alias + "\" note=\"" + note + "\">";
            if (hubAccounts.Count > 0)
            {
                foreach (HubAccount hubAccount in hubAccounts)
                    xml += hubAccount.ToXML();
            }
            xml += "</hubAccountType>";
            return xml;
        }
    }

    public class HubAccount
    {
        public string CurrencyCode = "";
        public string Number = "";
        public string IBAN = "";
        public string SWIFT = "";

        public string ToXML()
        {
            string xml = "";
            xml += "<hubAccount currencyCode=\"" + CurrencyCode + "\" number=\"" + Number + "\" iban=\"" + IBAN + "\" swift=\"" + SWIFT + "\"/>";
            return xml;
        }
    }
}
