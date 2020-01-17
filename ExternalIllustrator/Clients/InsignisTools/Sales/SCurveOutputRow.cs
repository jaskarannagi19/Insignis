using System;

namespace Insignis.Asset.Management.Tools.Sales
{
    public class SCurveOutputRow
    {
        public Guid ID = Guid.Empty;
        public int? InstitutionID = null;
        public string InstitutionName = "";
        public string InstitutionShortName = "";
        public decimal DepositSize = 0;
        public decimal Rate = 0;
        public Clients.Helper.InvestmentTerm InvestmentTerm;
        public decimal AnnualInterest = 0;

        public decimal AER100K = 0;
        public decimal AER250K = 0;
        public decimal AER50K = 0;
        public string InterestPaid = "";
        public decimal MinimumInvestment = 0;
        public decimal MaximumInvestment = 0;

        public Insignis.Asset.Management.Tools.Helper.HeatmapTerm heatmapTerm = null;

        public void CalculateAnnualInterest()
        {
            decimal rate = (Rate / 100);
            AnnualInterest = (DepositSize * rate);
        }
    }
}
