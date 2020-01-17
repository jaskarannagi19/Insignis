using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.Helper.UI
{
    public class Theme
    {
        public static Literal DrawLineBreak()
        {
            return DrawLiteral("<br />");
        }

        public static Literal DrawLiteral(string pText)
        {
            Literal literal = new Literal();
            literal.Text = pText;
            return literal;
        }

        public static Label DrawLabel(string pText)
        {
            Label label = new Label();
            label.Text = pText;
            return label;
        }

        public static HtmlGenericControl DrawParagraph(string pText)
        {
            HtmlGenericControl p = new HtmlGenericControl("p");
            p.InnerHtml = pText;
            return p;
        }

        public static Panel DrawHeadLine2(string pTitle)
        {
            Panel container = new Panel();
            container.CssClass = "headline-2";
            container.Controls.Add(DrawLiteral("<h4 style=\"color:black;\">" + pTitle + "</h4>"));
            return container;
        }
        public static Panel DrawHeadLine2(string pTitle, string pStyle)
        {
            Panel container = new Panel();
            container.CssClass = "headline-2";
            container.Controls.Add(DrawLiteral("<h4 style=\"color:black;"+ pStyle +"\">" + pTitle + "</h4>"));
            return container;
        }
        public static Panel DrawHeadLine2(string pTitle, int pHeight)
        {
            Panel container = new Panel();
            container.CssClass = "headline-2";
            container.Style.Add("height", pHeight.ToString() + "px");
            container.Controls.Add(DrawLiteral("<h4 style=\"color:black;\">" + pTitle + "</h4>"));
            return container;
        }

        public static Literal RenderTableAsHtmlLiteral(Table pTable)
        {
            Literal html = new Literal();
            html.Text += "<table";
            html.Text += RenderCssClass(pTable.CssClass);
            html.Text += ">";
            bool first = true;
            for (int rowIndex = 0; rowIndex < pTable.Rows.Count; rowIndex++)
            {
                if (pTable.Rows[rowIndex].TableSection == TableRowSection.TableHeader)
                    html.Text += "<thead>";
                else if (first)
                {
                    html.Text += "<tbody>";
                    first = false;
                }
                html.Text += "<tr";
                html.Text += RenderCssClass(pTable.Rows[rowIndex].CssClass);
                html.Text += RenderStyles(pTable.Rows[rowIndex].Style);
                html.Text += ">";

                foreach (TableCell cell in pTable.Rows[rowIndex].Cells)
                {
                    html.Text += "<";
                    if (cell.GetType() == typeof(TableHeaderCell))
                        html.Text += "th";
                    else
                        html.Text += "td";

                    html.Text += RenderCssClass(cell.CssClass);
                    html.Text += RenderStyles(cell.Style);
                    html.Text += ">";

                    if (cell.Text.Length > 0)
                        html.Text += cell.Text;
                    else
                        html.Text += RenderControls(cell.Controls);

                    html.Text += "</";
                    if (cell.GetType() == typeof(TableHeaderCell))
                        html.Text += "th";
                    else
                        html.Text += "td";
                    html.Text += ">";
                }

                html.Text += "</tr>";
                if (pTable.Rows[rowIndex].TableSection == TableRowSection.TableHeader)
                    html.Text += "</thead>";
            }
            if (first == false)
                html.Text += "</tbody>";

            html.Text += "</table>";

            return html;
        }

        private static string RenderCssClass(string pClassName)
        {
            string html = "";
            if (pClassName != null && pClassName.Trim().Length > 0)
                html += " class=\"" + pClassName + "\"";
            return html;
        }

        private static string RenderStyles(System.Web.UI.CssStyleCollection pStyles)
        {
            string html = "";
            if (pStyles.Count > 0)
            {
                html += " style=\"";
                foreach (string key in pStyles.Keys)
                {
                    if (key.Length > 0)
                    {
                        html += key;
                        html += ":";
                        html += pStyles[key];
                        html += ";";
                    }
                }
                html += "\"";
            }
            return html;
        }

        private static string RenderAttributes(System.Web.UI.AttributeCollection pAttributes)
        {
            string html = "";
            if (pAttributes.Count > 0)
            {
                foreach (string key in pAttributes.Keys)
                {
                    if (key.Length > 0)
                    {
                        if (key.ToLower().CompareTo("style") != 0 && key.ToLower().CompareTo("innerhtml") != 0 && key.ToLower().CompareTo("innertext") != 0)
                        {
                            html += " ";
                            html += key;
                            html += "=\"";
                            html += pAttributes[key];
                            html += "\"";
                        }
                    }
                }
            }
            return html;
        }

        private static string RenderControl(TextBox pTextBox)
        {
            string html = "";
            html += "<input type=\"text\"";
            if (pTextBox.ID.Length > 0)
            {
                html += " id=\"ContentBody_" + pTextBox.ID + "\"";
                html += " name=\"ctl00$ContentBody$" + pTextBox.ID + "\"";
            }
            html += RenderStyles(pTextBox.Style);
            html += RenderAttributes(pTextBox.Attributes);
            html += "/>";
            return html;
        }
        private static string RenderControl(DropDownList pDropDownList)
        {
            string html = "";
            html += "<select";
            if (pDropDownList.ID.Length > 0)
            {
                html += " id=\"ContentBody_" + pDropDownList.ID + "\"";
                html += " name=\"ctl00$ContentBody$" + pDropDownList.ID + "\"";
            }
            html += RenderStyles(pDropDownList.Style);
            html += RenderAttributes(pDropDownList.Attributes);
            html += ">";
            foreach (ListItem listItem in pDropDownList.Items)
            {
                html += "<option";
                if (listItem.Selected)
                    html += " selected";
                if (listItem.Value != null && listItem.Value.Trim().Length > 0)
                    html += " value=\"" + listItem.Value + "\"";
                html += ">";
                html += listItem.Text;
                html += "</option>";
            }
            html += "</select>";
            return html;
        }
        private static string RenderControl(HiddenField pHidden)
        {
            string html = "";
            html += "<input type=\"hidden\"";
            if (pHidden.ID.Length > 0)
            {
                html += " id=\"ContentBody_" + pHidden.ID + "\"";
                html += " name=\"ctl00$ContentBody$" + pHidden.ID + "\"";
            }
            if (pHidden.Value.Length > 0)
                html += " value=\"" + pHidden.Value + "\"";
            html += "/>";
            return html;
        }
        private static string RenderControl(System.Web.UI.LiteralControl pLiteral)
        {
            string html = "";
            html += pLiteral.Text;
            return html;
        }
        private static string RenderControl(Literal pLiteral)
        {
            string html = "";
            html += pLiteral.Text;
            return html;
        }
        public static string RenderControl(HtmlGenericControl pGenericControl)
        {
            string html = "";
            html += "<" + pGenericControl.TagName;
            html += RenderStyles(pGenericControl.Style);
            html += RenderAttributes(pGenericControl.Attributes);
            html += ">";
            if (pGenericControl.Controls.Count > 0)
                html += RenderControls(pGenericControl.Controls);
            else
            {
                if (pGenericControl.InnerText != null && pGenericControl.InnerText.Trim().Length > 0)
                    html += pGenericControl.InnerText;
                else if (pGenericControl.InnerHtml != null && pGenericControl.InnerHtml.Trim().Length > 0)
                    html += pGenericControl.InnerHtml;
            }
            html += "</" + pGenericControl.TagName + ">";
            return html;
        }

        private static string RenderControls(System.Web.UI.ControlCollection pControls)
        {
            string html = "";
            foreach (System.Web.UI.Control ctrl in pControls)
            {
                if (ctrl.GetType() == typeof(HtmlGenericControl))
                    html += RenderControl(ctrl as HtmlGenericControl);
                else if (ctrl.GetType() == typeof(TextBox))
                    html += RenderControl(ctrl as TextBox);
                else if (ctrl.GetType() == typeof(HiddenField))
                    html += RenderControl(ctrl as HiddenField);
                else if (ctrl.GetType() == typeof(DropDownList))
                    html += RenderControl(ctrl as DropDownList);
                else if (ctrl.GetType() == typeof(System.Web.UI.LiteralControl))
                    html += RenderControl(ctrl as System.Web.UI.LiteralControl);
                else if (ctrl.GetType() == typeof(Literal))
                    html += RenderControl(ctrl as Literal);
            }
            return html;
        }
    }
}