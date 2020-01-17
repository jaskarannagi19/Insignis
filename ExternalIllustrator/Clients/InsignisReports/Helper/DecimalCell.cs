using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class DecimalCell : RightAlignedCell
    {
        public DecimalCell() : base()
        {
        }
        public DecimalCell(string pValue) : base(pValue)
        {
        }
        public DecimalCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "decimalCell");
        }
    }
}
