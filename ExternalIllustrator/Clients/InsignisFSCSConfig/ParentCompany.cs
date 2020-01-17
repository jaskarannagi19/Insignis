using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class ParentCompany
    {
        public string ShortName = "";
        public string FullName = "";
        public string CountryOfOrigin = "";

        public List<BankingLicense> BankingLicenses = new List<BankingLicense>();

        public ParentCompany()
        {
        }

        public ParentCompany(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("parentCompany") == 0)
            {
                foreach (XmlAttribute attribute in pRoot.Attributes)
                {
                    if (attribute.Name.ToString().CompareTo("shortName") == 0)
                        ShortName = attribute.Value;
                    else if (attribute.Name.ToString().CompareTo("fullName") == 0)
                        FullName = attribute.Value.Replace("&amp;","&");
                    else if (attribute.Name.ToString().CompareTo("countryOfOrigin") == 0)
                        CountryOfOrigin = attribute.Value;
                }

                foreach (XmlElement bankingLicenseNode in pRoot.ChildNodes)
                {
                    if (bankingLicenseNode.Name.ToString().CompareTo("bankingLicense") == 0)
                        BankingLicenses.Add(new BankingLicense(bankingLicenseNode));
                }

                if (ShortName.CompareTo("AnexecutiveagencyoftheChancelloroftheExchequer") == 0)     // hard coding !!!!
                {
                    foreach (BankingLicense bankingLicense in BankingLicenses)
                    {
                        foreach (Provider provider in bankingLicense.Providers)
                        {
                            if (provider.ShortName.CompareTo("NationalSavingsInvestments") == 0)
                            {
                                foreach (Protection protection in bankingLicense.Protections)
                                    protection.Value = "1000000";                                   // NS&I protects up to 1,000,000
                                break;
                            }
                        }
                    }
                }
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<parentCompany ";
            xml += "shortName=\"" + ShortName + "\" ";
            xml += "fullName=\"" + FullName.Replace("&","&amp;") + "\" ";
            xml += "countryOfOrigin=\"" + CountryOfOrigin + "\" ";
            xml += ">";
            if (BankingLicenses.Count > 0)
            {
                foreach (BankingLicense bankingLicense in BankingLicenses)
                    xml += bankingLicense.ToXML();
            }
            xml += "</parentCompany>";
            return xml;
        }
    }
}