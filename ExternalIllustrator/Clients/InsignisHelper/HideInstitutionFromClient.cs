namespace Insignis.Asset.Management.Clients.Helper
{
    public class HideInstitutionFromClient
    {
        public static void Set(int pInstitutionID, bool pIsHidden, string pPreferencesRootFolder)
        {
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
            Octavo.Gate.Nabu.Preferences.Preference preferences = preferencesManager.GetPreference("Institution.Hide.From.Client", pInstitutionID, "Preferences");
            if (preferences == null)
                preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");

            if (preferences.Children.Count == 0)
            {
                Octavo.Gate.Nabu.Preferences.Preference hideFromClient = new Octavo.Gate.Nabu.Preferences.Preference("HideFromClient", ((pIsHidden == true) ? "true" : "false"));
                preferences.Children.Add(hideFromClient);
            }
            else
            {
                Octavo.Gate.Nabu.Preferences.Preference hideFromClient = preferences.GetChildPreference("HideFromClient");
                hideFromClient.Value = ((pIsHidden == true) ? "true" : "false");
                preferences.SetChildPreference(hideFromClient);
            }
            preferencesManager.SetPreference("Institution.Hide.From.Client", pInstitutionID, preferences);
        }
        public static bool IsHidden(int pInstitutionID, string pPreferencesRootFolder)
        {
            bool isHidden = false;
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
            Octavo.Gate.Nabu.Preferences.Preference preferences = preferencesManager.GetPreference("Institution.Hide.From.Client", pInstitutionID, "Preferences");
            if (preferences == null)
                preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");
            if (preferences.Children.Count != 0)
            {
                Octavo.Gate.Nabu.Preferences.Preference hideFromClient = preferences.GetChildPreference("HideFromClient");
                if (hideFromClient != null && hideFromClient.Value != null && hideFromClient.Value.CompareTo("true") == 0)
                    isHidden = true;
            }
            return isHidden;
        }
    }
}
