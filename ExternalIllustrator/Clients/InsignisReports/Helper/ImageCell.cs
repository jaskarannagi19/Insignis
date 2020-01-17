using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ImageCell : Cell
    {
        public ImageElement image = null;
        public ImageCell() : base()
        {
            Value = null;
        }
        public ImageCell(XmlElement pRoot)
        {
            Value = null;
            try
            {
                if (pRoot.Name.CompareTo("imageCell") == 0)
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
                        else if (child.Name.ToString().CompareTo("image") == 0)
                            image = new ImageElement(child);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
