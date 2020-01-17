using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class CorporateType
    {
        public string Name = "";

        public CorporateType(string pName)
        {
            Name = pName;
        }

        public CorporateType(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.ToString().CompareTo("corporateType") == 0)
                {
                    foreach (XmlAttribute attribute in pRoot.Attributes)
                    {
                        if (attribute.Name.ToString().CompareTo("name") == 0)
                            Name = attribute.Value.Replace("&amp;","&");
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
            xml += "<corporateType ";
            xml += "name=\"" + Name.Replace("&","&amp;") + "\" ";
            xml += "/>";
            return xml;
        }
    }
}
