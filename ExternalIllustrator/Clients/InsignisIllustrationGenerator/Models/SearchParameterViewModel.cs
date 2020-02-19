using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class SearchParameterViewModel
    {

        [RegularExpression(@"^[0-9]{0,10}", ErrorMessage = "Invalid Advisor Name")]
        public string AdvisorName { get; set; }

        
        
        [RegularExpression(@"^[a-zA-Z0-9-_,£$/\\. ""]{0,60}", ErrorMessage = "Invalid Client Name")]
        public string ClientName { get; set; }



        public string CompanyName { get; set; }

        
        [RegularExpression(@"^[a-zA-Z0-9-]", ErrorMessage = "Invalid Illustration Number")]
        public string IllustrationUniqueReference { get; set; }
        public string IllustrationFrom { get; set; }
        public string IllustrationTo { get; set; }


    }
}
