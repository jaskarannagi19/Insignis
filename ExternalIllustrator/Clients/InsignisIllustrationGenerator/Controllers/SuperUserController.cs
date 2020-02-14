using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InsignisIllustrationGenerator.Controllers
{
    public class SuperUserController : Controller
    {

        public SuperUserController(ILogger<HomeController> logger, AutoMapper.IMapper mapper, IOptions<AppSettings> settings, ApplicationDbContext context)
        {

        }

        public IActionResult IllustrationList()
        {

         
            return View();
        }
        public IActionResult IllustrationList(SearchParameterViewModel searchParameter)
        {

            var IllustrationsList = GetIllustrationsList(searchParameter);
                    return View();
        }

        private List<IllustrationListViewModel> GetIllustrationsList(SearchParameterViewModel searchParameter)
        {
            return new List<IllustrationListViewModel>();
        }
    }
}