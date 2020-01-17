namespace Insignis.Asset.Management.Clients.Helper
{
    public class IntroducerReports
    {
        public static bool HasAccessTo(string pReportAlias, int pPartyID, string pPreferencesRoot)
        {
            bool result = false;
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Introducer.Reports", pPartyID, "AccessPermission");
                if (accessPermission != null)
                {
                    if (accessPermission.GetChildPreference(pReportAlias) != null)
                    {
                        if (accessPermission.GetChildPreference(pReportAlias).Value.ToLower().CompareTo("true") == 0)
                            result = true;
                    }
                    else if (pReportAlias.CompareTo("FUM_By_Client_Introducer") == 0)
                        result = true;          // we assume that all introducers have access to this report
                }
                else if (pReportAlias.CompareTo("FUM_By_Client_Introducer") == 0)
                    result = true;          // we assume that all introducers have access to this report
            }
            catch
            {
            }
            return result;
        }

        public static void SetAccessLevel(string pReportAlias, int pPartyID, bool pHasAccess, string pPreferencesRoot)
        {
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference accessPermission = preferencesManager.GetPreference("Introducer.Reports", pPartyID, "AccessPermission");
                if (accessPermission == null)
                    accessPermission = new Octavo.Gate.Nabu.Preferences.Preference("AccessPermission", "");
                accessPermission.SetChildPreference(preferencesManager.ProcessPreference(pReportAlias, ((pHasAccess == true) ? "true" : "false"), accessPermission));
                preferencesManager.SetPreference("Introducer.Reports", pPartyID, accessPermission);
            }
            catch
            {
            }
        }
    }
}
