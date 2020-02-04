using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Globalisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Helper
{
    public class MultiLingual
    {
        private GlobalisationAbstraction globalisationAbstraction { get; set; } 

        public Language language;

        public MultiLingual(AppSettings settings, string pSystemLanguageName)
        {
            
            globalisationAbstraction= new GlobalisationAbstraction(settings.InsignisAM, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL,settings.ErrorLog);
            language = globalisationAbstraction.GetLanguageBySystemName(pSystemLanguageName);
        }

        

        public MultiLingual(int pLanguageID)
        {
            language = globalisationAbstraction.GetLanguage(pLanguageID);
        }

        public GlobalisationAbstraction GetAbstraction()
        {
            return globalisationAbstraction;
        }
    }
}
