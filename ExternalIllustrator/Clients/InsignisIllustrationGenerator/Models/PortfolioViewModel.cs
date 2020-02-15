using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class PortfolioViewModel : BaseEntity
    {
        public decimal TotalDeposited { get; set; }
        //public decimal TotalProjectedReturn = 0;
        //public decimal AverageRateOfReturn = 0;
        //public int AverageDaysUntilMaturity = 0;
        //public decimal FSCSLimitPerLicense = 0;

        public decimal AnnualGrossInterestEarned { get; set; }       // sum of all annual interest payments
        public decimal AnnualNetInterestEarned { get; set; }         // sum of all annual interest payments
        public decimal GrossAverageYield { get; set; }     // divide AnnualGrossInterestEarned by TotalDeposited
       [DefaultValue(0.15)]
        public decimal FeePercentage { get; set; }    // fee percentage
        public decimal Fee { get; set; }
        public decimal NetAverageYield { get; set; }              // GrossAverageYield - Fee Percentage
        public decimal FSCSAmountProtected { get; set; }         // amount protected under FSCS
        public decimal FSCSPercentProtected { get; set; }

        public ICollection<BankDepositTermViewModel> ProposedInvestments { get; set; }
    }
}
