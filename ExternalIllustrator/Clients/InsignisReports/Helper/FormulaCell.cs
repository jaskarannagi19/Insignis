using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class FormulaCell : RightAlignedCell
    {
        public string Formula = "";
        public FormulaCell() : base()
        {
        }
        public FormulaCell(string pValue, string pFormula) : base(pValue)
        {
            Formula = pFormula;
        }
        public FormulaCell(XmlElement pRoot)
        {
            FromXMLElement(pRoot, "formulaCell");
            try
            {
                if (pRoot.Name.CompareTo("formulaCell") == 0)
                {
                    foreach (XmlElement child in pRoot.ChildNodes)
                    {
                        if (child.Name.ToString().CompareTo("formula") == 0)
                        {
                            Formula = Escape.FromXML(child.InnerText);
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
