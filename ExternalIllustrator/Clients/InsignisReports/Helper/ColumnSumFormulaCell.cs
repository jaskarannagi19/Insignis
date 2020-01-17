using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ColumnSumFormulaCell : RightAlignedCell
    {
        public ColumnSumFormulaCell() : base()
        {
        }
        public ColumnSumFormulaCell(string pValue) : base(pValue)
        {
        }
        public ColumnSumFormulaCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "columnSumFormulaCell");
        }
    }
}
