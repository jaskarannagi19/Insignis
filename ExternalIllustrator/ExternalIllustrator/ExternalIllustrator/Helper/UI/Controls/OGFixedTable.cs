using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI.Controls
{
    public class OGFixedTable
    {
        public static Literal Render(System.Web.UI.WebControls.Table pTable)
        {
            Literal html = new Literal();

            // start row
            html.Text = "<div class=\"row\">";

            // start left column
            html.Text += "<div class=\"span2\">";
            html.Text += DrawLeftTable(pTable);
            html.Text += "</div>";
            // stop left column

            // start right column
            html.Text += "<div class=\"span10\" style=\"overflow: auto;\">";
            html.Text += DrawRightTable(pTable);
            html.Text += "</div>";
            // stop right column

            // stop row
            html.Text += "</div>";
            return html;
        }

        private static string DrawLeftTable(System.Web.UI.WebControls.Table pTable)
        {
            string html = "";
            // start the table
            html += "<table>";

            // draw the table header
            html += "<thead>";
            // draw the header row
            html += "<tr class=\"og-fixed-table-head-tr\">";
            html += "<th class=\"og-fixed-table-head-first-th\">&nbsp;</th>";
            html += "</tr>";
            html += "</thead>";

            // draw the table body
            html += "<tbody>";
            int rowIndex = 1;
            for (; rowIndex < pTable.Rows.Count; rowIndex++)
            {
                System.Web.UI.WebControls.TableCell cell = pTable.Rows[rowIndex].Cells[0] as System.Web.UI.WebControls.TableCell;
                html += "<tr>";
                if (cell.HasControls() == false)
                    html += "<td class=\"og-fixed-table-head-first-th\">" + cell.Text + "</td>";
                else
                {
                    html += "<td class=\"og-fixed-table-head-first-th\">";
                    foreach (System.Web.UI.WebControls.WebControl ctrl in cell.Controls)
                    {
                        if (ctrl.GetType() == typeof(CheckBox))
                        {
                            CheckBox checkBox = ctrl as CheckBox;
                            html += "<input type=\"checkbox\" name=\"ctl00$ContentBody$" + checkBox.ID + "\" id=\"ContentBody_" + checkBox.ID + "\"";
                            if (checkBox.Checked)
                                html += " checked";
                            html += "/>";
                        }
                        else if (ctrl.GetType() == typeof(Label))
                        {
                            Label label = ctrl as Label;
                            html += label.Text;
                        }
                    }
                    html += "</td>";
                }
                html += "</tr>";
            }
            html += "</tbody>";

            // end the table
            html += "</table>";
            return html;
        }

        private static string DrawRightTable(System.Web.UI.WebControls.Table pTable)
        {
            string html = "";
            // start the table
            html += "<table>";

            // draw the table header
            html += "<thead>";
            // draw the header row
            html += "<tr class=\"og-fixed-table-head-tr\">";
            int rowIndex = 0;
            for (; rowIndex < pTable.Rows.Count;)
            {
                int cellIndex = 1;
                for (; cellIndex < pTable.Rows[rowIndex].Cells.Count; cellIndex++)
                {
                    System.Web.UI.WebControls.TableHeaderCell cell = pTable.Rows[rowIndex].Cells[cellIndex] as System.Web.UI.WebControls.TableHeaderCell;

                    html += "<th class=\"og-fixed-table-head-th\"><p class=\"og-fixed-table-head-rotated\">" + cell.Text + "</p></th>";
                }
                break;
            }
            html += "</tr>";
            html += "</thead>";

            // draw the table body
            html += "<tbody>";
            rowIndex++;
            for (; rowIndex < pTable.Rows.Count; rowIndex++)
            {
                html += "<tr class=\"og-fixed-table-body-tr\">";
                int cellIndex = 1;
                for (; cellIndex < pTable.Rows[rowIndex].Cells.Count; cellIndex++)
                {
                    System.Web.UI.WebControls.TableCell cell = pTable.Rows[rowIndex].Cells[cellIndex] as System.Web.UI.WebControls.TableCell;
                    if (cell.Controls.Count > 0)
                    {
                        if (cell.Controls[0].GetType() == typeof(HyperLink))
                        {
                            HyperLink link = cell.Controls[0] as HyperLink;
                            string ahref = "<a";
                            if (link.NavigateUrl != null && link.NavigateUrl.Trim().Length > 0)
                                ahref += " href=\"" + link.NavigateUrl + "\"";
                            if (link.ToolTip != null && link.ToolTip.Trim().Length > 0)
                                ahref += " title=\"" + link.ToolTip + "\"";
                            ahref += " onclick=\"" + link.Attributes["onclick"] + "\"";
                            ahref += ">";
                            ahref += link.Text;
                            ahref += "</a>";

                            html += "<td class=\"og-fixed-table-body-td\"";
                            if (cell.Style.Count > 0)
                            {
                                html += " style=\"";
                                foreach (string key in cell.Style.Keys)
                                {
                                    html += key;
                                    html += ":";
                                    html += cell.Style[key];
                                    html += ";";
                                }
                                html += "\"";
                            }
                            html += ">" + ahref + "</td>";
                        }
                    }
                    else
                    {
                        html += "<td class=\"og-fixed-table-body-td\"";
                        if (cell.Style.Count > 0)
                        {
                            html += " style=\"";
                            foreach (string key in cell.Style.Keys)
                            {
                                html += key;
                                html += ":";
                                html += cell.Style[key];
                                html += ";";
                            }
                            html += "\"";
                        }
                        html += ">" + cell.Text + "</td>";
                    }
                }
                html += "</tr>";
            }
            html += "</tbody>";

            // end the table
            html += "</table>";
            return html;
        }
    }
}