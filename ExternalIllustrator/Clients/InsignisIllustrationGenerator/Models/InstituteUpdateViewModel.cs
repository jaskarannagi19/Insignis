using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class InstituteUpdateViewModel
    {
        public string IncludeBank { get; set; }
        public string BankId { get; set; }

        public string UpdatedAmount { get; set; }
        public string InstituteName { get; set; }
        public string InvestmentTerm { get; set; }
        public string Rate { get; set; }
        public string AnnualInterest { get; set; }
        public string ClientType { get; set; }
    }
}
