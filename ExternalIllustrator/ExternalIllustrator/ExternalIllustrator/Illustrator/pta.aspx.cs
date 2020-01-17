using System;
using System.Configuration;

namespace Insignis.Asset.Management.External.Illustrator.Illustrator
{
    public partial class pta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Response.Redirect(ConfigurationManager.AppSettings["publicRoot"]);
            }
            else
            {
                Helper.UI.Form formHelper = new Helper.UI.Form();
                if (formHelper.GetInput("_partnerOrganisation", Request).Trim().Length > 0)
                    Session["_partnerOrganisation"] = formHelper.GetInput("_partnerOrganisation", Request);
                else if (Session["_partnerOrganisation"] == null)
                    Session["_partnerOrganisation"] = "";

                if (formHelper.GetInput("_partnerName", Request).Trim().Length > 0)
                    Session["_partnerName"] = formHelper.GetInput("_partnerName", Request);
                else if (Session["_partnerName"] == null)
                    Session["_partnerName"] = "";

                if (formHelper.GetInput("_partnerEmailAddress", Request).Trim().Length > 0)
                    Session["_partnerEmailAddress"] = formHelper.GetInput("_partnerEmailAddress", Request);
                else if (Session["_partnerEmailAddress"] == null)
                    Session["_partnerEmailAddress"] = "";

                if (formHelper.GetInput("_partnerTelephone", Request).Trim().Length > 0)
                    Session["_partnerTelephone"] = formHelper.GetInput("_partnerTelephone", Request);
                else if (Session["_partnerTelephone"] == null)
                    Session["_partnerTelephone"] = "";

                if (Session["_partnerOrganisation"] != null && Session["_partnerOrganisation"].ToString().Length > 0 &&
                   Session["_partnerName"] != null && Session["_partnerName"].ToString().Length > 0 &&
                   Session["_partnerEmailAddress"] != null && Session["_partnerEmailAddress"].ToString().Length > 0 &&
                   Session["_partnerTelephone"] != null && Session["_partnerTelephone"].ToString().Length > 0)
                {
                    Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()),false);
                    Octavo.Gate.Nabu.Preferences.Preference scurveBuilder = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder", 1, "Settings");
                    if (scurveBuilder == null)
                        Response.Redirect("SCurveAvailableTo.aspx?reset=true");
                    else
                        Response.Redirect("SCurveAvailableTo.aspx");
                }
                else
                    Response.Redirect(ConfigurationManager.AppSettings["publicRoot"]);
            }
        }
    }
}
