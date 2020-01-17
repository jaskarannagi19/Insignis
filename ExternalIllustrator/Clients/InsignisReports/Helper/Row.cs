using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class Row : BaseRow
    {
        public Row()
        {
        }

        public Row(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.CompareTo("row") == 0)
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
                        else if (child.Name.ToString().CompareTo("cells") == 0)
                        {
                            foreach (XmlElement cellNode in child.ChildNodes)
                            {
                                if (cellNode.Name.CompareTo("cell") == 0)
                                    Cells.Add(new Cell(cellNode));
                                else if (cellNode.Name.CompareTo("rightAlignedCell") == 0)
                                    Cells.Add(new RightAlignedCell(cellNode));
                                else if (cellNode.Name.CompareTo("centerAlignedCell") == 0)
                                    Cells.Add(new CenterAlignedCell(cellNode));
                                else if (cellNode.Name.CompareTo("textCell") == 0)
                                    Cells.Add(new TextCell(cellNode));
                                else if (cellNode.Name.CompareTo("integerCell") == 0)
                                    Cells.Add(new IntegerCell(cellNode));
                                else if (cellNode.Name.CompareTo("decimalCell") == 0)
                                    Cells.Add(new DecimalCell(cellNode));
                                else if (cellNode.Name.CompareTo("formulaCell") == 0)
                                    Cells.Add(new FormulaCell(cellNode));
                                else if (cellNode.Name.CompareTo("columnSumFormulaCell") == 0)
                                    Cells.Add(new ColumnSumFormulaCell(cellNode));
                                else if (cellNode.Name.CompareTo("headerCell") == 0)
                                    Cells.Add(new HeaderCell(cellNode));
                                else if (cellNode.Name.CompareTo("imageCell") == 0)
                                    Cells.Add(new ImageCell(cellNode));
                                else if (cellNode.Name.CompareTo("listCell") == 0)
                                    Cells.Add(new ListCell(cellNode));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public string ToXML()
        {
            string xml = "<row>";
            if (Styles != null && Styles.Count > 0)
            {
                xml += "<styles>";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += "<style key=\"" + style.Key + "\" value=\"" + style.Value + "\"/>";
                xml += "</styles>";
            }
            if (Cells != null && Cells.Count > 0)
            {
                xml += "<cells>";
                foreach (Cell cell in Cells)
                {
                    if (cell.GetType() == typeof(CenterAlignedCell))
                        xml += cell.ToXML("centerAlignedCell");
                    else if (cell.GetType() == typeof(RightAlignedCell))
                        xml += cell.ToXML("rightAlignedCell");
                    else if (cell.GetType() == typeof(TextCell))
                        xml += cell.ToXML("textCell");
                    else if (cell.GetType() == typeof(IntegerCell))
                        xml += cell.ToXML("integerCell");
                    else if (cell.GetType() == typeof(DecimalCell))
                        xml += cell.ToXML("decimalCell");
                    else if (cell.GetType() == typeof(FormulaCell))
                        xml += cell.ToXML("formulaCell");
                    else if (cell.GetType() == typeof(ColumnSumFormulaCell))
                        xml += cell.ToXML("columnSumFormulaCell");
                    else if (cell.GetType() == typeof(ImageCell))
                    {
                        ImageCell imageCell = cell as ImageCell;
                        if(imageCell.image != null)
                            xml += cell.ToXML("imageCell", imageCell.image.ToXML());
                        else
                            xml += cell.ToXML("imageCell", "");
                    }
                    else if (cell.GetType() == typeof(ListCell))
                    {
                        ListCell listCell = cell as ListCell;
                        xml += listCell.OutputAsHTML();
                    }
                    else if (cell.GetType() == typeof(HeaderCell))
                    {
                        HeaderCell headerCell = cell as HeaderCell;
                        xml += headerCell.OutputAsHTML();
                    }
                    else
                        xml += cell.ToXML();
                }
                xml += "</cells>";
            }
            xml += "</row>";
            return xml;
        }

        public string ToHTML()
        {
            string html = "<tr";
            if (Styles != null && Styles.Count > 0)
            {
                html += " style=\"";
                foreach (KeyValuePair<string, string> style in Styles)
                    html += style.Key + ":" + style.Value + ";";
                html += "\"";
            }
            html += ">";
            if (Cells != null && Cells.Count > 0)
            {
                foreach (Cell cell in Cells)
                {
                    if (cell.GetType() == typeof(CenterAlignedCell))
                        html += cell.ToHTML("centerAlignedCell");
                    else if (cell.GetType() == typeof(RightAlignedCell))
                        html += cell.ToHTML("rightAlignedCell");
                    else if (cell.GetType() == typeof(TextCell))
                        html += cell.ToHTML("textCell");
                    else if (cell.GetType() == typeof(IntegerCell))
                        html += cell.ToHTML("integerCell");
                    else if (cell.GetType() == typeof(DecimalCell))
                        html += cell.ToHTML("decimalCell");
                    else if (cell.GetType() == typeof(FormulaCell))
                        html += cell.ToHTML("formulaCell");
                    else if (cell.GetType() == typeof(ColumnSumFormulaCell))
                        html += cell.ToHTML("columnSumFormulaCell");
                    else if (cell.GetType() == typeof(ImageCell))
                    {
                        ImageCell imageCell = cell as ImageCell;
                        if (imageCell.image != null)
                            html += cell.ToHTML("imageCell", imageCell.image.ToXML());
                        else
                            html += cell.ToHTML("imageCell", "");
                    }
                    else if (cell.GetType() == typeof(ListCell))
                    {
                        ListCell listCell = cell as ListCell;
                        html += listCell.OutputAsXML();
                    }
                    else if (cell.GetType() == typeof(HeaderCell))
                    {
                        HeaderCell headerCell = cell as HeaderCell;
                        html += headerCell.OutputAsXML();
                    }
                    else
                        html += cell.ToHTML();
                }
            }
            html += "</tr>";
            return html;
        }
    }
}
