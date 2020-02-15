using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class SearchParameterViewModel
    {
        public string AdviserName { get; set; }
        public string ClientName { get; set; }
        public string CompanyName { get; set; }
        public string IllustrationUniqueReference { get; set; }
        public DateTime? IllustrationFrom { get; set; }
        public DateTime? IllustrationTo { get; set; }


    }
}
