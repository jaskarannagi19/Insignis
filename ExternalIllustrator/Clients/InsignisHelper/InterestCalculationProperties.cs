namespace Insignis.Asset.Management.Clients.Helper
{
    public class InterestCalculationProperties
    {
        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        private Octavo.Gate.Nabu.Preferences.Preference interestCalculationPreferences = null;

        public InterestCalculationProperties(string pPreferencesRoot)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
            interestCalculationPreferences = preferencesManager.GetPreference("Insignis.Asset.Management.Clients.Helper", "InterestCalculations", "InterestCalculations");
            if (interestCalculationPreferences == null)
                interestCalculationPreferences = new Octavo.Gate.Nabu.Preferences.Preference("InterestCalculations", "");
        }

        public void SetPreferences(string pProductCode, decimal? pInterestAccruedRate, int? pInterestCalculatedDayWithinPeriod, string pInterestFrequencyOfCalculation, bool? pInterestIsCompounded, string pInterestRegime, string pInterestClientViewConfig)
        {
            if (interestCalculationPreferences == null)
                interestCalculationPreferences = new Octavo.Gate.Nabu.Preferences.Preference("InterestCalculations", "");

            Octavo.Gate.Nabu.Preferences.Preference productSpecificPreferences = interestCalculationPreferences.GetChildPreference(pProductCode);
            if (productSpecificPreferences == null)
                productSpecificPreferences = new Octavo.Gate.Nabu.Preferences.Preference(pProductCode, "");

            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestAccruedRate", ((pInterestAccruedRate.HasValue == true) ? pInterestAccruedRate.ToString() : ""),productSpecificPreferences));
            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestCalculatedDayWithinPeriod", ((pInterestCalculatedDayWithinPeriod.HasValue == true) ? pInterestCalculatedDayWithinPeriod.ToString() : ""), productSpecificPreferences));
            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestFrequencyOfCalculation", ((pInterestFrequencyOfCalculation != null) ? pInterestFrequencyOfCalculation.ToString() : ""), productSpecificPreferences));
            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestIsCompounded", ((pInterestIsCompounded.HasValue == true) ? pInterestIsCompounded.ToString().ToLower() : ""), productSpecificPreferences));
            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestRegime", ((pInterestRegime != null) ? pInterestRegime.ToString() : ""), productSpecificPreferences));
            productSpecificPreferences.SetChildPreference(preferencesManager.ProcessPreference("InterestClientViewConfig", ((pInterestClientViewConfig != null == true) ? pInterestClientViewConfig.ToString() : ""), productSpecificPreferences));

            interestCalculationPreferences.SetChildPreference(productSpecificPreferences);

            preferencesManager.SetPreference("Insignis.Asset.Management.Clients.Helper", "InterestCalculations", interestCalculationPreferences);
        }

        public string GetPreference(string pProductCode, string pPreferenceName)
        {
            string preferenceValue = "";
            try
            {
                if (interestCalculationPreferences == null)
                    interestCalculationPreferences = new Octavo.Gate.Nabu.Preferences.Preference("InterestCalculations", "");

                Octavo.Gate.Nabu.Preferences.Preference productSpecificPreferences = interestCalculationPreferences.GetChildPreference(pProductCode);
                if (productSpecificPreferences == null)
                    productSpecificPreferences = new Octavo.Gate.Nabu.Preferences.Preference(pProductCode, "");
                else
                {
                    Octavo.Gate.Nabu.Preferences.Preference childPreference = productSpecificPreferences.GetChildPreference(pPreferenceName);
                    if (childPreference != null)
                    {
                        if (childPreference.Value != null && childPreference.Value.Trim().Length > 0)
                            preferenceValue = childPreference.Value;
                    }
                }
            }
            catch
            {
            }
            return preferenceValue;
        }
    }
}
