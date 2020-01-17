using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class Config
    {
        public List<ParentCompany> ParentCompanies = new List<ParentCompany>();

        public Config()
        {
        }

        public Config(string pConfigFile)
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
                foreach (XmlNode InsignisAMNode in doc.ChildNodes)
                {
                    if (InsignisAMNode.Name.ToString().CompareTo("InsignisAM") == 0)
                    {
                        foreach (XmlElement fscsProtectionNode in InsignisAMNode.ChildNodes)
                        {
                            if (fscsProtectionNode.Name.ToString().CompareTo("fscsProtection") == 0)
                            {
                                foreach (XmlElement parentCompanyNode in fscsProtectionNode.ChildNodes)
                                {
                                    if (parentCompanyNode.Name.CompareTo("parentCompany") == 0)
                                        ParentCompanies.Add(new ParentCompany(parentCompanyNode));
                                }
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
            xml += "<InsignisAM>";
            xml += "<fscsProtection>";
            if (ParentCompanies.Count > 0)
            {
                foreach (ParentCompany parentCompany in ParentCompanies)
                    xml += parentCompany.ToXML();
            }
            xml += "</fscsProtection>";
            xml += "</InsignisAM>";
            return xml;
        }

        public void RegisterInvestment(string pCurrencyCode, decimal pInvestmentAmount, string pInstitutionShortName, bool pIsJointInvestment)
        {
            bool found = false;
            foreach (ParentCompany parentCompany in ParentCompanies)
            {
                foreach (BankingLicense bankingLicense in parentCompany.BankingLicenses)
                {
                    foreach (Provider provider in bankingLicense.Providers)
                    {
                        if (provider.ShortName.ToUpper().CompareTo(pInstitutionShortName.ToUpper()) == 0)
                        {
                            provider.RegisterInvestment(pCurrencyCode, pInvestmentAmount, pIsJointInvestment);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
                if (found)
                    break;
            }
        }

        public decimal CalculatePercentageProtected(string pCurrencyCode)
        {
            return CalculatePercentageProtected(pCurrencyCode, false);
        }
        public decimal CalculatePercentageProtected(string pCurrencyCode, bool pIsJointHubAccount)
        {
            decimal percentageProtected = 0;
            foreach (ParentCompany parentCompany in ParentCompanies)
            {
                foreach (BankingLicense bankingLicense in parentCompany.BankingLicenses)
                {
                    foreach (Protection protection in bankingLicense.Protections)
                    {
                        if (protection.CurrencyCode.ToUpper().CompareTo(pCurrencyCode.ToUpper()) == 0)
                        {
                            bankingLicense.TotalInvested = new CurrencyValue(pCurrencyCode, 0, pIsJointHubAccount);
                            foreach (Provider provider in bankingLicense.Providers)
                            {
                                foreach (CurrencyValue investedAmount in provider.RegisteredInvestments)
                                {
                                    if (investedAmount.CurrencyCode.ToUpper().CompareTo(pCurrencyCode.ToUpper()) == 0)
                                        bankingLicense.TotalInvested.Value += investedAmount.Value;
                                }
                            }

                            bankingLicense.PercentageProtected = 0;
                            if(bankingLicense.TotalInvested.Value > 0)
                            {
                                try
                                {
                                    if (protection.Value.Contains("%"))
                                        bankingLicense.PercentageProtected = Convert.ToDecimal(protection.Value.Replace("%", ""));
                                    else
                                    {
                                        if (bankingLicense.TotalInvested.Value <= Convert.ToDecimal(protection.Value))
                                            bankingLicense.PercentageProtected = 100;
                                        else
                                        {
                                            decimal protectionValue = Convert.ToDecimal(protection.Value);
                                            if (pIsJointHubAccount)
                                                protectionValue *= 2;
                                            bankingLicense.PercentageProtected = ((Convert.ToDecimal(protectionValue) / bankingLicense.TotalInvested.Value) * 100);
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }

            decimal sumPercentages = 0;
            int percentageCounter = 0;
            foreach (ParentCompany parentCompany in ParentCompanies)
            {
                foreach (BankingLicense bankingLicense in parentCompany.BankingLicenses)
                {
                    if (bankingLicense.PercentageProtected > 0)
                    {
                        sumPercentages += bankingLicense.PercentageProtected;
                        percentageCounter++;
                    }
                }
            }
            if (percentageCounter > 0)
            {
                decimal averagePercentage = sumPercentages / percentageCounter;
                percentageProtected = averagePercentage;
            }
            return percentageProtected;
        }

        public decimal CalculateAmountProtected(string pCurrencyCode, bool pIsJointHubAccount)
        {
            foreach (ParentCompany parentCompany in ParentCompanies)
            {
                foreach (BankingLicense bankingLicense in parentCompany.BankingLicenses)
                {
                    foreach (Protection protection in bankingLicense.Protections)
                    {
                        if (protection.CurrencyCode.ToUpper().CompareTo(pCurrencyCode.ToUpper()) == 0)
                        {
                            bankingLicense.TotalInvested = new CurrencyValue(pCurrencyCode, 0);
                            foreach (Provider provider in bankingLicense.Providers)
                            {
                                foreach (CurrencyValue investedAmount in provider.RegisteredInvestments)
                                {
                                    if (investedAmount.CurrencyCode.ToUpper().CompareTo(pCurrencyCode.ToUpper()) == 0)
                                        bankingLicense.TotalInvested.Value += investedAmount.Value;
                                }
                            }

                            bankingLicense.PercentageProtected = 0;
                            bankingLicense.AmountProtected = 0;
                            if (bankingLicense.TotalInvested.Value > 0)
                            {
                                try
                                {
                                    if (protection.Value.Contains("%"))
                                    {
                                        bankingLicense.PercentageProtected = Convert.ToDecimal(protection.Value.Replace("%", ""));
                                        bankingLicense.AmountProtected = (bankingLicense.TotalInvested.Value * (bankingLicense.PercentageProtected / 100));
                                    }
                                    else
                                    {
                                        decimal protectionValue = Convert.ToDecimal(protection.Value);
                                        if (pIsJointHubAccount)
                                            protectionValue *= 2;
                                        if (bankingLicense.TotalInvested.Value <= protectionValue)
                                        {
                                            bankingLicense.AmountProtected = bankingLicense.TotalInvested.Value;
                                            bankingLicense.PercentageProtected = 100;
                                        }
                                        else
                                        {
                                            bankingLicense.PercentageProtected = ((protectionValue / bankingLicense.TotalInvested.Value) * 100);
                                            bankingLicense.AmountProtected = protectionValue;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }

            decimal sumAmounts = 0;
            foreach (ParentCompany parentCompany in ParentCompanies)
            {
                foreach (BankingLicense bankingLicense in parentCompany.BankingLicenses)
                {
                    if (bankingLicense.PercentageProtected > 0)
                    {
                        sumAmounts += bankingLicense.AmountProtected;
                    }
                }
            }
            return sumAmounts;
        }
    }
}