using System;
using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper.Chart
{
    public class Bar
    {
        public string Label = "";

        public List<decimal> Values = new List<decimal>();

        public Bar()
        {
        }

        public Bar(string pLabel)
        {
            Label = pLabel;
        }

        public Bar(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("bar") == 0)
                {
                    if (pRoot.HasAttributes)
                    {
                        foreach (XmlAttribute attribute in pRoot.Attributes)
                        {
                            if (attribute.Name.CompareTo("label") == 0)
                                Label = Escape.FromXML(attribute.Value);
                        }
                    }
                    if (pRoot.HasChildNodes)
                    {
                        foreach (XmlElement child in pRoot.ChildNodes)
                        {
                            if (child.Name.CompareTo("values") == 0)
                            {
                                foreach (XmlElement valueNode in child.ChildNodes)
                                {
                                    if (valueNode.Name.CompareTo("value") == 0)
                                        Values.Add(Convert.ToDecimal(valueNode.InnerText));
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

            xml += "<bar";
            if (Label != null && Label.Trim().Length > 0)
                xml += " label=\"" + Escape.ToXML(Label) + "\"";
            xml += ">";
            if (Values != null && Values.Count > 0)
            {
                xml += "<values>";
                foreach (decimal value in Values)
                    xml += "<value>" + value + "</value>";
                xml += "</values>";
            }
            xml += "</bar>";
            return xml;
        }
    }
}
