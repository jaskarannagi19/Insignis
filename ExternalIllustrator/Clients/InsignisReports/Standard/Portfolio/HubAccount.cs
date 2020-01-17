using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Standard.Portfolio
{
    public class HubAccount
    {
        public int? ID = null;
        public string Bank = null;
        public DateTime? Opened = null;
        public string Comment = null;

        public string Type;                                 // Personal Hub Account		
        public string AccountHeldWith = null;               // this is filled with the partner/spouse name only on joint accounts
        public string Status;                               // Account Status : Open
        public string AvailableToInvest;                    // Available to Invest : £268,025.00
        public string WeightedAverageOfConfirmedAccounts;   // Weighted Average of Confirmed Accounts : 1.098%
        public string TotalPendingWithdrawalRequests;       // Pending Withdrawal Requests : £0.00
        public string TotalPendingInvestments;              // Pending Investments : £0.00
        public string FundsInvested;						// Funds Invested : £720,000.00
        public string MinimumHubBalance;                    // Fee Reserve : £1,975.00
        public string AvailableToWithdraw;                  // Available to Withdraw : £268,025.00
        public string Total;                                // Total : £990,000.00

        public List<BespokeAccount> pendingInvestments = new List<BespokeAccount>();
        public List<BespokeAccount> liveInvestments = new List<BespokeAccount>();

        public List<GemProduct> gemProducts = new List<GemProduct>();
    }
}
