using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ListCell : Cell
    {
        public string Alignment = "left";
        public List<ReportElement> Items = new List<ReportElement>();
        public ListCell() : base()
        {
            Value = null;
        }
        public ListCell(string pAlignment) : base()
        {
            Value = null;
            Alignment = pAlignment;
        }
        public ListCell(XmlElement pRoot)
        {
            Value = null;
            try
            {
                if (pRoot.Name.CompareTo("listCell") == 0)
                {
                    if (pRoot.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute attribute in pRoot.Attributes)
                        {
                            if (attribute.Name.CompareTo("alignment") == 0)
                                Alignment = attribute.Value;
                        }
                    }
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
                        else if (child.Name.ToString().CompareTo("items") == 0)
                        {
                            foreach (XmlElement itemNode in child.ChildNodes)
                            {
                                if (itemNode.Name.ToString().CompareTo("paragraph") == 0)
                                    Items.Add(new Paragraph(itemNode));
                            }
                        }
                        else if (child.Name.ToString().CompareTo("value") == 0)
                            this.Value = child.InnerText;
                    }
                }
            }
            catch
            {
            }
        }

        public string OutputAsXML()
        {
            string xml = "";
            string elementName = "listCell";

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
            if (Items != null && Items.Count > 0)
            {
                xml += "<items>";
                foreach (ReportElement item in Items)
                {
                    if (item.GetType() == typeof(Paragraph))
                    {
                        Paragraph paragraph = item as Paragraph;
                        xml += paragraph.ToXML();
                    }
                }
                xml += "</items>";
            }
            if (Value != null && Value.Length > 0)
                xml += "<value>" + Escape.ToXML(Value) + "</value>";
            xml += "</" + elementName + ">";
            return xml;
        }
        public string OutputAsHTML()
        {
            string html = "";
            html += "<td";
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
            if (Items != null && Items.Count > 0)
            {
                foreach (ReportElement item in Items)
                {
                    if (item.GetType() == typeof(Paragraph))
                    {
                        Paragraph paragraph = item as Paragraph;
                        html += paragraph.ToHTML();
                    }
                }
            }
            if (Value != null && Value.Length > 0)
                html += Escape.ToXML(Value);
            html += "</th>";
            return html;
        }
    }
}
