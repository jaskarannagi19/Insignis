using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Manager;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InsignisIllustrationGenerator.Controllers
{
    public class SuperUserController : Controller
    {

        private readonly IllustrationHelper _illustrationHelper;
        public SuperUserController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings, ApplicationDbContext context)
        {
            _illustrationHelper = new IllustrationHelper(mapper, context);
        }

        public  IActionResult IllustrationList()
        {
            //GetIllustrationList
            IEnumerable<IllustrationListViewModel> illustrationList =  GetIllustrationListAsync().Result;
            return View(illustrationList.ToList());
        }

        private async Task<IEnumerable<IllustrationListViewModel>> GetIllustrationListAsync(SearchParameterViewModel searchParameter = null)
        {

            return await _illustrationHelper.GetIllustrationListAsync(searchParameter);
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

            return View("_illustrationDetails", result);

        }

        
        public JsonResult SearchIllustrationAsync(SearchParameterViewModel searchParams)
        {

            //check model validation for any empty waitttttttt
            
            bool isNull = false;
            if(searchParams.AdvisorName == null & searchParams.ClientName == null & searchParams.CompanyName == null
                & searchParams.IllustrationFrom == null & searchParams.IllustrationTo == null & searchParams.IllustrationUniqueReference == null)
            {
                isNull = true;
                return Json(new {Error= isNull });
            }

            
            
            var result = _illustrationHelper.GetIllustrationList(searchParams);
            
            return Json(result);
        }

    }
}