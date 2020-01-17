using System;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator
{
    public partial class RunTimeError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.UI.Menu menuHelper = new Helper.UI.Menu(Helper.DomainRoot.GetPublic().ToString());
            this.Master.FindControl("ContentHeader").Controls.Add(Helper.UI.Header.HeaderBody(menuHelper.LoggedOutMenu("Default.aspx")));
            this.Master.FindControl("ContentFooterBottom").Controls.Add(Helper.UI.Footer.FooterBottom());

            DateTime dateOfError = DateTime.Now;
            string sourcePage = Request.QueryString["aspxerrorpath"];
            if (sourcePage == null)
                sourcePage = "";
            Literal subject = new Literal();
            subject.Text = "<h4>Run-time-error: " + dateOfError.ToString("dd-MM-yyyy HH:mm:ss") + "</h4>";
            panelDetails.Controls.Add(subject);
            Literal htmlBody = new Literal();
            htmlBody.Text = "<p>A runtime error has been caught on page [" + sourcePage + "]</p>";
            panelDetails.Controls.Add(htmlBody);

        }
    }
}