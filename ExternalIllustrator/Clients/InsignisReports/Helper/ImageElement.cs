using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ImageElement : ReportElement
    {
        public string PhysicalSource = "";

        public ImageElement()
        {
        }
        public ImageElement(string pPhysicalSource)
        {
            PhysicalSource = pPhysicalSource;
        }
        public ImageElement(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("image") == 0)
                {
                    if (pRoot.HasAttributes)
                    {
                        foreach (XmlAttribute attribute in pRoot.Attributes)
                        {
                            if (attribute.Name.CompareTo("source") == 0)
                                PhysicalSource = attribute.Value;
                            else if (attribute.Name.CompareTo("style") == 0)
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

            xml += "<image";
            xml += " source=\"" + PhysicalSource + "\"";
            if (Styles != null && Styles.Count > 0)
            {
                xml += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += style.Key + ":" + style.Value + ";";
                xml += "\"";
            }
            xml += "/>";
            return xml;
        }
    }
}
