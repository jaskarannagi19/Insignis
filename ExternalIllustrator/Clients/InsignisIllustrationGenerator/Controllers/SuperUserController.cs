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

        private async Task<IEnumerable<IllustrationListViewModel>> GetIllustrationsListAsync(SearchParameterViewModel searchParameter = null)
        {

            return await _illustrationHelper.GetIllustrationListAsync(searchParameter);
        }

    }
}