using Insignis.Asset.Management.Clients.Helper;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Manager;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Financial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
            var result = _illustrationHelper.GetIllustrationList(searchParameter, false);
            StringBuilder sbheader = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            sbheader.Append(string.Format("{0},", "Introducer"));
            sbheader.Append(string.Format("{0},", "Account Reference"));
            sbheader.Append(string.Format("{0},", "Account Name"));
            sbheader.Append(string.Format("{0},", "Lead Type or Name"));
            sbheader.Append(string.Format("{0},", "Last Name"));
            sbheader.Append(string.Format("{0},", "Name"));
            sbheader.Append(string.Format("{0},", "Adviser's Email Address"));
            sbheader.Append(string.Format("{0},", "Marketing Source"));
            sbheader.Append(string.Format("{0},", "Categories"));
            sbheader.Append(string.Format("{0},", "Client Category"));
            sbheader.Append(string.Format("{0},", "Marketing Source"));
            sbheader.Append(string.Format("{0},", "Categories"));
            sbheader.Append(string.Format("{0},", "Client Category"));
            sbheader.Append(string.Format("{0},", "Saleperson"));
            sbheader.Append(string.Format("{0},", "Account Manager"));
            sbheader.Append(string.Format("{0},", "Minimum Deposit"));
            sbheader.Append(string.Format("{0},", "Maximum Deposit"));
            sbheader.Append(string.Format("{0},", "Source Function"));
            sbheader.Append(string.Format("{0},", "Advisor"));
            sbheader.Append(string.Format("{0},", "Introducer Account"));
            sbheader.Append(string.Format("{0},", "Umbrella Organisation")).AppendLine(Environment.NewLine);


            foreach (var data in result)
            {
                sb.Append(string.Format("{0},", "No"));
                sb.Append(string.Format("{0},", data.IllustrationUniqueReference));
                sb.Append(string.Format("{0},", data.ClientName));
                sb.Append(string.Format("{0},", "Personal"));

                sb.Append(string.Format("{0},", ""));//Last Name
                sb.Append(string.Format("{0},", ""));//First Name
                sb.Append(string.Format("{0},", data.PartnerEmail));
                sb.Append(string.Format("{0},", "Introducer Referral"));
                sb.Append(string.Format("{0},", "Client"));
                sb.Append(string.Format("{0},", data.ClientType));
                sb.Append(string.Format("{0},", ""));
                sb.Append(string.Format("{0},", ""));
                sb.Append(string.Format("{0},", data.TotalDeposit));
                sb.Append(string.Format("{0},", data.TotalDeposit));
                sb.Append(string.Format("{0},", "Introducer"));
                sb.Append(string.Format("{0},", data.PartnerName));
                sb.Append(string.Format("{0},", data.PartnerOrganisation));
                sb.Append(string.Format("{0},", "")).AppendLine(Environment.NewLine);


                

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

            var session = new Session() { PartnerEmailAddress = email, PartnerName = name, PartnerOrganisation = organisation, PartnerTelephone = telephone, SuperUser = superUser, SessionId = Guid.NewGuid() };

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


            //Create new session ID
            Session session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            session.SessionId = Guid.NewGuid();

            HttpContext.Session.Remove("SessionPartner");
            HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(session));
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
            model.SessionId = partnerInfo.SessionId;

            if (reset)
            {
                HttpContext.Session.Remove("InputProposal");
                return View(model);
            }

            IllustrationDetailViewModel illustrationInfo = null;
            //Check for old
            if (!string.IsNullOrEmpty((HttpContext.Session.GetString("InputProposal"))))
            {
                illustrationInfo = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));
                partnerInfo.SessionId = Guid.NewGuid();

                HttpContext.Session.Remove("SessionPartner");
                HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(partnerInfo));

                return View(illustrationInfo);
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("InputProposal")))
            {
                illustrationInfo = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));
            }
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

            var illustrationList = _illustrationHelper.GetIllustrationList(searchParams, false);
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
                return Json(new { Data = errors, Success = false });
            }


            var partnerInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            searchParams.PartnerEmail = partnerInfo.PartnerEmailAddress;
            searchParams.PartnerOrganisation = partnerInfo.PartnerOrganisation;
            var result = _illustrationHelper.GetIllustrationList(searchParams, false);
            if (result.Count() > 0) return Json(new { Data = result, Success = true });
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
            //Change session id here to exclude banks
            var partnerInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
            partnerInfo.SessionId = _illustrationHelper.GetSessionIdForIllustration(uniqueReferenceId);

            HttpContext.Session.Remove("SessionPartner");
            HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(partnerInfo));



            ViewBag.URL = AppSettings.illustrationOutputPublicFacingFolder + "/" + uniqueReferenceId + "/" + uniqueReferenceId + "_CashIllustration.pdf";

            ViewBag.User = "";
            TempData["PreserverSession"] = true;
            return View("_illustrationDetails", result);

        }

        public IActionResult Calculate(IllustrationDetailViewModel model)
        {
            /*
             Post Method
             Arguments:- IllustrationDetailViewModel 
             Returns:- View and Errors
             */

            
            TempData["error"] = "false";
            TempData["AllowEdit"] = true;
            bool PreserverSession = false;
            var illustrationInfo = new Session();

            if (TempData["PreserverSession"] != null)
            {
                illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));
                PreserverSession = true;

                TempData["PreserverSession"] = TempData["PreserverSession"];
            }

            
            if (!string.IsNullOrEmpty((HttpContext.Session.GetString("SessionPartner"))) && !PreserverSession)
            {
                illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));

                illustrationInfo.SessionId = Guid.NewGuid();

                HttpContext.Session.Remove("SessionPartner");
                HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(illustrationInfo));
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
                    illustrationInfo.ThreeYearsPlus = model.ThreeYearsPlus;
                    illustrationInfo.TotalDeposit = model.TotalDeposit;
                    illustrationInfo.AdvisorName = model.PartnerName;
                    model.SessionId = illustrationInfo.SessionId;
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


                HttpContext.Session.SetString("GeneratedPorposals", JsonConvert.SerializeObject(sStore));
                HttpContext.Session.SetString("InputProposal", JsonConvert.SerializeObject(model));

                return View(model);
            }
            else
            {
                model.ProposedPortfolio = new Insignis.Asset.Management.Tools.Sales.SCurveOutput();
                return View("Index", model);
            }

        }

        private void CalculateIllustration(IllustrationDetailViewModel model, Session illustrationInfo)
        {
            model.ProposedPortfolio = null;

            Insignis.Asset.Management.Tools.Sales.SCurve scurve = new Insignis.Asset.Management.Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

            scurve.LoadHeatmap(7, "GBP", AppSettings.preferencesRoot);
            //scurve.LoadHeatmap(7, model.Currency, AppSettings.preferencesRoot);

            //Changes here for saved banks in illustrationInfo
            //if(_context.TempInstitution.Any(x => x.SessionId == illustrationInfo.SessionId)){
            //    var savedBank = _context.TempInstitution.OrderByDescending(x => x.Id).First(x => x.SessionId == illustrationInfo.SessionId);
            //    string dbInvestmentTerm = _context.InvestmentTermMapper.First(x => x.InvestmentText == savedBank.InvestmentTerm).InvestmentTerm;

            //    if (dbInvestmentTerm == "Instant Access")
            //    {
            //        illustrationInfo.EasyAccess -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }
                
            //    if (dbInvestmentTerm == "One Month")
            //    {
            //        illustrationInfo.OneMonth -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }

            //    if (dbInvestmentTerm == "Three Months")
            //    {
            //        illustrationInfo.ThreeMonths -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }
                
            //    if (dbInvestmentTerm == "Six Months")
            //    {
            //        illustrationInfo.SixMonths -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }

            //    if (dbInvestmentTerm == "One Year")
            //    {
            //        illustrationInfo.OneYear -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }

            //    if (dbInvestmentTerm == "Two Years")
            //    {
            //        illustrationInfo.TwoYears -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }
                
            //    if (dbInvestmentTerm == "Three Years")
            //    {
            //        illustrationInfo.ThreeYearsPlus -= Convert.ToDouble(savedBank.Amount);
            //        illustrationInfo.TotalDeposit -= Convert.ToDouble(savedBank.Amount);
            //    }
            //}

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

            var excludedInstituteIds = _context.ExcludedInstitutes.Where(x => x.SessionId == illustrationInfo.SessionId && x.IsUpdatedBank == false).Select(x => x.InstituteId).ToList();

            foreach (var childern in institutionInclusion.Children)
            {
                childern.Value = "true";
                if (excludedInstituteIds.Contains(Convert.ToInt32(childern.Name)))
                    childern.Value = "false";
            }
            
            var feeMatrix = new FeeMatrix(fscsProtectionConfigFile + "FeeMatrix.xml");

            








            model.ProposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);

            //var tempBanks = _context.TempInstitution.Where(x=>x.SessionId == illustrationInfo.SessionId).ToList();
            
            //foreach (var bank in tempBanks)
            //{
            //    Insignis.Asset.Management.Tools.Sales.SCurveOutputRow row = new Insignis.Asset.Management.Tools.Sales.SCurveOutputRow();
            //    row.InstitutionName = bank.InstitutionName;
            //    row.InstitutionID = bank.BankId;
            //    row.InvestmentTerm = new InvestmentTerm();
            //    row.InvestmentTerm.TermText = bank.InvestmentTerm;
            //    row.Rate = bank.Rate;
            //    row.DepositSize = bank.Amount;
            //    row.AnnualInterest = ((bank.Rate / 100) * bank.Amount);

            //    //_sStore.ProposedInvestments.Add(row);
            //    model.ProposedPortfolio.ProposedInvestments.Add(row);
            //}


            //Check for any saved banks TODO
            

            model.AnnualGrossInterestEarned = 0;
            model.TotalDeposit = 0;

            foreach (var investment in model.ProposedPortfolio.ProposedInvestments)
            {
                model.AnnualGrossInterestEarned += investment.AnnualInterest;
                model.TotalDeposit +=Convert.ToDouble(investment.DepositSize);
            }

            model.ProposedPortfolio.AnnualGrossInterestEarned = model.AnnualGrossInterestEarned;
            model.ProposedPortfolio.TotalDeposited =Convert.ToDecimal(model.TotalDeposit);
            model.GrossAverageYield = (model.ProposedPortfolio.AnnualGrossInterestEarned / Convert.ToDecimal(model.TotalDeposit)) * 100;

            if(model.TotalDeposit.Value >= 50000 && model.TotalDeposit <= 299999)
                model.ProposedPortfolio.FeePercentage = 0.25M;

            if (model.TotalDeposit.Value >= 300000 && model.TotalDeposit <= 999999)
                model.ProposedPortfolio.FeePercentage = 0.20M;

            model.NetAverageYield = (model.GrossAverageYield - model.ProposedPortfolio.FeePercentage);

            

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
            prefMaximumDepositInAnyOneInstitution.Value = sessionData.ClientType == 0 ? "85000" : "170000";



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
            _liquidityAmount.Add(sessionData.ThreeYearsPlus.ToString());//3 year bond
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
            CultureInfo gb = new CultureInfo("en-GB");
            //Getting Input Illustration from InputProposal Session
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("InputProposal"))) model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));

            var generatedInvestments = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals")); //scurve output

            //Insignis.Asset.Management.Tools.Sales.SCurveOutput _sc= new Insignis.Asset.Management.Tools.Sales.SCurveOutput();

            model.ProposedPortfolio = _mapper.Map(generatedInvestments, new Insignis.Asset.Management.Tools.Sales.SCurveOutput());

            Octavo.Gate.Nabu.Encryption.EncryptorDecryptor decryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();

            //illustration template
            string qsTemplateFile = ConfigurationManager.AppSettings.Get("illustrationTemplateRoot") + "\\" + model.PartnerOrganisation + "\\" + model.PartnerOrganisation + ".pptx";

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
                prefixName = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + _illustrationHelper.GetNextIllustrationRefernce().ToString();//Get Last prefix number from db
            }


            //....................................Save Excluded Institutes with unique Id..............................................


            bool excludedInstitutes = _context.ExcludedInstitutes.Any(x=>x.SessionId == model.SessionId && x.PartnerEmail == model.PartnerEmail && x.PartnerOrganisation == model.PartnerOrganisation && x.ClientReference == model.ClientName);
            if (excludedInstitutes)
            {
                var instList = _context.ExcludedInstitutes.Where(x => x.SessionId == model.SessionId && x.PartnerEmail == model.PartnerEmail && x.PartnerOrganisation == model.PartnerOrganisation && x.ClientReference == model.ClientName).ToList();

                foreach (var bank in instList)
                {
                    bank.ClientReference = string.Empty;
                    bank.UniqueReferenceId = prefixName;
                    _context.SaveChanges();
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------



            model.IllustrationUniqueReference = prefixName;
            string requiredOutputNameWithoutExtension = prefixName + "_CashIllustration";


            List<KeyValuePair<string, string>> textReplacements = new List<KeyValuePair<string, string>>();

            //var illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("SessionPartner"));

            textReplacements.Add(new KeyValuePair<string, string>("REFERENCE", prefixName));
            textReplacements.Add(new KeyValuePair<string, string>("DATE", DateTime.Now.ToString("dd/MM/yyyy")));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTNAME", model.ClientName));
            textReplacements.Add(new KeyValuePair<string, string>("CLIENTTYPE", model.ClientType == 0 ? "Individual" : "Joint"));
            textReplacements.Add(new KeyValuePair<string, string>("INTROORG", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEEDISCOUNT", ""));
            textReplacements.Add(new KeyValuePair<string, string>("FEE", model.ProposedPortfolio.FeePercentage.ToString()));
            textReplacements.Add(new KeyValuePair<string, string>("CHARGE", ""));
            textReplacements.Add(new KeyValuePair<string, string>("TOTAL", "£" + model.TotalDeposit.Value.ToString("N", gb)));
            textReplacements.Add(new KeyValuePair<string, string>("PROTECTION", "100"));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSYIELD", model.GrossAverageYield.ToString("#.###")));
            textReplacements.Add(new KeyValuePair<string, string>("GROSSINTEREST", "£" + model.AnnualGrossInterestEarned.ToString("N", gb)));
            textReplacements.Add(new KeyValuePair<string, string>("NETYIELD", model.NetAverageYield.ToString("#.###")));
            textReplacements.Add(new KeyValuePair<string, string>("NETINTEREST", "£" + model.AnnualNetInterestEarned.ToString("N", gb)));

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
                    institutionName = model.ProposedPortfolio.ProposedInvestments[i - 1].InstitutionName;
                    termDescription = model.ProposedPortfolio.ProposedInvestments[i - 1].InvestmentTerm.GetText() == "n/a" ? model.ProposedPortfolio.ProposedInvestments[i - 1].InvestmentTerm.TermText : model.ProposedPortfolio.ProposedInvestments[i - 1].InvestmentTerm.GetText();// heatmapTerm.InvestmentTerm.GetText();
                    rate = model.ProposedPortfolio.ProposedInvestments[i - 1].Rate.ToString("0.00") + "%";
                    deposit = "£" + model.ProposedPortfolio.ProposedInvestments[i - 1].DepositSize.ToString("N", gb);
                    interest = "£" + model.ProposedPortfolio.ProposedInvestments[i - 1].AnnualInterest.ToString("N", gb);
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
            if (Directory.Exists(AppSettings.illustrationOutputInternalFolder + "\\" + prefixName))
            {
                Directory.Delete(AppSettings.illustrationOutputInternalFolder + "\\" + prefixName, true);
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

        public IActionResult Update(string includeBank, string bankId, string updatedAmount, string instituteName, string investmentTerm, string rate, string annualInterest, string clientType, string oldAmount)
        {
            var illustrationInfo = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("InputProposal"));
            var partnerEmail = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));

            if (TempData["AllowEdit"].ToString() == "False" && includeBank != null)
            {
                ModelState.AddModelError("error", "Please create a new version of this illustration if you need to change the amount.");

                TempData["error"] = "true";
                TempData["AllowEdit"] = TempData["AllowEdit"];

                ViewBag.BankId = bankId;
                ViewBag.UpdatedAmount = updatedAmount;
                ViewBag.InstituteName = instituteName;
                ViewBag.InvestmentTerm = investmentTerm;
                ViewBag.Rate = rate;
                ViewBag.AnnualInterest = annualInterest;
                ViewBag.ClientType = clientType;

                SCurveOutput _sStore = new SCurveOutput();

                _sStore = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals"));


                var _model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));


                _model.ProposedPortfolio = _mapper.Map(_sStore, _model.ProposedPortfolio);

                ViewBag.Edited = true;
                return View("Calculate", _model);

            }


            if (Convert.ToDecimal(updatedAmount) > Convert.ToDecimal(oldAmount) && includeBank != null)
            {

                if ((clientType == "0" && Convert.ToDecimal(updatedAmount) > 85000) || (clientType == "1" && Convert.ToDecimal(updatedAmount) > 175000))
                {
                    ModelState.AddModelError("error", "The amount entered exceeds funds protected under the FSCS scheme. Please raise an Illustration Request and a member of the Account Management team will be in touch.");
                    TempData["error"] = "true";


                    ViewBag.BankId = bankId;
                    ViewBag.UpdatedAmount = updatedAmount;
                    ViewBag.InstituteName = instituteName;
                    ViewBag.InvestmentTerm = investmentTerm;
                    ViewBag.Rate = rate;
                    ViewBag.AnnualInterest = annualInterest;
                    ViewBag.ClientType = clientType;

                    SCurveOutput __sStore = new SCurveOutput();

                    __sStore = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals"));


                    var __model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));


                    __model.ProposedPortfolio = _mapper.Map(__sStore, __model.ProposedPortfolio);

                    return View("Calculate", __model);
                }

                //.........................................................Handle increased amount case...........................................

                //.............................Save bank to temp table and exclude it from calculation......................................................
                TempInstitution temp = new TempInstitution();
                temp.BankId = Convert.ToInt32(bankId);
                temp.ClientName = illustrationInfo.ClientName;
                temp.Amount = Convert.ToDecimal(updatedAmount);
                temp.InstitutionName = instituteName;
                temp.InvestmentTerm = investmentTerm;
                temp.PartnerEmail = partnerEmail.PartnerEmail;
                temp.PartnerName = illustrationInfo.PartnerName;
                temp.PartnerOrganisation = illustrationInfo.PartnerOrganisation;
                temp.SessionId = illustrationInfo.SessionId;
                var _rate = rate.Split(" ");
                temp.Rate = Convert.ToDecimal(_rate[0]);
                temp.AnnualInterest = Convert.ToDecimal(annualInterest);
                _context.TempInstitution.Add(temp);

                //bank saved to database

                ExcludedInstitute inst = new ExcludedInstitute();
                inst.ClientReference = partnerEmail.ClientName;
                inst.PartnerEmail = partnerEmail.PartnerEmail;
                inst.PartnerOrganisation = partnerEmail.PartnerOrganisation;
                inst.InstituteId = Convert.ToInt32(bankId);
                inst.SessionId = illustrationInfo.SessionId;

                _context.ExcludedInstitutes.Add(inst);
                _context.SaveChanges();

                //................................................................Update total amount.........................
                string differenceAmount = (Convert.ToDecimal(updatedAmount) - Convert.ToDecimal(oldAmount)).ToString();
                illustrationInfo.TotalDeposit += Convert.ToDouble(differenceAmount);

                //................................................................Remove amount from investment period...........................................
                var _dbInvestment = _context.InvestmentTermMapper.Where(x => x.InvestmentText == investmentTerm).SingleOrDefault();


                bool additionData = false;
                if (_dbInvestment.InvestmentTerm == "Instant Access")
                {
                    illustrationInfo.EasyAccess += Convert.ToDouble(differenceAmount);
                    illustrationInfo.EasyAccess -= Convert.ToDouble(updatedAmount);
                    additionData = true;
                }
                if (_dbInvestment.InvestmentTerm == "One Month")
                {
                    illustrationInfo.OneMonth += Convert.ToDouble(differenceAmount);
                    illustrationInfo.OneMonth -= Convert.ToDouble(updatedAmount);
                    additionData = true;

                }

                if (_dbInvestment.InvestmentTerm == "Three Months")
                {
                    illustrationInfo.ThreeMonths += Convert.ToDouble(differenceAmount);
                    illustrationInfo.ThreeMonths -= Convert.ToDouble(updatedAmount);
                    additionData = true;

                }
                if (_dbInvestment.InvestmentTerm == "Six Months")
                {
                    illustrationInfo.SixMonths += Convert.ToDouble(differenceAmount);
                    illustrationInfo.SixMonths -= Convert.ToDouble(updatedAmount);
                    additionData = true;

                }
                if (_dbInvestment.InvestmentTerm == "Nine Months")
                {
                    illustrationInfo.NineMonths += Convert.ToDouble(differenceAmount);
                    illustrationInfo.NineMonths -= Convert.ToDouble(updatedAmount);
                    additionData = true;

                }
                if (_dbInvestment.InvestmentTerm == "One Year")
                {
                    illustrationInfo.OneYear += Convert.ToDouble(differenceAmount);
                    illustrationInfo.OneYear -= Convert.ToDouble(updatedAmount);
                    additionData = true;


                }
                if (_dbInvestment.InvestmentTerm == "Two Years")
                {
                    illustrationInfo.TwoYears += Convert.ToDouble(differenceAmount);
                    illustrationInfo.TwoYears -= Convert.ToDouble(updatedAmount);
                    additionData = true;


                }
                if (_dbInvestment.InvestmentTerm == "Three Years")
                {
                    illustrationInfo.ThreeYearsPlus += Convert.ToDouble(differenceAmount);
                    illustrationInfo.ThreeYearsPlus -= Convert.ToDouble(updatedAmount);
                    additionData = true;

                }


                illustrationInfo.TotalDeposit -= Convert.ToDouble(updatedAmount);


                illustrationInfo.PartnerEmailAddress = partnerEmail.PartnerEmail;
                IllustrationDetailViewModel _model = new IllustrationDetailViewModel();

                _model.ProposedPortfolio = null;

                Insignis.Asset.Management.Tools.Sales.SCurve _scurve = new Insignis.Asset.Management.Tools.Sales.SCurve(multiLingual.GetAbstraction(), multiLingual.language);

                _scurve.LoadHeatmap(7, "GBP", AppSettings.preferencesRoot);
                //scurve.LoadHeatmap(7, model.Currency, AppSettings.preferencesRoot);

                Insignis.Asset.Management.Tools.Sales.SCurveSettings _settings = ProcessPostback(illustrationInfo, false, _scurve.heatmap);

                string _fscsProtectionConfigFile = AppSettings.ClientConfigRoot;// ConfigurationManager.AppSettings["clientConfigRoot"];
                if (_fscsProtectionConfigFile.EndsWith("\\") == false)
                    _fscsProtectionConfigFile += "\\";
                _fscsProtectionConfigFile += "FSCSProtection.xml";

                Octavo.Gate.Nabu.Preferences.Manager _preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(AppSettings.preferencesRoot + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerOrganisation) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(illustrationInfo.PartnerEmailAddress));

                _preferencesManager.DeletePreferences("Sales.Tools.SCurve.Settings", 1);

                Octavo.Gate.Nabu.Preferences.Preference _scurveBuilder = _preferencesManager.GetPreference("Sales.Tools.SCurve.Builder", 1, "Settings");
                int _availableToHubAccountTypeID = -1;
                if (_scurveBuilder != null)
                {
                    if (_scurveBuilder.GetChildPreference("AvailableTo") != null && _scurveBuilder.GetChildPreference("AvailableTo").Value.Trim().Length > 0)
                    {
                        try
                        {
                            _availableToHubAccountTypeID = Convert.ToInt32(_scurveBuilder.GetChildPreference("AvailableTo").Value);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        _availableToHubAccountTypeID = financialAbstraction.GetAccountTypeByAlias("ACT_PERSONALHUBACCOUNT", (int)multiLingual.language.LanguageID).AccountTypeID.Value;
                        _scurveBuilder.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference("AvailableTo", _availableToHubAccountTypeID.ToString()));
                    }
                }
                _preferencesManager.DeletePreferences("Sales.Tools.SCurve.Institutions", 1);
                Octavo.Gate.Nabu.Preferences.Preference _institutionInclusion = new Octavo.Gate.Nabu.Preferences.Preference("Institutions", "");
                Institution[] _allInstitutions = financialAbstraction.ListInstitutions((int)multiLingual.language.LanguageID);
                foreach (Institution institution in _allInstitutions)
                {
                    if (institution.ShortName.CompareTo("NationalSavingsInvestments") != 0)
                        _institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "true"));
                    else
                        _institutionInclusion.SetChildPreference(new Octavo.Gate.Nabu.Preferences.Preference(institution.PartyID.ToString(), "false"));
                }
                _preferencesManager.SetPreference("Sales.Tools.SCurve.Institutions", 1, _institutionInclusion);

                _preferencesManager.DeletePreferences("Sales.Tools.SCurve.Properties." + _availableToHubAccountTypeID, 1);

                Octavo.Gate.Nabu.Preferences.Preference _scurveBuilderDeposits = _preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + _availableToHubAccountTypeID, 1, "Deposits");
                if (_scurveBuilderDeposits != null && _scurveBuilderDeposits.Children.Count > 0)
                {
                    _scurveBuilderDeposits.Children.Clear();
                    _preferencesManager.SetPreference("Sales.Tools.SCurve.Builder." + _availableToHubAccountTypeID, 1, _scurveBuilderDeposits);
                }


                //get list of excluded institutes
                var _excludedInstituteIds = _context.ExcludedInstitutes.Where(x => x.ClientReference == partnerEmail.ClientName && x.SessionId == illustrationInfo.SessionId && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation).Select(x => x.InstituteId).ToList();


                foreach (var childern in _institutionInclusion.Children)
                {
                    if (childern.Name != bankId)
                        childern.Value = "true";
                    if (_excludedInstituteIds.Contains(Convert.ToInt32(childern.Name)))
                        childern.Value = "false";
                }

                var _feeMatrix = new FeeMatrix(_fscsProtectionConfigFile + "FeeMatrix.xml");
                _model.ProposedPortfolio = _scurve.Process(_settings, _fscsProtectionConfigFile, _institutionInclusion);


                _model.ClientName = illustrationInfo.ClientName;
                _model.ClientType = illustrationInfo.ClientType;
                _model.Currency = illustrationInfo.Currency;
                _model.EasyAccess = illustrationInfo.EasyAccess;
                _model.NineMonths = illustrationInfo.NineMonths;
                _model.OneMonth = illustrationInfo.OneMonth;
                _model.OneYear = illustrationInfo.OneYear;
                _model.PartnerEmail = illustrationInfo.PartnerEmailAddress;
                _model.PartnerName = illustrationInfo.PartnerName;
                _model.PartnerOrganisation = illustrationInfo.PartnerOrganisation;
                _model.SixMonths = illustrationInfo.SixMonths;
                _model.ThreeMonths = illustrationInfo.ThreeMonths;
                _model.ThreeYearsPlus = illustrationInfo.ThreeYearsPlus;
                _model.TwoYears = illustrationInfo.TwoYears;

                _model.SessionId = illustrationInfo.SessionId;

                SCurveOutput _sStore = new SCurveOutput();

                ////add saved one to display

                _sStore = _mapper.Map(_model.ProposedPortfolio, _sStore);


                //check db for any saved bank
                bool __savedBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.SessionId== partnerEmail.SessionId);
                if (__savedBank)
                {
                    var tempBanks = _context.TempInstitution.Where(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.SessionId == partnerEmail.SessionId).ToList();
                    TempData["AllowEdit"] = false;
                    foreach (var bank in tempBanks)
                    {
                        Insignis.Asset.Management.Tools.Sales.SCurveOutputRow row = new Insignis.Asset.Management.Tools.Sales.SCurveOutputRow();
                        row.InstitutionName = bank.InstitutionName;
                        row.InstitutionID = bank.BankId;
                        row.InvestmentTerm = new InvestmentTerm();
                        row.InvestmentTerm.TermText = bank.InvestmentTerm;
                        row.Rate = bank.Rate;
                        row.DepositSize = bank.Amount;
                        row.AnnualInterest = ((bank.Rate/100) * bank.Amount);

                        _sStore.ProposedInvestments.Add(row);
                        _model.ProposedPortfolio.ProposedInvestments.Add(row);
                    }
                }

                decimal _total = 0;
                //investment term
                foreach (var m in _model.ProposedPortfolio.ProposedInvestments)
                {
                    _total += m.DepositSize;
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

                _model.TotalDeposit = Convert.ToDouble(_total);
                

                _model.AnnualGrossInterestEarned = 0;
                //Saved bank da rate again gross interest calculate 
                
                foreach (var investment in _model.ProposedPortfolio.ProposedInvestments)
                {
                    _model.AnnualGrossInterestEarned += investment.AnnualInterest;
                }
                _model.ProposedPortfolio.AnnualGrossInterestEarned = _model.AnnualGrossInterestEarned;
                //divi
                _model.GrossAverageYield = (_model.ProposedPortfolio.AnnualGrossInterestEarned / Convert.ToDecimal(_model.TotalDeposit)) * 100;
                if (_model.TotalDeposit.Value >= 50000 && _model.TotalDeposit <= 299999)
                    _model.ProposedPortfolio.FeePercentage = 0.25M;

                if (_model.TotalDeposit.Value >= 300000 && _model.TotalDeposit <= 999999)
                    _model.ProposedPortfolio.FeePercentage = 0.20M;
                _model.NetAverageYield = (_model.GrossAverageYield - _model.ProposedPortfolio.FeePercentage);
                _model.ProposedPortfolio.Fee = (Convert.ToDecimal(_model.TotalDeposit) * (decimal)(_model.ProposedPortfolio.FeePercentage / 100));
                _model.AnnualNetInterestEarned = (_model.ProposedPortfolio.AnnualGrossInterestEarned - _model.ProposedPortfolio.Fee);


                if (additionData)
                {

                    if (_dbInvestment.InvestmentTerm == "Instant Access")
                    {
                        
                        _model.EasyAccess += Convert.ToDouble(updatedAmount);
                        
                     
                    }
                    if (_dbInvestment.InvestmentTerm == "One Month")
                    {

                        _model.OneMonth += Convert.ToDouble(updatedAmount);

                    }

                    if (_dbInvestment.InvestmentTerm == "Three Months")
                    {

                        _model.ThreeMonths += Convert.ToDouble(updatedAmount);
                     

                    }
                    if (_dbInvestment.InvestmentTerm == "Six Months")
                    {

                        _model.SixMonths += Convert.ToDouble(updatedAmount);
                     

                    }
                    if (_dbInvestment.InvestmentTerm == "Nine Months")
                    {
                        _model.NineMonths += Convert.ToDouble(updatedAmount);
                    }
                    if (_dbInvestment.InvestmentTerm == "One Year")
                    {
                        _model.OneYear += Convert.ToDouble(updatedAmount);
                    }
                    if (_dbInvestment.InvestmentTerm == "Two Years")
                    {
                        _model.TwoYears += Convert.ToDouble(updatedAmount);
                    }
                    if (_dbInvestment.InvestmentTerm == "Three Years")
                    {
                        _model.ThreeYearsPlus += Convert.ToDouble(updatedAmount);
                    }
                }

                _model.IllustrationUniqueReference = partnerEmail.IllustrationUniqueReference;
                HttpContext.Session.SetString("GeneratedPorposals", JsonConvert.SerializeObject(_sStore));
                HttpContext.Session.SetString("InputProposal", JsonConvert.SerializeObject(_model));
                return View("Calculate", _model);
            }
            //................................................Update Increase Case END...........................................................................................................

            if ((clientType=="0" && Convert.ToDecimal(updatedAmount)>85000) || (clientType == "1" && Convert.ToDecimal(updatedAmount)>175000))
            {
                ModelState.AddModelError("error", "The amount entered exceeds funds protected under the FSCS scheme. Please raise an Illustration Request and a member of the Account Management team will be in touch.");
                
                TempData["error"] = "true";


                ViewBag.BankId = bankId;
                ViewBag.UpdatedAmount = updatedAmount;
                ViewBag.InstituteName = instituteName;
                ViewBag.InvestmentTerm = investmentTerm;
                ViewBag.Rate = rate;
                ViewBag.AnnualInterest = annualInterest;
                ViewBag.ClientType = clientType;
                
                SCurveOutput _sStore = new SCurveOutput();

                _sStore = JsonConvert.DeserializeObject<SCurveOutput>(HttpContext.Session.GetString("GeneratedPorposals"));


                var _model = JsonConvert.DeserializeObject<IllustrationDetailViewModel>(HttpContext.Session.GetString("InputProposal"));


                _model.ProposedPortfolio = _mapper.Map(_sStore, _model.ProposedPortfolio);

                return View("Calculate",_model);

            }

            //Check if any deposit exists before allotment
            bool _savedBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.SessionId == partnerEmail.SessionId);


            bool sessionVariable = false;
            decimal amount = 0;
            string whichTerm = string.Empty;

            if (_savedBank)
            {
                TempData["AllowEdit"] = TempData["AllowEdit"];
                
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
                        
                        inst.SessionId = illustrationInfo.SessionId;
                        _context.ExcludedInstitutes.Add(inst);
                        _context.SaveChanges();
                        //HttpContext.Session.SetString("ExcludedInstitution", JsonConvert.SerializeObject(inst));

                        
                        if(_dbInvestment.InvestmentText != investmentTerm)
                        {


                            if (_dbInvestment.InvestmentTerm == "Instant Access")
                            {
                                illustrationInfo.EasyAccess -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Instant Access";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "One Month")
                            {
                                illustrationInfo.OneMonth -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "One Month";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }

                            if (_dbInvestment.InvestmentTerm == "Three Months")
                            {
                                illustrationInfo.ThreeMonths -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Three Months";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "Six Months" )
                            {
                                illustrationInfo.SixMonths -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Six Months";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "Nine Months" )
                            {
                                illustrationInfo.NineMonths -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Nine Months";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "One Year")
                            {
                                illustrationInfo.OneYear -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "One Year";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "Two Years")
                            {
                                illustrationInfo.TwoYears -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Two Years";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }
                            if (_dbInvestment.InvestmentTerm == "Three Years")
                            {
                                illustrationInfo.ThreeYearsPlus -= Convert.ToDouble(bank.Amount);
                                amount = bank.Amount;
                                whichTerm = "Three Years";
                                illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                            }




                        }
                        
                        
                        
                        
                        if (_dbInvestment.InvestmentTerm == "Instant Access" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.EasyAccess -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Instant Access";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "One Month" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.OneMonth -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "One Month";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }

                        if (_dbInvestment.InvestmentTerm == "Three Months" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.ThreeMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Three Months";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "Six Months" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.SixMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Six Months";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "Nine Months" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.NineMonths -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Nine Months";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "One Year" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.OneYear -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "One Year";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "Two Years" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.TwoYears -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Two Years";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }
                        if (_dbInvestment.InvestmentTerm == "Three Years" && _dbInvestment.InvestmentText == investmentTerm)
                        {
                            illustrationInfo.ThreeYearsPlus -= Convert.ToDouble(bank.Amount);
                            amount = bank.Amount;
                            whichTerm = "Three Years";
                            illustrationInfo.TotalDeposit -= Convert.ToDouble(bank.Amount);
                        }

                        

                        sessionVariable = true;

                    }

                }
            }


            //............Save if institute excluded................................................................................
            bool updatedBanks = true;
            if (includeBank == null || updatedAmount == "0")
            {
                TempData["AllowEdit"] = TempData["AllowEdit"];
                bool alreadyExcluded = _context.ExcludedInstitutes.Any(x => x.InstituteId == Convert.ToInt32(bankId) && x.SessionId == illustrationInfo.SessionId && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.ClientReference == partnerEmail.ClientName);


                if (!alreadyExcluded)
                {
                    ExcludedInstitute inst = new ExcludedInstitute();
                    inst.ClientReference = partnerEmail.ClientName;
                    inst.PartnerEmail = partnerEmail.PartnerEmail;
                    inst.PartnerOrganisation = partnerEmail.PartnerOrganisation;
                    inst.InstituteId = Convert.ToInt32(bankId);
                    inst.SessionId = illustrationInfo.SessionId;

                    _context.ExcludedInstitutes.Add(inst);
                    _context.SaveChanges();
                }
                bool _tempBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.InstitutionName == instituteName);
                if (_tempBank)
                {
                    var remov= _context.TempInstitution.Single(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.InstitutionName == instituteName);
                    _context.TempInstitution.Remove(remov);
                    _context.SaveChanges();

                    
                    var dbInvestmentTerm = _context.InvestmentTermMapper.Where(x => x.InvestmentText == investmentTerm).SingleOrDefault();


                    if(remov != null && remov.InvestmentTerm == investmentTerm)
                    {

                        if (dbInvestmentTerm.InvestmentTerm == "Instant Access")
                            illustrationInfo.EasyAccess +=  Convert.ToDouble(updatedAmount);


                        if (dbInvestmentTerm.InvestmentTerm == "One Month")
                            illustrationInfo.OneMonth += Convert.ToDouble(updatedAmount);


                        if (dbInvestmentTerm.InvestmentTerm == "Three Months")
                            illustrationInfo.ThreeMonths +=  Convert.ToDouble(updatedAmount);


                        if (dbInvestmentTerm.InvestmentTerm == "Six Months")
                            illustrationInfo.SixMonths +=  Convert.ToDouble(updatedAmount);


                        if (dbInvestmentTerm.InvestmentTerm == "Nine Months")
                            illustrationInfo.NineMonths +=  Convert.ToDouble(updatedAmount);


                        if (dbInvestmentTerm.InvestmentTerm == "One Year")
                            illustrationInfo.OneYear +=  Convert.ToDouble(updatedAmount);

                        if (dbInvestmentTerm.InvestmentTerm == "Two Years")
                            illustrationInfo.TwoYears +=  Convert.ToDouble(updatedAmount);

                        if (dbInvestmentTerm.InvestmentTerm == "Three Years")
                            illustrationInfo.ThreeYearsPlus +=  Convert.ToDouble(updatedAmount);


                    }

                    if( remov == null) { 
                    if (dbInvestmentTerm.InvestmentTerm == "Instant Access")
                        illustrationInfo.EasyAccess += 2 * Convert.ToDouble(updatedAmount);


                    if (dbInvestmentTerm.InvestmentTerm == "One Month")
                        illustrationInfo.OneMonth += 2 * Convert.ToDouble(updatedAmount);


                    if (dbInvestmentTerm.InvestmentTerm == "Three Months")
                        illustrationInfo.ThreeMonths += 2 * Convert.ToDouble(updatedAmount);


                    if (dbInvestmentTerm.InvestmentTerm == "Six Months")
                        illustrationInfo.SixMonths += 2 * Convert.ToDouble(updatedAmount);


                    if (dbInvestmentTerm.InvestmentTerm == "Nine Months")
                        illustrationInfo.NineMonths += 2 * Convert.ToDouble(updatedAmount);


                    if (dbInvestmentTerm.InvestmentTerm == "One Year")
                        illustrationInfo.OneYear += 2 * Convert.ToDouble(updatedAmount);

                    if (dbInvestmentTerm.InvestmentTerm == "Two Years")
                        illustrationInfo.TwoYears += 2 * Convert.ToDouble(updatedAmount);

                    if (dbInvestmentTerm.InvestmentTerm == "Three Years")
                        illustrationInfo.ThreeYearsPlus += 2 * Convert.ToDouble(updatedAmount);

                    }
                    updatedBanks = false;

                }
            }
            //......................................................................................................................



            //STEP1:.............save updated amount and bank to database..............
            if (includeBank != null && Convert.ToDecimal(updatedAmount) > 0)
            {
                TempInstitution temp = new TempInstitution();
                temp.BankId = Convert.ToInt32(bankId);
                temp.ClientName = illustrationInfo.ClientName;
                temp.Amount = Convert.ToDecimal(updatedAmount);
                temp.InstitutionName = instituteName;
                temp.InvestmentTerm = investmentTerm;
                temp.PartnerEmail = partnerEmail.PartnerEmail;
                temp.PartnerName = illustrationInfo.PartnerName;
                temp.PartnerOrganisation = illustrationInfo.PartnerOrganisation;
                var _rate = rate.Split(" ");
                temp.Rate = Convert.ToDecimal(_rate[0]);
                temp.SessionId = illustrationInfo.SessionId;
                temp.AnnualInterest = Convert.ToDecimal(annualInterest);
                _context.TempInstitution.Add(temp);
                TempData["AllowEdit"] = false;
                //bank saved to database

                ExcludedInstitute inst = new ExcludedInstitute();
                inst.ClientReference = partnerEmail.ClientName;
                inst.PartnerEmail = partnerEmail.PartnerEmail;
                inst.PartnerOrganisation = partnerEmail.PartnerOrganisation;
                inst.InstituteId = Convert.ToInt32(bankId);
                inst.SessionId = partnerEmail.SessionId;
                inst.IsUpdatedBank = true;

                _context.ExcludedInstitutes.Add(inst);
                _context.SaveChanges();
                //save to exclude
            }


            //CHECK IF CASE IS INCLUDED AND BANK AMOUNT IS BEING CHANGED
            //STEP2:................................Delete saved investment from calculation..............................
            var dbInvestment = _context.InvestmentTermMapper.Where(x => x.InvestmentText == investmentTerm).SingleOrDefault();

            if (includeBank != null && Convert.ToDecimal(updatedAmount) > 0)
            {

                if (dbInvestment.InvestmentTerm == "Instant Access")
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
                    illustrationInfo.ThreeYearsPlus -= Convert.ToDouble(updatedAmount);

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
            var excludedInstituteIds = _context.ExcludedInstitutes.Where(x => x.ClientReference == partnerEmail.ClientName && x.SessionId == illustrationInfo.SessionId && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation).Select(x => x.InstituteId).ToList();


            var savedexcludedInstituteIds = _context.ExcludedInstitutes.Where(x => x.SessionId == illustrationInfo.SessionId).Select(x => x.InstituteId).ToList();

            foreach (var childern in institutionInclusion.Children)
            {
                if (childern.Name != bankId)
                    childern.Value = "true";
                if (excludedInstituteIds.Contains(Convert.ToInt32(childern.Name)))
                    childern.Value = "false";
                if (savedexcludedInstituteIds.Contains(Convert.ToInt32(childern.Name)))
                    childern.Value = "false";

            }

            var feeMatrix = new FeeMatrix(fscsProtectionConfigFile + "FeeMatrix.xml");
            model.ProposedPortfolio = scurve.Process(settings, fscsProtectionConfigFile, institutionInclusion);


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
            model.ThreeYearsPlus = illustrationInfo.ThreeYearsPlus;
            model.TwoYears = illustrationInfo.TwoYears;
            model.SessionId = illustrationInfo.SessionId;


            SCurveOutput sStore = new SCurveOutput();

            ////add saved one to display

            sStore = _mapper.Map(model.ProposedPortfolio, sStore);


            //check db for any saved bank
            bool savedBank = _context.TempInstitution.Any(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.SessionId == partnerEmail.SessionId);
            if (savedBank)
            {
                var tempBanks = _context.TempInstitution.Where(x => x.ClientName == partnerEmail.ClientName && x.PartnerEmail == partnerEmail.PartnerEmail && x.PartnerOrganisation == partnerEmail.PartnerOrganisation && x.SessionId == partnerEmail.SessionId).ToList();

                foreach (var bank in tempBanks)
                {
                    Insignis.Asset.Management.Tools.Sales.SCurveOutputRow row = new Insignis.Asset.Management.Tools.Sales.SCurveOutputRow();
                    row.InstitutionName = bank.InstitutionName;
                    row.InstitutionID = bank.BankId;
                    row.InvestmentTerm = new InvestmentTerm();
                    row.InvestmentTerm.TermText = bank.InvestmentTerm;
                    row.Rate = bank.Rate;
                    row.DepositSize = bank.Amount;
                    row.AnnualInterest = bank.AnnualInterest;

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

            model.TotalDeposit = Convert.ToDouble(total);
            if (includeBank != null && Convert.ToDecimal(updatedAmount) > 0)
            {

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


            if (sessionVariable && updatedBanks)
            {
                if (whichTerm == "Instant Access")
                    model.EasyAccess += Convert.ToDouble(amount);
                if (whichTerm == "One Month")
                    model.OneMonth += Convert.ToDouble(amount);
                if (whichTerm == "Three Months")
                    model.ThreeMonths += Convert.ToDouble(amount);
                if (whichTerm == "Six Months")
                    model.SixMonths += Convert.ToDouble(amount);
                if (whichTerm == "Nine Months")
                    model.NineMonths += Convert.ToDouble(amount);
                if (whichTerm == "One Year")
                    model.OneYear += Convert.ToDouble(amount);
                if (whichTerm == "Two Years")
                    model.TwoYears += Convert.ToDouble(amount);
                if (whichTerm == "Three Years")
                    model.ThreeYearsPlus += Convert.ToDouble(amount);

            }

            model.AnnualGrossInterestEarned = 0;
            foreach (var investment in model.ProposedPortfolio.ProposedInvestments)
            {
                model.AnnualGrossInterestEarned += (investment.DepositSize * Convert.ToDecimal(investment.Rate)/100);
                
            }

            model.ProposedPortfolio.AnnualGrossInterestEarned = model.AnnualGrossInterestEarned;
            //divi
            model.GrossAverageYield = (model.AnnualGrossInterestEarned /Convert.ToDecimal(model.TotalDeposit)) * 100;
            if (model.TotalDeposit.Value >= 50000 && model.TotalDeposit <= 299999)
                model.ProposedPortfolio.FeePercentage = 0.25M;

            if (model.TotalDeposit.Value >= 300000 && model.TotalDeposit <= 999999)
                model.ProposedPortfolio.FeePercentage = 0.20M;
            model.NetAverageYield = (model.GrossAverageYield - model.ProposedPortfolio.FeePercentage);
            
            model.ProposedPortfolio.Fee = (Convert.ToDecimal(model.TotalDeposit) * (decimal)(model.ProposedPortfolio.FeePercentage / 100));
            
            model.AnnualNetInterestEarned = (model.ProposedPortfolio.AnnualGrossInterestEarned - model.ProposedPortfolio.Fee);


            model.SessionId = illustrationInfo.SessionId;
            model.IllustrationUniqueReference = partnerEmail.IllustrationUniqueReference;
            
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
            SetSession(session.PartnerEmailAddress, session.PartnerName, session.PartnerOrganisation, session.PartnerTelephone, false);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult SuperLogin(Session session)
        {
            SetSession(session.PartnerEmailAddress, session.PartnerName, session.PartnerOrganisation, session.PartnerTelephone, true);

            return RedirectToAction("Illustrationlist", "Superuser");

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
                return RedirectToAction("Index", "Home");
            }


        }

    }
}
