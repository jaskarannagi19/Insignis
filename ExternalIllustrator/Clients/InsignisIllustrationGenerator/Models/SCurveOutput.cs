using Insignis.Asset.Management.Tools.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class SCurveOutput
    {
        
            // Summary
            public decimal TotalDeposited = 0;
            //public decimal TotalProjectedReturn = 0;
            //public decimal AverageRateOfReturn = 0;
            //public int AverageDaysUntilMaturity = 0;
            //public decimal FSCSLimitPerLicense = 0;

            public decimal AnnualGrossInterestEarned = 0;       // sum of all annual interest payments
            public decimal AnnualNetInterestEarned = 0;         // sum of all annual interest payments
            public decimal GrossAverageYield = 0;               // divide AnnualGrossInterestEarned by TotalDeposited
            public decimal FeePercentage = (decimal)0.15;       // fee percentage
            public decimal Fee = 0;
            public decimal NetAverageYield = 0;                 // GrossAverageYield - Fee Percentage
            public decimal FSCSAmountProtected = 0;             // amount protected under FSCS
            public decimal FSCSPercentProtected = 0;            // percentage protected under FSCS

            // Detail
            public List<SCurveOutputRow> ProposedInvestments = new List<SCurveOutputRow>();
        
    }
}
