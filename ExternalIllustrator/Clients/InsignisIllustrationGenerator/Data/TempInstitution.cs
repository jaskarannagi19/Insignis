using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class TempInstitution
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public string InstitutionName { get; set; }
        public string InvestmentTerm { get; set; }

        // Client Info
        public string ClientName { get; set; }


        //Partner Info
        public string PartnerName { get; set; }
        public string PartnerEmail { get; set; }
        public string PartnerOrganisation { get; set; }
        
    }
}
