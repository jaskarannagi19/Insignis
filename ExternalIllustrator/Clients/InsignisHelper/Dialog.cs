using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.Helper.UI
{
    public class Dialog
    {
        public static string OpenDialog(string pDialogID)
        {
            return "jQuery('#" + pDialogID + "').dialog('open');";
        }

        public static string CloseDialog(string pDialogID)
        {
            return "jQuery(\"#" + pDialogID + "\").dialog(\"close\");";
        }

        public static Literal DrawAnimatedDialogScript(string pDialogID, bool pAutoOpen, string pOpenEffect, string pCloseEffect, bool pIsModal, int? pWidth, int? pHeight)
        {
            return DrawAnimatedDialogScript(pDialogID, pAutoOpen, pOpenEffect, pCloseEffect, pIsModal, pWidth, pHeight, "form1");
        }

        public static Literal DrawAnimatedDialogScript(string pDialogID, bool pAutoOpen, string pOpenEffect, string pCloseEffect, bool pIsModal, int? pWidth, int? pHeight, string pFormName)
        {
            Literal script = new Literal();
            script.Text = "";
            script.Text += "<script type=\"text/javascript\">\r\n";
            script.Text += "jQuery(function(){\r\n";
            script.Text += " jQuery(\"#" + pDialogID + "\").dialog({\r\n";
            if(pFormName!=null)
                script.Text += "  appendTo: '#" + pFormName +"',\r\n";
            script.Text += "  autoOpen: " + ((pAutoOpen == true) ? "true" : "false");
            if (pWidth.HasValue)
            {
                script.Text += ",\r\n";
                script.Text += "  maxWidth: "+ pWidth;
                script.Text += ",\r\n";
                script.Text += "  width: " + pWidth;
            }
            if (pHeight.HasValue)
            {
                script.Text += ",\r\n";
                script.Text += "  maxHeight: " + pHeight;
                script.Text += ",\r\n";
                script.Text += "  height: " + pHeight;
            }
            if (pIsModal)
            {
                script.Text += ",\r\n";
                script.Text += "  modal: true";
                script.Text += ",\r\n";
                script.Text += "  position: { my: 'center', at: 'center', of: window }";
            }
            /*if (pOpenEffect != null && pOpenEffect.Trim().Length > 0)
            {
                script.Text += ",\r\n";
                script.Text += "  show:{\r\n";
                script.Text += "   effect: \"" + pOpenEffect + "\",\r\n";
                script.Text += "   duration: 1000\r\n";
                script.Text += "  }";
            }
            if (pCloseEffect != null && pCloseEffect.Trim().Length > 0)
            {
                script.Text += ",\r\n";
                script.Text += "  hide:{\r\n";
                script.Text += "   effect: \"" + pCloseEffect + "\",\r\n";
                script.Text += "   duration: 1000\r\n";
                script.Text += "  }";
            }*/
            script.Text += "\r\n";
            script.Text += " });\r\n";
            script.Text += "});\r\n";
            script.Text += "</script>\r\n";
            return script;
        }
    }
}