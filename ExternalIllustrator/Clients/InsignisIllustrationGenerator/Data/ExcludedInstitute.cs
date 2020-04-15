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
        public string UniqueReferenceId { get; set; }

        //Excluded bank id
        public int InstituteId { get; set; }

        //Time to delete
        
        public Guid SessionId { get; set; } 

        //Updation flag
        public bool IsUpdatedBank { get; set; }

    }
}
