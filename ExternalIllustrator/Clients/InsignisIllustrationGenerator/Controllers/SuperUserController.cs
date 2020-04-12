using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Manager;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace InsignisIllustrationGenerator.Controllers
{
    public class SuperUserController : Controller
    {

        private readonly IllustrationHelper _illustrationHelper;
        private readonly AppSettings _appSettings;

        public SuperUserController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings, ApplicationDbContext context)
        {
            var session = new Session() { PartnerEmailAddress = "p.artner@partorg.com", PartnerName = "Peter Artner", PartnerOrganisation = "PartOrg Ltd.", PartnerTelephone = "01226 1234 567", SuperUser = true };

            //HttpContext.Session.SetString("SessionPartner", JsonConvert.SerializeObject(session));
            _illustrationHelper = new IllustrationHelper(mapper, context);
            _appSettings = settings.Value;

        }

        public IActionResult IllustrationList(SearchParameterViewModel searchParams)
        {

            //GetIllustrationList
            var illustrationList = _illustrationHelper.GetIllustrationList(searchParams, true).ToList();
            return View(illustrationList);

        }

        [HttpPost]
        public ActionResult ExportCSV(SearchParameterViewModel searchParameter)
        {
            var result = _illustrationHelper.GetIllustrationList(searchParameter, true);
            // StringBuilder sbheader = new StringBuilder();
            //StringBuilder sbheader = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0},", "Introducer"));
            sb.Append(string.Format("{0},", "Account Reference"));
            sb.Append(string.Format("{0},", "Account Name"));
            sb.Append(string.Format("{0},", "Lead Type or Name"));
            sb.Append(string.Format("{0},", "Last Name"));
            sb.Append(string.Format("{0},", "Name"));
            sb.Append(string.Format("{0},", "Adviser's Email Address"));
            sb.Append(string.Format("{0},", "Marketing Source"));
            sb.Append(string.Format("{0},", "Categories"));
            sb.Append(string.Format("{0},", "Client Category"));

            sb.Append(string.Format("{0},", "Salesperson"));
            sb.Append(string.Format("{0},", "Account Manager"));
            sb.Append(string.Format("{0},", "Minimum Deposit"));
            sb.Append(string.Format("{0},", "Maximum Deposit"));

            sb.Append(string.Format("{0},", "Source Function"));
            sb.Append(string.Format("{0},", "Advisor"));
            sb.Append(string.Format("{0},", "Introducer Account"));
            sb.Append(string.Format("{0},", "Umbrella Organisation")).AppendLine();


            foreach (var data in result)
            {
                string[] name;
                string fname = string.Empty;
                string LName = string.Empty;
                if (!string.IsNullOrEmpty(data.ClientName)) { 
                if (data.ClientName.Contains(" ")) { 
                name = data.ClientName.Split(" ");
                
                
                
                
                if (name.Length > 1) { 
                List<string> fName = name.Take(name.Length - 1).ToList();

                
                foreach (var item in fName)
                {
                    fname += item + " ";
                }

                LName = name.Last();
                }
                }
                else
                {
                    fname = data.ClientName;
                }
                }


                sb.Append(string.Format("{0},", "No"));
                sb.Append(string.Format("{0},", data.IllustrationUniqueReference));
                sb.Append(string.Format("{0},", data.ClientName));
                sb.Append(string.Format("{0},", "Personal"));
                sb.Append(string.Format("{0},", LName));//Last Name
                sb.Append(string.Format("{0},", fname));//First Name
                sb.Append(string.Format("{0},", data.PartnerEmail));
                sb.Append(string.Format("{0},", "Introducer Referral"));
                sb.Append(string.Format("{0},", "Client"));
                sb.Append(string.Format("{0},", data.ClientType == "0" ? "Individual" : "Joint"));
                sb.Append(string.Format("{0},", ""));
                sb.Append(string.Format("{0},", ""));
                sb.Append(string.Format("{0},", data.TotalDeposit));
                sb.Append(string.Format("{0},", data.TotalDeposit));
                sb.Append(string.Format("{0},", "Introducer"));
                sb.Append(string.Format("{0},", data.PartnerName));
                sb.Append(string.Format("{0},", data.PartnerOrganisation));
                sb.Append(string.Format("{0},", string.Empty)).AppendLine();

            }

            //  byte[] buffer = Encoding.ASCII.GetBytes();

            return Json(new { Data = sb.ToString(), FileName = "Illustration.csv" });

        }

        //Get Single Illustration Details

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

            ViewBag.URL = _appSettings.illustrationOutputPublicFacingFolder + "/" + uniqueReferenceId + "/" + uniqueReferenceId + "_CashIllustration.pdf";


            return View("_illustrationDetails", result);

        }


        public JsonResult SearchIllustration(SearchParameterViewModel searchParams)
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

            if (searchParams.IllustrationFrom > searchParams.IllustrationTo)
            {
                var errors = new List<string>();
                errors.Add("To Date cannot be less than the From Date.");
                return Json(new { Data = errors, Success = false });
            }


            var result = _illustrationHelper.GetIllustrationList(searchParams, true);

            return Json(new { Data = result, Success = true });
        }

    }
}