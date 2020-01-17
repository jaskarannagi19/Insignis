using Octavo.Gate.Nabu.Entities.Financial;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Tools.Helper
{
    public class HeatmapInstitution
    {
        public Institution institution = null;
        public List<HeatmapTerm> investmentTerms = new List<HeatmapTerm>();

        public HeatmapInstitution(Institution pInstitution)
        {
            institution = pInstitution;
        }

        public decimal GetBestRate()
        {
            decimal bestRate = 0;
            foreach (HeatmapTerm investmentTerm in investmentTerms)
            {
                if (investmentTerm.GetBestRate() > bestRate)
                    bestRate = investmentTerm.GetBestRate();
            }
            return bestRate;
        }

        public HeatmapTerm GetBest()
        {
            HeatmapTerm bestRate = new HeatmapTerm();
            foreach (HeatmapTerm investmentTerm in investmentTerms)
            {
                if (investmentTerm.GetBestRate() > bestRate.GetBestRate())
                    bestRate = investmentTerm;
            }
            return bestRate;
        }
    }
}
