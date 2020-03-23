using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class ExcludedInstitute
    {
        /// <summary>
        /// Entity to store excluded institutes for clients
        /// </summary>
        public int Id { get; set; }
        
        //Identifiers
        
        public string PartnerEmail { get; set; }
        public string PartnerOrganisation { get; set; }
        public string ClientReference { get; set; }

        //Excluded bank id
        public int InstituteId { get; set; }

    }
}
