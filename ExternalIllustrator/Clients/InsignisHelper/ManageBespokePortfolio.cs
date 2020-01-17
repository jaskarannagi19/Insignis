namespace Insignis.Asset.Management.Clients.Helper
{
    public class ManageBespokePortfolio
    {
        public static bool OnlyAdmin(int pPartyID, string pPreferencesRoot)
        {
            bool result = false;
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Manage.Bespoke.Portfolio", pPartyID, "AccessPermission");
                if (accessPermission != null)
                {
                    if (accessPermission.GetChildPreference("OnlyAdminHasAccess") != null)
                    {
                        if (accessPermission.GetChildPreference("OnlyAdminHasAccess").Value.ToLower().CompareTo("true") == 0)
                            result = true;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static void SetAccessLevel(int pPartyID, bool pOnlyAdmin, string pPreferencesRoot)
        {
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Manage.Bespoke.Portfolio", pPartyID, "AccessPermission");
                if(accessPermission == null)
                    accessPermission = new Octavo.Gate.Nabu.Preferences.Preference("AccessPermission", "");
                accessPermission.SetChildPreference(preferencesManager.ProcessPreference("OnlyAdminHasAccess", ((pOnlyAdmin == true) ? "true" : "false"), accessPermission));
                preferencesManager.SetPreference("Manage.Bespoke.Portfolio", pPartyID, accessPermission);
            }
            catch
            {
            }
        }
    }
}