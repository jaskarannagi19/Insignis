using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class FieldDefinition
    {
        public string DisplayName = "";
        public string Name = "";
        public bool IncludeInFilter = false;
        public bool IsVisible = true;
        public string FromRange = "";
        public string ToRange = "";
        public string Equals = "";
        public string Like = "";
        public string Convert = "";
        public string In = "";
        public bool Not = false;

        public FieldDefinition(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.ToString().CompareTo("fieldDefinition") == 0)
                {
                    foreach (XmlAttribute attribute in pRoot.Attributes)
                    {
                        if (attribute.Name.ToString().CompareTo("displayName") == 0)
                            DisplayName = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("name") == 0)
                            Name = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("includeInFilter") == 0)
                            IncludeInFilter = ((attribute.Value.ToLower().CompareTo("true")==0) ? true : false);
                        else if (attribute.Name.ToString().CompareTo("isVisible") == 0)
                            IsVisible = ((attribute.Value.ToLower().CompareTo("true") == 0) ? true : false);
                        else if (attribute.Name.ToString().CompareTo("fromRange") == 0)
                            FromRange = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("toRange") == 0)
                            ToRange = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("equals") == 0)
                            Equals = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("like") == 0)
                            Like = attribute.Value.ToLower();
                        else if (attribute.Name.ToString().CompareTo("convert") == 0)
                            Convert = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("in") == 0)
                            In = attribute.Value.Replace("&amp;","&");
                        else if (attribute.Name.ToString().CompareTo("not") == 0)
                            Not = ((attribute.Value.ToLower().CompareTo("true") == 0) ? true : false);
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
            xml += "<fieldDefinition ";
            xml += "displayName=\"" + DisplayName.Replace("&","&amp;") + "\" ";
            xml += "name=\"" + Name + "\" ";
            xml += "includeInFilter=\"" + ((IncludeInFilter == true) ? "true" : "false") + "\" ";
            xml += "isVisible=\"" + ((IsVisible == true) ? "true" : "false") + "\" ";
            xml += "fromRange=\"" + FromRange + "\" ";
            xml += "toRange=\"" + ToRange + "\" ";
            xml += "equals=\"" + Equals + "\" ";
            xml += "like=\"" + Like + "\" ";
            if(In.Contains("&")==true && In.Contains("&amp;")==false)
                xml += "in=\"" + In.Replace("&", "&amp;") + "\" ";
            else
                xml += "in=\"" + In + "\" ";
            xml += "not=\"" + ((Not == true) ? "true" : "false") + "\" ";
            if (Convert.Trim().Length > 0)
                xml += "convert=\"" + Convert + "\" ";
            xml += "/>";
            return xml;
        }

        public bool WithinInClause(string pLookFor)
        {
            bool isWithin = false;

            if (In.Trim().Length > 0)
            {
                if (In.Contains(","))
                {
                    string separator = ",";
                    string[] items = In.Split(separator.ToCharArray());
                    foreach (string item in items)
                    {
                        if (item.CompareTo(pLookFor) == 0)
                        {
                            isWithin = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (In.CompareTo(pLookFor) == 0)
                        isWithin = true;
                }
            }
            return isWithin;
        }
    }
}
