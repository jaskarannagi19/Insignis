using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class IllustrationDetail
    {
        //Contains all the fields of Illustration 
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public string PartnerOrganisation { get; set; }
        public string PartnerEmail { get; set; }
        public string ClientName { get; set; }
        public int ClientType { get; set; }
        
        public string IllustrationUniqueReference { get; set; }
        public DateTime GenerateDate { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }

        //Liquidity Requirements
        public double EasyAccess { get; set; }
        public double OneMonth { get; set; }
        public double ThreeMonths { get; set; }
        public double SixMonths { get; set; }
        public double NineMonths { get; set; }
        public double OneYear { get; set; }
        public double TwoYears { get; set; }
        public double ThreeYearsPlus { get; set; }
        public double TotalDeposit { get; set; }

        public List<ProposedPortfolio> IllustrationProposedPortfolio { get; set; }

        public string Comment { get; set; }
        public string ReferredBy { get; set; }
        
        
        //Interest fields
        public decimal AnnualGrossInterestEarned { get; set; }
        public decimal AnnualNetInterestEarned { get; set; }
        public decimal GrossAverageYield { get; set; }
        public decimal NetAverageYield { get; set; }
    }
}
