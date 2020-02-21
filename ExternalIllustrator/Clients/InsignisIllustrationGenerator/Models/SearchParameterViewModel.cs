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

        //&, @, £, $, €, #, full stop, comma, colon, semi-colon and hyphen
        [RegularExpression(@"^[a-zA-Z0-9-;:,.&@$£€#/\\. ""]{0,160}", ErrorMessage = "Invalid Client Name")]
        public string CompanyName { get; set; }

        
        [RegularExpression(@"^[a-zA-Z0-9-_]", ErrorMessage = "Invalid Illustration Number")]
        public string IllustrationUniqueReference { get; set; }
        public DateTime? IllustrationFrom { get; set; }
        public DateTime? IllustrationTo { get; set; }


    }
}
