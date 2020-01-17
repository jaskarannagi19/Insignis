using System;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class FeeMatrixItem
    {
        public decimal? From = null;
        public decimal? To = null;
        public decimal? Rate = null;

        public FeeMatrixItem()
        {
        }

        public FeeMatrixItem(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("feeMatrixItem") == 0)
            {
                foreach (XmlAttribute attribute in pRoot.Attributes)
                {
                    if (attribute.Name.ToString().CompareTo("from") == 0)
                    {
                        if (attribute.Value.Trim().Length > 0)
                        {
                            try
                            {
                                From = Convert.ToDecimal(attribute.Value);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if (attribute.Name.ToString().CompareTo("to") == 0)
                    {
                        if (attribute.Value.Trim().Length > 0)
                        {
                            try
                            {
                                To = Convert.ToDecimal(attribute.Value);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if (attribute.Name.ToString().CompareTo("rate") == 0)
                    {
                        if (attribute.Value.Trim().Length > 0)
                        {
                            try
                            {
                                Rate = Convert.ToDecimal(attribute.Value);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<feeMatrixItem ";
            xml += "from=\"" + From + "\" ";
            xml += "to=\"" + To + "\" ";
            xml += "rate=\"" + Rate + "\" ";
            xml += "/>";
            return xml;
        }
    }
}