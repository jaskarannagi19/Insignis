using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class PoolingAccountConfig
    {
        public List<PoolingAccountBranch> poolingAccountBranches = new List<PoolingAccountBranch>();

        public PoolingAccountConfig()
        {
        }

        public PoolingAccountConfig(string pConfigFile)
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
                    if (hubAccountDepositsNode.Name.ToString().CompareTo("poolingAccountConfig") == 0)
                    {
                        foreach (XmlElement hubAccountBranchNode in hubAccountDepositsNode.ChildNodes)
                        {
                            if (hubAccountBranchNode.Name.ToString().CompareTo("poolingAccountBranch") == 0)
                            {
                                PoolingAccountBranch branch = new PoolingAccountBranch();
                                foreach (XmlAttribute attribute in hubAccountBranchNode.Attributes)
                                {
                                    if (attribute.Name.CompareTo("branchID") == 0)
                                        branch.branchID = Convert.ToInt32(attribute.Value);
                                    else if (attribute.Name.CompareTo("bankName") == 0)
                                        branch.bankName = attribute.Value;
                                    else if (attribute.Name.CompareTo("institutionID") == 0)
                                        branch.institutionID = Convert.ToInt32(attribute.Value);
                                }

                                if (hubAccountBranchNode.HasChildNodes)
                                {
                                    foreach (XmlElement hubAccountNode in hubAccountBranchNode.ChildNodes)
                                    {
                                        if (hubAccountNode.Name.ToString().CompareTo("poolingAccountDefinition") == 0)
                                        {
                                            PoolingAccountDefinition poolingAccountDefinition = new PoolingAccountDefinition();
                                            foreach (XmlAttribute accountAttribute in hubAccountNode.Attributes)
                                            {
                                                if (accountAttribute.Name.CompareTo("currencyCode") == 0)
                                                    poolingAccountDefinition.CurrencyCode = accountAttribute.Value;
                                                else if (accountAttribute.Name.CompareTo("number") == 0)
                                                    poolingAccountDefinition.Number = accountAttribute.Value;
                                                else if (accountAttribute.Name.CompareTo("paccount") == 0)
                                                    poolingAccountDefinition.PAccount = accountAttribute.Value;
                                                else if (accountAttribute.Name.CompareTo("iban") == 0)
                                                    poolingAccountDefinition.IBAN = accountAttribute.Value;
                                                else if (accountAttribute.Name.CompareTo("swift") == 0)
                                                    poolingAccountDefinition.SWIFT = accountAttribute.Value;
                                                else if (accountAttribute.Name.CompareTo("subnumber") == 0)
                                                    poolingAccountDefinition.SUBNUMBER = accountAttribute.Value;
                                            }
                                            branch.poolingAccountDefinitions.Add(poolingAccountDefinition);
                                        }
                                    }
                                }
                                poolingAccountBranches.Add(branch);
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
            xml += "<poolingAccountConfig>";
            if (poolingAccountBranches.Count > 0)
            {
                foreach (PoolingAccountBranch branch in poolingAccountBranches)
                    xml += branch.ToXML();
            }
            xml += "</poolingAccountConfig>";
            return xml;
        }

        public string GetBankNameByBranch(int pBranchID)
        {
            string bankName = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.branchID == pBranchID)
                {
                    bankName = branch.bankName;
                    break;
                }
            }

            return bankName;
        }

        public string GetAccountNumberByBranch(int pBranchID, string pCurrencyCode)
        {
            string accountNumber = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.branchID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            accountNumber = poolingAccountDefinition.Number;
                            break;
                        }
                    }
                    break;
                }
            }

            return accountNumber;
        }

        public string GetPAccountByBranch(int pBranchID, string pCurrencyCode)
        {
            string paccountNumber = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.branchID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            paccountNumber = poolingAccountDefinition.PAccount;
                            break;
                        }
                    }
                    break;
                }
            }

            return paccountNumber;
        }

        public string GetIBANByBranch(int pBranchID, string pCurrencyCode)
        {
            string iban = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.branchID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            iban = poolingAccountDefinition.IBAN;
                            break;
                        }
                    }
                    break;
                }
            }

            return iban;
        }

        public string GetSWIFTByBranch(int pBranchID, string pCurrencyCode)
        {
            string swift = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.branchID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            swift = poolingAccountDefinition.SWIFT;
                            break;
                        }
                    }
                    break;
                }
            }

            return swift;
        }

        public string GetBankNameByInstitution(int pBranchID)
        {
            string bankName = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.institutionID == pBranchID)
                {
                    bankName = branch.bankName;
                    break;
                }
            }

            return bankName;
        }

        public string GetAccountNumberByInstitution(int pBranchID, string pCurrencyCode)
        {
            string accountNumber = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.institutionID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            accountNumber = poolingAccountDefinition.Number;
                            break;
                        }
                    }
                    break;
                }
            }

            return accountNumber;
        }

        public string GetIBANByInstitution(int pBranchID, string pCurrencyCode)
        {
            string iban = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.institutionID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            iban = poolingAccountDefinition.IBAN;
                            break;
                        }
                    }
                    break;
                }
            }

            return iban;
        }

        public string GetSWIFTByInstitution(int pBranchID, string pCurrencyCode)
        {
            string swift = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.institutionID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            swift = poolingAccountDefinition.SWIFT;
                            break;
                        }
                    }
                    break;
                }
            }

            return swift;
        }
        public string GetSUBNUMBERByInstitution(int pBranchID, string pCurrencyCode)
        {
            string subnumber = "";

            foreach (PoolingAccountBranch branch in poolingAccountBranches)
            {
                if (branch.institutionID == pBranchID)
                {
                    foreach (PoolingAccountDefinition poolingAccountDefinition in branch.poolingAccountDefinitions)
                    {
                        if (poolingAccountDefinition.CurrencyCode.CompareTo(pCurrencyCode) == 0)
                        {
                            subnumber = poolingAccountDefinition.SUBNUMBER;
                            break;
                        }
                    }
                    break;
                }
            }

            return subnumber;
        }
    }

    public class PoolingAccountBranch
    {
        public int? branchID = null;
        public List<PoolingAccountDefinition> poolingAccountDefinitions = new List<PoolingAccountDefinition>();
        public string bankName = "";
        public int? institutionID = null;

        public string ToXML()
        {
            string xml = "";
            xml += "<poolingAccountBranch branchID=\"" + branchID + "\" bankName=\"" + bankName + "\" institutionID=\"" + institutionID + "\">";
            if (poolingAccountDefinitions.Count > 0)
            {
                foreach (PoolingAccountDefinition poolingAccountDefinition in poolingAccountDefinitions)
                    xml += poolingAccountDefinition.ToXML();
            }
            xml += "</poolingAccountBranch>";
            return xml;
        }
    }

    public class PoolingAccountDefinition
    {
        public string CurrencyCode = "";
        public string PAccount = "";
        public string Number = "";
        public string IBAN = "";
        public string SWIFT = "";
        public string SUBNUMBER = "";

        public string ToXML()
        {
            string xml = "";
            xml += "<poolingAccountDefinition currencyCode=\"" + CurrencyCode + "\" number=\"" + Number + "\" paccount=\"" + PAccount + "\" iban=\"" + IBAN + "\" swift=\"" + SWIFT + "\" subnumber=\"" + SUBNUMBER + "\"/>";
            return xml;
        }
    }
}
