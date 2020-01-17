using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Globalisation;
using System.Configuration;

namespace Insignis.Asset.Management.External.Illustrator.Helper
{
    public class MultiLingual
    {
        private GlobalisationAbstraction globalisationAbstraction = new GlobalisationAbstraction(ConfigurationManager.ConnectionStrings["Octavo.Gate.Nabu.Data.Source.InsignisAM"].ConnectionString, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings.Get("errorLog"));

        public Language language;

        public MultiLingual()
        {
            language = null;
        }

        public MultiLingual(string pSystemLanguageName)
        {
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