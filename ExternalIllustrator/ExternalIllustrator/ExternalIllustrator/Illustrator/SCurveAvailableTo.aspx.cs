using Insignis.Asset.Management.Clients.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

namespace Insignis.Asset.Management.External.Illustrator.Illustrator
{
    public partial class SCurveAvailableTo : System.Web.UI.Page
    {
        private Helper.MultiLingual multiLingual;

        private bool HideCalculateButton = false;

        private CoreAbstraction coreAbstraction = new CoreAbstraction(ConfigurationManager.ConnectionStrings["Octavo.Gate.Nabu.Data.Source.InsignisAM"].ConnectionString, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings.Get("errorLog"));
        private FinancialAbstraction financialAbstraction = new FinancialAbstraction(ConfigurationManager.ConnectionStrings["Octavo.Gate.Nabu.Data.Source.InsignisAM"].ConnectionString, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings.Get("errorLog"));

        private FeeMatrix feeMatrix = null;

        private List<Literal> animatedDialogScripts = new List<Literal>();
        private List<HtmlGenericControl> animatedDialogBodies = new List<HtmlGenericControl>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper.UI.Notify notifications = new Helper.UI.Notify(_panelNotify);
            if(Session["_partnerOrganisation"] != null && Session["_partnerOrganisation"].ToString().Length > 0 &&
               Session["_partnerName"] != null && Session["_partnerName"].ToString().Length > 0 &&
               Session["_partnerEmailAddress"] != null && Session["_partnerEmailAddress"].ToString().Length > 0 &&
               Session["_partnerTelephone"] != null && Session["_partnerTelephone"].ToString().Length > 0)
            {
                multiLingual = new Helper.MultiLingual("English");

                Helper.UI.Menu menuHelper = new Helper.UI.Menu(Helper.DomainRoot.Get().ToString());
                this.Master.FindControl("ContentHeader").Controls.Add(Helper.UI.Header.HeaderBody(menuHelper.LandingMenu("Landing.aspx")));
                this.Master.FindControl("ContentFooterBottom").Controls.Add(Helper.UI.Footer.FooterBottom());

                string clientConfigRoot = ConfigurationManager.AppSettings["clientConfigRoot"];
                if (clientConfigRoot.EndsWith("\\") == false)
                    clientConfigRoot += "\\";
                feeMatrix = new FeeMatrix(clientConfigRoot + "FeeMatrix.xml");

                bool redirecting = false;
                if (Request.QueryString["reset"] != null && Request.QueryString["reset"].Trim().Length > 0)
                {
                    if (Request.QueryString["reset"].ToLower().CompareTo("true") == 0)
                    {
                        Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                        preferencesManager.DeletePreferences("Sales.Tools.SCurve.Settings", 1);

                        Octavo.Gate.Nabu.Preferences.Preference scurveBuilder = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder", 1, "Settings");
                        int availableToHubAccountTypeID = -1;
                        if (scurveBuilder != null)
                        {
                            if (scurveBuilder.GetChildPreference("AvailableTo") != null && scurveBuilder.GetChildPreference("AvailableTo").Value.Trim().Length > 0)
                            {
                                try
                                {
                                    availableToHubAccountTypeID = Convert.ToInt32(scurveBuilder.GetChildPreference("AvailableTo").Value);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                availableToHubAccountTypeID = financialAbstraction.GetAccountTypeByAlias("ACT_PERSONALHUBACCOUNT", (int)multiLingual.language.LanguageID).AccountTypeID.Value;
                                scurveBuilder.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AvailableTo", availableToHubAccountTypeID.ToString()));
                            }
                        }
                        preferencesManager.DeletePreferences("Sales.Tools.SCurve.Institutions", 1);
                        Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = new Octavo.Gate.Nabu.Preferences.Preference("Institutions", "");
                        Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
                        foreach (Institution institution in allInstitutions)
                        {
                            if (institution.ShortName.CompareTo("NationalSavingsInvestments") != 0)
                                institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
                            else
                                institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
                        }
                        preferencesManager.SetPreference("Sales.Tools.SCurve.Institutions", 1, institutionInclusion);

                        preferencesManager.DeletePreferences("Sales.Tools.SCurve.Properties." + availableToHubAccountTypeID, 1);

                        Octavo.Gate.Nabu.Preferences.Preference scurveBuilderDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                        if (scurveBuilderDeposits != null && scurveBuilderDeposits.Children.Count > 0)
                        {
                            scurveBuilderDeposits.Children.Clear();
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveBuilderDeposits);
                        }

                        redirecting = true;
                        Response.Redirect("SCurveAvailableTo.aspx");
                    }
                }
                else if (Request.QueryString["clear"] != null && Request.QueryString["clear"].Trim().Length > 0)
                {
                    if (Request.QueryString["clear"].ToLower().CompareTo("true") == 0)
                    {
                        Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                        Octavo.Gate.Nabu.Preferences.Preference scurveBuilder = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");
                        int availableToHubAccountTypeID = -1;
                        if (scurveBuilder != null)
                        {
                            if (scurveBuilder.GetChildPreference("AvailableTo") != null && scurveBuilder.GetChildPreference("AvailableTo").Value.Trim().Length > 0)
                            {
                                try
                                {
                                    availableToHubAccountTypeID = Convert.ToInt32(scurveBuilder.GetChildPreference("AvailableTo").Value);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                scurveBuilder.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AvailableTo", financialAbstraction.GetAccountTypeByAlias("ACT_PERSONALHUBACCOUNT", (int)multiLingual.language.LanguageID).AccountTypeID.ToString()));
                            }
                        }

                        Octavo.Gate.Nabu.Preferences.Preference scurveBuilderDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                        if (scurveBuilderDeposits != null && scurveBuilderDeposits.Children.Count > 0)
                        {
                            scurveBuilderDeposits.Children.Clear();
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveBuilderDeposits);
                        }
                        redirecting = true;
                        Response.Redirect("SCurveAvailableTo.aspx?#ContentBody__panelBottomLeft");
                    }
                }
                else if (Request.QueryString["Sort"] != null && Request.QueryString["Sort"].Trim().Length > 0)
                {
                    Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                    Octavo.Gate.Nabu.Preferences.Preference scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");
                    if (scurvePreferences == null || scurvePreferences.Children.Count == 0)
                    {
                        scurvePreferences = new Octavo.Gate.Nabu.Preferences.Preference("Settings", "");
                        scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("SortPortfolioBy", Request.QueryString["Sort"].Trim()));
                        preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);
                    }
                    else if (scurvePreferences != null && scurvePreferences.GetChildPreference("SortPortfolioBy") != null && scurvePreferences.GetChildPreference("SortPortfolioBy").Value != null && scurvePreferences.GetChildPreference("SortPortfolioBy").Value.Trim().Length > 0)
                    {
                        scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("SortPortfolioBy", Request.QueryString["Sort"].Trim()));
                        preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);
                    }
                    redirecting = true;
                    Response.Redirect("SCurveAvailableTo.aspx");
                }
                if (redirecting == false)
                {
                    DisplayPage();
                    // if there are any dialog box bodies then render them to the content body
                    if (animatedDialogBodies.Count > 0)
                    {
                        foreach (HtmlGenericControl animatedDialogBody in animatedDialogBodies)
                        {
                            this.Master.FindControl("ContentBody").Controls.Add(animatedDialogBody);
                        }
                    }
                    // if there are any dialog box scripts then render them to the content footer
                    if (animatedDialogScripts.Count > 0)
                    {
                        foreach (Literal animatedDialogScripts in animatedDialogScripts)
                        {
                            this.Master.FindControl("ContentFooterBottom").Controls.Add(animatedDialogScripts);
                        }
                    }
                }
            }
            else
                Response.Redirect(Helper.DomainRoot.Get() + "LoggedOut/Landing.aspx");
        }

        private void DisplayPage()
        {
            Insignis.Asset.Management.Tools.Sales.SCurve scurve = new Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

            int availableToHubAccountTypeID = -1;
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
            Octavo.Gate.Nabu.Preferences.Preference scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");
            if (scurvePreferences != null)
            {
                if (scurvePreferences.GetChildPreference("AvailableTo") != null && scurvePreferences.GetChildPreference("AvailableTo").Value.Trim().Length > 0)
                {
                    try
                    {
                        availableToHubAccountTypeID = Convert.ToInt32(scurvePreferences.GetChildPreference("AvailableTo").Value);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    availableToHubAccountTypeID = financialAbstraction.GetAccountTypeByAlias("ACT_PERSONALHUBACCOUNT", (int)multiLingual.language.LanguageID).AccountTypeID.Value;
                }
            }

            string currencyCode = "GBP";
            if (scurvePreferences != null)
            {
                if (scurvePreferences.GetChildPreference("CurrencyCode") != null && scurvePreferences.GetChildPreference("CurrencyCode").Value.Trim().Length > 0)
                    currencyCode = scurvePreferences.GetChildPreference("CurrencyCode").Value;
            }

            bool includePooledProducts = true;
            if (scurvePreferences != null)
            {
                if (scurvePreferences.GetChildPreference("IncludePooledProducts") != null && scurvePreferences.GetChildPreference("IncludePooledProducts").Value.Trim().Length > 0)
                    includePooledProducts = ((scurvePreferences.GetChildPreference("IncludePooledProducts").Value.ToLower().CompareTo("true") == 0) ? true : false);
            }

            bool calculatePortfolio = false;
            bool skipPostback = false;
            bool redirecting = false;
            if (IsPostBack)
            {
                try
                {
                    Helper.UI.Form formHelper = new Helper.UI.Form();
                    if (_userAction.Value.CompareTo("addDeposit") == 0 || _userAction.Value.CompareTo("updateDeposit") == 0 || _userAction.Value.CompareTo("removeDeposit") == 0)
                    {
                        skipPostback = true;
                        if(_userAction.Value.CompareTo("updateDeposit") == 0 || _userAction.Value.CompareTo("removeDeposit") == 0)
                            _hiddenScrollIntoView.Value = "ContentBody__panelBottomLeft";
                    }
                }
                catch
                {
                }

                if (skipPostback == false)
                {
                    try
                    {
                        Helper.UI.Form formHelper = new Helper.UI.Form();
                        if (formHelper.ControlExists("_buttonCalculate", Request))
                        {
                            _userAction.Value = "";
                            if (formHelper.GetInput("_availableTo", Request).Trim().Length > 0)
                            {
                                try
                                {
                                    availableToHubAccountTypeID = Convert.ToInt32(formHelper.GetInput("_availableTo", Request));
                                }
                                catch
                                {
                                }
                            }
                            if (formHelper.GetInput("_currencyCode", Request) != null)
                            {
                                if (formHelper.GetInput("_currencyCode", Request).Trim().Length > 0)
                                    currencyCode = formHelper.GetInput("_currencyCode", Request).Trim();
                            }
                            if (formHelper.GetInput("_includePooledProducts", Request) != null)
                            {
                                if (formHelper.GetInput("_includePooledProducts", Request).Trim().Length > 0)
                                    includePooledProducts = ((formHelper.GetInput("_includePooledProducts", Request).Trim().ToLower().CompareTo("true") == 0) ? true : false);
                            }

                            calculatePortfolio = true;
                            // now delete any saved deposits so we start again on each calculate
                            Octavo.Gate.Nabu.Preferences.Preference scurveBuilderDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                            if (scurveBuilderDeposits != null && scurveBuilderDeposits.Children.Count > 0)
                            {
                                scurveBuilderDeposits.Children.Clear();
                                preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveBuilderDeposits);
                            }
                            _hiddenScrollIntoView.Value = "ContentBody__panelBottomLeft";
                        }
                        else if (_userAction.Value.CompareTo("Anonymise")==0)
                        {
                            if (scurvePreferences.GetChildPreference("AnonymiseDeposits") == null || scurvePreferences.GetChildPreference("AnonymiseDeposits").Value == null || scurvePreferences.GetChildPreference("AnonymiseDeposits").Value.Trim().Length == 0)
                                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AnonymiseDeposits", "true"));
                            else
                            {
                                Octavo.Gate.Nabu.Preferences.Preference prefAnonymiseDeposits = scurvePreferences.GetChildPreference("AnonymiseDeposits");
                                prefAnonymiseDeposits.Value = "true";
                                scurvePreferences.SetChildPreference(prefAnonymiseDeposits);
                            }
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);

                            redirecting = true;
                            Response.Redirect("SCurveAvailableTo.aspx?#ContentBody__panelBottomLeft");
                        }
                        else if (_userAction.Value.CompareTo("UnAnonymise") == 0)
                        {
                            if (scurvePreferences.GetChildPreference("AnonymiseDeposits") == null || scurvePreferences.GetChildPreference("AnonymiseDeposits").Value == null || scurvePreferences.GetChildPreference("AnonymiseDeposits").Value.Trim().Length == 0)
                                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AnonymiseDeposits", "false"));
                            else
                            {
                                Octavo.Gate.Nabu.Preferences.Preference prefAnonymiseDeposits = scurvePreferences.GetChildPreference("AnonymiseDeposits");
                                prefAnonymiseDeposits.Value = "false";
                                scurvePreferences.SetChildPreference(prefAnonymiseDeposits);
                            }
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);

                            redirecting = true;
                            Response.Redirect("SCurveAvailableTo.aspx?#ContentBody__panelBottomLeft");
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (redirecting == false)
            {
                string fscsProtectionConfigFile = ConfigurationManager.AppSettings["clientConfigRoot"];
                if (fscsProtectionConfigFile.EndsWith("\\") == false)
                    fscsProtectionConfigFile += "\\";
                fscsProtectionConfigFile += "FSCSProtection.xml";

                scurve.LoadHeatmap(availableToHubAccountTypeID, currencyCode, System.Configuration.ConfigurationManager.AppSettings["preferencesRoot"]);

                Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = ProcessPostback(skipPostback, scurve.heatmap);

                scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");

                // display the settings
                _panelTop.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Product/Inputs"));
                _panelTop.Controls.Add(RenderSettings(settings, false));

                _panelTopLeft.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Liquidity Requirements"));
                System.Web.UI.WebControls.Table liquidityRequirements = RenderLiquidityRequirements(settings, scurve.ListAllTerms(), false);
                _panelTopLeft.Controls.Add(SplitTable(liquidityRequirements, "top"));
                _panelTopRight.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Liquidity Requirements"));
                _panelTopRight.Controls.Add(SplitTable(liquidityRequirements, "bottom"));

                HtmlGenericControl p = new HtmlGenericControl("p");
                p.Style.Add("width", "100%");
                p.Style.Add("text-align", "right");
                HtmlAnchor linkReset = new HtmlAnchor();
                linkReset.Attributes.Add("class", "btn");
                linkReset.Style.Add("margin-right", "15px");
                linkReset.InnerText = "Reset";
                linkReset.Title = "Reset to defaults";
                linkReset.HRef = "SCurveAvailableTo.aspx?reset=true";
                linkReset.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                p.Controls.Add(linkReset);
                HtmlInputSubmit buttonCalculate = new HtmlInputSubmit();
                buttonCalculate.Attributes.Add("class", "btn");
                buttonCalculate.ID = "_buttonCalculate";
                buttonCalculate.Value = "Calculate";
                buttonCalculate.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                if (HideCalculateButton)
                    buttonCalculate.Style.Add("display", "none");
                p.Controls.Add(buttonCalculate);
                _panelTopRight.Controls.Add(p);

                // generate the scurve
                Tools.Sales.SCurveOutput proposedPortfolio = null;
                if (IsPostBack && skipPostback == true)
                {
                    try
                    {
                        Helper.UI.Form formHelper = new Helper.UI.Form();
                        if (_userAction.Value.CompareTo("addDeposit") == 0)
                        {
                            Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                            if (scurveDeposits == null || scurveDeposits.Children.Count == 0)
                                scurveDeposits = new Octavo.Gate.Nabu.Preferences.Preference("Deposits", "");

                            int selectedInstitutionID = Convert.ToInt32(_addDepositForInstitutionID.Value);
                            int selectedTermIndex = Convert.ToInt32(_addDepositForTermIndex.Value);
                            decimal depositAmount = Convert.ToDecimal(formHelper.GetInput("_dialogDepositAmount", Request));

                            foreach (Insignis.Asset.Management.Tools.Helper.HeatmapInstitution institution in scurve.heatmap.heatmapInstitutions)
                            {
                                if (institution.institution.PartyID == selectedInstitutionID)
                                {
                                    int termIndex = 0;
                                    foreach (Insignis.Asset.Management.Tools.Helper.HeatmapTerm term in institution.investmentTerms)
                                    {
                                        if (termIndex == selectedTermIndex)
                                        {
                                            Octavo.Gate.Nabu.Preferences.Preference newDeposit = new Octavo.Gate.Nabu.Preferences.Preference();
                                            string tempInstitutionName = institution.institution.Name;
                                            if (tempInstitutionName.Contains("&amp;"))
                                                tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionName", Helper.TextFormatter.RemoveNonASCIICharacters(tempInstitutionName)));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionID", institution.institution.PartyID.Value.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("DepositAmount", depositAmount.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER100K", term.AER100K.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER250K", term.AER250K.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER50K", term.AER50K.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InterestPaid", term.InterestPaid));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MinimumInvestment", term.MinimumInvestment.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MaximumInvestment", term.MaximumInvestment.ToString()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InvestmentTerm", term.InvestmentTerm.ConvertToSemiColonSeparatedValues()));
                                            newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionShortName", institution.institution.ShortName));
                                            scurveDeposits.Children.Add(newDeposit);
                                            break;
                                        }
                                        termIndex++;
                                    }
                                    break;
                                }
                            }
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
                        }
                        else if (_userAction.Value.CompareTo("updateDeposit") == 0)
                        {
                            Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                            if (scurveDeposits != null && scurveDeposits.Children.Count > 0)
                            {
                                foreach (Octavo.Gate.Nabu.Preferences.Preference depositAsPreference in scurveDeposits.Children)
                                {
                                    if (depositAsPreference.ID.CompareTo(Guid.Parse(_userValue.Value)) == 0)
                                    {
                                        decimal depositAmount = Convert.ToDecimal(formHelper.GetInput("_dialogEditDepositAmount", Request));
                                        depositAsPreference.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("DepositAmount", depositAmount.ToString()));
                                        break;
                                    }
                                }
                                preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
                            }
                        }
                        else if (_userAction.Value.CompareTo("removeDeposit") == 0)
                        {
                            Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
                            if (scurveDeposits != null && scurveDeposits.Children.Count > 0)
                            {
                                scurveDeposits.RemoveChildPreference(Guid.Parse(_userValue.Value));
                                preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");
                if (institutionInclusion == null || institutionInclusion.Children.Count == 0)
                {
                    institutionInclusion = new Octavo.Gate.Nabu.Preferences.Preference("Institutions", "");

                    Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
                    foreach (Institution institution in allInstitutions)
                    {
                        if (institution.ShortName.CompareTo("NationalSavingsInvestments") != 0)
                            institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
                        else
                            institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
                    }
                }

                if (calculatePortfolio)
                {
                    proposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);
                    SavePortfolio(proposedPortfolio, availableToHubAccountTypeID);
                }
                else
                {
                    proposedPortfolio = LoadPortfolio(availableToHubAccountTypeID);
                    scurve.Calculate(proposedPortfolio, settings);
                    //scurve.Calculate(proposedPortfolio, settings, fscsProtectionConfigFile);
                }

                proposedPortfolio.FeePercentage = feeMatrix.GetRateForDeposit(proposedPortfolio.TotalDeposited);
                if (settings.CurrencyCode.CompareTo("USD") == 0 || settings.CurrencyCode.CompareTo("EUR") == 0)
                    proposedPortfolio.FeePercentage += Convert.ToDecimal("0.05");
                try
                {
                    if (scurvePreferences.GetChildPreference("FeeDiscount") != null && scurvePreferences.GetChildPreference("FeeDiscount").Value.Length > 0)
                    {
                        decimal feeDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("FeeDiscount").Value);
                        if (feeDiscount > 0)
                        {
                            if (feeDiscount < proposedPortfolio.FeePercentage)
                                proposedPortfolio.FeePercentage -= feeDiscount;
                            else
                                proposedPortfolio.FeePercentage = 0;
                        }
                    }

                    if (scurvePreferences.GetChildPreference("IntroducerDiscount") != null && scurvePreferences.GetChildPreference("IntroducerDiscount").Value.Length > 0)
                    {
                        decimal introducerDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("IntroducerDiscount").Value);
                        if (introducerDiscount > 0)
                        {
                            if (introducerDiscount < proposedPortfolio.FeePercentage)
                                proposedPortfolio.FeePercentage -= introducerDiscount;
                            else
                                proposedPortfolio.FeePercentage = 0;
                        }
                    }

                    proposedPortfolio.NetAverageYield = (proposedPortfolio.GrossAverageYield - proposedPortfolio.FeePercentage);

                    proposedPortfolio.Fee = (proposedPortfolio.TotalDeposited * (decimal)(proposedPortfolio.FeePercentage / 100));

                    proposedPortfolio.AnnualNetInterestEarned = (proposedPortfolio.AnnualGrossInterestEarned - proposedPortfolio.Fee);
                }
                catch
                {
                }

                // Draw the heatmap
                DrawHeatmap(settings, scurve, proposedPortfolio, institutionInclusion, financialAbstraction.GetAccountType(Convert.ToInt32(settings.AvailableToHubAccountTypeID), (int)multiLingual.language.LanguageID));

                // output the table to the bottom left column
                _panelBottomLeft.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Deposits"));
                System.Web.UI.WebControls.Table tableProposedPortfolio = RenderProposedPortfolio(proposedPortfolio, scurvePreferences, false, settings.ShowFitchRating, settings.AnonymiseDeposits);
                _panelBottomLeft.Controls.Add(tableProposedPortfolio);
                if (tableProposedPortfolio.Rows.Count > 1)
                {
                    HtmlGenericControl pTableMenu = new HtmlGenericControl("p");
                    pTableMenu.Style.Add("width", "100%;");
                    pTableMenu.Style.Add("text-align", "right");
                    _panelBottomLeft.Controls.Add(pTableMenu);

                    HtmlAnchor linkClearDeposits = new HtmlAnchor();
                    linkClearDeposits.Attributes.Add("class", "btn");
                    linkClearDeposits.Style.Add("margin-right", "15px");
                    linkClearDeposits.InnerText = "Clear";
                    linkClearDeposits.Title = "Clear Deposits Table";
                    linkClearDeposits.HRef = Helper.DomainRoot.Get() + "Illustrator/SCurveAvailableTo.aspx?clear=true";
                    linkClearDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                    pTableMenu.Controls.Add(linkClearDeposits);

                    if (settings.AnonymiseDeposits == false)
                    {
                        HtmlAnchor linkAnonymiseDeposits = new HtmlAnchor();
                        linkAnonymiseDeposits.ID = "_buttonAnonymiseDeposits";
                        linkAnonymiseDeposits.Attributes.Add("class", "btn");
                        linkAnonymiseDeposits.Style.Add("margin-right", "15px");
                        linkAnonymiseDeposits.InnerText = "Anonymise";
                        linkAnonymiseDeposits.Attributes.Add("title", "Anonymise Banks");
                        linkAnonymiseDeposits.HRef = "javascript:void(0);";
                        linkAnonymiseDeposits.Attributes.Add("onclick", "return Anonymise();");
                        linkAnonymiseDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                        pTableMenu.Controls.Add(linkAnonymiseDeposits);
                    }
                    else
                    {
                        HtmlAnchor linkUnAnonymiseDeposits = new HtmlAnchor();
                        linkUnAnonymiseDeposits.ID = "_buttonUnAnonymiseDeposits";
                        linkUnAnonymiseDeposits.Attributes.Add("class", "btn");
                        linkUnAnonymiseDeposits.Style.Add("margin-right", "15px");
                        linkUnAnonymiseDeposits.InnerText = "Un-Anonymise";
                        linkUnAnonymiseDeposits.Attributes.Add("title", "Un-Anonymise Banks");
                        linkUnAnonymiseDeposits.HRef = "javascript:void(0);";
                        linkUnAnonymiseDeposits.Attributes.Add("onclick", "return UnAnonymise();");
                        linkUnAnonymiseDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                        pTableMenu.Controls.Add(linkUnAnonymiseDeposits);
                    }
                }

                // output the table to the bottom right column
                _panelBottomRight.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Properties"));
                _panelBottomRight.Controls.Add(RenderProperties(proposedPortfolio));
                SavePortfolioProperties(proposedPortfolio, availableToHubAccountTypeID);

                // popup if proposed portfolio total is less than the total within the liquidity boxes
                if (proposedPortfolio.TotalDeposited < Convert.ToDecimal(_hiddenCalculatedTotalDeposit.Value))
                {
                    // show popup - the document ready will render a dialog if this value is true
                    Helper.UI.Form formHelper = new Helper.UI.Form();
                    if (formHelper.ControlExists("_buttonCalculate", Request))
                    {
                        // we only want to show this dialog if we have just clicked Calculate
                        _hiddenShowLiquidityWarningDialog.Value = "true";
                    }
                    else
                        _hiddenShowLiquidityWarningDialog.Value = "false";
                }
                else
                    _hiddenShowLiquidityWarningDialog.Value = "false";

                if (tableProposedPortfolio.Rows.Count > 1)
                {
                    HtmlGenericControl belowTableMenu = new HtmlGenericControl("p");
                    belowTableMenu.Style.Add("width", "100%;");
                    belowTableMenu.Style.Add("text-align", "right");
                    _panelBottomRight.Controls.Add(belowTableMenu);

                    HtmlGenericControl span = new HtmlGenericControl("span");
                    span.InnerHtml = "Generate Illustration From Template:<br/>";
                    belowTableMenu.Controls.Add(span);

                    DropDownList comboSelectTemplate = new DropDownList();
                    comboSelectTemplate.Attributes.Add("onchange", "generateTemplatedIllustration(this);");
                    comboSelectTemplate.Items.Add(new ListItem("Choose Template", "-1"));
                    if (ConfigurationManager.AppSettings["illustrationTemplateRoot"] != null && System.IO.Directory.Exists(ConfigurationManager.AppSettings["illustrationTemplateRoot"]))
                    {
                        Octavo.Gate.Nabu.Encryption.EncryptorDecryptor encryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
                        string[] organisationIDFolders = System.IO.Directory.GetDirectories(ConfigurationManager.AppSettings["illustrationTemplateRoot"]);
                        foreach (string organisationIDFolder in organisationIDFolders)
                        {
                            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(organisationIDFolder);
                            Octavo.Gate.Nabu.Entities.Core.Organisation organisation = coreAbstraction.GetOrganisation(Convert.ToInt32(di.Name), (int)multiLingual.language.LanguageID);
                            bool showTemplate = false;
                            if (organisation.Name.ToUpper().StartsWith("INSIGNIS"))
                                showTemplate = true;
                            else
                                showTemplate = ShowTemplate(Convert.ToInt32(di.Name), ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()));
                            if (showTemplate)
                            {
                                comboSelectTemplate.Items.Add(new ListItem(organisation.Name, "-2"));
                                string[] templateFiles = System.IO.Directory.GetFiles(organisationIDFolder);
                                foreach (string templateFile in templateFiles)
                                {
                                    System.IO.FileInfo fi = new System.IO.FileInfo(templateFile);
                                    comboSelectTemplate.Items.Add(new ListItem(" + " + fi.Name, encryptor.UrlEncode(encryptor.Encrypt(templateFile))));
                                }
                            }
                        }
                    }
                    belowTableMenu.Controls.Add(comboSelectTemplate);
                }
            }
        }
        public bool ShowTemplate(int pIntroducerPartyID, string pIntroducerSpecificPreferencesFolder)
        {
            bool showTemplate = false;
            try
            {
                if (System.IO.Directory.Exists(pIntroducerSpecificPreferencesFolder))
                {
                    string configFile = pIntroducerSpecificPreferencesFolder;
                    if (configFile.EndsWith("\\") == false)
                        configFile += "\\";
                    configFile += "PartyID.xml";
                    if (System.IO.File.Exists(configFile))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(configFile);
                        foreach (XmlNode introducerNode in doc.ChildNodes)
                        {
                            if (introducerNode.Name.CompareTo("introducer") == 0)
                            {
                                foreach (XmlNode partyIDNode in introducerNode.ChildNodes)
                                {
                                    if (partyIDNode.Name.CompareTo("partyID") == 0)
                                    {
                                        int partyID = -1;
                                        try
                                        {
                                            partyID = Convert.ToInt32(partyIDNode.InnerText);

                                            if (partyID == pIntroducerPartyID)
                                                showTemplate = true;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return showTemplate;
        }

        public Insignis.Asset.Management.Tools.Sales.SCurveSettings ProcessPostback(bool pSkipPostback, Insignis.Asset.Management.Tools.Helper.Heatmap pHeatmap)
        {
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
            Octavo.Gate.Nabu.Preferences.Preference scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");
            if (scurvePreferences == null || scurvePreferences.Children.Count == 0)
            {
                scurvePreferences = new Octavo.Gate.Nabu.Preferences.Preference("Settings", "");
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("TotalAvailableToDeposit", "750000"));
                //scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MaximumDurationInMonths", "60"));
                //scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("ClientType", Tools.Sales.SCurveClientType.Individual.ToString()));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MaximumDepositInAnyOneInstitution", "85000"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("NumberOfLiquidityRequirements", "11"));
                //scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_1", "2555"));           // 7 years
                //scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_1", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_1", "1890"));           // 5 years
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_1", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_2", "1460"));           // 4 years
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_2", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_3", "1190"));           // 3 years
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_3", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_4", "790"));            // 2 years
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_4", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_5", "590"));            // 18 months
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_5", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_6", "366"));            // 1 year
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_6", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_7", "291"));            // 9 months
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_7", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_8", "190"));            // 6 months
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_8", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_9", "100"));             // 3 months
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_9", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_10", "35"));             // 1 month <=35
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_10", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_11", "0"));             // instant access < 1
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_11", "0"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("SortPortfolioBy", "RateDesc"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("FeeDiscount", "0.000"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("IntroducerDiscount", "0.000"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("CurrencyCode", "GBP"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("FullProtection", "true"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("ShowFitchRating", "false"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MinimumFitchRating", "All"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("IncludePooledProducts", "true"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("OptionalClientName", "unspecified"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("OptionalIntroducerOrganisationName", "unspecified"));
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AnonymiseDeposits", "false"));
                // default to personal
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AvailableTo", financialAbstraction.GetAccountTypeByAlias("ACT_PERSONALHUBACCOUNT", (int)multiLingual.language.LanguageID).AccountTypeID.ToString()));
            }
            Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");
            if (institutionInclusion == null || institutionInclusion.Children.Count == 0)
            {
                institutionInclusion = new Octavo.Gate.Nabu.Preferences.Preference("Institutions", "");

                Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
                foreach (Institution institution in allInstitutions)
                {
                    if (institution.ShortName.CompareTo("NationalSavingsInvestments") != 0)
                        institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
                    else
                        institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
                }
            }

            if (IsPostBack)
            {
                Helper.UI.Form formHelper = new Helper.UI.Form();
                if (pSkipPostback == false)
                {
                    if (formHelper.ControlExists("_buttonCalculate", Request))
                    {
                        try
                        {
                            Octavo.Gate.Nabu.Preferences.Preference prefTotalAvailableToDeposit = scurvePreferences.GetChildPreference("TotalAvailableToDeposit");
                            prefTotalAvailableToDeposit.Value = formHelper.GetInput("_hiddenCalculatedTotalDeposit", Request);
                            scurvePreferences.SetChildPreference(prefTotalAvailableToDeposit);

                            Octavo.Gate.Nabu.Preferences.Preference prefAvailableTo = scurvePreferences.GetChildPreference("AvailableTo");
                            prefAvailableTo.Value = formHelper.GetInput("_availableTo", Request);
                            scurvePreferences.SetChildPreference(prefAvailableTo);

                            Octavo.Gate.Nabu.Preferences.Preference prefMaximumDepositInAnyOneInstitution = scurvePreferences.GetChildPreference("MaximumDepositInAnyOneInstitution");
                            prefMaximumDepositInAnyOneInstitution.Value = formHelper.GetInput("_maximumDepositInAnyOneInstitution", Request);
                            try
                            {
                                if (prefMaximumDepositInAnyOneInstitution.Value.Trim().Length == 0)
                                    prefMaximumDepositInAnyOneInstitution.Value = "0.00";
                                else
                                    prefMaximumDepositInAnyOneInstitution.Value = Convert.ToDecimal(prefMaximumDepositInAnyOneInstitution.Value).ToString("0.00");
                            }
                            catch
                            {
                                prefMaximumDepositInAnyOneInstitution.Value = "0.00";
                            }
                            scurvePreferences.SetChildPreference(prefMaximumDepositInAnyOneInstitution);

                            Octavo.Gate.Nabu.Preferences.Preference prefNumberOfLiquidityRequirements = scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements");
                            for (int i = 1; i <= Convert.ToInt32(prefNumberOfLiquidityRequirements.Value); i++)
                            {
                                //    scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_" + i, formHelper.GetInput("_liquidityDays_" + i, Request)));
                                string liquidityAmount = formHelper.GetInput("_liquidityAmount_" + i, Request);
                                if (liquidityAmount == null || liquidityAmount.Trim().Length == 0)
                                    liquidityAmount = "0.00";
                                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_" + i, liquidityAmount));
                            }

                            Octavo.Gate.Nabu.Preferences.Preference prefFeeDiscount = scurvePreferences.GetChildPreference("FeeDiscount");
                            if (prefFeeDiscount == null)
                                prefFeeDiscount = new Octavo.Gate.Nabu.Preferences.Preference("FeeDiscount");
                            prefFeeDiscount.Value = formHelper.GetInput("_feeDiscount", Request);
                            scurvePreferences.SetChildPreference(prefFeeDiscount);

                            Octavo.Gate.Nabu.Preferences.Preference prefIntroducerDiscount = scurvePreferences.GetChildPreference("IntroducerDiscount");
                            if (prefIntroducerDiscount == null)
                                prefIntroducerDiscount = new Octavo.Gate.Nabu.Preferences.Preference("IntroducerDiscount");
                            prefIntroducerDiscount.Value = formHelper.GetInput("_introducerDiscount", Request);
                            scurvePreferences.SetChildPreference(prefIntroducerDiscount);

                            Octavo.Gate.Nabu.Preferences.Preference prefCurrencyCode = scurvePreferences.GetChildPreference("CurrencyCode");
                            if (prefCurrencyCode == null)
                                prefCurrencyCode = new Octavo.Gate.Nabu.Preferences.Preference("CurrencyCode");
                            prefCurrencyCode.Value = formHelper.GetInput("_currencyCode", Request);
                            scurvePreferences.SetChildPreference(prefCurrencyCode);

                            Octavo.Gate.Nabu.Preferences.Preference prefFullProtection = scurvePreferences.GetChildPreference("FullProtection");
                            if (prefFullProtection == null)
                                prefFullProtection = new Octavo.Gate.Nabu.Preferences.Preference("FullProtection");
                            prefFullProtection.Value = formHelper.GetInput("_fullProtection", Request);
                            scurvePreferences.SetChildPreference(prefFullProtection);

                            Octavo.Gate.Nabu.Preferences.Preference prefShowFitchRating = scurvePreferences.GetChildPreference("ShowFitchRating");
                            if (prefShowFitchRating == null)
                                prefShowFitchRating = new Octavo.Gate.Nabu.Preferences.Preference("ShowFitchRating");
                            prefShowFitchRating.Value = formHelper.GetInput("_showFitchRating", Request);
                            scurvePreferences.SetChildPreference(prefShowFitchRating);

                            Octavo.Gate.Nabu.Preferences.Preference prefMinimumFitchRating = scurvePreferences.GetChildPreference("MinimumFitchRating");
                            if (prefMinimumFitchRating == null)
                                prefMinimumFitchRating = new Octavo.Gate.Nabu.Preferences.Preference("MinimumFitchRating");
                            prefMinimumFitchRating.Value = formHelper.GetInput("_minFitchRating", Request);
                            scurvePreferences.SetChildPreference(prefMinimumFitchRating);

                            Octavo.Gate.Nabu.Preferences.Preference prefIncludePooledProducts = scurvePreferences.GetChildPreference("IncludePooledProducts");
                            if (prefIncludePooledProducts == null)
                                prefIncludePooledProducts = new Octavo.Gate.Nabu.Preferences.Preference("IncludePooledProducts");
                            prefIncludePooledProducts.Value = formHelper.GetInput("_includePooledProducts", Request);
                            scurvePreferences.SetChildPreference(prefIncludePooledProducts);

                            Octavo.Gate.Nabu.Preferences.Preference prefOptionalClientName = scurvePreferences.GetChildPreference("OptionalClientName");
                            if (prefOptionalClientName == null)
                                prefOptionalClientName = new Octavo.Gate.Nabu.Preferences.Preference("OptionalClientName");
                            prefOptionalClientName.Value = formHelper.GetInput("_optionalClientName", Request);
                            scurvePreferences.SetChildPreference(prefOptionalClientName);

                            Octavo.Gate.Nabu.Preferences.Preference prefOptionalIntroducerOrganisationName = scurvePreferences.GetChildPreference("OptionalIntroducerOrganisationName");
                            if (prefOptionalIntroducerOrganisationName == null)
                                prefOptionalIntroducerOrganisationName = new Octavo.Gate.Nabu.Preferences.Preference("OptionalIntroducerOrganisationName");
                            prefOptionalIntroducerOrganisationName.Value = formHelper.GetInput("_optionalIntroducerOrganisationName", Request);
                            scurvePreferences.SetChildPreference(prefOptionalIntroducerOrganisationName);

                            // now save the checkboxes against each institution in the heatmap
                            Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
                            foreach (Institution institution in allInstitutions)
                            {
                                if (formHelper.GetInput("_checkIncludeInstitution" + institution.PartyID.ToString(), Request).ToLower().CompareTo("on") == 0)
                                    institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
                                else
                                    institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
                            }
                            preferencesManager.SetPreference("Sales.Tools.SCurve.Institutions", 1, institutionInclusion);
                        }
                        catch
                        {
                        }
                    }
                    //else if (formHelper.ControlExists("_buttonAddRequirement", Request))
                    //{
                    //    try
                    //    {
                    //        Octavo.Gate.Nabu.Preferences.Preference prefNumberOfLiquidityRequirements = scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements");
                    //        prefNumberOfLiquidityRequirements.Value = Convert.ToString(Convert.ToInt32(prefNumberOfLiquidityRequirements.Value) + 1);
                    //        scurvePreferences.SetChildPreference(prefNumberOfLiquidityRequirements);
                    //        scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_" + prefNumberOfLiquidityRequirements.Value, "0"));
                    //        scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_" + prefNumberOfLiquidityRequirements.Value, "0.00"));
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}
                    //else if (formHelper.ControlExistsContaining("_buttonRemoveRequirement", Request))
                    //{
                    //    if (_hiddenID.Value != null && _hiddenID.Value.Trim().Length > 0)
                    //    {
                    //        try
                    //        {
                    //            int index = Convert.ToInt32(_hiddenID.Value);
                    //
                    //            Octavo.Gate.Nabu.Preferences.Preference prefNumberOfLiquidityRequirements = scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements");
                    //            int x = Convert.ToInt32(prefNumberOfLiquidityRequirements.Value);
                    //            List<System.Collections.Generic.KeyValuePair<int, decimal>> liquidityNeeds = new List<KeyValuePair<int, decimal>>();
                    //            for (int i = 1; i <= x; i++)
                    //            {
                    //                Octavo.Gate.Nabu.Preferences.Preference prefDays = scurvePreferences.GetChildPreference("LiquidityDays_" + i);
                    //                Octavo.Gate.Nabu.Preferences.Preference prefAmount = scurvePreferences.GetChildPreference("LiquidityAmount_" + i);
                    //                int days = Convert.ToInt32(prefDays.Value);
                    //                decimal amount = Convert.ToDecimal(prefAmount.Value);
                    //                liquidityNeeds.Add(new System.Collections.Generic.KeyValuePair<int, decimal>(days, amount));
                    //
                    //                scurvePreferences.RemoveChildPreference(prefDays.ID);
                    //                scurvePreferences.RemoveChildPreference(prefAmount.ID);
                    //            }
                    //
                    //            liquidityNeeds.RemoveAt(index - 1);
                    //
                    //            prefNumberOfLiquidityRequirements.Value = liquidityNeeds.Count.ToString();
                    //            scurvePreferences.SetChildPreference(prefNumberOfLiquidityRequirements);
                    //            int rowCounter = 1;
                    //            foreach (System.Collections.Generic.KeyValuePair<int, decimal> need in liquidityNeeds)
                    //            {
                    //                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityDays_" + rowCounter, need.Key.ToString()));
                    //                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_" + rowCounter, need.Value.ToString()));
                    //                rowCounter++;
                    //            }
                    //        }
                    //        catch
                    //        {
                    //        }
                    //    }
                    //}
                }
            }
            preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);

            Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = new Insignis.Asset.Management.Tools.Sales.SCurveSettings();
            settings.TotalAvailableToDeposit = Convert.ToDecimal(scurvePreferences.GetChildPreference("TotalAvailableToDeposit").Value);
            if (scurvePreferences.GetChildPreference("AvailableTo") != null && scurvePreferences.GetChildPreference("AvailableTo").Value.Length > 0)
            {
                settings.AvailableToHubAccountTypeID = Convert.ToInt32(scurvePreferences.GetChildPreference("AvailableTo").Value);
                AccountType hubAccountType = financialAbstraction.GetAccountType(settings.AvailableToHubAccountTypeID,(int)multiLingual.language.LanguageID);
                if(hubAccountType!=null && hubAccountType.ErrorsDetected == false && hubAccountType.AccountTypeID.HasValue)
                {
                    if (hubAccountType.Detail.Alias.Contains("JOINTHUBACCOUNT"))
                        settings.ClientType = Tools.Sales.SCurveClientType.Joint;
                    else if (hubAccountType.Detail.Alias.Contains("PERSONALHUBACCOUNT"))
                        settings.ClientType = Tools.Sales.SCurveClientType.Individual;
                    else
                        settings.ClientType = Tools.Sales.SCurveClientType.Corporate;
                }
            }
            else
            {
                settings.AvailableToHubAccountTypeID = -1;
                settings.ClientType = Tools.Sales.SCurveClientType.Individual;
            }
            settings.MaximumDepositInAnyOneInstitution = Convert.ToDecimal(scurvePreferences.GetChildPreference("MaximumDepositInAnyOneInstitution").Value);
            if (scurvePreferences.GetChildPreference("FeeDiscount") != null && scurvePreferences.GetChildPreference("FeeDiscount").Value.Length > 0)
                settings.FeeDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("FeeDiscount").Value);
            else
                settings.FeeDiscount = 0;
            if (scurvePreferences.GetChildPreference("IntroducerDiscount") != null && scurvePreferences.GetChildPreference("IntroducerDiscount").Value.Length > 0)
                settings.IntroducerDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("IntroducerDiscount").Value);
            else
                settings.IntroducerDiscount = 0;
            if (scurvePreferences.GetChildPreference("CurrencyCode") != null && scurvePreferences.GetChildPreference("CurrencyCode").Value.Length > 0)
                settings.CurrencyCode = scurvePreferences.GetChildPreference("CurrencyCode").Value;
            if (scurvePreferences.GetChildPreference("FullProtection") != null && scurvePreferences.GetChildPreference("FullProtection").Value.Length > 0)
                settings.FullProtection = ((scurvePreferences.GetChildPreference("FullProtection").Value.CompareTo("true") == 0) ? true : false);
            if (scurvePreferences.GetChildPreference("ShowFitchRating") != null && scurvePreferences.GetChildPreference("ShowFitchRating").Value.Length > 0)
                settings.ShowFitchRating = ((scurvePreferences.GetChildPreference("ShowFitchRating").Value.CompareTo("true") == 0) ? true : false);
            if (scurvePreferences.GetChildPreference("MinimumFitchRating") != null && scurvePreferences.GetChildPreference("MinimumFitchRating").Value.Length > 0)
                settings.MinimumFitchRating = scurvePreferences.GetChildPreference("MinimumFitchRating").Value;
            if (scurvePreferences.GetChildPreference("IncludePooledProducts") != null && scurvePreferences.GetChildPreference("IncludePooledProducts").Value.Length > 0)
                settings.IncludePooledProducts = ((scurvePreferences.GetChildPreference("IncludePooledProducts").Value.CompareTo("true") == 0) ? true : false);
            if (scurvePreferences.GetChildPreference("OptionalClientName") != null && scurvePreferences.GetChildPreference("OptionalClientName").Value.Length > 0)
                settings.OptionalClientName = scurvePreferences.GetChildPreference("OptionalClientName").Value;
            if (scurvePreferences.GetChildPreference("OptionalIntroducerOrganisationName") != null && scurvePreferences.GetChildPreference("OptionalIntroducerOrganisationName").Value.Length > 0)
                settings.OptionalIntroducerOrganisationName = scurvePreferences.GetChildPreference("OptionalIntroducerOrganisationName").Value;
            if (scurvePreferences.GetChildPreference("AnonymiseDeposits") != null && scurvePreferences.GetChildPreference("AnonymiseDeposits").Value.Length > 0)
                settings.AnonymiseDeposits = ((scurvePreferences.GetChildPreference("AnonymiseDeposits").Value.CompareTo("true") == 0) ? true : false);

            int numberOfLiquidityRequirements = Convert.ToInt32(scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements").Value);
            for (int i = 1; i <= numberOfLiquidityRequirements; i++)
            {
                try
                {
                    int days = Convert.ToInt32(scurvePreferences.GetChildPreference("LiquidityDays_" + i).Value);
                    decimal amount = Convert.ToDecimal(scurvePreferences.GetChildPreference("LiquidityAmount_" + i).Value);
                    settings.LiquidityNeedsDaysAndAmounts.Add(new System.Collections.Generic.KeyValuePair<int, decimal>(days, amount));
                }
                catch
                {
                }
            }
            return settings;
        }

        public System.Web.UI.WebControls.Table RenderSettings(Insignis.Asset.Management.Tools.Sales.SCurveSettings pSettings, bool isReadOnly)
        {
            System.Web.UI.WebControls.Table tableTopLeft = new System.Web.UI.WebControls.Table();
            tableTopLeft.CssClass = "table-bordered table-striped";

            if (isReadOnly)
            {
                tableTopLeft.Rows.Add(PropertyRow("Currency", pSettings.CurrencyCode));
                tableTopLeft.Rows.Add(PropertyRow("Available To", financialAbstraction.GetAccountType(pSettings.AvailableToHubAccountTypeID,(int)multiLingual.language.LanguageID).Detail.Name));
                tableTopLeft.Rows.Add(PropertyRow("Maximum in any one institution", pSettings.MaximumDepositInAnyOneInstitution.Value.ToString("0.00")));
                tableTopLeft.Rows.Add(PropertyRow("Sales Discount", pSettings.FeeDiscount.Value.ToString("0.000")));
                tableTopLeft.Rows.Add(PropertyRow("Full Protection", pSettings.FullProtection.ToString().ToLower()));
                tableTopLeft.Rows.Add(PropertyRow("Fitch Rating", ((pSettings.ShowFitchRating == true) ? "Show" : "Hide")));
                tableTopLeft.Rows.Add(PropertyRow("Min. Fitch Rating", pSettings.MinimumFitchRating));
                tableTopLeft.Rows.Add(PropertyRow("Include Pooled", ((pSettings.IncludePooledProducts == true) ? "Yes" : "No")));
                tableTopLeft.Rows.Add(PropertyRow("Client Name", pSettings.OptionalClientName));
                tableTopLeft.Rows.Add(PropertyRow("Introducer Org", pSettings.OptionalIntroducerOrganisationName));
                tableTopLeft.Rows.Add(PropertyRow("Introduced Discount", pSettings.IntroducerDiscount.Value.ToString("0.000")));
            }
            else
            {
                tableTopLeft.Rows.Add(PropertyRow("Currency", RenderDropDownListCurrency("_currencyCode", pSettings.CurrencyCode)));
                tableTopLeft.Rows.Add(PropertyRow("Available To", RenderDropDownListAvailableTo("_availableTo", pSettings.AvailableToHubAccountTypeID, pSettings.CurrencyCode)));
                tableTopLeft.Rows.Add(PropertyRow("Maximum in any one institution", RenderDecimalTextbox("_maximumDepositInAnyOneInstitution", pSettings.MaximumDepositInAnyOneInstitution)));
                tableTopLeft.Rows.Add(PropertyRow("Sales Discount", RenderDecimalTextbox("_feeDiscount", pSettings.FeeDiscount)));
                tableTopLeft.Rows.Add(PropertyRow("Full Protection", RenderDropDownListFullProtection("_fullProtection", pSettings.FullProtection)));
                tableTopLeft.Rows.Add(PropertyRow("Fitch Rating", RenderDropDownListFitchRating("_showFitchRating", pSettings.ShowFitchRating)));
                tableTopLeft.Rows.Add(PropertyRow("Min. Fitch Rating", RenderDropDownListMinimumFitchRating("_minFitchRating", pSettings.MinimumFitchRating)));
                tableTopLeft.Rows.Add(PropertyRow("Include Pooled", RenderDropDownListIncludePooledProducts("_includePooledProducts", pSettings.IncludePooledProducts)));
                tableTopLeft.Rows.Add(PropertyRow("Client Name (optional)", RenderTextbox("_optionalClientName", pSettings.OptionalClientName)));
                tableTopLeft.Rows.Add(PropertyRow("Introducer Org (optional)", RenderTextbox("_optionalIntroducerOrganisationName", pSettings.OptionalIntroducerOrganisationName)));
                tableTopLeft.Rows.Add(PropertyRow("Introduced Discount", RenderDecimalTextbox("_introducerDiscount", pSettings.IntroducerDiscount)));
            }
            return tableTopLeft;
        }
        public System.Web.UI.WebControls.Table RenderLiquidityRequirements(Insignis.Asset.Management.Tools.Sales.SCurveSettings pSettings, List<InvestmentTerm> pInvestmentTerms, bool isReadOnly)
        {
            System.Web.UI.WebControls.Table tableTopLeft = new System.Web.UI.WebControls.Table();
            tableTopLeft.CssClass = "table-bordered table-striped";

            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.TableSection = TableRowSection.TableHeader;

            TableHeaderCell hcellLiquidityDays = new TableHeaderCell();
            hcellLiquidityDays.Text = "Up To";
            headerRow.Cells.Add(hcellLiquidityDays);

            TableHeaderCell hcellLiquidityAmount = new TableHeaderCell();
            hcellLiquidityAmount.Text = "Allocate";
            headerRow.Cells.Add(hcellLiquidityAmount);

            //TableHeaderCell hcellFiller = new TableHeaderCell();
            //headerRow.Cells.Add(hcellFiller);
            //tableTopLeft.Rows.Add(headerRow);

            decimal totalLiquidity = 0;
            int numberOfLiquidityRequirements = 1;
            foreach (System.Collections.Generic.KeyValuePair<int, decimal> liquidityRequirement in pSettings.LiquidityNeedsDaysAndAmounts)
            {
                TableRow rowLiquidityRequirement = new TableRow();
                TableCell cellLiquidityDays = new TableCell();
                cellLiquidityDays.Style.Add("text-align", "left");
                //if (isReadOnly)
                //{
                //    foreach (InvestmentTerm investmentTerm in pInvestmentTerms)
                //    {
                //        if (investmentTerm.GetLiquidityDays() == liquidityRequirement.Key)
                //            cellLiquidityDays.Text = GetShortText(investmentTerm.GetText());
                //    }
                //}
                //else
                //    cellLiquidityDays.Controls.Add(RenderDropDownList("_liquidityDays_" + numberOfLiquidityRequirements, pInvestmentTerms, liquidityRequirement.Key));
                cellLiquidityDays.Text = GetLiquidtyTextFromDays(liquidityRequirement.Key);
                rowLiquidityRequirement.Cells.Add(cellLiquidityDays);

                TableCell cellLiquidityAmount = new TableCell();
                cellLiquidityAmount.Style.Add("text-align", "left");
                if (isReadOnly)
                    cellLiquidityAmount.Text = liquidityRequirement.Value.ToString("0.00");
                else
                {
                    TextBox amount = RenderDecimalTextbox("_liquidityAmount_" + numberOfLiquidityRequirements, liquidityRequirement.Value);
                    amount.Attributes.Add("onkeyup", "RecalculateLiquidityTotal();");
                    cellLiquidityAmount.Controls.Add(amount);
                }
                totalLiquidity += liquidityRequirement.Value;
                rowLiquidityRequirement.Cells.Add(cellLiquidityAmount);
                //if (isReadOnly == false)
                //{
                //    TableCell cellAction = new TableCell();
                //    HtmlInputSubmit buttonRemoveRequirement = new HtmlInputSubmit();
                //    buttonRemoveRequirement.Attributes.Add("class", "btn");
                //    buttonRemoveRequirement.Style.Add("margin", "0px");
                //    buttonRemoveRequirement.ID = "_buttonRemoveRequirement" + numberOfLiquidityRequirements;
                //    buttonRemoveRequirement.Value = "-";
                //    buttonRemoveRequirement.Attributes.Add("onclick", "SetID('" + numberOfLiquidityRequirements.ToString() + "');");
                //    buttonRemoveRequirement.Attributes.Add("onkeypress", "return disableEnterKey(event);");
                //    cellAction.Controls.Add(buttonRemoveRequirement);
                //    rowLiquidityRequirement.Cells.Add(cellAction);
                //}
                tableTopLeft.Rows.Add(rowLiquidityRequirement);
                numberOfLiquidityRequirements++;
            }
            TableRow rowAddRequirement = new TableRow();
            TableCell cellAdd = new TableCell();
            cellAdd.Style.Add("text-align", "left");
            //if (isReadOnly == false)
            //{
            //    HtmlInputSubmit buttonAddRequirement = new HtmlInputSubmit();
            //    buttonAddRequirement.Attributes.Add("class", "btn");
            //    buttonAddRequirement.Style.Add("margin", "0px");
            //    buttonAddRequirement.ID = "_buttonAddRequirement";
            //    buttonAddRequirement.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //    buttonAddRequirement.Value = "+";
            //    cellAdd.Controls.Add(buttonAddRequirement);
            //}
            rowAddRequirement.Cells.Add(cellAdd);
            TableCell cellTotalAmount = new TableCell();
            cellTotalAmount.Style.Add("text-align", "left");
            //if (isReadOnly)
            //    cellTotalAmount.Text = "Allocated from total: " + totalLiquidity.ToString("#,##0.00");
            //else
            //    cellTotalAmount.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("<p style=\"padding:0px; margin:0px;\"><span>Allocated from total: </span><span id=\"_totalLiquidity\" style=\"color:" + ((pSettings.TotalAvailableToDeposit == totalLiquidity) ? "black" : "red") + "\">" + totalLiquidity.ToString("#,##0.00") + "</span></p>"));
            cellTotalAmount.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("<p style=\"padding:0px; margin:0px;\"><span>Total Deposit: </span><span id=\"_totalLiquidity\">" + totalLiquidity.ToString("#,##0.00") + "</span></p>"));
            //if (pSettings.TotalAvailableToDeposit == totalLiquidity)
            //    HideCalculateButton = false;
            //else
            //    HideCalculateButton = true;
            _hiddenCalculatedTotalDeposit.Value = totalLiquidity.ToString("0.00");

            rowAddRequirement.Cells.Add(cellTotalAmount);
            //if (isReadOnly == false)
            //{
            //    TableCell cellFiller = new TableCell();
            //    HiddenField hiddenNumberOfLiquidityRequirements = new HiddenField();
            //    hiddenNumberOfLiquidityRequirements.ID = "_hiddenNumberOfLiquidityRequirements";
            //    hiddenNumberOfLiquidityRequirements.Value = Convert.ToString(numberOfLiquidityRequirements);
            //    cellFiller.Controls.Add(hiddenNumberOfLiquidityRequirements);
            //    rowAddRequirement.Cells.Add(cellFiller);
            //}
            HiddenField hiddenNumberOfLiquidityRequirements = new HiddenField();
            hiddenNumberOfLiquidityRequirements.ID = "_hiddenNumberOfLiquidityRequirements";
            hiddenNumberOfLiquidityRequirements.Value = Convert.ToString(numberOfLiquidityRequirements);
            cellAdd.Controls.Add(hiddenNumberOfLiquidityRequirements);
            tableTopLeft.Rows.Add(rowAddRequirement);
            return tableTopLeft;
        }
        public System.Web.UI.WebControls.Table RenderProperties(Tools.Sales.SCurveOutput pProposedPortfolio)
        {
            System.Web.UI.WebControls.Table tableBottomLeft = new System.Web.UI.WebControls.Table();
            tableBottomLeft.CssClass = "table-bordered table-striped";

            TableHeaderRow hrowProperty = new TableHeaderRow();
            hrowProperty.TableSection = TableRowSection.TableHeader;
            TableHeaderCell hcellProperty = new TableHeaderCell();
            hcellProperty.Style.Add("text-align", "left");
            hcellProperty.Text = "Property";
            hrowProperty.Cells.Add(hcellProperty);
            TableHeaderCell hcellValue = new TableHeaderCell();
            hcellValue.Style.Add("text-align", "right");
            hcellValue.Text = "Value";
            hrowProperty.Cells.Add(hcellValue);
            tableBottomLeft.Rows.Add(hrowProperty);

            tableBottomLeft.Rows.Add(PropertyRow("Total deposited", pProposedPortfolio.TotalDeposited.ToString("#,##0.00")));
            //tableBottomLeft.Rows.Add(PropertyRow("Return", pProposedPortfolio.TotalProjectedReturn.ToString("#,##0.00")));
            //tableBottomLeft.Rows.Add(PropertyRow("Rate of return", pProposedPortfolio.AverageRateOfReturn.ToString("0.00") + "%"));
            //tableBottomLeft.Rows.Add(PropertyRow("Average days until maturity", pProposedPortfolio.AverageDaysUntilMaturity.ToString()));
            //tableBottomLeft.Rows.Add(PropertyRow("FSCS limit per licence", pProposedPortfolio.FSCSLimitPerLicense.ToString("#,##0.00")));
            tableBottomLeft.Rows.Add(PropertyRow("Annual gross interest earned", pProposedPortfolio.AnnualGrossInterestEarned.ToString("#,##0.00")));
            tableBottomLeft.Rows.Add(PropertyRow("Fee Percentage", pProposedPortfolio.FeePercentage.ToString("0.000") + "%"));
            tableBottomLeft.Rows.Add(PropertyRow("Annual net interest earned", pProposedPortfolio.AnnualNetInterestEarned.ToString("#,##0.00")));
            tableBottomLeft.Rows.Add(PropertyRow("Gross average yield", pProposedPortfolio.GrossAverageYield.ToString("0.000") + "%"));
            tableBottomLeft.Rows.Add(PropertyRow("Net average yield", pProposedPortfolio.NetAverageYield.ToString("0.000") + "%"));
            tableBottomLeft.Rows.Add(PropertyRow("FSCS eligibility %", pProposedPortfolio.FSCSPercentProtected.ToString("0.00") + "%"));
            return tableBottomLeft;
        }
        public System.Web.UI.WebControls.Table RenderProposedPortfolio(Tools.Sales.SCurveOutput pProposedPortfolio, Octavo.Gate.Nabu.Preferences.Preference pSCurvePreferences, bool pIsReadOnly, bool pShowFitchRating, bool pAnonymiseDeposits)
        {
            string sortOrder = "RateDesc";
            if (pSCurvePreferences != null && pSCurvePreferences.GetChildPreference("SortPortfolioBy") != null && pSCurvePreferences.GetChildPreference("SortPortfolioBy").Value != null && pSCurvePreferences.GetChildPreference("SortPortfolioBy").Value.Trim().Length > 0)
                sortOrder = pSCurvePreferences.GetChildPreference("SortPortfolioBy").Value;

            System.Web.UI.WebControls.Table tableBottomRight = new System.Web.UI.WebControls.Table();
            tableBottomRight.CssClass = "table-bordered table-striped";
            tableBottomRight.ID = "_proposedPortfolioTable";

            TableHeaderRow hrowOutput = new TableHeaderRow();
            hrowOutput.TableSection = TableRowSection.TableHeader;
            hrowOutput.Attributes.Add("data-sort-method", "none");
            TableHeaderCell hcellInstitution = new TableHeaderCell();
            hcellInstitution.Style.Add("text-align", "left");
            hcellInstitution.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Institution"));
            hrowOutput.Cells.Add(hcellInstitution);
            TableHeaderCell hcellTerm = new TableHeaderCell();
            hcellTerm.Style.Add("text-align", "left");
            hcellTerm.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Term"));
            hcellTerm.Attributes.Add("data-sort-default", "");
            hrowOutput.Cells.Add(hcellTerm);
            TableHeaderCell hcellRate = new TableHeaderCell();
            hcellRate.Style.Add("text-align", "right");
            hcellRate.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Rate"));
            hrowOutput.Cells.Add(hcellRate);
            TableHeaderCell hcellDepositSize = new TableHeaderCell();
            hcellDepositSize.Style.Add("text-align", "right");
            hcellDepositSize.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Deposit Size"));
            hrowOutput.Cells.Add(hcellDepositSize);
            tableBottomRight.Rows.Add(hrowOutput);

            if (sortOrder.CompareTo("RateDesc") == 0)
            {
                // do nothing as this is the default sort order
            }
            else
            {
                if (sortOrder.CompareTo("RateAsc") == 0)
                {
                    // now sort that product list into best rate order
                    pProposedPortfolio.ProposedInvestments.Sort((x, y) => x.Rate.CompareTo(y.Rate));
                }
                else
                {
                    if (sortOrder.CompareTo("InstitutionAsc") == 0)
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => x.InstitutionName.CompareTo(y.InstitutionName));
                    else if (sortOrder.CompareTo("InstitutionDesc") == 0)
                    {
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => x.InstitutionName.CompareTo(y.InstitutionName));
                        pProposedPortfolio.ProposedInvestments.Reverse();
                    }
                    else if (sortOrder.CompareTo("DepositSizeAsc") == 0)
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => x.DepositSize.CompareTo(y.DepositSize));
                    else if (sortOrder.CompareTo("DepositSizeDesc") == 0)
                    {
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => x.DepositSize.CompareTo(y.DepositSize));
                        pProposedPortfolio.ProposedInvestments.Reverse();
                    }
                    else if (sortOrder.CompareTo("TermAsc") == 0)
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => Convert.ToInt32((x.InvestmentTerm.GetLiquidityDays().HasValue) ? x.InvestmentTerm.GetLiquidityDays().Value : 0).CompareTo(Convert.ToInt32((y.InvestmentTerm.GetLiquidityDays().HasValue) ? y.InvestmentTerm.GetLiquidityDays().Value : 0)));
                    else if (sortOrder.CompareTo("TermDesc") == 0)
                    {
                        pProposedPortfolio.ProposedInvestments.Sort((x, y) => Convert.ToInt32((x.InvestmentTerm.GetLiquidityDays().HasValue) ? x.InvestmentTerm.GetLiquidityDays().Value : 0).CompareTo(Convert.ToInt32((y.InvestmentTerm.GetLiquidityDays().HasValue) ? y.InvestmentTerm.GetLiquidityDays().Value : 0)));
                        pProposedPortfolio.ProposedInvestments.Reverse();
                    }
                }
            }

            int rowCount = 0;            
            foreach (Tools.Sales.SCurveOutputRow proposedInvestment in pProposedPortfolio.ProposedInvestments)
            {
                TableRow rowOutput = new TableRow();
                if (proposedInvestment.ID != Guid.Empty)
                    rowOutput.Attributes.Add("ondblclick", "openEditDepositDialog('" + proposedInvestment.InstitutionName + "', '" + proposedInvestment.InvestmentTerm.GetText() + "', " + proposedInvestment.Rate + ", " + proposedInvestment.Rate + ", " + proposedInvestment.Rate + ", " + proposedInvestment.DepositSize + ", '" + proposedInvestment.ID.ToString() + "');");
                TableCell cellInstitution = new TableCell();
                cellInstitution.ID = "_depositTable_InstitutionName_" + rowCount;
                cellInstitution.Style.Add("text-align", "left");
                if(pAnonymiseDeposits==true)
                    cellInstitution.Text = "Bank " + Convert.ToString(rowCount+1);
                else
                    cellInstitution.Text = proposedInvestment.InstitutionName.Replace("&", "&amp;");
                if (pShowFitchRating)
                {
                    Management.Clients.Helper.InstitutionProperties institutionProperties = new Management.Clients.Helper.InstitutionProperties(proposedInvestment.InstitutionID.Value, System.Configuration.ConfigurationManager.AppSettings["preferencesRoot"]);
                    Octavo.Gate.Nabu.Preferences.Preference fitchRating = institutionProperties.preferences.GetChildPreference("FitchRating");
                    if (fitchRating != null && fitchRating.Value != null && fitchRating.Value.Trim().Length > 0)
                    {
                        cellInstitution.Text += " (FIR: " + fitchRating.Value + ")";
                    }
                }

                rowOutput.Cells.Add(cellInstitution);
                TableCell cellTerm = new TableCell();
                cellTerm.Style.Add("text-align", "left");
                cellTerm.Text = proposedInvestment.InvestmentTerm.GetText();
                rowOutput.Cells.Add(cellTerm);
                TableCell cellRate = new TableCell();
                cellRate.Style.Add("text-align", "right");
                cellRate.Text = proposedInvestment.Rate.ToString("0.00") + "%";
                rowOutput.Cells.Add(cellRate);
                TableCell cellDepositSize = new TableCell();
                cellDepositSize.Style.Add("text-align", "right");
                cellDepositSize.Text = proposedInvestment.DepositSize.ToString("#,##0.00");
                rowOutput.Cells.Add(cellDepositSize);
                tableBottomRight.Rows.Add(rowOutput);
                rowCount++;
            }
            TableRow rowTotal = new TableRow();
            rowTotal.Attributes.Add("data-sort-method", "none");
            TableCell tcellInstitution = new TableCell();
            rowTotal.Cells.Add(tcellInstitution);
            TableCell tcellTerm = new TableCell();
            rowTotal.Cells.Add(tcellTerm);
            TableCell tcellRate = new TableCell();
            rowTotal.Cells.Add(tcellRate);
            TableCell tcellDepositSize = new TableCell();
            tcellDepositSize.Style.Add("text-align", "right");
            tcellDepositSize.Style.Add("font-weight", "bold");
            tcellDepositSize.Text = pProposedPortfolio.TotalDeposited.ToString("#,##0.00");
            rowTotal.Cells.Add(tcellDepositSize);
            tableBottomRight.Rows.Add(rowTotal);
            return tableBottomRight;
        }

        public TableRow PropertyRow(string pLabel, string pValue)
        {
            TableRow rowProperty = new TableRow();
            TableCell cellProperty = new TableCell();
            cellProperty.Style.Add("text-align", "left");
            cellProperty.Text = pLabel;
            rowProperty.Cells.Add(cellProperty);
            TableCell cellValue = new TableCell();
            cellValue.Style.Add("text-align", "right");
            cellValue.Text = pValue;
            rowProperty.Cells.Add(cellValue);
            return rowProperty;
        }
        public TableRow PropertyRow(string pLabel, TextBox pTextBox)
        {
            TableRow rowProperty = new TableRow();
            TableCell cellProperty = new TableCell();
            cellProperty.Style.Add("text-align", "left");
            cellProperty.Text = pLabel;
            rowProperty.Cells.Add(cellProperty);
            TableCell cellValue = new TableCell();
            cellValue.Style.Add("text-align", "left");
            cellValue.Controls.Add(pTextBox);
            rowProperty.Cells.Add(cellValue);
            return rowProperty;
        }
        public TableRow PropertyRow(string pLabel, DropDownList pDropDownList)
        {
            TableRow rowProperty = new TableRow();
            TableCell cellProperty = new TableCell();
            cellProperty.Style.Add("text-align", "left");
            cellProperty.Text = pLabel;
            rowProperty.Cells.Add(cellProperty);
            TableCell cellValue = new TableCell();
            cellValue.Style.Add("text-align", "left");
            cellValue.Controls.Add(pDropDownList);
            rowProperty.Cells.Add(cellValue);
            return rowProperty;
        }

        public TextBox RenderTextbox(string pID, string pValue)
        {
            TextBox textBox = new TextBox();
            textBox.ID = pID;
            textBox.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            textBox.Text = pValue;
            return textBox;
        }
        public TextBox RenderDecimalTextbox(string pID, decimal? pValue)
        {
            TextBox textBox = new TextBox();
            textBox.ID = pID;
            textBox.Attributes.Add("onkeydown", "return jsDecimals(event, this.id);");
            textBox.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            if (pValue.HasValue)
                textBox.Text = pValue.Value.ToString("0.00");
            return textBox;
        }
        public DropDownList RenderDropDownListRange(string pID, int pFrom, int pTo, int pSelectedValue)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;
            for (int i = pFrom; i < pTo; i++)
                dropDownList.Items.Add(new ListItem(i.ToString()));
            if (pSelectedValue > 0)
                dropDownList.SelectedValue = pSelectedValue.ToString();
            return dropDownList;
        }
        public DropDownList RenderDropDownListAvailableTo(string pID, int pSelectedValue, string pCurrencyCode)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;
            dropDownList.Attributes.Add("onchange", "AvailableToChanged();");

            decimal fscsMagicNumber = Convert.ToDecimal("85000");
            if(pCurrencyCode.CompareTo("USD")==0)
                fscsMagicNumber = Convert.ToDecimal("100000");

            AccountType[] accountTypes = financialAbstraction.ListAccountTypes((int)multiLingual.language.LanguageID);
            foreach (AccountType accountType in accountTypes)
            {
                if (accountType.ErrorsDetected == false)
                {
                    if (accountType.Detail.Alias.Contains("HUBACCOUNT"))
                    {
                        ListItem item = new ListItem(accountType.Detail.Name, accountType.AccountTypeID.ToString());
                        if (accountType.AccountTypeID == pSelectedValue)
                        {
                            if (accountType.Detail.Alias.Contains("JOINTHUBACCOUNT"))
                                fscsMagicNumber *= 2;
                        }
                        dropDownList.Items.Add(item);
                    }
                }
            }

            _fscsValue.Value = fscsMagicNumber.ToString("0.00");

            if (pSelectedValue > 0)
                dropDownList.SelectedValue = pSelectedValue.ToString();
            return dropDownList;
        }
        public DropDownList RenderDropDownListCurrency(string pID, string pSelectedValue)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.Attributes.Add("onchange", "CurrencyChanged();");
            dropDownList.ID = pID;

            Currency[] currencies = financialAbstraction.ListCurrencies((int)multiLingual.language.LanguageID);
            foreach (Currency currency in currencies)
            {
                if (currency.ErrorsDetected == false)
                    dropDownList.Items.Add(new ListItem(currency.CurrencyCode, currency.CurrencyCode));
            }

            if (pSelectedValue != null && pSelectedValue.Trim().Length > 0)
                dropDownList.SelectedValue = pSelectedValue.ToString();
            return dropDownList;
        }
        public DropDownList RenderDropDownListFullProtection(string pID, bool pFullProtection)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;

            dropDownList.Items.Add(new ListItem("Yes", "true"));
            dropDownList.Items.Add(new ListItem("No", "false"));

            if (pFullProtection)
                dropDownList.SelectedIndex = 0;
            else
                dropDownList.SelectedIndex = 1;
            return dropDownList;
        }
        public DropDownList RenderDropDownListFitchRating(string pID, bool pShowFitchRating)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;

            dropDownList.Items.Add(new ListItem("Yes", "true"));
            dropDownList.Items.Add(new ListItem("No", "false"));

            if (pShowFitchRating)
                dropDownList.SelectedIndex = 0;
            else
                dropDownList.SelectedIndex = 1;
            return dropDownList;
        }
        public DropDownList RenderDropDownListMinimumFitchRating(string pID, string pSelectedValue)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;

            dropDownList.Items.Add(new ListItem("All", "All"));
            List<string> fitchRatings = Insignis.Asset.Management.Clients.Helper.FitchRatings.ListRatings();
            foreach (string fitchRating in fitchRatings)
                dropDownList.Items.Add(new ListItem(fitchRating, fitchRating));

            if (pSelectedValue != null && pSelectedValue.Trim().Length > 0)
                dropDownList.SelectedValue = pSelectedValue;
            return dropDownList;
        }
        public DropDownList RenderDropDownList(string pID, List<InvestmentTerm> pInvestmentTerms, int pSelectedValue)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;

            int index = 0;
            int selectedIndex = -1;
            foreach (InvestmentTerm term in pInvestmentTerms)
            {
                dropDownList.Items.Add(new ListItem(GetShortText(term.GetText()), term.GetLiquidityDays().Value.ToString()));
                if (term.GetLiquidityDays().Value == pSelectedValue)
                    selectedIndex = index;
                index++;
            }

            if (selectedIndex != -1)
                dropDownList.SelectedIndex = selectedIndex;
            return dropDownList;
        }
        public DropDownList RenderDropDownListIncludePooledProducts(string pID, bool pFullProtection)
        {
            DropDownList dropDownList = new DropDownList();
            dropDownList.ID = pID;

            dropDownList.Items.Add(new ListItem("Yes", "true"));
            dropDownList.Items.Add(new ListItem("No", "false"));

            if (pFullProtection)
                dropDownList.SelectedIndex = 0;
            else
                dropDownList.SelectedIndex = 1;
            return dropDownList;
        }        
        public string GetShortText(string pLongText)
        {
            string shortText = pLongText;
            if (shortText.Contains(" ("))
                shortText = shortText.Substring(0, shortText.IndexOf(" (")).Trim();
            if (shortText.ToUpper().Contains("BOND"))
                shortText = shortText.Substring(0, shortText.ToUpper().IndexOf("BOND")).Trim();
            if (shortText.ToUpper().Contains("FIXED"))
                shortText = shortText.Substring(0, shortText.ToUpper().IndexOf("FIXED")).Trim();
            if (shortText.ToUpper().Contains("TERM"))
                shortText = shortText.Substring(0, shortText.ToUpper().IndexOf("TERM")).Trim();
            if (shortText.ToUpper().Contains("NOTICE"))
                shortText = shortText.Substring(0, shortText.ToUpper().IndexOf("NOTICE")).Trim();
            return shortText;
        }

        public string GetLiquidtyTextFromDays(int pLiquidityDays)
        {
            string result = "";
            if (pLiquidityDays < 1)
                result = "Instant Access";
            else if (pLiquidityDays <= 35)
                result = "1 month";
            else if (pLiquidityDays <= 100)
                result = "3 months";
            else if (pLiquidityDays <= 190)
                result = "6 months";
            else if (pLiquidityDays <= 291)
                result = "9 months";
            else if (pLiquidityDays <= 396)
                result = "1 year";
            else if (pLiquidityDays <= 590)
                result = "18 months";
            else if (pLiquidityDays <= 790)
                result = "2 years";
            else if (pLiquidityDays <= 1190)
                result = "3 years";
            else if (pLiquidityDays <= 1460)
                result = "4 years";
            else if (pLiquidityDays <= 1890)
                result = "5 years";
            //else if (pLiquidityDays > 1890)
            //    result = "7 years";
            else
                result = "unrecognised";
            return result;
        }

        public System.Web.UI.WebControls.Table SplitTable(System.Web.UI.WebControls.Table pSourceTable, string pPart)
        {
            System.Web.UI.WebControls.Table destinationTable = new System.Web.UI.WebControls.Table();

            int half = 0;
            //int from = 0;
            //int to = 0;

            //TableRow[] bert = new TableRow[pSourceTable.Rows.Count];

            if (pPart.ToUpper().CompareTo("TOP") == 0)
            {
                half = (pSourceTable.Rows.Count / 2);
                //pSourceTable.Rows.CopyTo(bert, 0);
            }
            else
            {
                half = pSourceTable.Rows.Count;
                //pSourceTable.Rows.CopyTo(bert, half);
            }

            for (int i = 0; i < half; i++)
                destinationTable.Rows.Add(pSourceTable.Rows[0]);

            return destinationTable;
        }

        private void DrawHeatmap(Insignis.Asset.Management.Tools.Sales.SCurveSettings pSettings, Insignis.Asset.Management.Tools.Sales.SCurve scurve, Tools.Sales.SCurveOutput pProposedPortfolio, Octavo.Gate.Nabu.Preferences.Preference pInstitutionInclusion, AccountType pAvailableToHubAccountType)
        {
            List<IllustrationBuilderDepositRow> depositRows = new List<IllustrationBuilderDepositRow>();
            if (pSettings.FullProtection)
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + pSettings.AvailableToHubAccountTypeID, 1, "Deposits");
                if (scurveDeposits != null && scurveDeposits.Children != null && scurveDeposits.Children.Count > 0)
                {
                    foreach (Octavo.Gate.Nabu.Preferences.Preference deposit in scurveDeposits.Children)
                        depositRows.Add(new IllustrationBuilderDepositRow(deposit));
                }
            }

            List<KeyValuePair<int, string>> columnHeadings = new List<KeyValuePair<int, string>>();
            foreach (Insignis.Asset.Management.Tools.Helper.HeatmapInstitution institution in scurve.heatmap.heatmapInstitutions)
            {
                foreach (Insignis.Asset.Management.Tools.Helper.HeatmapTerm term in institution.investmentTerms)
                {
                    string termText = term.InvestmentTerm.GetText();
                    int? days = term.InvestmentTerm.GetLiquidityDays();
                    if (days.HasValue == false)
                        days = 0;

                    bool found = false;
                    foreach (KeyValuePair<int, string> columnHeading in columnHeadings)
                    {
                        if (columnHeading.Value.ToLower().CompareTo(termText.ToLower()) == 0)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                        columnHeadings.Add(new KeyValuePair<int, string>(days.Value, termText));
                }
            }
            columnHeadings.Sort(CompareValue);

            // we need to create a table with the same dimensions of rows and columns as the data set
            System.Web.UI.WebControls.Table sortedTable = new System.Web.UI.WebControls.Table();
            //sortedTable.CssClass = "table-bordered table-striped";
            //divStickyHeaderAndFirstColumnContainer.Controls.Add(sortedTable);

            System.Web.UI.WebControls.TableHeaderRow headerRow = new System.Web.UI.WebControls.TableHeaderRow();
            headerRow.TableSection = TableRowSection.TableHeader;

            headerRow.Cells.Add(RenderHeaderCell("Institutions", false));
            foreach (KeyValuePair<int, string> columnHeading in columnHeadings)
                headerRow.Cells.Add(RenderHeaderCell(columnHeading.Value, true));
            sortedTable.Rows.Add(headerRow);

            foreach (Insignis.Asset.Management.Tools.Helper.HeatmapInstitution institution in scurve.heatmap.heatmapInstitutions)
            {
                // format the name for display
                string tempInstitutionName = institution.institution.Name;
                if (tempInstitutionName.Contains("&amp;"))
                    tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");

                bool showInstitution = true;
                if (pSettings.ShowFitchRating)
                {
                    Management.Clients.Helper.InstitutionProperties institutionProperties = new Management.Clients.Helper.InstitutionProperties(institution.institution.PartyID.Value, System.Configuration.ConfigurationManager.AppSettings["preferencesRoot"]);
                    Octavo.Gate.Nabu.Preferences.Preference fitchRating = institutionProperties.preferences.GetChildPreference("FitchRating");
                    if (fitchRating != null && fitchRating.Value != null && fitchRating.Value.Trim().Length > 0)
                    {
                        tempInstitutionName += " (FIR: " + fitchRating.Value + ")";

                        // if the filter panel has defined a miniumum rating
                        if (pSettings.MinimumFitchRating.CompareTo("All") != 0)
                        {
                            showInstitution = false;
                            if (Insignis.Asset.Management.Clients.Helper.FitchRatings.IsRatingLessThanOrEqualTo(fitchRating.Value, pSettings.MinimumFitchRating))
                                showInstitution = true;
                        }
                    }
                }

                if (showInstitution)
                {
                    // create a row for the institution
                    System.Web.UI.WebControls.TableRow institutionRow = new System.Web.UI.WebControls.TableRow();

                    TableCell institutionNameCell = new TableCell();
                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = "_checkIncludeInstitution" + institution.institution.PartyID.ToString();
                    if (pInstitutionInclusion.GetChildPreference(institution.institution.PartyID.ToString()) != null && pInstitutionInclusion.GetChildPreference(institution.institution.PartyID.ToString()).Value != null && pInstitutionInclusion.GetChildPreference(institution.institution.PartyID.ToString()).Value.ToLower().CompareTo("true") == 0)
                        checkBox.Checked = true;
                    institutionNameCell.Controls.Add(checkBox);
                    Label label = new Label();
                    label.Text = Helper.TextFormatter.RemoveNonASCIICharacters(tempInstitutionName);
                    institutionNameCell.Controls.Add(label);
                    institutionNameCell.Style.Add("width", "200px !important");
                    institutionRow.Cells.Add(institutionNameCell);

                    bool hasRates = false;
                    // create the the institution row putting a blank if no liquidity days match
                    foreach (KeyValuePair<int, string> columnHeading in columnHeadings)
                    {
                        bool foundTerm = false;
                        int termCounter = 0;
                        foreach (Insignis.Asset.Management.Tools.Helper.HeatmapTerm term in institution.investmentTerms)
                        {
                            int? liquidityDays = term.InvestmentTerm.GetLiquidityDays();
                            if (liquidityDays.HasValue == false)
                                liquidityDays = 0;

                            if ((liquidityDays.Value >= columnHeading.Key - 3 && liquidityDays.Value <= columnHeading.Key + 3) && columnHeading.Value.ToLower().CompareTo(term.InvestmentTerm.GetText().ToLower()) == 0)
                            {
                                hasRates = true;
                                TableCell cellLink = new TableCell();
                                bool showLinks = false;
                                if (pSettings.FullProtection == false)
                                    showLinks = true;
                                else
                                {
                                    // if full protection is true, we need to con
                                    if (depositRows != null && depositRows.Count > 0)
                                    {
                                        decimal totalDepositsForInstitution = 0;
                                        foreach (IllustrationBuilderDepositRow depositRow in depositRows)
                                        {
                                            if (depositRow.institutionID == institution.institution.PartyID)
                                                totalDepositsForInstitution += depositRow.depositAmount;
                                        }

                                        if (pAvailableToHubAccountType.Detail.Alias.Contains("JOINTHUBACCOUNT"))
                                        {
                                            pSettings.ClientType = Tools.Sales.SCurveClientType.Joint;
                                            if (totalDepositsForInstitution < Convert.ToDecimal(_fscsValue.Value))
                                                showLinks = true;
                                        }
                                        else
                                        {
                                            if (totalDepositsForInstitution < Convert.ToDecimal(_fscsValue.Value))
                                                showLinks = true;
                                        }
                                    }
                                    else
                                        showLinks = true;
                                }

                                if (showLinks)
                                {
                                    HyperLink linkAddDeposit = new HyperLink();
                                    linkAddDeposit.Attributes.Add("onclick", "return openAddDepositDialog(" + institution.institution.PartyID + ",'" + tempInstitutionName + "','" + term.InvestmentTerm.GetText() + "'," + (term.AER50K.HasValue ? term.AER50K.Value : 0) + "," + (term.AER100K.HasValue ? term.AER100K.Value : 0) + "," + (term.AER250K.HasValue ? term.AER250K.Value : 0) + "," + termCounter + ");");
                                    linkAddDeposit.Text = term.GetBestRate().ToString("0.00") + "%";
                                    cellLink.Controls.Add(linkAddDeposit);
                                }
                                else
                                {
                                    cellLink.Text = term.GetBestRate().ToString("0.00") + "%";
                                }

                                bool isIncludedInProposedPortfolio = false;
                                foreach (Tools.Sales.SCurveOutputRow proposedInvestment in pProposedPortfolio.ProposedInvestments)
                                {
                                    if (proposedInvestment.InstitutionID == institution.institution.PartyID)
                                    {
                                        if (proposedInvestment.InvestmentTerm.GetLiquidityDays() == term.InvestmentTerm.GetLiquidityDays())
                                        {
                                            if (proposedInvestment.Rate == term.GetBestRate())
                                            {
                                                isIncludedInProposedPortfolio = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (isIncludedInProposedPortfolio == true)
                                    cellLink.Style.Add("background-color", "#B9F9B5");

                                institutionRow.Cells.Add(cellLink);
                                foundTerm = true;
                                break;
                            }
                            termCounter++;
                        }
                        if (foundTerm == false)
                            institutionRow.Cells.Add(RenderCell(""));
                    }
                    if(hasRates)
                        sortedTable.Rows.Add(institutionRow);
                }
            }

            _panelHeatmap.Controls.Add(Helper.UI.Controls.OGFixedTable.Render(sortedTable));
            DrawAddDepositDialog();
            DrawEditDepositDialog();
        }
        private TableHeaderCell RenderHeaderCell(string pText, bool pRotate)
        {
            TableHeaderCell cell = new TableHeaderCell();
            cell.ToolTip = pText;
            //if (pRotate == false)
            //{
            cell.Text = pText;
            //    cell.Style.Add("width", "200px !important");
            //}
            //else
            //    cell.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("<p class=\"text-rotation\">" + pText + "</p>"));
            return cell;
        }
        private TableCell RenderCell(string pText)
        {
            TableCell cell = new TableCell();
            cell.Text = pText;
            return cell;
        }
        private int CompareValue(KeyValuePair<int, string> a, KeyValuePair<int, string> b)
        {
            return a.Key.CompareTo(b.Key);
        }

        private void DrawAddDepositDialog()
        {
            animatedDialogScripts.Add(Management.Helper.UI.Dialog.DrawAnimatedDialogScript("ContentBody_" + "addDepositDialog", false, "blind", "explode", true, 770, 340));

            HtmlGenericControl addDepositDialogContainer = new HtmlGenericControl("div");
            addDepositDialogContainer.ID = "addDepositDialog";
            addDepositDialogContainer.Attributes.Add("title", "Add Deposit");

            System.Web.UI.WebControls.Table tableAddDeposit = new System.Web.UI.WebControls.Table();
            tableAddDeposit.CssClass = "table-bordered table-striped";

            TableHeaderRow tableHeaderRow = new TableHeaderRow();
            tableHeaderRow.TableSection = TableRowSection.TableHeader;
            TableHeaderCell hcellInstitution = new TableHeaderCell();
            hcellInstitution.Text = "Institution";
            tableHeaderRow.Cells.Add(hcellInstitution);
            TableHeaderCell hcellLiquidity = new TableHeaderCell();
            hcellLiquidity.Text = "Liquidity";
            tableHeaderRow.Cells.Add(hcellLiquidity);
            TableHeaderCell hcellRate = new TableHeaderCell();
            hcellRate.Text = "Rate";
            tableHeaderRow.Cells.Add(hcellRate);
            TableHeaderCell hcellDepositAmount = new TableHeaderCell();
            hcellDepositAmount.Text = "Deposit Amount";
            tableHeaderRow.Cells.Add(hcellDepositAmount);
            tableAddDeposit.Rows.Add(tableHeaderRow);

            TableRow tableRow = new TableRow();
            TableCell cellInstitution = new TableCell();
            cellInstitution.ID = "_dialogInstitutionName";
            tableRow.Cells.Add(cellInstitution);
            TableCell cellLiquidity = new TableCell();
            cellLiquidity.ID = "_dialogLiquidity";
            tableRow.Cells.Add(cellLiquidity);
            TableCell cellRate = new TableCell();
            Helper.UI.Controls.RateGuage rateGuage = new Helper.UI.Controls.RateGuage();
            cellRate.Controls.Add(rateGuage.Render("add"));
            tableRow.Cells.Add(cellRate);
            TableCell cellDepositAmount = new TableCell();
            TextBox textDepositAmount = new TextBox();
            textDepositAmount.ID = "_dialogDepositAmount";
            textDepositAmount.Attributes.Add("onkeydown", "return jsDecimals(event, this.id);");
            textDepositAmount.Attributes.Add("onkeyup", "amountChanged();");
            textDepositAmount.Style.Add("text-align", "right");
            cellDepositAmount.Controls.Add(textDepositAmount);
            tableRow.Cells.Add(cellDepositAmount);
            tableAddDeposit.Rows.Add(tableRow);

            TableRow rowSubmitRequest = new TableRow();
            TableCell cellSubmitRequest = new TableCell();
            cellSubmitRequest.ColumnSpan = 4;
            HtmlAnchor buttonSubmitRequest = new HtmlAnchor();
            buttonSubmitRequest.Attributes.Add("class", "btn");
            buttonSubmitRequest.Style.Add("float", "right");
            buttonSubmitRequest.Style.Add("margin-right", "5px");
            buttonSubmitRequest.Style.Add("color", "white");
            buttonSubmitRequest.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Add Deposit"));
            buttonSubmitRequest.Attributes.Add("onclick", "submitDeposit();");
            cellSubmitRequest.Controls.Add(buttonSubmitRequest);
            rowSubmitRequest.Cells.Add(cellSubmitRequest);
            tableAddDeposit.Rows.Add(rowSubmitRequest);

            addDepositDialogContainer.Controls.Add(tableAddDeposit);

            animatedDialogBodies.Add(addDepositDialogContainer);
        }
        private void DrawEditDepositDialog()
        {
            animatedDialogScripts.Add(Management.Helper.UI.Dialog.DrawAnimatedDialogScript("ContentBody_" + "editDepositDialog", false, "blind", "explode", true, 770, 340));

            HtmlGenericControl addDepositDialogContainer = new HtmlGenericControl("div");
            addDepositDialogContainer.ID = "editDepositDialog";
            addDepositDialogContainer.Attributes.Add("title", "Edit Deposit");

            System.Web.UI.WebControls.Table tableAddDeposit = new System.Web.UI.WebControls.Table();
            tableAddDeposit.CssClass = "table-bordered table-striped";

            TableHeaderRow tableHeaderRow = new TableHeaderRow();
            tableHeaderRow.TableSection = TableRowSection.TableHeader;
            TableHeaderCell hcellInstitution = new TableHeaderCell();
            hcellInstitution.Text = "Institution";
            tableHeaderRow.Cells.Add(hcellInstitution);
            TableHeaderCell hcellLiquidity = new TableHeaderCell();
            hcellLiquidity.Text = "Liquidity";
            tableHeaderRow.Cells.Add(hcellLiquidity);
            TableHeaderCell hcellRate = new TableHeaderCell();
            hcellRate.Text = "Rate";
            tableHeaderRow.Cells.Add(hcellRate);
            TableHeaderCell hcellDepositAmount = new TableHeaderCell();
            hcellDepositAmount.Text = "Deposit Amount";
            tableHeaderRow.Cells.Add(hcellDepositAmount);
            tableAddDeposit.Rows.Add(tableHeaderRow);

            TableRow tableRow = new TableRow();
            TableCell cellInstitution = new TableCell();
            cellInstitution.ID = "_dialogEditInstitutionName";
            tableRow.Cells.Add(cellInstitution);
            TableCell cellLiquidity = new TableCell();
            cellLiquidity.ID = "_dialogEditLiquidity";
            tableRow.Cells.Add(cellLiquidity);
            TableCell cellRate = new TableCell();
            Helper.UI.Controls.RateGuage rateGuage = new Helper.UI.Controls.RateGuage();
            cellRate.Controls.Add(rateGuage.Render("edit"));
            tableRow.Cells.Add(cellRate);
            TableCell cellDepositAmount = new TableCell();
            TextBox textDepositAmount = new TextBox();
            textDepositAmount.ID = "_dialogEditDepositAmount";
            textDepositAmount.Attributes.Add("onkeydown", "return jsDecimals(event, this.id);");
            textDepositAmount.Attributes.Add("onkeyup", "editAmountChanged();");
            textDepositAmount.Style.Add("text-align", "right");
            cellDepositAmount.Controls.Add(textDepositAmount);
            tableRow.Cells.Add(cellDepositAmount);
            HiddenField hidden = new HiddenField();
            hidden.ID = "_dialogEditDepositGUID";
            cellDepositAmount.Controls.Add(hidden);
            tableAddDeposit.Rows.Add(tableRow);

            TableRow rowSubmitRequest = new TableRow();
            TableCell cellSubmitRequest = new TableCell();
            cellSubmitRequest.ColumnSpan = 4;
            HtmlAnchor buttonSubmitRequest = new HtmlAnchor();
            buttonSubmitRequest.Attributes.Add("class", "btn");
            buttonSubmitRequest.Style.Add("float", "right");
            buttonSubmitRequest.Style.Add("margin-right", "5px");
            buttonSubmitRequest.Style.Add("color", "white");
            buttonSubmitRequest.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Update"));
            buttonSubmitRequest.Attributes.Add("onclick", "updateDeposit();");
            cellSubmitRequest.Controls.Add(buttonSubmitRequest);
            HtmlAnchor buttonRemoveDeposit = new HtmlAnchor();
            buttonRemoveDeposit.Attributes.Add("class", "btn");
            buttonRemoveDeposit.Style.Add("float", "right");
            buttonRemoveDeposit.Style.Add("margin-right", "5px");
            buttonRemoveDeposit.Style.Add("color", "white");
            buttonRemoveDeposit.Controls.Add(Management.Helper.UI.Theme.DrawLiteral("Remove"));
            buttonRemoveDeposit.Attributes.Add("onclick", "editRemoveDeposit();");
            cellSubmitRequest.Controls.Add(buttonRemoveDeposit);
            rowSubmitRequest.Cells.Add(cellSubmitRequest);
            tableAddDeposit.Rows.Add(rowSubmitRequest);

            addDepositDialogContainer.Controls.Add(tableAddDeposit);

            animatedDialogBodies.Add(addDepositDialogContainer);
        }

        // Persistence of proposed portfolio table
        private void SavePortfolio(Tools.Sales.SCurveOutput pProposedPortfolio, int pAvailableToHubAccountTypeID)
        {
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));

            Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + pAvailableToHubAccountTypeID, 1, "Deposits");
            if (scurveDeposits == null || scurveDeposits.Children.Count == 0)
                scurveDeposits = new Octavo.Gate.Nabu.Preferences.Preference("Deposits", "");

            foreach (Tools.Sales.SCurveOutputRow proposedInvestment in pProposedPortfolio.ProposedInvestments)
            {
                Octavo.Gate.Nabu.Preferences.Preference newDeposit = new Octavo.Gate.Nabu.Preferences.Preference();
                newDeposit.ID = Guid.NewGuid();
                proposedInvestment.ID = newDeposit.ID;
                string tempInstitutionName = proposedInvestment.InstitutionName;
                if (tempInstitutionName.Contains("&amp;"))
                    tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionName", Helper.TextFormatter.RemoveNonASCIICharacters(tempInstitutionName)));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionID", proposedInvestment.InstitutionID.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("DepositAmount", proposedInvestment.DepositSize.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER100K", proposedInvestment.AER100K.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER250K", proposedInvestment.AER250K.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER50K", proposedInvestment.AER50K.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AnnualInterest", proposedInvestment.AnnualInterest.ToString("0.00")));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InterestPaid", proposedInvestment.InterestPaid));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MinimumInvestment", proposedInvestment.MinimumInvestment.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MaximumInvestment", proposedInvestment.MaximumInvestment.ToString()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InvestmentTerm", proposedInvestment.InvestmentTerm.ConvertToSemiColonSeparatedValues()));
                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionShortName", proposedInvestment.InstitutionShortName));
                scurveDeposits.Children.Add(newDeposit);
            }
            preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + pAvailableToHubAccountTypeID, 1, scurveDeposits);
        }
        private Tools.Sales.SCurveOutput LoadPortfolio(int pAvailableToHubAccountTypeID)
        {
            Tools.Sales.SCurveOutput proposedPortfolio = new Tools.Sales.SCurveOutput();

            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));

            Octavo.Gate.Nabu.Preferences.Preference savedManualDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + pAvailableToHubAccountTypeID, 1, "Deposits");
            if (savedManualDeposits != null && savedManualDeposits.Children.Count > 0)
            {
                foreach (Octavo.Gate.Nabu.Preferences.Preference savedDepositPreference in savedManualDeposits.Children)
                {
                    try
                    {
                        // Now add any saved deposits to the proposed portfolio
                        Tools.Sales.SCurveOutputRow proposedInvestment = new Tools.Sales.SCurveOutputRow();
                        proposedInvestment.ID = savedDepositPreference.ID;
                        proposedInvestment.DepositSize = Convert.ToDecimal(savedDepositPreference.GetChildPreference("DepositAmount").Value);
                        proposedInvestment.InstitutionID = Convert.ToInt32(savedDepositPreference.GetChildPreference("InstitutionID").Value);
                        proposedInvestment.InstitutionName = savedDepositPreference.GetChildPreference("InstitutionName").Value;
                        proposedInvestment.InstitutionShortName = savedDepositPreference.GetChildPreference("InstitutionShortName").Value;

                        Insignis.Asset.Management.Tools.Helper.HeatmapTerm heatmapTerm = new Tools.Helper.HeatmapTerm();
                        heatmapTerm.InvestmentTerm = new InvestmentTerm();
                        heatmapTerm.InvestmentTerm.ConvertFromSemiColonSeparatedValues(savedDepositPreference.GetChildPreference("InvestmentTerm").Value);
                        heatmapTerm.MinimumInvestment = Convert.ToDecimal(savedDepositPreference.GetChildPreference("MinimumInvestment").Value);
                        heatmapTerm.MaximumInvestment = Convert.ToDecimal(savedDepositPreference.GetChildPreference("MaximumInvestment").Value);
                        heatmapTerm.AER50K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER50K").Value);
                        heatmapTerm.AER100K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER100K").Value);
                        heatmapTerm.AER250K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER250K").Value);
                        heatmapTerm.InterestPaid = savedDepositPreference.GetChildPreference("InterestPaid").Value;

                        proposedInvestment.InvestmentTerm = heatmapTerm.InvestmentTerm;
                        proposedInvestment.Rate = heatmapTerm.GetBestRate();
                        proposedInvestment.CalculateAnnualInterest();
                        proposedPortfolio.ProposedInvestments.Add(proposedInvestment);
                    }
                    catch
                    {
                    }
                }
                proposedPortfolio.TotalDeposited = 0;
                proposedPortfolio.AnnualGrossInterestEarned = 0;
            }

            return proposedPortfolio;
        }

        private void SavePortfolioProperties(Tools.Sales.SCurveOutput pProposedPortfolio, int pAvailableToHubAccountTypeID)
        {
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
            preferencesManager.DeletePreferences("Sales.Tools.SCurve.Properties." + pAvailableToHubAccountTypeID, 1);

            Octavo.Gate.Nabu.Preferences.Preference scurveProperties = new Octavo.Gate.Nabu.Preferences.Preference("Properties");

            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("TOTAL", pProposedPortfolio.TotalDeposited.ToString("#,##0.00")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("GROSSINTEREST", pProposedPortfolio.AnnualGrossInterestEarned.ToString("#,##0.00")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("FEE", pProposedPortfolio.FeePercentage.ToString("0.000")));
            decimal charge = 0;
            try
            {
                charge = ((pProposedPortfolio.FeePercentage / 100) * pProposedPortfolio.TotalDeposited);
            }
            catch
            {
            }
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("CHARGE", charge.ToString("#,##0.00")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("NETINTEREST", pProposedPortfolio.AnnualNetInterestEarned.ToString("#,##0.00")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("GROSSYIELD", pProposedPortfolio.GrossAverageYield.ToString("0.000")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("NETYIELD", pProposedPortfolio.NetAverageYield.ToString("0.000")));
            scurveProperties.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("FSCSELIGIBILITY", pProposedPortfolio.FSCSPercentProtected.ToString("0")));

            preferencesManager.SetPreference("Sales.Tools.SCurve.Properties." + pAvailableToHubAccountTypeID, 1, scurveProperties);
        }
    }

    public class IllustrationBuilderDepositRow
    {
        public Guid ID = Guid.Empty;
        public int institutionID;
        public string institutionName = "";
        public string institutionShortName = "";
        public InvestmentTerm investmentTerm = null;
        public string errorMessage = "";
        public decimal aer50K = 0;
        public decimal aer100K = 0;
        public decimal aer250K = 0;
        public decimal minimumInvestment = 0;
        public decimal maximumInvestment = 0;
        public decimal depositAmount = 0;
        public decimal selectedRate = 0;

        public IllustrationBuilderDepositRow(Octavo.Gate.Nabu.Preferences.Preference pDeposit)
        {
            ID = pDeposit.ID;
            institutionName = pDeposit.GetChildPreference("InstitutionName").Value;
            institutionID = Convert.ToInt32(pDeposit.GetChildPreference("InstitutionID").Value);
            institutionShortName = pDeposit.GetChildPreference("InstitutionShortName").Value;

            investmentTerm = new InvestmentTerm();
            investmentTerm.ConvertFromSemiColonSeparatedValues(pDeposit.GetChildPreference("InvestmentTerm").Value);

            try
            {
                aer100K = Convert.ToDecimal(pDeposit.GetChildPreference("AER100K").Value);
            }
            catch
            {
            }
            try
            {
                aer250K = Convert.ToDecimal(pDeposit.GetChildPreference("AER250K").Value);
            }
            catch
            {
            }
            try
            {
                aer50K = Convert.ToDecimal(pDeposit.GetChildPreference("AER50K").Value);
            }
            catch
            {
            }
            try
            {
                string minDep = pDeposit.GetChildPreference("MinimumInvestment").Value;
                if (minDep.Length == 0 || minDep.Trim().Length == 0 || minDep.Trim().CompareTo("-") == 0)
                    minimumInvestment = 0;
                else
                    minimumInvestment = Convert.ToDecimal(pDeposit.GetChildPreference("MinimumInvestment").Value);
            }
            catch
            {
            }
            try
            {
                string maxDep = pDeposit.GetChildPreference("MaximumInvestment").Value;
                if (maxDep.Length == 0 || maxDep.Trim().Length == 0 || maxDep.Trim().CompareTo("-") == 0)
                    maximumInvestment = 2000000;
                else
                    maximumInvestment = Convert.ToDecimal(pDeposit.GetChildPreference("MaximumInvestment").Value);
            }
            catch
            {
            }
            try
            {
                depositAmount = Convert.ToDecimal(pDeposit.GetChildPreference("DepositAmount").Value);
            }
            catch
            {
            }
            if (depositAmount > 0)
            {
                if (minimumInvestment > 0 && depositAmount >= minimumInvestment)
                {
                }
                else if (minimumInvestment == 0)
                {
                    // don't do anything because zero for a minimum is ok
                }
                else
                    errorMessage = "Amount is less than minimum";

                if (errorMessage.Length == 0)
                {
                    if (maximumInvestment > 0 && depositAmount <= maximumInvestment)
                    {
                    }
                    else
                        errorMessage = "Amount is greater than maximum";
                }
                if (errorMessage.Length == 0)
                {
                    if (depositAmount <= 50000)
                        selectedRate = aer50K;
                    else if (depositAmount <= 100000)
                        selectedRate = aer100K;
                    else
                        selectedRate = aer250K;
                }
            }
            else
                errorMessage = "Amount is zero";
        }
    }
}
