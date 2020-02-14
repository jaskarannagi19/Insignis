using Insignis.Asset.Management.Illustrator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class BankDepositTermViewModel:BaseEntity
    {
        public int? InstitutionID { get; set; }
         public decimal DepositSize { get; set; }
        public decimal Rate { get; set; }
        public string InvestmentTerm { get; set; }
        public decimal AnnualInterest { get; set; }
        public decimal AER100K { get; set; }
        public decimal AER250K { get; set; }
        public decimal AER50K { get; set; }
        public string InterestPaid { get; set; }
        public decimal MinimumInvestment { get; set; }
        public decimal MaximumInvestment { get; set; }
        public Bank? Institution { get; set; }


    }
}
