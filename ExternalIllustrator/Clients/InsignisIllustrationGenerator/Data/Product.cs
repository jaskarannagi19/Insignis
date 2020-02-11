using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class Product
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }
        public string ProductCode  { get; set;  }
        public string ProductName  {get;set;  }
        public string ProductType  { get; set;}

        public string LiquidityType { get; set; }
        public string NoticeText { get; set; }
        public string NoticeDays { get; set; }
        public string TermText { get; set; }
        public string TermDays { get; set; }
        public string RateFor50KDeposit { get; set; }
        public string RateFor100KDeposit { get; set; }
        public string RateFor250KDeposit { get; set; }
        public string InterestPaid { get; set; }
        public string MinimumDeposit { get; set; }
        public string MaximumDeposit { get; set; }
        public bool IsAvailableToPersonalHubAccounts { get; set; }
        public bool IsAvailableToJointHubAccounts { get; set; }
        public bool IsAvailableToSMEHubAccounts { get; set; }
        public bool IsAvailableToTrustHubAccounts { get; set; }
        public bool IsAvailableToIncorporatedCharityHubAccounts { get; set; }
        public bool IsAvailableToPowerOfAttorneyHubAccounts { get; set; }
        public bool IsAvailableToPersonalTrustHubAccounts { get; set; }
        public bool IsAvailableToLocalAuthorityHubAccounts { get; set; }
        public bool IsAvailableToSSASHubAccounts { get; set; }
        public bool IsAvailableToSIPPHubAccounts { get; set; }
        public bool IsAvailableToLargeCorporateHubAccounts { get; set; }
        public bool IsAvailableToUnincorporatedCharityHubAccounts { get; set; }
        public bool IsAvailableToCourtOfProtectionHubAccounts { get; set; }

        [ForeignKey("BankID")]
        public virtual Bank Bank { get; set; }


    }
}