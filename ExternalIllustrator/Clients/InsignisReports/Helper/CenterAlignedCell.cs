using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class CenterAlignedCell : Cell
    {
        public CenterAlignedCell() : base()
        {
        }
        public CenterAlignedCell(string pValue) : base(pValue)
        {
        }
        public CenterAlignedCell(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("centerAlignedCell") == 0)
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
                        else if (child.Name.ToString().CompareTo("value") == 0)
                        {
                            Value = Escape.FromXML(child.InnerText);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
