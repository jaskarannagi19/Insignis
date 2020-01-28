using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Http;
using InsignisIllustrationGenerator.Manager;
using Newtonsoft.Json;
using InsignisIllustrationGenerator.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using InsignisIllustrationGenerator.Helper;
using Microsoft.Extensions.Options;
using Octavo.Gate.Nabu.Abstraction;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Bibliography;

namespace InsignisIllustrationGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IConfiguration _configuration;
        private AppSettings AppSettings { get; set; }

        //Session State Management
        public void SetSession()
        {
            /*Sets user session
             Arguments:- None
             Returns:- None
             
             */

            //Set Info in session
            var session = new Session() { PartnerEmailAddress = "p.artner@partorg.com", PartnerName = "Peter Artner", PartnerOrganisation = "PartOrg Ltd.", PartnerTelephone = "01226 1234 567" };

            HttpContext.Session.SetString("SessionPartner",JsonConvert.SerializeObject(session));
            

            //if (Session["_partnerOrganisation"] != null && Session["_partnerOrganisation"].ToString().Length > 0 &&
            //   Session["_partnerName"] != null && Session["_partnerName"].ToString().Length > 0 &&
            //   Session["_partnerEmailAddress"] != null && Session["_partnerEmailAddress"].ToString().Length > 0 &&
            //   Session["_partnerTelephone"] != null && Session["_partnerTelephone"].ToString().Length > 0)
            //{
            //    Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()), false);
            //    Octavo.Gate.Nabu.Preferences.Preference scurveBuilder = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder", 1, "Settings");
            //    if (scurveBuilder == null)
            //        Response.Redirect("SCurveAvailableTo.aspx?reset=true");
            //    else
            //        Response.Redirect("SCurveAvailableTo.aspx");
            //}


        } 


        public HomeController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _mapper = mapper;
            AppSettings = settings.Value;
        }

        public IActionResult Index()
        {
            /*
             Create Illusration Detail Page
             Arguments:- None
             Returns:- IllustrationModel and Display to user
             */
            //Set Dumy Session
            SetSession();
            //Get Session Values;
            var partnerInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));

            IllustrationDetailViewModel model = new IllustrationDetailViewModel();
            model.PartnerEmail = partnerInfo.PartnerEmailAddress;
            model.PartnerName = partnerInfo.PartnerName;
            model.PartnerOrganisation = partnerInfo.PartnerOrganisation;
            
            //render view
            return View(model);
        }
        
        public IActionResult Calculate(IllustrationDetailViewModel model)
        {
            /*
             Post Method
             Arguments:- IllustrationDetailViewModel 
             Returns:- View and Errors
             */
            if (ModelState.IsValid) {
                var illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));

                illustrationInfo.ClientName = model.ClientName;
                illustrationInfo.ClientType = model.ClientType;
                illustrationInfo.Currency = model.Currency;
                illustrationInfo.EasyAccess = model.EasyAccess;
                illustrationInfo.OneMonth = model.OneMonth;
                illustrationInfo.ThreeMonths = model.ThreeMonths;
                illustrationInfo.SixMonths = model.SixMonths;
                illustrationInfo.NineMonths = model.NineMonths;
                illustrationInfo.OneYear = model.OneYear;
                illustrationInfo.TwoYears = model.TwoYears;
                illustrationInfo.ThreeYears = model.ThreeYearsPlus;
                illustrationInfo.TotalDeposit = model.TotalDeposit;
                HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(illustrationInfo));




















            //    if (redirecting == false)
            //{
            //    string fscsProtectionConfigFile = ConfigurationManager.AppSettings["clientConfigRoot"];
            //    if (fscsProtectionConfigFile.EndsWith("\\") == false)
            //        fscsProtectionConfigFile += "\\";
            //    fscsProtectionConfigFile += "FSCSProtection.xml";

            //    scurve.LoadHeatmap(availableToHubAccountTypeID, currencyCode, System.Configuration.ConfigurationManager.AppSettings["preferencesRoot"]);

            //    Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = ProcessPostback(skipPostback, scurve.heatmap);

            //    scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");

            //    // display the settings
            //    _panelTop.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Product/Inputs"));
            //    _panelTop.Controls.Add(RenderSettings(settings, false));

            //    _panelTopLeft.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Liquidity Requirements"));
            //    System.Web.UI.WebControls.Table liquidityRequirements = RenderLiquidityRequirements(settings, scurve.ListAllTerms(), false);
            //    _panelTopLeft.Controls.Add(SplitTable(liquidityRequirements, "top"));
            //    _panelTopRight.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Liquidity Requirements"));
            //    _panelTopRight.Controls.Add(SplitTable(liquidityRequirements, "bottom"));

            //    HtmlGenericControl p = new HtmlGenericControl("p");
            //    p.Style.Add("width", "100%");
            //    p.Style.Add("text-align", "right");
            //    HtmlAnchor linkReset = new HtmlAnchor();
            //    linkReset.Attributes.Add("class", "btn");
            //    linkReset.Style.Add("margin-right", "15px");
            //    linkReset.InnerText = "Reset";
            //    linkReset.Title = "Reset to defaults";
            //    linkReset.HRef = "SCurveAvailableTo.aspx?reset=true";
            //    linkReset.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //    p.Controls.Add(linkReset);
            //    HtmlInputSubmit buttonCalculate = new HtmlInputSubmit();
            //    buttonCalculate.Attributes.Add("class", "btn");
            //    buttonCalculate.ID = "_buttonCalculate";
            //    buttonCalculate.Value = "Calculate";
            //    buttonCalculate.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //    if (HideCalculateButton)
            //        buttonCalculate.Style.Add("display", "none");
            //    p.Controls.Add(buttonCalculate);
            //    _panelTopRight.Controls.Add(p);

            //    // generate the scurve
            //    Tools.Sales.SCurveOutput proposedPortfolio = null;
            //    if (IsPostBack && skipPostback == true)
            //    {
            //        try
            //        {
            //            Helper.UI.Form formHelper = new Helper.UI.Form();
            //            if (_userAction.Value.CompareTo("addDeposit") == 0)
            //            {
            //                Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
            //                if (scurveDeposits == null || scurveDeposits.Children.Count == 0)
            //                    scurveDeposits = new Octavo.Gate.Nabu.Preferences.Preference("Deposits", "");

            //                int selectedInstitutionID = Convert.ToInt32(_addDepositForInstitutionID.Value);
            //                int selectedTermIndex = Convert.ToInt32(_addDepositForTermIndex.Value);
            //                decimal depositAmount = Convert.ToDecimal(formHelper.GetInput("_dialogDepositAmount", Request));

            //                foreach (Insignis.Asset.Management.Tools.Helper.HeatmapInstitution institution in scurve.heatmap.heatmapInstitutions)
            //                {
            //                    if (institution.institution.PartyID == selectedInstitutionID)
            //                    {
            //                        int termIndex = 0;
            //                        foreach (Insignis.Asset.Management.Tools.Helper.HeatmapTerm term in institution.investmentTerms)
            //                        {
            //                            if (termIndex == selectedTermIndex)
            //                            {
            //                                Octavo.Gate.Nabu.Preferences.Preference newDeposit = new Octavo.Gate.Nabu.Preferences.Preference();
            //                                string tempInstitutionName = institution.institution.Name;
            //                                if (tempInstitutionName.Contains("&amp;"))
            //                                    tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionName", Helper.TextFormatter.RemoveNonASCIICharacters(tempInstitutionName)));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionID", institution.institution.PartyID.Value.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("DepositAmount", depositAmount.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER100K", term.AER100K.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER250K", term.AER250K.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AER50K", term.AER50K.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InterestPaid", term.InterestPaid));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MinimumInvestment", term.MinimumInvestment.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("MaximumInvestment", term.MaximumInvestment.ToString()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InvestmentTerm", term.InvestmentTerm.ConvertToSemiColonSeparatedValues()));
            //                                newDeposit.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("InstitutionShortName", institution.institution.ShortName));
            //                                scurveDeposits.Children.Add(newDeposit);
            //                                break;
            //                            }
            //                            termIndex++;
            //                        }
            //                        break;
            //                    }
            //                }
            //                preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
            //            }
            //            else if (_userAction.Value.CompareTo("updateDeposit") == 0)
            //            {
            //                Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
            //                if (scurveDeposits != null && scurveDeposits.Children.Count > 0)
            //                {
            //                    foreach (Octavo.Gate.Nabu.Preferences.Preference depositAsPreference in scurveDeposits.Children)
            //                    {
            //                        if (depositAsPreference.ID.CompareTo(Guid.Parse(_userValue.Value)) == 0)
            //                        {
            //                            decimal depositAmount = Convert.ToDecimal(formHelper.GetInput("_dialogEditDepositAmount", Request));
            //                            depositAsPreference.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("DepositAmount", depositAmount.ToString()));
            //                            break;
            //                        }
            //                    }
            //                    preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
            //                }
            //            }
            //            else if (_userAction.Value.CompareTo("removeDeposit") == 0)
            //            {
            //                Octavo.Gate.Nabu.Preferences.Preference scurveDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, "Deposits");
            //                if (scurveDeposits != null && scurveDeposits.Children.Count > 0)
            //                {
            //                    scurveDeposits.RemoveChildPreference(Guid.Parse(_userValue.Value));
            //                    preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountTypeID, 1, scurveDeposits);
            //                }
            //            }
            //        }
            //        catch
            //        {
            //        }
            //    }

            //    Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");
            //    if (institutionInclusion == null || institutionInclusion.Children.Count == 0)
            //    {
            //        institutionInclusion = new Octavo.Gate.Nabu.Preferences.Preference("Institutions", "");

            //        Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
            //        foreach (Institution institution in allInstitutions)
            //        {
            //            if (institution.ShortName.CompareTo("NationalSavingsInvestments") != 0)
            //                institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
            //            else
            //                institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
            //        }
            //    }

            //    if (calculatePortfolio)
            //    {
            //        proposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);
            //        SavePortfolio(proposedPortfolio, availableToHubAccountTypeID);
            //    }
            //    else
            //    {
            //        proposedPortfolio = LoadPortfolio(availableToHubAccountTypeID);
            //        scurve.Calculate(proposedPortfolio, settings);
            //        //scurve.Calculate(proposedPortfolio, settings, fscsProtectionConfigFile);
            //    }

            //    proposedPortfolio.FeePercentage = feeMatrix.GetRateForDeposit(proposedPortfolio.TotalDeposited);
            //    if (settings.CurrencyCode.CompareTo("USD") == 0 || settings.CurrencyCode.CompareTo("EUR") == 0)
            //        proposedPortfolio.FeePercentage += Convert.ToDecimal("0.05");
            //    try
            //    {
            //        if (scurvePreferences.GetChildPreference("FeeDiscount") != null && scurvePreferences.GetChildPreference("FeeDiscount").Value.Length > 0)
            //        {
            //            decimal feeDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("FeeDiscount").Value);
            //            if (feeDiscount > 0)
            //            {
            //                if (feeDiscount < proposedPortfolio.FeePercentage)
            //                    proposedPortfolio.FeePercentage -= feeDiscount;
            //                else
            //                    proposedPortfolio.FeePercentage = 0;
            //            }
            //        }

            //        if (scurvePreferences.GetChildPreference("IntroducerDiscount") != null && scurvePreferences.GetChildPreference("IntroducerDiscount").Value.Length > 0)
            //        {
            //            decimal introducerDiscount = Convert.ToDecimal(scurvePreferences.GetChildPreference("IntroducerDiscount").Value);
            //            if (introducerDiscount > 0)
            //            {
            //                if (introducerDiscount < proposedPortfolio.FeePercentage)
            //                    proposedPortfolio.FeePercentage -= introducerDiscount;
            //                else
            //                    proposedPortfolio.FeePercentage = 0;
            //            }
            //        }

            //        proposedPortfolio.NetAverageYield = (proposedPortfolio.GrossAverageYield - proposedPortfolio.FeePercentage);

            //        proposedPortfolio.Fee = (proposedPortfolio.TotalDeposited * (decimal)(proposedPortfolio.FeePercentage / 100));

            //        proposedPortfolio.AnnualNetInterestEarned = (proposedPortfolio.AnnualGrossInterestEarned - proposedPortfolio.Fee);
            //    }
            //    catch
            //    {
            //    }

            //    // Draw the heatmap
            //    DrawHeatmap(settings, scurve, proposedPortfolio, institutionInclusion, financialAbstraction.GetAccountType(Convert.ToInt32(settings.AvailableToHubAccountTypeID), (int)multiLingual.language.LanguageID));

            //    // output the table to the bottom left column
            //    _panelBottomLeft.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Deposits"));
            //    System.Web.UI.WebControls.Table tableProposedPortfolio = RenderProposedPortfolio(proposedPortfolio, scurvePreferences, false, settings.ShowFitchRating, settings.AnonymiseDeposits);
            //    _panelBottomLeft.Controls.Add(tableProposedPortfolio);
            //    if (tableProposedPortfolio.Rows.Count > 1)
            //    {
            //        HtmlGenericControl pTableMenu = new HtmlGenericControl("p");
            //        pTableMenu.Style.Add("width", "100%;");
            //        pTableMenu.Style.Add("text-align", "right");
            //        _panelBottomLeft.Controls.Add(pTableMenu);

            //        HtmlAnchor linkClearDeposits = new HtmlAnchor();
            //        linkClearDeposits.Attributes.Add("class", "btn");
            //        linkClearDeposits.Style.Add("margin-right", "15px");
            //        linkClearDeposits.InnerText = "Clear";
            //        linkClearDeposits.Title = "Clear Deposits Table";
            //        linkClearDeposits.HRef = Helper.DomainRoot.Get() + "Illustrator/SCurveAvailableTo.aspx?clear=true";
            //        linkClearDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //        pTableMenu.Controls.Add(linkClearDeposits);

            //        if (settings.AnonymiseDeposits == false)
            //        {
            //            HtmlAnchor linkAnonymiseDeposits = new HtmlAnchor();
            //            linkAnonymiseDeposits.ID = "_buttonAnonymiseDeposits";
            //            linkAnonymiseDeposits.Attributes.Add("class", "btn");
            //            linkAnonymiseDeposits.Style.Add("margin-right", "15px");
            //            linkAnonymiseDeposits.InnerText = "Anonymise";
            //            linkAnonymiseDeposits.Attributes.Add("title", "Anonymise Banks");
            //            linkAnonymiseDeposits.HRef = "javascript:void(0);";
            //            linkAnonymiseDeposits.Attributes.Add("onclick", "return Anonymise();");
            //            linkAnonymiseDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //            pTableMenu.Controls.Add(linkAnonymiseDeposits);
            //        }
            //        else
            //        {
            //            HtmlAnchor linkUnAnonymiseDeposits = new HtmlAnchor();
            //            linkUnAnonymiseDeposits.ID = "_buttonUnAnonymiseDeposits";
            //            linkUnAnonymiseDeposits.Attributes.Add("class", "btn");
            //            linkUnAnonymiseDeposits.Style.Add("margin-right", "15px");
            //            linkUnAnonymiseDeposits.InnerText = "Un-Anonymise";
            //            linkUnAnonymiseDeposits.Attributes.Add("title", "Un-Anonymise Banks");
            //            linkUnAnonymiseDeposits.HRef = "javascript:void(0);";
            //            linkUnAnonymiseDeposits.Attributes.Add("onclick", "return UnAnonymise();");
            //            linkUnAnonymiseDeposits.Attributes.Add("onkeypress", "return disableEnterKey(event);");
            //            pTableMenu.Controls.Add(linkUnAnonymiseDeposits);
            //        }
            //    }

            //    // output the table to the bottom right column
            //    _panelBottomRight.Controls.Add(Management.Helper.UI.Theme.DrawHeadLine2("Properties"));
            //    _panelBottomRight.Controls.Add(RenderProperties(proposedPortfolio));
            //    SavePortfolioProperties(proposedPortfolio, availableToHubAccountTypeID);

            //    // popup if proposed portfolio total is less than the total within the liquidity boxes
            //    if (proposedPortfolio.TotalDeposited < Convert.ToDecimal(_hiddenCalculatedTotalDeposit.Value))
            //    {
            //        // show popup - the document ready will render a dialog if this value is true
            //        Helper.UI.Form formHelper = new Helper.UI.Form();
            //        if (formHelper.ControlExists("_buttonCalculate", Request))
            //        {
            //            // we only want to show this dialog if we have just clicked Calculate
            //            _hiddenShowLiquidityWarningDialog.Value = "true";
            //        }
            //        else
            //            _hiddenShowLiquidityWarningDialog.Value = "false";
            //    }
            //    else
            //        _hiddenShowLiquidityWarningDialog.Value = "false";

            //    if (tableProposedPortfolio.Rows.Count > 1)
            //    {
            //        HtmlGenericControl belowTableMenu = new HtmlGenericControl("p");
            //        belowTableMenu.Style.Add("width", "100%;");
            //        belowTableMenu.Style.Add("text-align", "right");
            //        _panelBottomRight.Controls.Add(belowTableMenu);

            //        HtmlGenericControl span = new HtmlGenericControl("span");
            //        span.InnerHtml = "Generate Illustration From Template:<br/>";
            //        belowTableMenu.Controls.Add(span);

            //        DropDownList comboSelectTemplate = new DropDownList();
            //        comboSelectTemplate.Attributes.Add("onchange", "generateTemplatedIllustration(this);");
            //        comboSelectTemplate.Items.Add(new ListItem("Choose Template", "-1"));
            //        if (ConfigurationManager.AppSettings["illustrationTemplateRoot"] != null && System.IO.Directory.Exists(ConfigurationManager.AppSettings["illustrationTemplateRoot"]))
            //        {
            //            Octavo.Gate.Nabu.Encryption.EncryptorDecryptor encryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
            //            string[] organisationIDFolders = System.IO.Directory.GetDirectories(ConfigurationManager.AppSettings["illustrationTemplateRoot"]);
            //            foreach (string organisationIDFolder in organisationIDFolders)
            //            {
            //                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(organisationIDFolder);
            //                Octavo.Gate.Nabu.Entities.Core.Organisation organisation = coreAbstraction.GetOrganisation(Convert.ToInt32(di.Name), (int)multiLingual.language.LanguageID);
            //                bool showTemplate = false;
            //                if (organisation.Name.ToUpper().StartsWith("INSIGNIS"))
            //                    showTemplate = true;
            //                else
            //                    showTemplate = ShowTemplate(Convert.ToInt32(di.Name), ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()));
            //                if (showTemplate)
            //                {
            //                    comboSelectTemplate.Items.Add(new ListItem(organisation.Name, "-2"));
            //                    string[] templateFiles = System.IO.Directory.GetFiles(organisationIDFolder);
            //                    foreach (string templateFile in templateFiles)
            //                    {
            //                        System.IO.FileInfo fi = new System.IO.FileInfo(templateFile);
            //                        comboSelectTemplate.Items.Add(new ListItem(" + " + fi.Name, encryptor.UrlEncode(encryptor.Encrypt(templateFile))));
            //                    }
            //                }
            //            }
            //        }
            //        belowTableMenu.Controls.Add(comboSelectTemplate);
            //    }
            //}

            }
            return View(model);
        }
        public IActionResult GenerateIllustration()
        {
            /*
             Generate Illustration for using the data
             Arguments:- 
                BankModel with Values
             Returns:- 
                View

             */
            string fileName = "PPT/illustration.pptx";

            Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction powerpointRenderAbstraction = new Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction(AppSettings.illustrationOutputInternalFolder, AppSettings.illustrationOutputPublicFacingFolder);
            List<KeyValuePair<string, string>> textReplacements = new List<KeyValuePair<string, string>>();

            textReplacements.Add(new KeyValuePair<string, string>("REFERENCE", "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss")));
            textReplacements.Add(new KeyValuePair<string, string>("DATE", DateTime.Now.ToString("dd/MM/yyyy")));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTNAME", "Jaskaran"));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTTYPE", "Individual"));
            textReplacements.Add(new KeyValuePair<string, string>("INTROORG", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEEDISCOUNT", "100%"));
            textReplacements.Add(new KeyValuePair<string, string>("FEE",""));
            textReplacements.Add(new KeyValuePair<string, string>("CHARGE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("TOTAL", "10,0000"));
            textReplacements.Add(new KeyValuePair<string, string>("PROTECTION", "65000"));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSYIELD", "200%"));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSINTEREST", "16"));
            textReplacements.Add(new KeyValuePair<string, string>("NETYIELD", "17"));
            textReplacements.Add(new KeyValuePair<string, string>("NETINTEREST", "19"));


            Octavo.Gate.Nabu.Encryption.EncryptorDecryptor decryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
            string qsTemplateFile = "C:\\InsignisAM\\NET\\ExternalIllustrator\\ExternalIllustrator\\Template\\1\\illustration.pptx";// decryptor.Decrypt(decryptor.UrlDecode(Request.QueryString["T"]));
            System.IO.FileInfo templateFile = new System.IO.FileInfo(qsTemplateFile);

            string requiredOutputNameWithoutExtension = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss") + "_CashIllustration";

            Insignis.Asset.Management.Reports.Helper.ExtendedReportContent extendedReportContent = powerpointRenderAbstraction.MergeDataWithPowerPointTemplate("ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss"), textReplacements, templateFile.FullName, requiredOutputNameWithoutExtension, true);



            return View();
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
