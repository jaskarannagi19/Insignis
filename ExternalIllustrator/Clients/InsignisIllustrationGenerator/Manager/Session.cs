using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Manager
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public bool SuperUser { get; set; }
        public string PartnerOrganisation { get; set; }
        public string PartnerName { get; set; }
        public string PartnerEmailAddress { get; set; }
        public string PartnerTelephone { get; set; }

        //Illustration Seesoin Fields
        public string ClientName { get; set; }
        public int ClientType { get; set; }
        public string Currency { get; set; }
        public double? EasyAccess { get; set; }
        public double? OneMonth { get; set; }
        public double? ThreeMonths { get; set; }
        public double? SixMonths { get; set; }
        public double? NineMonths { get; set; }
        public double? OneYear { get; set; }
        public double? TwoYears { get; set; }
        public double? ThreeYearsPlus { get; set; }
        public double? TotalDeposit { get; set; }
        public Insignis.Asset.Management.Tools.Sales.SCurveOutput ProposedPortfolio{get;set;}
        public string AdvisorName { get; internal set; }
    }
}
