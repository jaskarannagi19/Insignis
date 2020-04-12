using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class ProposedPortfolio
    {
        public int Id { get; set; }
        public int IllustrationID { get; set; }
        public int? InstitutionID { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionShortName { get; set; }
        public decimal DepositSize { get; set; }
        public decimal Rate { get; set; }

        public string InvestmentTerm { get; set; }
        public decimal AnnualInterest { get; set; }
        public IllustrationDetail IllustrationDetail { get; set; }

        
    }
}
