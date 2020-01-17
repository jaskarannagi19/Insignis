using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class TextCell : Cell
    {
        public TextCell() : base()
        {
        }
        public TextCell(string pValue) : base(pValue)
        {
        }
        public TextCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "textCell");
        }
    }
}
