namespace Insignis.Asset.Management.Clients.Helper
{
    public class BMAPSubAccountHolder
    {
        public static void Set(int pHubAccountID, string pSubAccountHolder, string pPreferencesRootFolder)
        {
            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
            Octavo.Gate.Nabu.Preferences.Preference preferences = preferencesManager.GetPreference("BMAP.Sub.Account.Holder", pHubAccountID, "Preferences");
            if (preferences == null)
                preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");

            if (preferences.Children.Count == 0)
            {
                Octavo.Gate.Nabu.Preferences.Preference accountHolder = new Octavo.Gate.Nabu.Preferences.Preference("AccountHolder", pSubAccountHolder);
                preferences.Children.Add(accountHolder);
            }
            else
            {
                Octavo.Gate.Nabu.Preferences.Preference accountHolder = preferences.GetChildPreference("AccountHolder");
                accountHolder.Value = pSubAccountHolder;
                preferences.SetChildPreference(accountHolder);
            }
            preferencesManager.SetPreference("BMAP.Sub.Account.Holder", pHubAccountID, preferences);
        }
        public static string Get(int pHubAccountID, string pPreferencesRootFolder, string pClientReference)
        {
            string subAccountHolder = "";
            if (pHubAccountID != -1)
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
                Octavo.Gate.Nabu.Preferences.Preference preferences = preferencesManager.GetPreference("BMAP.Sub.Account.Holder", pHubAccountID, "Preferences");
                if (preferences == null)
                {
                    preferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");
                }
                else
                {
                    if (preferences.Children.Count != 0)
                    {
                        Octavo.Gate.Nabu.Preferences.Preference accountHolder = preferences.GetChildPreference("AccountHolder");
                        if (accountHolder != null && accountHolder.Value != null && accountHolder.Value.Trim().Length > 0)
                            subAccountHolder = accountHolder.Value;
                    }
                }

                if (subAccountHolder.Trim().Length == 0)
                {
                    subAccountHolder = "00036400000000" + pClientReference;

                    Set(pHubAccountID, subAccountHolder, pPreferencesRootFolder);
                }
            }
            return subAccountHolder;
        }
    }
}
