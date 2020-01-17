using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class HeaderCell : Cell
    {
        public string Alignment = "center";

        public HeaderCell() : base()
        {
        }

        public HeaderCell(string pAlignment) : base()
        {
            Alignment = pAlignment;
        }
        public HeaderCell(string pValue, string pAlignment) : base(pValue)
        {
        }
        public HeaderCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "headerCell");
            if (pRoot.Attributes.Count > 0)
            {
                foreach (XmlAttribute attribute in pRoot.Attributes)
                {
                    if (attribute.Name.CompareTo("alignment") == 0)
                        Alignment = attribute.Value;
                }
            }
        }
        public string OutputAsXML()
        {
            string xml = "";
            string elementName = "headerCell";

            xml += "<" + elementName;
            if (Alignment != null && Alignment.Trim().Length > 0)
                xml += " alignment=\"" + Alignment + "\"";
            xml += ">";
            if (Styles != null && Styles.Count > 0)
            {
                xml += "<styles>";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += "<style key=\"" + style.Key + "\" value=\"" + style.Value + "\"/>";
                xml += "</styles>";
            }
            if (Value != null && Value.Length > 0)
                xml += "<value>" + Escape.ToXML(Value) + "</value>";
            xml += "</" + elementName + ">";
            return xml;
        }
        public string OutputAsHTML()
        {
            string html = "";

            html += "<th";
            if (Styles != null && Styles.Count > 0)
            {
                html += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    html += style.Key + ":" + style.Value + ";";
                if (Alignment != null && Alignment.Trim().Length > 0)
                    html += "text-align:" + Alignment + ";";
                html += "\"";
            }
            html += ">";
            if (Value != null && Value.Length > 0)
                html += Escape.ToXML(Value);
            html += "</th>";
            return html;
        }
    }
}
