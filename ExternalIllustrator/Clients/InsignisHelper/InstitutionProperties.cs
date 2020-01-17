using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class InstitutionProperties
    {
        public int institutionID = -1;

        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        public Octavo.Gate.Nabu.Preferences.Preference preferences = null;

        public InstitutionProperties(int pInstitutionID, string pPreferencesRootFolder)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
            institutionID = pInstitutionID;
            Load();
        }
        private void Load()
        {
            preferences = preferencesManager.GetPreference("Institution.Properties", institutionID, "Preferences");
            if (preferences == null)
                preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");

            if(preferences.Children.Count == 0)
            {
                Octavo.Gate.Nabu.Preferences.Preference institutionCode = new Octavo.Gate.Nabu.Preferences.Preference("InstitutionCode", "unspecified");
                preferences.Children.Add(institutionCode);

                Octavo.Gate.Nabu.Preferences.Preference institutionPayee = new Octavo.Gate.Nabu.Preferences.Preference("InstitutionPayee", "unspecified");
                preferences.Children.Add(institutionPayee);

                Octavo.Gate.Nabu.Preferences.Preference fitchRating = new Octavo.Gate.Nabu.Preferences.Preference("FitchRating", "unspecified");
                preferences.Children.Add(fitchRating);
            }
        }

        public void Save()
        {
            if (preferences == null)
            {
                preferences = preferencesManager.GetPreference("Institution.Properties", institutionID, "Preferences");
                if (preferences == null)
                    preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");

                if(preferences.Children.Count == 0)
                {
                    Octavo.Gate.Nabu.Preferences.Preference institutionCode = new Octavo.Gate.Nabu.Preferences.Preference("InstitutionCode", "unspecified");
                    preferences.Children.Add(institutionCode);

                    Octavo.Gate.Nabu.Preferences.Preference institutionPayee = new Octavo.Gate.Nabu.Preferences.Preference("InstitutionPayee", "unspecified");
                    preferences.Children.Add(institutionPayee);

                    Octavo.Gate.Nabu.Preferences.Preference fitchRating = new Octavo.Gate.Nabu.Preferences.Preference("FitchRating", "unspecified");
                    preferences.Children.Add(fitchRating);
                }
            }
            preferencesManager.SetPreference("Institution.Properties", institutionID, preferences);
        }
    }
}
