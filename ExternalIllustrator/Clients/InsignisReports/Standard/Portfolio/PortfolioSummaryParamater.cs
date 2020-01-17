using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Standard.Portfolio
{
    public class PortfolioSummaryParameter
    {
        // Reference
        public string ReferenceNumber = "";

        // Personal Information
        public string FullName = "";

        public List<HubAccount> hubAccounts = new List<HubAccount>();
    }
}
