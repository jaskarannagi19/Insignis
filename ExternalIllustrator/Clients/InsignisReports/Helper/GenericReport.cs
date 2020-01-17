using System;
using System.Collections.Generic;
using System.Xml;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class GenericReport
    {
        public bool RenderHeader = true;
        public string Title = "";

        public List<KeyValuePair<string, string>> Styles = new List<KeyValuePair<string, string>>();
        public List<ReportElement> HeaderElements = new List<ReportElement>();
        public List<ReportElement> Elements = new List<ReportElement>();
        public List<ReportElement> FooterElements = new List<ReportElement>();

        public GenericReport()
        {
        }

        public GenericReport(string pTitle)
        {
            Title = pTitle;
        }

        public void SerialiseToDisk(string pXmlFileName)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(pXmlFileName))
                {
                    writer.Write(ToXML());
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception exc)
            {
            }
        }

        private string ToXML()
        {
            string xml = "<?xml version=\"1.0\"?>";
            xml += "<genericReport renderHeader=\"" + ((RenderHeader==true)?"true":"false") + "\">";
            if(Title != null && Title.Trim().Length > 0)
                xml += "<title>" + Escape.ToXML(Title) + "</title>";
            if (Styles.Count > 0)
            {
                xml += "<styles>";
                foreach (KeyValuePair<string, string> style in Styles)
                    xml += "<style name=\"" + style.Key + "\" value=\"" + style.Value + "\"/>";
                xml += "</styles>";
            }
            if (HeaderElements != null && HeaderElements.Count > 0)
            {
                xml += "<header>";
                xml += ElementsToXML(HeaderElements);
                xml += "</header>";
            }
            xml += "<elements>";
            xml += ElementsToXML(Elements);
            xml += "</elements>";
            if (FooterElements != null && FooterElements.Count > 0)
            {
                xml += "<footer>";
                xml += ElementsToXML(FooterElements);
                xml += "</footer>";
            }
            xml += "</genericReport>";
            return xml;
        }

        private string ElementsToXML(List<ReportElement> pElements)
        {
            string xml = "";
            foreach (ReportElement element in pElements)
            {
                if (element.GetType() == typeof(Paragraph))
                {
                    Paragraph paragraph = element as Paragraph;
                    xml += paragraph.ToXML();
                }
                else if (element.GetType() == typeof(Chart.HorizontalBarChart))
                {
                    Chart.HorizontalBarChart horizontalBarChart = element as Chart.HorizontalBarChart;
                    xml += horizontalBarChart.ToXML();
                }
                else if (element.GetType() == typeof(Table))
                {
                    Table table = element as Table;
                    xml += table.ToXML();
                }
                else if (element.GetType() == typeof(SubHeading))
                {
                    SubHeading subHeading = element as SubHeading;
                    xml += subHeading.ToXML();
                }
                else if (element.GetType() == typeof(LineBreak))
                {
                    LineBreak lineBreak = element as LineBreak;
                    xml += lineBreak.ToXML();
                }
                else if (element.GetType() == typeof(PageBreak))
                {
                    PageBreak pageBreak = element as PageBreak;
                    xml += pageBreak.ToXML();
                }
                else if (element.GetType() == typeof(ContainerElement))
                {
                    ContainerElement containerElement = element as ContainerElement;
                    xml += containerElement.ToXML();
                }
                else if (element.GetType() == typeof(ImageElement))
                {
                    ImageElement imageElement = element as ImageElement;
                    xml += imageElement.ToXML();
                }
            }
            return xml;
        }

        public static GenericReport DeserialiseFromDisk(string pXmlFileName)
        {
            GenericReport genericReport = null;
            try
            {
                if (System.IO.File.Exists(pXmlFileName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(pXmlFileName);
                    foreach (XmlNode genericReportNode in doc.ChildNodes)
                    {
                        if (genericReportNode.Name.ToString().CompareTo("genericReport") == 0)
                        {
                            genericReport = new GenericReport();
                            foreach (XmlAttribute attribute in genericReportNode.Attributes)
                            {
                                if (attribute.Name.CompareTo("renderHeader") == 0)
                                    genericReport.RenderHeader = ((attribute.Value.ToLower().CompareTo("true") == 0) ? true : false);
                            }
                            foreach (XmlElement childNode in genericReportNode.ChildNodes)
                            {
                                if (childNode.Name.ToString().CompareTo("title") == 0)
                                    genericReport.Title = Escape.FromXML(childNode.InnerText);
                                else if (childNode.Name.ToString().CompareTo("styles") == 0)
                                {
                                    foreach (XmlElement styleNode in childNode.ChildNodes)
                                    {
                                        string name = "";
                                        string value = "";
                                        foreach (XmlAttribute styleAttribtue in styleNode.Attributes)
                                        {
                                            if (styleAttribtue.Name.CompareTo("name") == 0)
                                                name = styleAttribtue.Value;
                                            else if (styleAttribtue.Name.CompareTo("value") == 0)
                                                value = styleAttribtue.Value;
                                        }
                                        genericReport.Styles.Add(new KeyValuePair<string, string>(name, value));
                                    }
                                }
                                else if (childNode.Name.ToString().CompareTo("header") == 0)
                                    LoadElements(genericReport.HeaderElements, childNode);
                                else if (childNode.Name.ToString().CompareTo("elements") == 0)
                                    LoadElements(genericReport.Elements, childNode);
                                else if (childNode.Name.ToString().CompareTo("footer") == 0)
                                    LoadElements(genericReport.FooterElements, childNode);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
            }
            return genericReport;
        }

        private static void LoadElements(List<ReportElement> pElements, XmlElement pRootNode)
        {
            foreach (XmlElement elementNode in pRootNode.ChildNodes)
            {
                if (elementNode.Name.ToString().CompareTo("horizontalBarChart") == 0)
                    pElements.Add(new Chart.HorizontalBarChart(elementNode));
                else if (elementNode.Name.ToString().CompareTo("paragraph") == 0)
                    pElements.Add(new Paragraph(elementNode));
                else if (elementNode.Name.ToString().CompareTo("table") == 0)
                    pElements.Add(new Table(elementNode));
                else if (elementNode.Name.ToString().CompareTo("subHeading") == 0)
                    pElements.Add(new SubHeading(elementNode));
                else if (elementNode.Name.ToString().CompareTo("lineBreak") == 0)
                    pElements.Add(new LineBreak());
                else if (elementNode.Name.ToString().CompareTo("pageBreak") == 0)
                    pElements.Add(new PageBreak());
                else if (elementNode.Name.ToString().CompareTo("container") == 0)
                    pElements.Add(new ContainerElement(elementNode));
                else if (elementNode.Name.ToString().CompareTo("image") == 0)
                    pElements.Add(new ImageElement(elementNode));
            }
        }

        public static GenericReport Convertor(System.Web.UI.WebControls.Table pSourceTable, string pReportName)
        {
            GenericReport genericReport = new GenericReport(pReportName);
            try
            {
                Reports.Helper.Table genericReportTable = new Reports.Helper.Table();

                foreach (System.Web.UI.WebControls.TableRow sourceRow in pSourceTable.Rows)
                {
                    if (sourceRow.TableSection == System.Web.UI.WebControls.TableRowSection.TableHeader)
                    {
                        Reports.Helper.Row headerRow = new Reports.Helper.Row();
                        foreach (System.Web.UI.WebControls.TableCell sourceCell in sourceRow.Cells)
                        {
                            if (sourceCell.Text != null && sourceCell.Controls.Count == 0)
                                headerRow.Cells.Add(new Reports.Helper.TextCell(sourceCell.Text.Replace("&pound;","").Replace("$","").Replace("&euro;","").Replace(",", "").Trim()));
                            else if (sourceCell.Controls.Count > 0)
                            {
                            }
                            else
                                headerRow.Cells.Add(new Reports.Helper.TextCell(""));
                        }
                        genericReportTable.Rows.Add(headerRow);
                    }
                    else
                    {
                        Reports.Helper.Row bodyRow = new Reports.Helper.Row();
                        foreach (System.Web.UI.WebControls.TableCell sourceCell in sourceRow.Cells)
                        {
                            if (sourceCell.Text != null && sourceCell.Controls.Count == 0)
                                bodyRow.Cells.Add(new Reports.Helper.TextCell(sourceCell.Text.Replace("&pound;", "").Replace("$", "").Replace("&euro;", "").Replace(",","").Trim()));
                            else if (sourceCell.Controls.Count > 0)
                            {
                                try
                                {
                                    foreach (System.Web.UI.HtmlControls.HtmlGenericControl htmlGenericControl in sourceCell.Controls)
                                    {
                                        if (htmlGenericControl.TagName.ToLower().CompareTo("p") == 0)
                                        {
                                            if (htmlGenericControl.InnerText != null && htmlGenericControl.InnerText.Length > 0)
                                                bodyRow.Cells.Add(new Reports.Helper.TextCell(htmlGenericControl.InnerText.Replace("&pound;", "").Replace("$", "").Replace("&euro;", "").Replace(",", "").Trim()));
                                            else if (htmlGenericControl.InnerHtml != null && htmlGenericControl.InnerHtml.Length > 0)
                                                bodyRow.Cells.Add(new Reports.Helper.TextCell(htmlGenericControl.InnerHtml.Replace("&pound;", "").Replace("$", "").Replace("&euro;", "").Replace(",", "").Trim()));
                                        }
                                        else if (htmlGenericControl.TagName.ToLower().CompareTo("div") == 0)
                                        {
                                            foreach (System.Web.UI.HtmlControls.HtmlGenericControl htmlChildControl in htmlGenericControl.Controls)
                                            {
                                                if (htmlChildControl.TagName.ToLower().CompareTo("p") == 0)
                                                {
                                                    if (htmlChildControl.InnerText != null && htmlChildControl.InnerText.Length > 0)
                                                        bodyRow.Cells.Add(new Reports.Helper.TextCell(htmlChildControl.InnerText.Replace("&pound;", "").Replace("$", "").Replace("&euro;", "").Replace(",", "").Trim()));
                                                    else if (htmlChildControl.InnerHtml != null && htmlChildControl.InnerHtml.Length > 0)
                                                        bodyRow.Cells.Add(new Reports.Helper.TextCell(htmlChildControl.InnerHtml.Replace("&pound;", "").Replace("$", "").Replace("&euro;", "").Replace(",", "").Trim()));
                                                }
                                                else
                                                {
                                                }
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                                bodyRow.Cells.Add(new Reports.Helper.TextCell(""));
                        }
                        genericReportTable.Rows.Add(bodyRow);
                    }
                }
                genericReport.Elements.Add(genericReportTable);
            }
            catch
            {
            }
            return genericReport;
        }
    }
}
