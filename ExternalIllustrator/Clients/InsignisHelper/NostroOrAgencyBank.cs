namespace Insignis.Asset.Management.Admin.Helper
{
    public class NostroOrAgencyBank
    {
        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        private Octavo.Gate.Nabu.Preferences.Preference institutionNostroOrAgencyBankPreference = null;
        private Octavo.Gate.Nabu.Preferences.Preference institutionDefaultAccountNumberPreference = null;

        public NostroOrAgencyBank(string pPreferencesRoot)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
            institutionNostroOrAgencyBankPreference = preferencesManager.GetPreference("Insignis.Asset.Management.Admin.Helper", 0, "NostroOrAgencyBank");
            if (institutionNostroOrAgencyBankPreference == null)
                institutionNostroOrAgencyBankPreference = new Octavo.Gate.Nabu.Preferences.Preference("NostroOrAgencyBank", "true");

            institutionDefaultAccountNumberPreference = preferencesManager.GetPreference("Insignis.Asset.Management.Admin.Helper", 0, "DefaultAccountNumber");
            if (institutionDefaultAccountNumberPreference == null)
                institutionDefaultAccountNumberPreference = new Octavo.Gate.Nabu.Preferences.Preference("DefaultAccountNumber", "unspecified");
        }

        public void Set(int pInstitutionID, bool pIsNostroBank)
        {
            if (institutionNostroOrAgencyBankPreference == null)
                institutionNostroOrAgencyBankPreference = new Octavo.Gate.Nabu.Preferences.Preference("NostroOrAgencyBank", "true");

            institutionNostroOrAgencyBankPreference.SetChildPreference(preferencesManager.ProcessPreference(pInstitutionID.ToString(), ((pIsNostroBank == true) ? "true" : "false"), institutionNostroOrAgencyBankPreference));
            preferencesManager.SetPreference("Insignis.Asset.Management.Admin.Helper", 0, institutionNostroOrAgencyBankPreference);
        }

        public void Set(int pInstitutionID, string pAccountNumber)
        {
            if (institutionDefaultAccountNumberPreference == null)
                institutionDefaultAccountNumberPreference = new Octavo.Gate.Nabu.Preferences.Preference("DefaultAccountNumber", pAccountNumber);

            institutionDefaultAccountNumberPreference.SetChildPreference(preferencesManager.ProcessPreference(pInstitutionID.ToString(), pAccountNumber, institutionDefaultAccountNumberPreference));
            preferencesManager.SetPreference("Insignis.Asset.Management.Admin.Helper", 0, institutionDefaultAccountNumberPreference);
        }

        public bool IsNostroBank(int pInstitutionID)
        {
            bool isNostroBank = false;
            if (institutionNostroOrAgencyBankPreference != null)
            {
                string preferenceValue = preferencesManager.GetPreference(pInstitutionID.ToString(), institutionNostroOrAgencyBankPreference);
                if (preferenceValue != null && preferenceValue.Trim().Length > 0)
                {
                    if (preferenceValue.CompareTo("true") == 0)
                        isNostroBank = true;
                }
                else
                    isNostroBank = true;        // default to true
            }
            else
                isNostroBank = true;            // default to true
            return isNostroBank;
        }

        public string GetDefaultAccountNumber(int pInstitutionID)
        {
            string defaultAccountNumber = "unspecified";
            if (institutionDefaultAccountNumberPreference != null)
            {
                string preferenceValue = preferencesManager.GetPreference(pInstitutionID.ToString(), institutionDefaultAccountNumberPreference);
                if (preferenceValue != null && preferenceValue.Trim().Length > 0)
                    defaultAccountNumber = preferenceValue;
            }
            return defaultAccountNumber;
        }
    }
}