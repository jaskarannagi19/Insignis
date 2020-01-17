using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class BaseExcludeInstitution
    {
        public string Name = "";

        public BaseExcludeInstitution(string pName)
        {
            Name = pName;
        }

        public BaseExcludeInstitution(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.ToString().CompareTo("excludeInstitution") == 0)
                {
                    foreach (XmlAttribute attribute in pRoot.Attributes)
                    {
                        if (attribute.Name.ToString().CompareTo("name") == 0)
                        {
                            Name = attribute.Value;
                            Name = Name.Replace("&amp;", "&");
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
            xml += "<excludeInstitution ";
            if (Name.Contains("&"))
            {
                if(Name.Contains("&amp;"))
                    xml += "name=\"" + Name + "\" ";
                else
                    xml += "name=\"" + Name.Replace("&", "&amp;") + "\" ";
            }
            else
                xml += "name=\"" + Name + "\" ";
            xml += "/>";
            return xml;
        }
    }
}
