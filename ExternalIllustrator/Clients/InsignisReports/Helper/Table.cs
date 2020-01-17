using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class Table : ReportElement
    {
        public List<BaseRow> Rows = new List<BaseRow>();
        public List<KeyValuePair<string, string>> Styles = new List<KeyValuePair<string, string>>();

        public Table()
        {
        }
        public Table(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("table") == 0)
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
                        else if (child.Name.ToString().CompareTo("rows") == 0)
                        {
                            foreach (XmlElement rowNode in child.ChildNodes)
                            {
                                if (rowNode.Name.CompareTo("row") == 0)
                                    Rows.Add(new Row(rowNode));
                                else if (rowNode.Name.CompareTo("transparentRow") == 0)
                                    Rows.Add(new TransparentRow(rowNode));
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
            string xml = "<table>";
            if (Styles != null && Styles.Count > 0)
            {
                xml += "<styles>";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += "<style key=\"" + style.Key + "\" value=\"" + style.Value + "\"/>";
                xml += "</styles>";
            }
            if (Rows != null && Rows.Count > 0)
            {
                xml += "<rows>";
                foreach (BaseRow loopRow in Rows)
                {
                    if (loopRow.GetType() == typeof(Row))
                    {
                        Row row = loopRow as Row;
                        xml += row.ToXML();
                    }
                    else if (loopRow.GetType() == typeof(TransparentRow))
                    {
                        TransparentRow transparentRow = loopRow as TransparentRow;
                        xml += transparentRow.ToXML();
                    }
                }
                xml += "</rows>";
            }
            xml += "</table>";
            return xml;
        }
        public string ToHTML()
        {
            string html = "<table";
            if (Styles != null && Styles.Count > 0)
            {
                html += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    html += style.Key + ":" + style.Value + ";";
                html += "\"";
            }
            html += ">";
            if (Rows != null && Rows.Count > 0)
            {
                foreach (BaseRow loopRow in Rows)
                {
                    if (loopRow.GetType() == typeof(Row))
                    {
                        Row row = loopRow as Row;
                        html += row.ToHTML();
                    }
                    else if (loopRow.GetType() == typeof(TransparentRow))
                    {
                        TransparentRow transparentRow = loopRow as TransparentRow;
                        html += transparentRow.ToHTML();
                    }
                }
            }
            html += "</table>";
            return html;
        }
    }
}
