using System.Configuration;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class IfJointProduct
    {
        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        private Octavo.Gate.Nabu.Preferences.Preference ifJointProductPreferences = null;

        public IfJointProduct(string pPreferencesRoot)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
            ifJointProductPreferences = preferencesManager.GetPreference("Insignis.Asset.Management.Clients.Helper", "IfJointProduct", "IfJointProduct");
            if (ifJointProductPreferences == null)
                ifJointProductPreferences = new Octavo.Gate.Nabu.Preferences.Preference("IfJointProduct", "B");
        }

        public void Set(string pProductCodeID, string pValue)
        {
            if (ifJointProductPreferences == null)
                ifJointProductPreferences = new Octavo.Gate.Nabu.Preferences.Preference("IfJointProduct", "B");

            ifJointProductPreferences.SetChildPreference(preferencesManager.ProcessPreference(pProductCodeID, pValue, ifJointProductPreferences));
            preferencesManager.SetPreference("Insignis.Asset.Management.Clients.Helper", "IfJointProduct", ifJointProductPreferences);
        }

        public string Get(string pProductCodeID)
        {
            string value = "B";
            if (ifJointProductPreferences != null)
            {
                string tValue = preferencesManager.GetPreference(pProductCodeID, ifJointProductPreferences);
                if (tValue != null && tValue.Trim().Length > 0)
                    value = tValue;
            }
            return value;
        }
    }
}