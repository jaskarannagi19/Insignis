using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class IntegerCell : RightAlignedCell
    {
        public IntegerCell() : base()
        {
        }
        public IntegerCell(string pValue) : base(pValue)
        {
        }
        public IntegerCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "integerCell");
        }
    }
}
