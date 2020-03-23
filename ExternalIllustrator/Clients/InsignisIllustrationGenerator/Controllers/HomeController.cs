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
using InsignisIllustrationGenerator.Data;
using System.Linq;
using System.Text;
using Insignis.Asset.Management.Clients.Helper;
using System.IO;

namespace InsignisIllustrationGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;



        private AppSettings AppSettings { get; set; }

        private MultiLingual multiLingual;
        private readonly BankHelper _bankHelper;
        private readonly IllustrationHelper _illustrationHelper;


        private FinancialAbstraction financialAbstraction { get; set; }
        // [HttpPost, ActionName("PreviousIllustration")]
        [HttpPost]
        public ActionResult ExportCSV(SearchParameterViewModel searchParameter)
        {
            var result = _illustrationHelper.GetIllustrationList(searchParameter,false);
            StringBuilder sbheader = new StringBuilder();
            StringBuilder sb = new StringBuilder();


            sbheader.Append(string.Format("{0},", "Company Name"));
            sbheader.Append(string.Format("{0},", "Client Name"));
            sbheader.Append(string.Format("{0},", "Client Type"));
            sbheader.Append(string.Format("{0},", "Illustration Unique Reference"));
            sbheader.Append(string.Format("{0},", "Generate Date"));
            sbheader.Append(string.Format("{0},", "Status")).AppendLine(Environment.NewLine);



            foreach (var data in result)
            {

                sb.Append(string.Format("{0},", data.PartnerOrganisation));
                sb.Append(string.Format("{0},", data.ClientName));
                sb.Append(string.Format("{0},", data.ClientType));
                sb.Append(string.Format("{0},", data.IllustrationUniqueReference));
                sb.Append(string.Format("{0},", data.GenerateDate));
                sb.Append(string.Format("{0},", data.Status)).AppendLine(Environment.NewLine);

            }


          //  byte[] buffer = Encoding.ASCII.GetBytes();

            return Json(new { Data = $"{string.Join(",", sbheader)}\r\n{sb.ToString()}", FileName = "Illustration.csv" });


               /// File(buffer, "text/csv", "Illustration.csv");



        }

        //Session State Management
        public void SetSession(string email, string name, string organisation, string telephone, bool superUser)
        {
            /*Sets user session
             Arguments:- 
                Email:- Partner Email
                Name:- Partner Name
                Organisation: Partner Organisation
                Telephone: Partner Telephone
             Returns:- None
             
             */

            //Set Info in session

            var session = new Session() { PartnerEmailAddress = email, PartnerName = name, PartnerOrganisation = organisation, PartnerTelephone = telephone, SuperUser = superUser };

            HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(session));
            
        }

        //private readonly BankHelper _bankHelper;
        public HomeController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings, ApplicationDbContext context)
        {
            _logger = logger;
            _mapper = mapper;
            AppSettings = settings.Value;
            multiLingual = new MultiLingual(AppSettings, "English");
            financialAbstraction = new FinancialAbstraction(AppSettings.InsignisAM, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings.Get("errorLog"));
            _context = context;
            _bankHelper = new BankHelper(mapper, _context);
            _illustrationHelper = new IllustrationHelper(mapper, _context);

        }

        public IActionResult Reset(string actionid)
        {
            HttpContext.Session.Remove("GeneratedPorposals");
            HttpContext.Session.Remove("InputProposal");
            return RedirectToAction(actionid, "Home");
        }

        public IActionResult Index(bool reset)
        {
            /*
             Create Illusration Detail Page
             Arguments:- None
             Returns:- IllustrationModel and Display to user
             */

            var partnerInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            IllustrationDetailViewModel model = new IllustrationDetailViewModel();
            model.PartnerEmail = partnerInfo.PartnerEmailAddress;
            model.PartnerName = partnerInfo.PartnerName;
            model.PartnerOrganisation = partnerInfo.PartnerOrganisation;


            if (reset)
            {
                HttpContext.Session.Remove("InputProposal");
                return View(model);
            }

            IllustrationDetailViewModel illustrationInfo = null;
            //Check for old
            if (!string.IsNullOrEmpty((HttpContext.Session.GetString("InputProposal")))){
                illustrationInfo = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));
                return View(illustrationInfo);
            }

            
            
            

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("InputProposal")))
            {
                illustrationInfo = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));
            }
            

            //model.AdviserName = partnerInfo.PartnerName; ;
            

            if (illustrationInfo != null)
            {
                model.ClientName = illustrationInfo.ClientName;
                model.ClientType = illustrationInfo.ClientType;
                model.Currency = illustrationInfo.Currency;
                model.EasyAccess = illustrationInfo.EasyAccess;
                model.OneMonth = illustrationInfo.OneMonth;
                model.ThreeMonths = illustrationInfo.ThreeMonths;
                model.SixMonths = illustrationInfo.SixMonths;
                model.NineMonths = illustrationInfo.NineMonths;
                model.OneYear = illustrationInfo.OneYear;
                model.TwoYears = illustrationInfo.TwoYears;
                model.ThreeYearsPlus = illustrationInfo.ThreeYearsPlus;
                model.TotalDeposit = illustrationInfo.TotalDeposit;
            }
            //render view
            return View(model);
        }
        //[HttpPost]
        //public async Task<IActionResult> PreviousIllustration(SearchParameterViewModel searchParameter)
        //{


        //    IEnumerable<IllustrationListViewModel> illustrationList = await GetIllustrationListAsync(searchParameter);

        //    return PartialView("_IllustrationList",   illustrationList.ToList());

        //}

        public IActionResult PreviousIllustration(SearchParameterViewModel searchParams)
        {
            /*
             Returns list of previous illustration

            Arguments:-
                Search Params
            
            Returns:-
                View with previous list
             */

            var illustrationList = _illustrationHelper.GetIllustrationList(searchParams,false);
            return View(illustrationList);
        }
        public IActionResult SearchIllustration(SearchParameterViewModel searchParams)
        {

            //check model validation for any empty waitttttttt

            //bool isNull = false;
            //if (searchParams.AdvisorName == null & searchParams.ClientName == null & searchParams.CompanyName == null
            //    & searchParams.IllustrationFrom == null & searchParams.IllustrationTo == null & searchParams.IllustrationUniqueReference == null)
            //{
            //    isNull = true;
            //    return Json(new { Error = isNull });
            //}
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                return Json(new { Data=errors, Success = false });
            }


            var partnerInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            searchParams.PartnerEmail = partnerInfo.PartnerEmailAddress;
            searchParams.PartnerOrganisation = partnerInfo.PartnerOrganisation;
            var result = _illustrationHelper.GetIllustrationList(searchParams,false);
            if (result.Count()>0) return Json(new { Data = result, Success = true });
            return Json(new { Data = "Sorry, we couldn’t find any illustrations matching your search criteria.", Success = false });
        }




        public IActionResult GetIllustration(string uniqueReferenceId)
        {
            /*
             Get summary details of Illustration from unique reference ID 
             
            Arguments:- 
                unique illustration id eg:-
             
            Return:-
                View with Illustration Details
             */
            var result = _illustrationHelper.GetIllustrationByUniqueReferenceId(uniqueReferenceId);

            ViewBag.URL = AppSettings.illustrationOutputPublicFacingFolder + "/" + uniqueReferenceId + "/" + uniqueReferenceId + "_CashIllustration.pdf";

            ViewBag.User = "";
            return View("_illustrationDetails", result);

        }

        public IActionResult Calculate(IllustrationDetailViewModel model)
        {
            /*
             Post Method
             Arguments:- IllustrationDetailViewModel 
             Returns:- View and Errors
             */

            
            var illustrationInfo = new Session();
            if (!string.IsNullOrEmpty((HttpContext.Session.GetString("SessionPartner"))))
            {
                illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            }
            
            if (string.IsNullOrEmpty(model.PartnerName) && !string.IsNullOrEmpty(HttpContext.Session.GetString("InputProposal")))
            {
                model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));
                var folio = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals"));

                var scurve = _mapper.Map<Insignis.Asset.Management.Tools.Sales.SCurveOutput>(folio);

                model.ProposedPortfolio = scurve;

                return View(model);
            }
            

            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
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
                    illustrationInfo.AdvisorName = model.PartnerName;

                    CalculateIllustration(model, illustrationInfo);

                }
                SCurveOutput sStore = new SCurveOutput();

                sStore = _mapper.Map(model.ProposedPortfolio, sStore);

                //Database save our database Super User get 
                model.TotalDeposit = Convert.ToDouble(sStore.TotalDeposited);
                


                foreach (var m in model.ProposedPortfolio.ProposedInvestments)
                {
                    if (string.IsNullOrEmpty(m.InvestmentTerm.TermText))
                    {
                        if(m.InvestmentTerm.investmentAccountType == Insignis.Asset.Management.Clients.Helper.InvestmentAccountType.InstantAccessAccount)
                        {
                            m.InvestmentTerm.TermText = "Instant Access";
                        }
                        else
                        {
                        m.InvestmentTerm.TermText = m.InvestmentTerm.NoticeText;
                        }
                    }
                }


                HttpContext.Session.SetString("GeneratedPorposals", JsonConvert.SerializeObject(sStore));
                HttpContext.Session.SetString("InputProposal", JsonConvert.SerializeObject(model));

                return View(model);
            }
            else
            {
                model.ProposedPortfolio = new Insignis.Asset.Management.Tools.Sales.SCurveOutput();
                return View("Index", model);
            }

            return View(model);
        }

        private void CalculateIllustration(IllustrationDetailViewModel model, Session illustrationInfo)
        {
            model.ProposedPortfolio = null;



            Insignis.Asset.Management.Tools.Sales.SCurve scurve = new Insignis.Asset.Management.Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

            scurve.LoadHeatmap(7, "GBP", AppSettings.preferencesRoot);
            //scurve.LoadHeatmap(7, model.Currency, AppSettings.preferencesRoot);

            Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = ProcessPostback(illustrationInfo, false, scurve.heatmap);

            string fscsProtectionConfigFile = AppSettings.ClientConfigRoot;// ConfigurationManager.AppSettings["clientConfigRoot"];
            if (fscsProtectionConfigFile.EndsWith("\\") == false)
                fscsProtectionConfigFile += "\\";
            fscsProtectionConfigFile += "FSCSProtection.xml";

            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(AppSettings.preferencesRoot + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerOrganisation) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerEmailAddress));

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

            //Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");



            foreach (var childern in institutionInclusion.Children)
            {
                childern.Value = "true";
            }
            var feeMatrix = new FeeMatrix(fscsProtectionConfigFile + "FeeMatrix.xml");
            model.ProposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);
            
            model.AnnualGrossInterestEarned = 0;

            foreach (var investment in model.ProposedPortfolio.ProposedInvestments)
            {
                model.AnnualGrossInterestEarned += investment.AnnualInterest;
            }

            model.GrossAverageYield  = (model.ProposedPortfolio.AnnualGrossInterestEarned / model.ProposedPortfolio.TotalDeposited) * 100;



            model.NetAverageYield = (model.GrossAverageYield - model.ProposedPortfolio.FeePercentage);

            model.ProposedPortfolio.FeePercentage = 0.20M;

            model.ProposedPortfolio.Fee = (model.ProposedPortfolio.TotalDeposited * (decimal)(model.ProposedPortfolio.FeePercentage / 100));

            model.AnnualNetInterestEarned = (model.ProposedPortfolio.AnnualGrossInterestEarned - model.ProposedPortfolio.Fee);



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
            prefMaximumDepositInAnyOneInstitution.Value = sessionData.ClientType == 0?"85000":"170000";



            if (prefMaximumDepositInAnyOneInstitution.Value.Trim().Length == 0)
                prefMaximumDepositInAnyOneInstitution.Value = "0.00";
            else
                prefMaximumDepositInAnyOneInstitution.Value = Convert.ToDecimal(prefMaximumDepositInAnyOneInstitution.Value).ToString("0.00");



            scurvePreferences.SetChildPreference(prefMaximumDepositInAnyOneInstitution);

            Octavo.Gate.Nabu.Preferences.Preference prefNumberOfLiquidityRequirements = scurvePreferences.GetChildPreference("NumberOfLiquidityRequirements");



            List<string> _liquidityAmount = new List<string>();
            //DONOT CHANGE THE ORDER
            _liquidityAmount.Add("");//5 year bond
            _liquidityAmount.Add("");//4 year bond
            _liquidityAmount.Add(sessionData.ThreeYears.ToString());//3 year bond
            _liquidityAmount.Add(sessionData.TwoYears.ToString());// 2 year bond
            _liquidityAmount.Add("");//18 month bonds
            _liquidityAmount.Add(sessionData.OneYear.ToString());//1 year bond
            _liquidityAmount.Add(sessionData.NineMonths.ToString());//9 months
            _liquidityAmount.Add(sessionData.SixMonths.ToString());// 6 months bond
            _liquidityAmount.Add(sessionData.ThreeMonths.ToString());// 3 months
            _liquidityAmount.Add(sessionData.OneMonth.ToString());//1 month
            _liquidityAmount.Add(sessionData.EasyAccess.ToString());//Easy Access


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
            IllustrationDetailViewModel model = null;
            //Getting Input Illustration from InputProposal Session
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("InputProposal"))) model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));

            var generatedInvestments = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals")); //scurve output

            //Insignis.Asset.Management.Tools.Sales.SCurveOutput _sc= new Insignis.Asset.Management.Tools.Sales.SCurveOutput();

            model.ProposedPortfolio = _mapper.Map(generatedInvestments, new Insignis.Asset.Management.Tools.Sales.SCurveOutput());

            Octavo.Gate.Nabu.Encryption.EncryptorDecryptor decryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
            
            //illustration template
            string qsTemplateFile = ConfigurationManager.AppSettings.Get("illustrationTemplateRoot") +"\\"+ model.PartnerOrganisation+"\\"+ model.PartnerOrganisation+".pptx";

            if (!System.IO.File.Exists(qsTemplateFile)) qsTemplateFile = ConfigurationManager.AppSettings.Get("illustrationTemplateRoot") + "\\Insignis\\Insignis.pptx";


            System.IO.FileInfo templateFile = new System.IO.FileInfo(qsTemplateFile);

            string prefixName = string.Empty; 

            if (!string.IsNullOrEmpty(model.IllustrationUniqueReference))
            {
                //prefixName = model.IllustrationUniqueReference;
                prefixName = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + _illustrationHelper.GetNextIllustrationRefernce().ToString();//Get Last prefix number from db
            }
            else
            {
                prefixName = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-"+ _illustrationHelper.GetNextIllustrationRefernce().ToString();//Get Last prefix number from db
            }


            model.IllustrationUniqueReference = prefixName;
            string requiredOutputNameWithoutExtension = prefixName + "_CashIllustration";


            List<KeyValuePair<string, string>> textReplacements = new List<KeyValuePair<string, string>>();

            //var illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));

            textReplacements.Add(new KeyValuePair<string, string>("REFERENCE", prefixName));
            textReplacements.Add(new KeyValuePair<string, string>("DATE", DateTime.Now.ToString("dd/MM/yyyy")));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTNAME", model.ClientName));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTTYPE", model.ClientType.ToString()));
            textReplacements.Add(new KeyValuePair<string, string>("INTROORG", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEEDISCOUNT", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("CHARGE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("TOTAL", model.TotalDeposit.ToString()));
            textReplacements.Add(new KeyValuePair<string, string>("PROTECTION", ""));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSYIELD", model.GrossAverageYield.ToString("#.###")));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSINTEREST", model.AnnualGrossInterestEarned.ToString("#.###")));
            textReplacements.Add(new KeyValuePair<string, string>("NETYIELD", model.NetAverageYield.ToString("#.###")));
            textReplacements.Add(new KeyValuePair<string, string>("NETINTEREST", model.AnnualNetInterestEarned.ToString("#.###")));

            string institutionName = " ";
            string termDescription = " ";
            string rate = " ";
            string deposit = " ";
            string interest = " ";


            for (int i = 1; i < 30; i++)
            {
                institutionName = " ";
                termDescription = " ";
                rate = " ";
                deposit = " ";
                interest = " ";

                try
                {
                    institutionName = model.ProposedPortfolio.ProposedInvestments[i-1].InstitutionName;
                    termDescription = model.ProposedPortfolio.ProposedInvestments[i-1].InvestmentTerm.GetText();// heatmapTerm.InvestmentTerm.GetText();
                    rate = model.ProposedPortfolio.ProposedInvestments[i-1].Rate.ToString("0.00") + "%";
                    deposit = "£"+ model.ProposedPortfolio.ProposedInvestments[i-1].DepositSize.ToString("00");
                    interest = "£"+ model.ProposedPortfolio.ProposedInvestments[i-1].AnnualInterest.ToString("00");
                }
                catch
                {

                }

                textReplacements.Add(new KeyValuePair<string, string>("INSTITUTION" + i.ToString("00"), institutionName));
                textReplacements.Add(new KeyValuePair<string, string>("TERM" + i.ToString("00"), termDescription));
                textReplacements.Add(new KeyValuePair<string, string>("RATE" + i.ToString("00"), rate));
                textReplacements.Add(new KeyValuePair<string, string>("DEPOSIT" + i.ToString("00"), deposit));
                textReplacements.Add(new KeyValuePair<string, string>("INTEREST" + i.ToString("00"), interest));

            }

            //check if ppt or pdf.................exist..............//
            if (Directory.Exists(AppSettings.illustrationOutputInternalFolder +"\\"+ prefixName))
            {
                Directory.Delete(AppSettings.illustrationOutputInternalFolder + "\\" + prefixName,true);
            }



            Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction powerpointRenderAbstraction = new Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction(AppSettings.illustrationOutputInternalFolder, AppSettings.illustrationOutputPublicFacingFolder);

            


            model.Status = InsignisEnum.IllustrationStatus.Current.ToString();
            model.GenerateDate = DateTime.Now;
            
            
            
            SaveIllustraion(model);


            Insignis.Asset.Management.Reports.Helper.ExtendedReportContent extendedReportContent = powerpointRenderAbstraction.MergeDataWithPowerPointTemplate(prefixName, textReplacements, templateFile.FullName, requiredOutputNameWithoutExtension, true);
            string filename = AppSettings.illustrationOutputInternalFolder + "\\" + prefixName + "\\" + requiredOutputNameWithoutExtension + ".pdf";

            

            ViewBag.PDF = extendedReportContent.URI;
            return View();
        }

        private bool SaveIllustraion(IllustrationDetailViewModel model)
        {
            return _illustrationHelper.SaveIllustraionAsync(model);
        }

        public IActionResult Update(string includeBank, string bankId, string updatedAmount,string instituteName, string investmentTerm,string updatePage)
        {

            

            var illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("InputProposal"));

            var partnerEmail = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));

            //Check if any deposit exists before allotment
            bool _savedBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation);


            bool sessionVariable = false;
            decimal amount = 0;
            string whichTerm = string.Empty;
            
            if (_savedBank)
            {
               var _list = _context.TempInstitution.Where(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation).ToList();
                if (_list.Count > 0)
                {
                    foreach (var bank in _list)
                    {
                        var _dbInvestment = _context.InvestmentTermMapper.Where(x => x.InvestmentText == bank.InvestmentTerm).SingleOrDefault();

                        ExcludedInstitute inst = new ExcludedInstitute();
                        inst.ClientReference = partnerEmail.ClientName;
                        inst.PartnerEmail = partnerEmail.PartnerEmail;
                        inst.PartnerOrganisation = partnerEmail.PartnerOrganisation;
                        inst.InstituteId = Convert.ToInt32(bank.BankId);

                        _context.Add(inst);
                        _context.SaveChanges();



                        if (_dbInvestment.InvestmentTerm == "Instant Access")
                        {
                            illustrationInfo.EasyAccess -=Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Instant Access";
                        }
                        if (_dbInvestment.InvestmentTerm == "One Month")
                        {
                            illustrationInfo.OneMonth -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "One Month";
                        }
                        if (_dbInvestment.InvestmentTerm == "Three Months")
                        {
                            illustrationInfo.ThreeMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Three Month";
                        }
                        if (_dbInvestment.InvestmentTerm == "Six Months")
                        {
                            illustrationInfo.SixMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Six Months";
                        }
                        if (_dbInvestment.InvestmentTerm == "Nine Months")
                        {
                            illustrationInfo.NineMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Nine Months";
                        }
                        if (_dbInvestment.InvestmentTerm == "One Year")
                        {
                            illustrationInfo.OneYear -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "One Year";
                        }
                        if (_dbInvestment.InvestmentTerm == "Two Years")
                        {
                            illustrationInfo.TwoYears -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Two Years";
                        }
                        if (_dbInvestment.InvestmentTerm == "Three Years")
                        {
                            illustrationInfo.ThreeYears -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Three Years";
                        }

                        illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);

                        sessionVariable = true;

                    }

                }
            }
            






                //............Save if institute excluded................................................................................
                if (includeBank == null) {

                bool alreadyExcluded = _context.ExcludedInstitutes.Any(x => x.InstituteId == Convert.ToInt32(bankId) && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.ClientReference == partnerEmail.ClientName);
                if (!alreadyExcluded) { 
                ExcludedInstitute inst = new ExcludedInstitute();
                inst.ClientReference = partnerEmail.ClientName;
                inst.PartnerEmail = partnerEmail.PartnerEmail;
                inst.PartnerOrganisation = partnerEmail.PartnerOrganisation;
                inst.InstituteId = Convert.ToInt32(bankId);

                _context.Add(inst);
                _context.SaveChanges();
                }
            }
            //......................................................................................................................



            //STEP1:.............save updated amount and bank to database..............
            if(includeBank != null) { 
                TempInstitution temp = new TempInstitution();
                temp.BankId = Convert.ToInt32(bankId);
                temp.ClientName = illustrationInfo.ClientName;
                temp.Amount =Convert.ToDecimal(updatedAmount);
                temp.InstitutionName = instituteName;
                temp.InvestmentTerm = investmentTerm;
                temp.PartnerEmail = partnerEmail.PartnerEmail;
                temp.PartnerName = illustrationInfo.PartnerName;
                temp.PartnerOrganisation = illustrationInfo.PartnerOrganisation;
                _context.TempInstitution.Add(temp);

                //bank saved to database
                _context.SaveChanges();
            }


            //CHECK IF CASE IS INCLUDED AND BANK AMOUNT IS BEING CHANGED
            //STEP2:................................Delete saved investment from calculation..............................
            var dbInvestment = _context.InvestmentTermMapper.Where(x => x.InvestmentText == investmentTerm).SingleOrDefault();
            
            if(includeBank != null ) { 

            if (investmentTerm == "Instant Access")
                illustrationInfo.EasyAccess -= Convert.ToDouble(updatedAmount);
            

            if (dbInvestment.InvestmentTerm == "One Month")
                illustrationInfo.OneMonth -= Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Three Months")
                illustrationInfo.ThreeMonths -= Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Six Months")
                illustrationInfo.SixMonths -= Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Nine Months")
                illustrationInfo.NineMonths -= Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "One Year")
                illustrationInfo.OneYear -= Convert.ToDouble(updatedAmount);

            if (dbInvestment.InvestmentTerm == "Two Years")
                illustrationInfo.TwoYears -= Convert.ToDouble(updatedAmount);

            if (dbInvestment.InvestmentTerm == "Three Years")
                illustrationInfo.ThreeYears -= Convert.ToDouble(updatedAmount);



            illustrationInfo.TotalDeposit -= Convert.ToDouble(updatedAmount);

            }



            illustrationInfo.PartnerEmailAddress = partnerEmail.PartnerEmail;
            IllustrationDetailViewModel model = new IllustrationDetailViewModel();

            model.ProposedPortfolio = null;

            

            Insignis.Asset.Management.Tools.Sales.SCurve scurve = new Insignis.Asset.Management.Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

            scurve.LoadHeatmap(7, "GBP", AppSettings.preferencesRoot);
            //scurve.LoadHeatmap(7, model.Currency, AppSettings.preferencesRoot);

            Insignis.Asset.Management.Tools.Sales.SCurveSettings settings = ProcessPostback(illustrationInfo, false, scurve.heatmap);

            string fscsProtectionConfigFile = AppSettings.ClientConfigRoot;// ConfigurationManager.AppSettings["clientConfigRoot"];
            if (fscsProtectionConfigFile.EndsWith("\\") == false)
                fscsProtectionConfigFile += "\\";
            fscsProtectionConfigFile += "FSCSProtection.xml";

            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(AppSettings.preferencesRoot + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerOrganisation) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerEmailAddress));

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


            //get list of excluded institutes
            var excludedInstituteIds = _context.ExcludedInstitutes.Where(x => x.ClientReference == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation).Select(x=>x.InstituteId).ToList();

            foreach (var childern in institutionInclusion.Children)
            {
                
                if (childern.Name != bankId)
                    childern.Value = "true";
                //if (childern.Name == bankId && includeBank == "on")//Exclusion of bank
                //    childern.Value = "false";
                if (excludedInstituteIds.Contains(Convert.ToInt32(childern.Name)))
                    childern.Value = "false";

                //string x = "DONOT MOVE FOEA";
            }

            var feeMatrix = new FeeMatrix(fscsProtectionConfigFile + "FeeMatrix.xml");
            model.ProposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);

            model.AnnualGrossInterestEarned = 0;

            foreach (var investment in model.ProposedPortfolio.ProposedInvestments)
            {
                model.AnnualGrossInterestEarned += investment.AnnualInterest;
            }

            model.GrossAverageYield = (model.ProposedPortfolio.AnnualGrossInterestEarned / model.ProposedPortfolio.TotalDeposited) * 100;

            model.NetAverageYield = (model.GrossAverageYield - model.ProposedPortfolio.FeePercentage);

            model.ProposedPortfolio.FeePercentage = 0.20M;

            model.ProposedPortfolio.Fee = (model.ProposedPortfolio.TotalDeposited * (decimal)(model.ProposedPortfolio.FeePercentage / 100));

            model.AnnualNetInterestEarned = (model.ProposedPortfolio.AnnualGrossInterestEarned - model.ProposedPortfolio.Fee);

            model.ClientName = illustrationInfo.ClientName;
            
            model.ClientType = illustrationInfo.ClientType;
            model.Currency = illustrationInfo.Currency;
            model.EasyAccess = illustrationInfo.EasyAccess;
            model.NineMonths = illustrationInfo.NineMonths;
            model.OneMonth = illustrationInfo.OneMonth;
            model.OneYear = illustrationInfo.OneYear;
            model.PartnerEmail = illustrationInfo.PartnerEmailAddress;
            model.PartnerName = illustrationInfo.PartnerName;
            model.PartnerOrganisation = illustrationInfo.PartnerOrganisation;
            model.SixMonths = illustrationInfo.SixMonths;
            model.ThreeMonths = illustrationInfo.ThreeMonths;
            model.ThreeYearsPlus = illustrationInfo.ThreeYears;
            model.TwoYears = illustrationInfo.TwoYears;


            SCurveOutput sStore = new SCurveOutput();

            ////add saved one to display

            sStore = _mapper.Map(model.ProposedPortfolio, sStore);


            //check db for any saved bank
            bool savedBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation);
            if (savedBank)
            {
                var tempBanks = _context.TempInstitution.Where(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation).ToList();
                foreach (var bank in tempBanks)
                {
                    Insignis.Asset.Management.Tools.Sales.SCurveOutputRow row = new Insignis.Asset.Management.Tools.Sales.SCurveOutputRow();
                    row.InstitutionName = bank.InstitutionName;
                    row.InvestmentTerm = new InvestmentTerm();
                    row.InvestmentTerm.TermText = bank.InvestmentTerm;
                    row.DepositSize = bank.Amount;

                    sStore.ProposedInvestments.Add(row);
                    model.ProposedPortfolio.ProposedInvestments.Add(row);
                }
            
            
            }

            decimal total = 0;
            //investment term
            foreach (var m in model.ProposedPortfolio.ProposedInvestments)
            {
                total += m.DepositSize;
                if (string.IsNullOrEmpty(m.InvestmentTerm.TermText))
                {
                    if (m.InvestmentTerm.investmentAccountType == Insignis.Asset.Management.Clients.Helper.InvestmentAccountType.InstantAccessAccount)
                    {
                        m.InvestmentTerm.TermText = "Instant Access";
                    }
                    else
                    {
                        m.InvestmentTerm.TermText = m.InvestmentTerm.NoticeText;
                    }
                }
            }

            model.TotalDeposit =Convert.ToDouble(total);
            if(includeBank != null) { 

            if (dbInvestment.InvestmentTerm == "Instant Access")
                model.EasyAccess += Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "One Month")
                model.OneMonth += Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Three Months")
                model.ThreeMonths += Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Six Months")
                model.SixMonths += Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "Nine Months")
                model.NineMonths += Convert.ToDouble(updatedAmount);


            if (dbInvestment.InvestmentTerm == "One Year")
                model.OneYear += Convert.ToDouble(updatedAmount);

            if (dbInvestment.InvestmentTerm == "Two Years")
                model.TwoYears += Convert.ToDouble(updatedAmount);

            if (dbInvestment.InvestmentTerm == "Three Years")
                model.ThreeYearsPlus += Convert.ToDouble(updatedAmount);

            }




            if(sessionVariable == true)
            {
                if(whichTerm == "Instant Access")
                    model.EasyAccess +=Convert.ToDouble(amount);
                if (whichTerm == "One Month")
                    model.OneMonth += Convert.ToDouble(amount);
                if (whichTerm == "Three Months")
                    model.ThreeMonths += Convert.ToDouble(amount);
                if (whichTerm == "Six Months")
                    model.SixMonths += Convert.ToDouble(amount);
                if(whichTerm == "Nine Months")
                    model.NineMonths += Convert.ToDouble(amount);
                if (whichTerm == "One Year")
                    model.OneYear += Convert.ToDouble(amount);
                if (whichTerm == "Two Years")
                    model.TwoYears += Convert.ToDouble(amount);
                if (whichTerm == "Three Years")
                    model.ThreeYearsPlus += Convert.ToDouble(amount);
            }
            HttpContext.Session.SetString("GeneratedPorposals", JsonConvert.SerializeObject(sStore));
            HttpContext.Session.SetString("InputProposal", JsonConvert.SerializeObject(model));
            return View("Calculate", model);

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



        public JsonResult UpdateStatus(UpdateStatusViewModel model)
        {
            /*
             Update status of given illustration
             Arguments:- 
                premitive type 
                comment
                referredby
                status
                illustration id
            Return:-
                Json true/false on success
             */
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                return Json(new { Data = errors, Success = false });
            }
            
            var result = _illustrationHelper.UpdateIllustrationStatus(model);
            return Json(new { Data = result, Success = true });
        }

        public IActionResult UpdateIllustration(string uniqueReferenceId)
        {
            /*
             Update Illustration 
            
            Arguments:-
                Unique Illustration Id
            
            Return:-
                View with model
             */

            var response = _illustrationHelper.GetIllustrationByUniqueReferenceId(uniqueReferenceId);
            ViewBag.IllustrationId = uniqueReferenceId;
            return View("Index", response);

        }


        public IActionResult Login()
        {
            /*
             Login page for session transfer
             Arguments:-
                None
            Returns:-
                View
             */
            return View();
        }
        [HttpPost]
        public IActionResult Login(Session session)
        {
            SetSession(session.PartnerEmailAddress, session.PartnerName, session.PartnerOrganisation, session.PartnerTelephone,false);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult SuperLogin(Session session)
        {
            SetSession(session.PartnerEmailAddress, session.PartnerName, session.PartnerOrganisation, session.PartnerTelephone, true);

            return RedirectToAction("Illustrationlist","Superuser");

        }

        public IActionResult Logo()
        {
            /*
             Return redirected view
             Arguments:- 
                None
            Returns:-
                View
             */
            var sessionData = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            HttpContext.Session.Remove("InputProposal");
            if (sessionData.SuperUser == true)
            {
                return RedirectToAction("Illustrationlist", "Superuser");
            }
            else
            {
                return  RedirectToAction("Index", "Home");
            }


        }

    }
}
