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
            var illustrationList = _illustrationHelper.GetIllustrationList(searchParams).ToList();
            return View(illustrationList);
        
        }

        [HttpPost]
        public ActionResult ExportCSV(SearchParameterViewModel searchParameter)
        {
            var result = _illustrationHelper.GetIllustrationList(searchParameter);
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
            var result = _illustrationHelper.GetIllustrationByByUniqueReference(uniqueReferenceId);
            
            ViewBag.URL = _appSettings.illustrationOutputPublicFacingFolder + "/" + uniqueReferenceId + "/" + uniqueReferenceId + "_CashIllustration.pdf";

            return View("_illustrationDetails", result);

        }


        public JsonResult SearchIllustration(SearchParameterViewModel searchParams)
        {

            //check model validation for any empty waitttttttt

            bool isNull = false;
            if (searchParams.AdvisorName == null & searchParams.ClientName == null & searchParams.CompanyName == null
                & searchParams.IllustrationFrom == null & searchParams.IllustrationTo == null & searchParams.IllustrationUniqueReference == null)
            {
                isNull = true;
                return Json(new { Error = isNull });
            }



            var result = _illustrationHelper.GetIllustrationList(searchParams);

            return Json(result);
        }

    }
}