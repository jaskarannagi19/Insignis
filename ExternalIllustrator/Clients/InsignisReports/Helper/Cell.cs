using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class Cell
    {
        public string Value = "";
        public List<KeyValuePair<string, string>> Styles = new List<KeyValuePair<string, string>>();

        public Cell()
        {
        }
        public Cell(string pValue)
        {
            Value = pValue;
        }
        public Cell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "cell");
        }
        public void FromXMLElement(XmlElement pRoot,string pElementName)
        {
            try
            {
                if (pRoot.Name.CompareTo(pElementName) == 0)
                {
                    foreach (XmlElement child in pRoot.ChildNodes)
                    {
                        if (child.Name.ToString().CompareTo("styles") == 0)
                        {
                            foreach (XmlElement styleNode in child.ChildNodes)
                            {
                                if (styleNode.Name.CompareTo("style") == 0)
                                {
                                    string key = "";
                                    string value = "";
                                    foreach (XmlAttribute attribute in styleNode.Attributes)
                                    {
                                        if (attribute.Name.CompareTo("key") == 0)
                                            key = attribute.Value;
                                        else if (attribute.Name.CompareTo("value") == 0)
                                            value = attribute.Value;
                                    }
                                    if (key != null && key.Trim().Length > 0)
                                        Styles.Add(new KeyValuePair<string, string>(key, value));
                                }
                            }
                        }
                        else if (child.Name.ToString().CompareTo("value") == 0)
                        {
                            Value = Escape.FromXML(child.InnerText);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public string ToXML()
        {
            return ToXML(null);
        }
        public string ToXML(string pElementName)
        {
            return ToXML(pElementName, null);
        }
        public string ToXML(string pElementName, string pInnerElement)
        {
            string xml = "";
            string elementName = "cell";
            if (pElementName != null && pElementName.Length > 0)
                elementName = pElementName;

            xml += "<" + elementName + ">";
            if (Styles != null && Styles.Count > 0)
            {
                xml += "<styles>";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += "<style key=\"" + style.Key + "\" value=\"" + style.Value + "\"/>";
                xml += "</styles>";
            }
            if (Value != null && Value.Length > 0)
                xml += "<value>" + Escape.ToXML(Value) + "</value>";
            if (pInnerElement != null && pInnerElement.Trim().Length > 0)
                xml += pInnerElement;
            xml += "</" + elementName + ">";
            return xml;
        }

        public string ToHTML()
        {
            return ToHTML(null);
        }
        public string ToHTML(string pElementName)
        {
            return ToHTML(pElementName, null);
        }
        public string ToHTML(string pElementName, string pInnerElement)
        {
            string html = "";
            html += "<td";
            if (Styles != null && Styles.Count > 0)
            {
                html += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    html += style.Key + ":" + style.Value + ";";
                html += "\"";
            }
            html += ">";
            if (Value != null && Value.Length > 0)
                html += Escape.ToXML(Value);
            if (pInnerElement != null && pInnerElement.Trim().Length > 0)
                html += pInnerElement;
            html += "</td>";
            return html;
        }
    }
}
