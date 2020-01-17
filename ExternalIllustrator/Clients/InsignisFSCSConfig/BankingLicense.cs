using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class BankingLicense
    {
        public List<Note> Notes = new List<Note>();
        public List<Protection> Protections = new List<Protection>();
        public List<Provider> Providers = new List<Provider>();

        public CurrencyValue TotalInvested = new CurrencyValue();
        public decimal PercentageProtected = 0;
        public decimal AmountProtected = 0;

        public BankingLicense()
        {
        }

        public BankingLicense(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("bankingLicense") == 0)
            {
                foreach (XmlElement childNode in pRoot.ChildNodes)
                {
                    if (childNode.Name.ToString().CompareTo("protection") == 0)
                        Protections.Add(new Protection(childNode));
                    else if (childNode.Name.ToString().CompareTo("note") == 0)
                        Notes.Add(new Note(childNode));
                    else if (childNode.Name.ToString().CompareTo("provider") == 0)
                        Providers.Add(new Provider(childNode));
                }
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<bankingLicense>";
            if (Notes.Count > 0)
            {
                foreach (Note note in Notes)
                    xml += note.ToXML();
            }
            if (Protections.Count > 0)
            {
                foreach (Protection protection in Protections)
                    xml += protection.ToXML();
            }
            if (Providers.Count > 0)
            {
                foreach (Provider provider in Providers)
                    xml += provider.ToXML();
            }
            xml += "</bankingLicense>";
            return xml;
        }
    }
}