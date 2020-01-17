using System.IO;
using System.Linq;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class InstitutionTsAndCs
    {
        private string publicFacingURL = "";
        private string internalFacingURL = "";
        private int institutionID = -1;

        private Octavo.Gate.Nabu.Preferences.Manager preferencesManager = null;
        public Octavo.Gate.Nabu.Preferences.Preference termsAndConditionsPreferences = null;

        public InstitutionTsAndCs(string pPublicFacingURL, string pinternalFacingURL, int pInstitutionID, string pPreferencesRootFolder)
        {
            preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRootFolder);
            publicFacingURL = pPublicFacingURL;
            internalFacingURL = pinternalFacingURL;
            institutionID = pInstitutionID;
        }

        public bool Exists(string pProductType)
        {
            bool result = false;
            try
            {
                string folder = internalFacingURL;
                if (Directory.Exists(folder))
                {
                    if (folder.EndsWith("\\") == false)
                        folder += "\\";
                    folder += "Institutions";
                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        folder += institutionID;
                        if (Directory.Exists(folder))
                        {
                            folder += "\\";
                            folder += pProductType;
                            if (Directory.Exists(folder))
                            {
                                if (Directory.GetFiles(folder, "*.pdf").Length > 0)
                                    result = true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public string GetURL(string pProductType)
        {
            string url = "";
            try
            {
                string folder = internalFacingURL;
                string publicFacing = publicFacingURL;

                if (Directory.Exists(folder))
                {
                    if (folder.EndsWith("\\") == false)
                        folder += "\\";

                    if (publicFacing.EndsWith("/") == false)
                        publicFacing += "/";

                    folder += "Institutions";
                    publicFacing += "Institutions";

                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        publicFacing += "/";

                        folder += institutionID;
                        publicFacing += institutionID;

                        if (Directory.Exists(folder))
                        {
                            folder += "\\";
                            publicFacing += "/";

                            folder += pProductType;
                            publicFacing += pProductType;

                            if (Directory.Exists(folder))
                            {
                                DirectoryInfo di = new DirectoryInfo(folder);
                                FileSystemInfo[] files = di.GetFileSystemInfos();
                                FileSystemInfo[] orderedFiles = files.OrderBy(f => f.LastWriteTime).ToArray();
                                orderedFiles = orderedFiles.Reverse().ToArray();
                                url = publicFacing + "/" + orderedFiles[0].Name;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return url;
        }

        public void LoadCheckBoxes()
        {
            termsAndConditionsPreferences = preferencesManager.GetPreference("Terms.And.Conditions", institutionID, "Preferences");
            if (termsAndConditionsPreferences == null)
            {
                termsAndConditionsPreferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");
                Octavo.Gate.Nabu.Preferences.Preference agreeToTermsAndConditions = new Octavo.Gate.Nabu.Preferences.Preference("MandatoryCheckBox", "I confirm that I have read and understood the Terms and Conditions");
                termsAndConditionsPreferences.Children.Add(agreeToTermsAndConditions);
            }
        }

        public void SaveCheckBoxes()
        {
            if (termsAndConditionsPreferences == null)
            {
                termsAndConditionsPreferences = preferencesManager.GetPreference("Terms.And.Conditions", institutionID, "Preferences");
                if (termsAndConditionsPreferences == null)
                {
                    termsAndConditionsPreferences = new Octavo.Gate.Nabu.Preferences.Preference("Preferences", "");
                    Octavo.Gate.Nabu.Preferences.Preference agreeToTermsAndConditions = new Octavo.Gate.Nabu.Preferences.Preference("MandatoryCheckBox", "I confirm that I have read and understood the Terms and Conditions");
                    termsAndConditionsPreferences.Children.Add(agreeToTermsAndConditions);
                }
            }
            preferencesManager.SetPreference("Terms.And.Conditions", institutionID, termsAndConditionsPreferences);
        }
    }
}
