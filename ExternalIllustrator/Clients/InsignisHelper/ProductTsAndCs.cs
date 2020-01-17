using System.IO;
using System.Linq;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class ProductTsAndCs
    {
        private string publicFacingURL = "";
        private string internalFacingURL = "";
        private int institutionID = -1;
        private int partID = -1;

        public ProductTsAndCs(string pPublicFacingURL, string pInternalFacingURL, int pInstitutionID, int pPartID)
        {
            publicFacingURL = pPublicFacingURL;
            internalFacingURL = pInternalFacingURL;
            institutionID = pInstitutionID;
            partID = pPartID;
        }

        public ProductTsAndCs(string pPublicFacingURL, string pInternalFacingURL, int pInstitutionID, Octavo.Gate.Nabu.Entities.Operations.Part pPart)
        {
            publicFacingURL = pPublicFacingURL;
            internalFacingURL = pInternalFacingURL;
            institutionID = pInstitutionID;
            if (pPart != null && pPart.ErrorsDetected == false && pPart.PartID.HasValue)
                partID = pPart.PartID.Value;
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
                    folder += "Products";
                    if (Directory.Exists(folder))
                    {
                        folder += "\\";
                        folder += institutionID;
                        if (Directory.Exists(folder))
                        {
                            folder += "\\";
                            folder += partID;
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

                    folder += "Products";
                    publicFacing += "Products";

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

                            folder += partID;
                            publicFacing += partID;

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
