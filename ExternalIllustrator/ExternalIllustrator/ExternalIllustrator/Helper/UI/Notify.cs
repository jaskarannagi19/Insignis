using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Notify
    {
        private HtmlGenericControl panelNotify = null;
        public bool ErrorDetected = false;
        public bool WarningDeteceted = false;

        public Notify()
        {
        }

        public Notify(HtmlGenericControl pPanelNotify)
        {
            panelNotify = pPanelNotify;
        }
        
        public void InformationPanel(string pMessage)
        {
            if(panelNotify!=null)
            {
                if (panelNotify.Controls.Count == 0)
                {
                    Literal informationPanel = new Literal();
                    informationPanel.Text = "<div class=\"alert info\"><i class=\"ifc-info\"></i>" + pMessage + "</div>";
                    panelNotify.Controls.Add(informationPanel);
                    panelNotify.Style.Remove("display");
                    panelNotify.Style.Add("display", "block");
                }
            }
        }

        public void Information(string pMessage)
        {
            if (panelNotify != null)
            {
                if (panelNotify.Controls.Count == 0)
                {
                    Literal informationPanel = new Literal();
                    informationPanel.Text = "<div class=\"alert info\"><i class=\"ifc-info\"></i>" + pMessage + "</div>";
                    panelNotify.Controls.Add(informationPanel);
                    panelNotify.Style.Remove("display");
                    panelNotify.Style.Add("display", "block");
                }
            }
        }

        public void Success(string pMessage)
        {
            if (panelNotify != null)
            {
                if (panelNotify.Controls.Count == 0)
                {
                    Literal informationPanel = new Literal();
                    informationPanel.Text = "<div class=\"alert success\"><i class=\"ifc-checkmark\"></i>" + pMessage + "</div>";
                    panelNotify.Controls.Add(informationPanel);
                    panelNotify.Style.Remove("display");
                    panelNotify.Style.Add("display", "block");
                }
            }
        }

        public void Warning(string pMessage)
        {
            if (panelNotify != null)
            {
                Literal informationPanel = new Literal();
                informationPanel.Text = "<div class=\"alert warning\"><i class=\"ifc-error\"></i>" + pMessage + "</div>";
                panelNotify.Controls.Add(informationPanel);
                panelNotify.Style.Remove("display");
                panelNotify.Style.Add("display", "block");
                WarningDeteceted = true;
            }
        }

        public void Error(string pMessage)
        {
            if (panelNotify != null)
            {
                Literal informationPanel = new Literal();
                informationPanel.Text = "<div class=\"alert error\"><i class=\"ifc-close\"></i>" + pMessage + "</div>";
                panelNotify.Controls.Add(informationPanel);
                panelNotify.Style.Remove("display");
                panelNotify.Style.Add("display", "block");
                ErrorDetected = true;
            }
        }

        public void Default(string pMessage)
        {
            if (panelNotify != null)
            {
                Literal informationPanel = new Literal();
                informationPanel.Text = "<div class=\"alert\">" + pMessage + "</div>";
                panelNotify.Controls.Add(informationPanel);
                panelNotify.Style.Remove("display");
                panelNotify.Style.Add("display", "block");
            }
        }
    }
}