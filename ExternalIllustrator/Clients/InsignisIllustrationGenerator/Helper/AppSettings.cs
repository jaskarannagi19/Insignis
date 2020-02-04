using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Helper
{
    public class AppSettings
    {
        public string illustrationOutputPublicFacingFolder { get; set; }
        public string illustrationOutputInternalFolder { get; set; }
        public string preferencesRoot { get; set; }
        public string ClientConfigRoot { get; set; }
        public string ErrorLog { get; set; }
        public string InsignisAM { get; set; }
    }
}
