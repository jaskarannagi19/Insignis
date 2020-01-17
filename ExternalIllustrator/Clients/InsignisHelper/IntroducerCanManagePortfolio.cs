namespace Insignis.Asset.Management.Clients.Helper
{
    public class IntroducerCanManagePortfolio
    {
        public static bool CanIntroducerManagePortfolio(int pPartyID, string pPreferencesRoot)
        {
            bool result = false;
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Introducer.Can.Manage.Portfolio", pPartyID, "AccessPermission");
                if (accessPermission != null)
                {
                    if (accessPermission.GetChildPreference("CanIntroducerManage") != null)
                    {
                        if (accessPermission.GetChildPreference("CanIntroducerManage").Value.ToLower().CompareTo("true") == 0)
                            result = true;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static void SetAccessLevel(int pPartyID, bool pCanIntroducerManagePortfolio, string pPreferencesRoot)
        {
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Introducer.Can.Manage.Portfolio", pPartyID, "AccessPermission");
                if (accessPermission == null)
                    accessPermission = new Octavo.Gate.Nabu.Preferences.Preference("AccessPermission", "");
                accessPermission.SetChildPreference(preferencesManager.ProcessPreference("CanIntroducerManage", ((pCanIntroducerManagePortfolio == true) ? "true" : "false"), accessPermission));
                preferencesManager.SetPreference("Introducer.Can.Manage.Portfolio", pPartyID, accessPermission);
            }
            catch
            {
            }
        }
    }
}
