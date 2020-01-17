using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper.Chart
{
    public class HorizontalBarChart : BaseChart
    {
        public string Title = "";

        public List<string> Legends = new List<string>();
        public List<string> BarColours = new List<string>();
        public List<Bar> Bars = new List<Bar>();

        public HorizontalBarChart()
        {
        }

        public HorizontalBarChart(string pTitle)
        {
            Title = pTitle;
        }

        public HorizontalBarChart(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("horizontalBarChart") == 0)
                {
                    if (pRoot.HasAttributes)
                    {
                        foreach (XmlAttribute attribute in pRoot.Attributes)
                        {
                            if (attribute.Name.CompareTo("title") == 0)
                                Title = Escape.FromXML(attribute.Value);
                        }
                    }
                    if (pRoot.HasChildNodes)
                    {
                        foreach (XmlElement child in pRoot.ChildNodes)
                        {
                            if (child.Name.CompareTo("colours") == 0)
                            {
                                foreach (XmlElement colourNode in child.ChildNodes)
                                {
                                    if (colourNode.Name.CompareTo("colour") == 0)
                                        BarColours.Add(colourNode.InnerText);
                                }
                            }
                            else if (child.Name.CompareTo("legends") == 0)
                            {
                                foreach (XmlElement legendNode in child.ChildNodes)
                                {
                                    if (legendNode.Name.CompareTo("legend") == 0)
                                        Legends.Add(legendNode.InnerText);
                                }
                            }
                            else if (child.Name.CompareTo("bars") == 0)
                            {
                                foreach (XmlElement barNode in child.ChildNodes)
                                {
                                    if (barNode.Name.CompareTo("bar") == 0)
                                        Bars.Add(new Bar(barNode));
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

            xml += "<horizontalBarChart";
            if (Title != null && Title.Trim().Length > 0)
                xml += " title=\"" + Escape.ToXML(Title) +  "\"";
            xml += ">";
            if (BarColours != null && BarColours.Count > 0)
            {
                xml += "<colours>";
                foreach (string barColour in BarColours)
                    xml += "<colour>" + barColour + "</colour>";
                xml += "</colours>";
            }
            if (Legends != null && Legends.Count > 0)
            {
                xml += "<legends>";
                foreach (string legend in Legends)
                    xml += "<legend>" + legend + "</legend>";
                xml += "</legends>";
            }
            if (Bars != null && Bars.Count > 0)
            {
                xml += "<bars>";
                foreach (Bar bar in Bars)
                    xml += bar.ToXML();
                xml += "</bars>";
            }
            xml += "</horizontalBarChart>";
            return xml;
        }
    }
}
