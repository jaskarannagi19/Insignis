using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class FitchRatings
    {
        private string publicFacingURL = "";
        private string internalFacingURL = "";
        private int institutionID = -1;             // if 0 it means no specific insitution i.e. the general summary

        public FitchRatings(string pPublicFacingURL, string pinternalFacingURL, int pInstitutionID)
        {
            publicFacingURL = pPublicFacingURL;
            internalFacingURL = pinternalFacingURL;
            institutionID = pInstitutionID;
        }

        public bool Exists()
        {
            bool result = false;
            try
            {
                string folder = internalFacingURL;
                if (Directory.Exists(folder))
                {
                    if (folder.EndsWith("\\") == false)
                        folder += "\\";
                    folder += "FitchRatings";
                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        folder += institutionID;
                        if (Directory.Exists(folder))
                        {
                            if (Directory.GetFiles(folder, "*.pdf").Length > 0)
                                result = true;
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public string GetURL()
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

                    folder += "FitchRatings";
                    publicFacing += "FitchRatings";

                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        publicFacing += "/";

                        folder += institutionID;
                        publicFacing += institutionID;

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
            catch
            {
            }
            return url;
        }

        public static bool CanSeeFitchMenu(int pPartyID, string pPreferencesRoot)
        {
            bool result = false;
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference visibilityPreferences = preferencesManager.GetPreference("Fitch.Ratings.Menu", pPartyID, "Visibility");
                if (visibilityPreferences != null)
                {
                    if (visibilityPreferences.GetChildPreference("IsVisible") != null)
                    {
                        if (visibilityPreferences.GetChildPreference("IsVisible").Value.ToLower().CompareTo("true") == 0)
                            result = true;
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static void SetFitchMenuVisibility(int pPartyID, bool pIsVisible, string pPreferencesRoot)
        {
            try
            {
                Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(pPreferencesRoot);
                Octavo.Gate.Nabu.Preferences.Preference visibilityPreferences = preferencesManager.GetPreference("Fitch.Ratings.Menu", pPartyID, "Visibility");
                if(visibilityPreferences == null)
                    visibilityPreferences = new Octavo.Gate.Nabu.Preferences.Preference("Visibility", "");
                visibilityPreferences.SetChildPreference(preferencesManager.ProcessPreference("IsVisible", ((pIsVisible==true)?"true":"false"), visibilityPreferences));
                preferencesManager.SetPreference("Fitch.Ratings.Menu", pPartyID, visibilityPreferences);
            }
            catch
            {
            }
        }

        public static List<string> ListRatings()
        {
            List<string> ratings = new List<string>();

            ratings.Add("AAA");
            ratings.Add("AA+");
            ratings.Add("AA");
            ratings.Add("AA-");
            ratings.Add("A+");
            ratings.Add("A");
            ratings.Add("A-");
            ratings.Add("BBB+");
            ratings.Add("BBB");
            ratings.Add("BBB-");
            ratings.Add("BB+");
            ratings.Add("BB");
            ratings.Add("BB-");
            ratings.Add("B+");
            ratings.Add("B");
            ratings.Add("B-");
            ratings.Add("CCC+");
            ratings.Add("CCC");
            ratings.Add("CCC-");
            ratings.Add("CC");
            ratings.Add("C");
            ratings.Add("RD");
            ratings.Add("D");

            return ratings;
        }

        public static bool IsRatingLessThanOrEqualTo(string pRating, string pMinimumRatingRequired)
        {
            bool result = false;

            try
            {
                List<string> ratings = ListRatings();

                int minimumRequiredRatingIndex = -1;
                int ratingIndex = -1;

                for (int index = 0; index < ratings.Count; index++)
                {
                    if (ratings[index].CompareTo(pMinimumRatingRequired) == 0)
                        minimumRequiredRatingIndex = index;

                    if (ratings[index].CompareTo(pRating) == 0)
                        ratingIndex = index;
                }

                if (ratingIndex != -1 && minimumRequiredRatingIndex != -1)
                {
                    if (ratingIndex <= minimumRequiredRatingIndex)
                        result = true;
                }
            }
            catch
            {
            }
            return result;
        }
    }
}