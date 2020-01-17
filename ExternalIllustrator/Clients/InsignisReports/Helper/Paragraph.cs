using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class Paragraph : ReportElement
    {
        public string Text = "";
        public Paragraph()
        {
        }
        public Paragraph(string pText)
        {
            Text = pText;
        }
        public Paragraph(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("paragraph") == 0)
                {
                    Text = Escape.FromXML(pRoot.InnerText);
                    if (pRoot.HasAttributes)
                    {
                        foreach (XmlAttribute attribute in pRoot.Attributes)
                        {
                            if (attribute.Name.CompareTo("style") == 0)
                            {
                                string semiColonSeparator = ";";
                                string colonSeparator = ":";

                                string[] styles = attribute.Value.Split(semiColonSeparator.ToCharArray());
                                foreach (string style in styles)
                                {
                                    string[] parts = style.Split(colonSeparator.ToCharArray());
                                    if (parts.Length == 2)
                                    {
                                        Styles.Add(new KeyValuePair<string, string>(parts[0], parts[1]));
                                    }
                                }
                            }
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
            string xml = "";

            xml += "<paragraph";
            if (Styles != null && Styles.Count > 0)
            {
                xml += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += style.Key + ":" + style.Value + ";";
                xml += "\"";
            }
            xml += ">" + Escape.ToXML(Text) + "</paragraph>";
            return xml;
        }
        public string ToHTML()
        {
            string html = "";

            html += "<p";
            if (Styles != null && Styles.Count > 0)
            {
                html += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    html += style.Key + ":" + style.Value + ";";
                html += "\"";
            }
            html += ">" + Escape.ToXML(Text) + "</p>";
            return html;
        }
    }
}
