using Insignis.Asset.Management.Clients.Helper;

namespace Insignis.Asset.Management.Tools.Helper
{
    public class HeatmapTerm
    {
        public InvestmentTerm InvestmentTerm = null;

        public decimal? MinimumInvestment = null;
        public decimal? MaximumInvestment = null;

        public decimal? AER50K = null;
        public decimal? AER100K = null;
        public decimal? AER250K = null;

        public string InterestPaid = "";

        public HeatmapTerm()
        {
        }

        public decimal GetBestRate()
        {
            decimal bestRate = 0;
            if (AER50K.HasValue)
            {
                if (AER50K.Value > bestRate)
                    bestRate = AER50K.Value;
            }
            if (AER100K.HasValue)
            {
                if (AER100K.Value > bestRate)
                    bestRate = AER100K.Value;
            }
            if (AER250K.HasValue)
            {
                if (AER250K.Value > bestRate)
                    bestRate = AER250K.Value;
            }
            return bestRate;
        }
    }
}
