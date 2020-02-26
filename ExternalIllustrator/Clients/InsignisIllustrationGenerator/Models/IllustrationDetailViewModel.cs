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


        public string IllustrationUniqueReference { get; set; }

        [Required(ErrorMessage = "Please enter client name.")]
        [Display(Name ="Client Reference")]
        [RegularExpression(@"^[a-zA-Z0-9-_,£$/\\. ""]{0,60}", ErrorMessage = "Allowed characters in Client Name are: Alphabets, Numbers, Hyphen (-), Underscore (_), Comma (,), Double Quote (“), Pound Sign (£), Dollar Sign ($), Forward Slash (/), Back Slash (\\), Space and Full Stop (.)")]
        [StringLengthAttribute(maximumLength: 60, MinimumLength = 1, ErrorMessage = "Client Name field allow upto 60 character")]
        public string ClientName { get; set; }
        
        [Required(ErrorMessage ="Client Type is required")]
        public int ClientType { get; set; }

        [Required(ErrorMessage = "Please select a Currency")]
        public string Currency { get; set; }

        //Liquidity Requirements
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Easy Access")]
        public double? EasyAccess { get; set; }

        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for One Month")]
        public double? OneMonth { get; set; }

        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Three Months")]
        public double? ThreeMonths { get; set; }
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Six Months")]
        public double? SixMonths { get; set; }
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Nine Months")]
        public double? NineMonths { get; set; }
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for One Year")]
        public double? OneYear { get; set; }
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Two Years")]
        public double? TwoYears { get; set; }
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Invalid Amount for Three Years Plus")]
        public double? ThreeYearsPlus { get; set; }
        
        [RegularExpression(@"[0-9]{0,8}.[0-9]{2}", ErrorMessage = "Please Enter at Least One Liquidity Amount")]
        [Range(1,double.MaxValue,ErrorMessage = "Please Enter at Least One Liquidity Amount")]
     
        public double TotalDeposit { get; set; }

        public Insignis.Asset.Management.Tools.Sales.SCurveOutput ProposedPortfolio { get; set; }
        public string AdviserName { get; internal set; }
        public DateTime GenerateDate { get; set; }

        public string Status   { get; set; }
        public string Comment { get; set; }
        public string ReferredBy { get; set; }
    }
}
