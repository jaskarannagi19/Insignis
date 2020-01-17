using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class Provider
    {
        public string ShortName = "";
        public string FullName = "";

        public List<CurrencyValue> RegisteredInvestments = new List<CurrencyValue>();

        public Provider(string pShortName, string pFullName)
        {
            ShortName = pShortName;
            FullName = pFullName;
        }

        public Provider(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("provider") == 0)
            {
                foreach (XmlAttribute attribute in pRoot.Attributes)
                {
                    if (attribute.Name.ToString().CompareTo("shortName") == 0)
                        ShortName = attribute.Value;
                    else if (attribute.Name.ToString().CompareTo("fullName") == 0)
                        FullName = attribute.Value.Replace("&amp;","&");
                }
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<provider ";
            xml += "shortName=\"" + ShortName + "\" ";
            xml += "fullName=\"" + FullName.Replace("&","&amp;") + "\" ";
            xml += "/>";
            return xml;
        }

        public void RegisterInvestment(string pCurrencyCode, decimal pInvestmentAmount, bool pIsJointInvestment)
        {
            bool found = false;
            foreach (CurrencyValue registeredInvestment in RegisteredInvestments)
            {
                if (registeredInvestment.CurrencyCode.ToUpper().CompareTo(pCurrencyCode.ToUpper()) == 0)
                {
                    registeredInvestment.Value += pInvestmentAmount;
                    found = true;
                    break;
                }
            }
            if (found == false)
                RegisteredInvestments.Add(new CurrencyValue(pCurrencyCode, pInvestmentAmount, pIsJointInvestment));
        }
    }
}