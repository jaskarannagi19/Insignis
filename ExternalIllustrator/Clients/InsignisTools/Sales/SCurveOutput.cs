using Octavo.Gate.Nabu.Entities;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Tools.Sales
{
    public class SCurveOutput : BaseType
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
