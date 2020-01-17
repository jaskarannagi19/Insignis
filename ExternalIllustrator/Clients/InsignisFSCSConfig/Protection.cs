using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class Protection
    {
        public string CurrencyCode = "";
        public string Value = "";

        public Protection(string pCurrencyCode, string pValue)
        {
            CurrencyCode = pCurrencyCode;
            Value = pValue;
        }

        public Protection(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("protection") == 0)
            {
                foreach (XmlAttribute attribute in pRoot.Attributes)
                {
                    if (attribute.Name.ToString().CompareTo("currencyCode") == 0)
                        CurrencyCode = attribute.Value;
                    else if (attribute.Name.ToString().CompareTo("value") == 0)
                        Value = attribute.Value;
                }
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<protection ";
            xml += "currencyCode=\"" + CurrencyCode + "\" ";
            xml += "value=\"" + Value + "\" ";
            xml += "/>";
            return xml;
        }
    }
}