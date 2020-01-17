using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class RightAlignedCell : Cell
    {
        public RightAlignedCell() : base()
        {
        }
        public RightAlignedCell(string pValue) : base(pValue)
        {
        }
        public RightAlignedCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "rightAlignedCell");
        }
    }
}
