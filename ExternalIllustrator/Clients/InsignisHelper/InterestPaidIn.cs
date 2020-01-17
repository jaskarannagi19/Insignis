using System.Configuration;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class InterestPaidIn
    {
        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        private Octavo.Gate.Nabu.Preferences.Preference interestPaidInPreferences = null;

        public InterestPaidIn(string pPreferencesRoot)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
            interestPaidInPreferences = preferencesManager.GetPreference("Insignis.Asset.Management.Clients.Helper", "InterestPaidIn", "InterestPaidIn");
            if (interestPaidInPreferences == null)
                interestPaidInPreferences = new Octavo.Gate.Nabu.Preferences.Preference("InterestPaidIn", "");
        }

        public void SetInterestPaidIn(string pProductCodeID, bool pIsInterestPaidIn)
        {
            if (interestPaidInPreferences == null)
                interestPaidInPreferences = new Octavo.Gate.Nabu.Preferences.Preference("InterestPaidIn", "");

            interestPaidInPreferences.SetChildPreference(preferencesManager.ProcessPreference(pProductCodeID, ((pIsInterestPaidIn == true) ? "true" : "false"), interestPaidInPreferences));
            preferencesManager.SetPreference("Insignis.Asset.Management.Clients.Helper", "InterestPaidIn", interestPaidInPreferences);
        }

        public bool IsInterestToBePaidIn(string pProductCodeID)
        {
            bool isInterestToBePaidIn = false;
            if (interestPaidInPreferences != null)
            {
                if (preferencesManager.GetPreference(pProductCodeID, interestPaidInPreferences).CompareTo("true") == 0)
                    isInterestToBePaidIn = true;
            }
            return isInterestToBePaidIn;
        }
    }
}