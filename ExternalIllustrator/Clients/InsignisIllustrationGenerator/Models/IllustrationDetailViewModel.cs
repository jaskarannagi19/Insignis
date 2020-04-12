using InsignisIllustrationGenerator.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class IllustrationDetailViewModel
    {
        public int Id { get; set; }
        
        public string PartnerName { get; set; }

        public string PartnerOrganisation { get; set; }
        public string PartnerEmail { get; set; }
        public Guid SessionId { get; set; }


        public string IllustrationUniqueReference { get; set; }

        [Required(ErrorMessage = "Please enter client reference.")]
        [Display(Name ="Client Reference")]
        [RegularExpression(@"^[a-zA-Z0-9-_,£$/\\. ""]{0,60}", ErrorMessage = "Allowed characters in Client reference are: Alphabets, Numbers, Hyphen (-), Underscore (_), Comma (,), Double Quote (“), Pound Sign (£), Dollar Sign ($), Forward Slash (/), Back Slash (\\), Space and Full Stop (.)")]
        [StringLengthAttribute(maximumLength: 60, MinimumLength = 1, ErrorMessage = "Client Name field allow upto 60 character")]
        public string ClientName { get; set; }
        
        [Required(ErrorMessage ="Client Type is required")]
        public int ClientType { get; set; }

        [Required(ErrorMessage = "Please select a Currency")]
        public string Currency { get; set; }

        
        //Liquidity Requirements
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in easy access amount field is £999,999.99")]
        public double? EasyAccess { get; set; }

        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 1 month amount field is £999,999.99")]
        public double? OneMonth { get; set; }

        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 3 months amount field is £999,999.99")]
        public double? ThreeMonths { get; set; }
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 6 months amount field is £999,999.99")]
        public double? SixMonths { get; set; }
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 9 months amount field is £999,999.99")]
        public double? NineMonths { get; set; }
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 1 year amount field is £999,999.99")]
        public double? OneYear { get; set; }
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 2 years amount field is £999,999.99")]
        public double? TwoYears { get; set; }
        
        
        
        [Range(0, 999999.99, ErrorMessage = " Maximum value allowed in 3+ years amount field is £999,999.99")]
        public double? ThreeYearsPlus { get; set; }
        
        //[RegularExpression(@"[0]", ErrorMessage = "Please enter at least one amount in the liquidity requirements section.")]
        [Required(ErrorMessage = "Please enter at least one amount in the liquidity requirements section.")]
        [Range(1, 999999.99, ErrorMessage = "The total deposit cannot be more than £999,999.99")]
        public double? TotalDeposit { get; set; }

        public Insignis.Asset.Management.Tools.Sales.SCurveOutput ProposedPortfolio { get; set; }
        
        public DateTime GenerateDate { get; set; }

        public string Status   { get; set; }
        public string Comment { get; set; }
        public string ReferredBy { get; set; }

        
        //Interest fields
        public decimal AnnualGrossInterestEarned { get; set; }
        public decimal AnnualNetInterestEarned { get; set; }
        public decimal GrossAverageYield { get; set; }
        public decimal NetAverageYield { get; set; }


    }
}
