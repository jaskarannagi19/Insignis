using System;
using System.Configuration;

namespace Insignis.Asset.Management.External.Illustrator
{
    public partial class HttpError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.UI.Menu menuHelper = new Helper.UI.Menu(Helper.DomainRoot.GetPublic().ToString());
            this.Master.FindControl("ContentHeader").Controls.Add(Helper.UI.Header.HeaderBody(menuHelper.LoggedOutMenu("Default.aspx")));
            this.Master.FindControl("ContentFooterBottom").Controls.Add(Helper.UI.Footer.FooterBottom());

            _bannerTitle.InnerText = "Error " + Request.QueryString["SC"];
            _bodyTitle.InnerText = _bannerTitle.InnerText;

            if (Convert.ToInt32(Request.QueryString["SC"]) == 404)
            {
                _bodyParagraph.InnerText = "The page you are looking for cannot be found";
            }
        }
    }
}