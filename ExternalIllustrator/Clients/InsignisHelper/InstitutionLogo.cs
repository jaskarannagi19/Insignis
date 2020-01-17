using System.IO;
using System.Linq;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class InstitutionLogo
    {
        private string publicFacingURL = "";
        private string internalFacingURL = "";
        private int institutionID = -1;

        public InstitutionLogo(string pPublicFacingURL, string pinternalFacingURL, int pInstitutionID)
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
                    folder += "Institutions";
                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        folder += institutionID;
                        if (Directory.Exists(folder))
                        {
                            folder += "\\";
                            folder += "Logo";
                            if (Directory.Exists(folder))
                            {
                                if (Directory.GetFiles(folder, "*.jpg").Length > 0)
                                    result = true;
                                else if (Directory.GetFiles(folder, "*.png").Length > 0)
                                    result = true;
                                else if (Directory.GetFiles(folder, "*.jpeg").Length > 0)
                                    result = true;
                                else if (Directory.GetFiles(folder, "*.gif").Length > 0)
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

                            folder += "Logo";
                            publicFacing += "Logo";

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
    }
}