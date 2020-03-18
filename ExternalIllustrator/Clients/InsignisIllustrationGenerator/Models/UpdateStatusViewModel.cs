using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Models
{
    public class UpdateStatusViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9-,_.$£€/\\. ""]{0,512}", ErrorMessage = "Allowed characters in Comments are: Alphabets, Numbers, Hyphen (-), Underscore (_), Comma (,), Double Quote (“), Pound Sign (£), Dollar Sign ($), Forward Slash (/), Back Slash (\\), Space and Full Stop (.)")]
        public string Comment { get; set; }

        public string Status { get; set; }
        
        [RegularExpression(@"^[a-zA-Z]{0,60}", ErrorMessage = "Invalid Referred by")]
        public string ReferredBy { get; set; }
        public string UniqueReferenceId { get; set; }
    }
}
