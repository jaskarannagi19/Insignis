
namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Image
    {
        string domainRoot = "https://localhost:65531";

        public Image(string pDomainRoot)
        {
            domainRoot = pDomainRoot;
        }

        public string ImageReference(string pImageFilename)
        {
            return domainRoot + "LoggedIn/Images/" + pImageFilename;
        }
        public string RootImageReference(string pImageFilename)
        {
            return domainRoot + "/Images/" + pImageFilename;
        }
    }
}