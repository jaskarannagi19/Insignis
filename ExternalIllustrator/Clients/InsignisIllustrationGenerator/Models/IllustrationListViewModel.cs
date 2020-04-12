using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class IllustrationListViewModel
    {
        //Contains all the fields of Illustration 
        public int Id { get; set; }
        public string PartnerOrganisation { get; set; }
        public string AdviserName { get; set; }
        public string ClientName { get; set; }
        public string ClientType { get; set; }
        public string PartnerName { get; set; }
        public string PartnerEmail { get; set; }

        public string IllustrationUniqueReference { get; set; }
        public DateTime? GenerateDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }


        public double TotalDeposit { get; set; }

    }
}
