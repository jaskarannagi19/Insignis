using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ContainerElement : ReportElement
    {
        public List<ReportElement> Elements = new List<ReportElement>();

        public ContainerElement()
        {
        }

        public ContainerElement(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("container") == 0)
                {
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
                    if (pRoot.HasChildNodes)
                    {
                        foreach (XmlElement elementNode in pRoot.ChildNodes)
                        {
                            if (elementNode.Name.ToString().CompareTo("paragraph") == 0)
                                Elements.Add(new Paragraph(elementNode));
                            else if (elementNode.Name.ToString().CompareTo("table") == 0)
                                Elements.Add(new Table(elementNode));
                            else if (elementNode.Name.ToString().CompareTo("subHeading") == 0)
                                Elements.Add(new SubHeading(elementNode));
                            else if (elementNode.Name.ToString().CompareTo("lineBreak") == 0)
                                Elements.Add(new LineBreak());
                            else if (elementNode.Name.ToString().CompareTo("container") == 0)
                                Elements.Add(new ContainerElement(elementNode));
                            else if (elementNode.Name.ToString().CompareTo("image") == 0)
                                Elements.Add(new ImageElement(elementNode));
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

            xml += "<container";
            if (Styles != null && Styles.Count > 0)
            {
                xml += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += style.Key + ":" + style.Value + ";";
                xml += "\"";
            }
            xml += ">";
            foreach (ReportElement element in Elements)
            {
                if (element.GetType() == typeof(Paragraph))
                {
                    Paragraph paragraph = element as Paragraph;
                    xml += paragraph.ToXML();
                }
                else if (element.GetType() == typeof(Table))
                {
                    Table table = element as Table;
                    xml += table.ToXML();
                }
                else if (element.GetType() == typeof(SubHeading))
                {
                    SubHeading subHeading = element as SubHeading;
                    xml += subHeading.ToXML();
                }
                else if (element.GetType() == typeof(LineBreak))
                {
                    LineBreak lineBreak = element as LineBreak;
                    xml += lineBreak.ToXML();
                }
                else if (element.GetType() == typeof(PageBreak))
                {
                    PageBreak pageBreak = element as PageBreak;
                    xml += pageBreak.ToXML();
                }
                else if (element.GetType() == typeof(ContainerElement))
                {
                    ContainerElement containerElement = element as ContainerElement;
                    xml += containerElement.ToXML();
                }
                else if (element.GetType() == typeof(ImageElement))
                {
                    ImageElement imageElement = element as ImageElement;
                    xml += imageElement.ToXML();
                }
            }
            xml += "</container>";
            return xml;
        }
    }
}
