using System.Collections.Generic;

namespace Insignis.Asset.Management.Tools.Sales
{
    public class SCurveSettings
    {
        public decimal? TotalAvailableToDeposit = null;
        //public int MaximumDurationInMonths = 0;
        public SCurveClientType ClientType = SCurveClientType.Individual;
        public int AvailableToHubAccountTypeID = -1;
        public decimal? MaximumDepositInAnyOneInstitution = null;
        public List<KeyValuePair<int, decimal>> LiquidityNeedsDaysAndAmounts = new List<KeyValuePair<int, decimal>>();
        public decimal? FeeDiscount = null;
        public decimal? IntroducerDiscount = null;
        public string CurrencyCode = "GBP";
        public bool FullProtection = true;
        public bool ShowFitchRating = false;
        public string MinimumFitchRating = "All";
        public bool IncludePooledProducts = true;
        public string OptionalIntroducerOrganisationName = "unspecified";
        public string OptionalClientName = "unspecified";
        public bool AnonymiseDeposits = false;
    }
}
