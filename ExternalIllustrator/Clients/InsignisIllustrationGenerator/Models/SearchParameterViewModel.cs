using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class SearchParameterViewModel
    {

        [RegularExpression(@"^[A-Za-z ]{0,60}", ErrorMessage = "Invalid advisor name")]
        public string PartnerName { get; set; }
        
        
        [RegularExpression(@"^[a-zA-Z0-9-_,£$/\\. ""]{0,60}", ErrorMessage = "Invalid client name")]
        public string ClientName { get; set; }

        //&, @, £, $, €, #, full stop, comma, colon, semi-colon and hyphen
        [RegularExpression(@"^[a-zA-Z0-9-;:,.&@$£€#/\\. ""]{0,160}", ErrorMessage = "Invalid company name")]
        public string CompanyName { get; set; }

        
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Invalid illustration number")]
        public string IllustrationUniqueReference { get; set; }




        public DateTime? IllustrationFrom { get; set; }
        
        
        
        public DateTime? IllustrationTo { get; set; }

        public string PartnerEmail { get; set; }
        public string PartnerOrganisation { get; set; }


    }

    
}
