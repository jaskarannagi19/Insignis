using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class SubHeading : ReportElement
    {
        public string Title = "";
        public SubHeading()
        {
        }
        public SubHeading(string pTitle)
        {
            Title = pTitle;
        }
        public SubHeading(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("subHeading") == 0)
                    Title = Escape.FromXML(pRoot.InnerText);
            }
            catch
            {
            }
        }
        public string ToXML()
        {
            return "<subHeading>" + Escape.ToXML(Title) + "</subHeading>";
        }
    }
}
