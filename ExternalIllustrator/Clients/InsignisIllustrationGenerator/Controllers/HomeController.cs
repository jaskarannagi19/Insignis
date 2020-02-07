using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Http;
using InsignisIllustrationGenerator.Manager;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using InsignisIllustrationGenerator.Helper;
using Microsoft.Extensions.Options;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Financial;
using Spire.Presentation;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Scheduler;

namespace InsignisIllustrationGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        private readonly FetchDataTask _fetchDataTask;
        private readonly DataProvider _dataprovider;

        private AppSettings AppSettings { get; set; }

        private MultiLingual multiLingual;


        private FinancialAbstraction financialAbstraction { get; set; }



        //Session State Management
        public void SetSession()
        {
            /*Sets user session
             Arguments:- None
             Returns:- None
             
             */

            //Set Info in session
            var session = new Session() { PartnerEmailAddress = "p.artner@partorg.com", PartnerName = "Peter Artner", PartnerOrganisation = "PartOrg Ltd.", PartnerTelephone = "01226 1234 567" };

            HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(session));


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

        private readonly BankHelper _bankHelper;
        public HomeController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings, ApplicationDbContext context, DataProvider data)
        {
            _logger = logger;
            _mapper = mapper;
            AppSettings = settings.Value;
            multiLingual = new MultiLingual(AppSettings, "English");
            financialAbstraction = new FinancialAbstraction(AppSettings.InsignisAM, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings.Get("errorLog"));
            _context = context;
            _bankHelper = new BankHelper(mapper,_context);
            _dataprovider = data; 
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


                model.proposedPortfolio = null;
                Insignis.Asset.Management.Tools.Sales.SCurve scurve = new Insignis.Asset.Management.Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

                scurve.LoadHeatmap(7, "GBP", AppSettings.preferencesRoot);

                Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = ProcessPostback(illustrationInfo, false, scurve.heatmap);



                string fscsProtectionConfigFile = AppSettings.ClientConfigRoot;// ConfigurationManager.AppSettings["clientConfigRoot"];
                if (fscsProtectionConfigFile.EndsWith("\\") == false)
                    fscsProtectionConfigFile += "\\";
                fscsProtectionConfigFile += "FSCSProtection.xml";

                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(AppSettings.preferencesRoot + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerOrganisation) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerEmailAddress));

                Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");

                institutionInclusion.Children[0].Value = "true";
                institutionInclusion.Children[12].Value = "true";
                institutionInclusion.Children[13].Value = "true";
                institutionInclusion.Children[14].Value = "true";
                institutionInclusion.Children[15].Value = "true";





                model.proposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);
            }
            return View(model);
        }






        public Insignis.Asset.Management.Tools.Sales.SCurveSettings ProcessPostback(Session sessionData, bool pSkipPostback, Insignis.Asset.Management.Tools.Helper.Heatmap pHeatmap)
        {



            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(AppSettings.preferencesRoot + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters("Insignis") + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters("p.artner@partorg.com"));
            Octavo.Gate.Nabu.Preferences.Preference scurvePreferences = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");



            Octavo.Gate.Nabu.Preferences.Preference prefTotalAvailableToDeposit = scurvePreferences.GetChildPreference("TotalAvailableToDeposit");
            prefTotalAvailableToDeposit.Value = sessionData.TotalDeposit.ToString();  //Total Deposits from our model
            scurvePreferences.SetChildPreference(prefTotalAvailableToDeposit);


            Octavo.Gate.Nabu.Preferences.Preference prefAvailableTo = scurvePreferences.GetChildPreference("AvailableTo");
            prefAvailableTo.Value = "7";
            scurvePreferences.SetChildPreference(prefAvailableTo);



            Octavo.Gate.Nabu.Preferences.Preference prefMaximumDepositInAnyOneInstitution = scurvePreferences.GetChildPreference("MaximumDepositInAnyOneInstitution");
            prefMaximumDepositInAnyOneInstitution.Value = "85000";



            if (prefMaximumDepositInAnyOneInstitution.Value.Trim().Length == 0)
                prefMaximumDepositInAnyOneInstitution.Value = "0.00";
            else
                prefMaximumDepositInAnyOneInstitution.Value = Convert.ToDecimal(prefMaximumDepositInAnyOneInstitution.Value).ToString("0.00");



            scurvePreferences.SetChildPreference(prefMaximumDepositInAnyOneInstitution);

            Octavo.Gate.Nabu.Preferences.Preference prefNumberOfLiquidityRequirements = scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements");



            List<string> _liquidityAmount = new List<string>();
            //DONOT CHANGE THE ORDER
            _liquidityAmount.Add(sessionData.EasyAccess.ToString());
            _liquidityAmount.Add(sessionData.ThreeMonths.ToString());
            _liquidityAmount.Add(sessionData.NineMonths.ToString());
            _liquidityAmount.Add(sessionData.TwoYears.ToString());
            _liquidityAmount.Add(sessionData.OneMonth.ToString());
            _liquidityAmount.Add(sessionData.SixMonths.ToString());
            _liquidityAmount.Add(sessionData.OneYear.ToString());
            _liquidityAmount.Add(sessionData.ThreeYears.ToString());

            //9th , 10th, 11th unknown fields find more.
            _liquidityAmount.Add("");
            _liquidityAmount.Add("");
            _liquidityAmount.Add("");



            for (int i = 1; i <= Convert.ToInt32(prefNumberOfLiquidityRequirements.Value); i++)
            {

                string liquidityAmount = _liquidityAmount[i - 1];

                if (liquidityAmount == null || liquidityAmount.Trim().Length == 0)
                    liquidityAmount = "0.00";
                scurvePreferences.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("LiquidityAmount_" + i, liquidityAmount));
            }
            Octavo.Gate.Nabu.Preferences.Preference prefFeeDiscount = scurvePreferences.GetChildPreference("FeeDiscount");

            if (prefFeeDiscount == null)
                prefFeeDiscount = new Octavo.Gate.Nabu.Preferences.Preference("FeeDiscount");
            prefFeeDiscount.Value = "0.00";//discount fee formHelper.GetInput("_feeDiscount", Request);
            scurvePreferences.SetChildPreference(prefFeeDiscount);

            Octavo.Gate.Nabu.Preferences.Preference prefIntroducerDiscount = scurvePreferences.GetChildPreference("IntroducerDiscount");

            if (prefIntroducerDiscount == null)
                prefIntroducerDiscount = new Octavo.Gate.Nabu.Preferences.Preference("IntroducerDiscount");
            prefIntroducerDiscount.Value = "0.00";//formHelper.GetInput("_introducerDiscount", Request);
            scurvePreferences.SetChildPreference(prefIntroducerDiscount);


            Octavo.Gate.Nabu.Preferences.Preference prefCurrencyCode = scurvePreferences.GetChildPreference("CurrencyCode");
            if (prefCurrencyCode == null)
                prefCurrencyCode = new Octavo.Gate.Nabu.Preferences.Preference("CurrencyCode");
            prefCurrencyCode.Value = "GBP";//Currency
            scurvePreferences.SetChildPreference(prefCurrencyCode);

            Octavo.Gate.Nabu.Preferences.Preference prefFullProtection = scurvePreferences.GetChildPreference("FullProtection");
            if (prefFullProtection == null)
                prefFullProtection = new Octavo.Gate.Nabu.Preferences.Preference("FullProtection");
            prefFullProtection.Value = "true";//formHelper.GetInput("_fullProtection", Request);
            scurvePreferences.SetChildPreference(prefFullProtection);


            Octavo.Gate.Nabu.Preferences.Preference prefShowFitchRating = scurvePreferences.GetChildPreference("ShowFitchRating");
            if (prefShowFitchRating == null)
                prefShowFitchRating = new Octavo.Gate.Nabu.Preferences.Preference("ShowFitchRating");
            prefShowFitchRating.Value = "false";//formHelper.GetInput("_showFitchRating", Request);
            scurvePreferences.SetChildPreference(prefShowFitchRating);


            Octavo.Gate.Nabu.Preferences.Preference prefMinimumFitchRating = scurvePreferences.GetChildPreference("MinimumFitchRating");
            if (prefMinimumFitchRating == null)
                prefMinimumFitchRating = new Octavo.Gate.Nabu.Preferences.Preference("MinimumFitchRating");
            prefMinimumFitchRating.Value = "All";//formHelper.GetInput("_minFitchRating", Request);
            scurvePreferences.SetChildPreference(prefMinimumFitchRating);


            Octavo.Gate.Nabu.Preferences.Preference prefIncludePooledProducts = scurvePreferences.GetChildPreference("IncludePooledProducts");
            if (prefIncludePooledProducts == null)
                prefIncludePooledProducts = new Octavo.Gate.Nabu.Preferences.Preference("IncludePooledProducts");
            prefIncludePooledProducts.Value = "true";//formHelper.GetInput("_includePooledProducts", Request);
            scurvePreferences.SetChildPreference(prefIncludePooledProducts);


            Octavo.Gate.Nabu.Preferences.Preference prefOptionalClientName = scurvePreferences.GetChildPreference("OptionalClientName");
            if (prefOptionalClientName == null)
                prefOptionalClientName = new Octavo.Gate.Nabu.Preferences.Preference("OptionalClientName");
            prefOptionalClientName.Value = "unspecified";//formHelper.GetInput("_optionalClientName", Request);
            scurvePreferences.SetChildPreference(prefOptionalClientName);

            Octavo.Gate.Nabu.Preferences.Preference prefOptionalIntroducerOrganisationName = scurvePreferences.GetChildPreference("OptionalIntroducerOrganisationName");
            if (prefOptionalIntroducerOrganisationName == null)
                prefOptionalIntroducerOrganisationName = new Octavo.Gate.Nabu.Preferences.Preference("OptionalIntroducerOrganisationName");
            prefOptionalIntroducerOrganisationName.Value = "unspecified";//formHelper.GetInput("_optionalIntroducerOrganisationName", Request);
            scurvePreferences.SetChildPreference(prefOptionalIntroducerOrganisationName);


            Institution[] allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
            Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");


            foreach (Institution institution in allInstitutions)
            {
                //institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("142", "true"));
                institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
            }

            preferencesManager.SetPreference("Sales.Tools.SCurve.Institutions", 1, institutionInclusion);

            preferencesManager.SetPreference("Sales.Tools.SCurve.Settings", 1, scurvePreferences);

            Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = new Insignis.Asset.Management.Tools.Sales.SCurveSettings();

            settings.TotalAvailableToDeposit = Convert.ToDecimal(scurvePreferences.GetChildPreference("TotalAvailableToDeposit").Value);


            if (scurvePreferences.GetChildPreference("AvailableTo") != null && scurvePreferences.GetChildPreference("AvailableTo").Value.Length > 0)
            {
                settings.AvailableToHubAccountTypeID = Convert.ToInt32(scurvePreferences.GetChildPreference("AvailableTo").Value);
                AccountType hubAccountType = financialAbstraction.GetAccountType(settings.AvailableToHubAccountTypeID, (int)multiLingual.language.LanguageID);

                if (hubAccountType != null && hubAccountType.ErrorsDetected == false && hubAccountType.AccountTypeID.HasValue)
                {
                    if (hubAccountType.Detail.Alias.Contains("JOINTHUBACCOUNT"))
                        settings.ClientType = Insignis.Asset.Management.Tools.Sales.SCurveClientType.Joint;
                    else if (hubAccountType.Detail.Alias.Contains("PERSONALHUBACCOUNT"))
                        settings.ClientType = Insignis.Asset.Management.Tools.Sales.SCurveClientType.Individual;
                    else
                        settings.ClientType = Insignis.Asset.Management.Tools.Sales.SCurveClientType.Corporate;
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
            }

            return settings;
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
            Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction powerpointRenderAbstraction = new Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction(AppSettings.illustrationOutputInternalFolder, AppSettings.illustrationOutputPublicFacingFolder);
            List<KeyValuePair<string, string>> textReplacements = new List<KeyValuePair<string, string>>();

            textReplacements.Add(new KeyValuePair<string, string>("REFERENCE", "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss")));
            textReplacements.Add(new KeyValuePair<string, string>("DATE", DateTime.Now.ToString("dd/MM/yyyy")));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTNAME", "Ajaybir Jaskaran"));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTTYPE", "Joint"));
            textReplacements.Add(new KeyValuePair<string, string>("INTROORG", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEEDISCOUNT", "100%"));
            textReplacements.Add(new KeyValuePair<string, string>("FEE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("CHARGE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("TOTAL", "10,0000"));
            textReplacements.Add(new KeyValuePair<string, string>("PROTECTION", "65000"));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSYIELD", "200%"));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSINTEREST", "16"));
            textReplacements.Add(new KeyValuePair<string, string>("NETYIELD", "17"));
            textReplacements.Add(new KeyValuePair<string, string>("NETINTEREST", "19"));


            Octavo.Gate.Nabu.Encryption.EncryptorDecryptor decryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
            string qsTemplateFile = "C:\\InsignisAM\\NET\\ExternalIllustrator\\ExternalIllustrator\\Template\\1\\illustration.pptx";
            System.IO.FileInfo templateFile = new System.IO.FileInfo(qsTemplateFile);
            string prefixName = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss");

            string requiredOutputNameWithoutExtension = prefixName + "_CashIllustration";

            Insignis.Asset.Management.Reports.Helper.ExtendedReportContent extendedReportContent = powerpointRenderAbstraction.MergeDataWithPowerPointTemplate(prefixName, textReplacements, templateFile.FullName, requiredOutputNameWithoutExtension, true);
            string filename = AppSettings.illustrationOutputInternalFolder + "\\" + prefixName + "\\" + requiredOutputNameWithoutExtension + ".pdf";

            System.IO.File.Delete(filename);

            Presentation presentation = new Presentation();

            presentation.LoadFromFile(AppSettings.illustrationOutputInternalFolder + "\\" + prefixName + "\\" + requiredOutputNameWithoutExtension + ".pptx");
            presentation.SaveToFile(AppSettings.illustrationOutputInternalFolder + "\\" + prefixName + "\\" + requiredOutputNameWithoutExtension + ".pdf", Spire.Presentation.FileFormat.PDF);

            byte[] filedata = System.IO.File.ReadAllBytes(filename);

            //return View();
            return File(filedata, "application/pdf");
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
