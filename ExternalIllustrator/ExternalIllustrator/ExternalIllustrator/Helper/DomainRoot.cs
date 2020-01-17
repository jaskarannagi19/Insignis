using System.Configuration;

namespace Insignis.Asset.Management.External.Illustrator.Helper
{
    public class DomainRoot
    {
        public static string Get()
        {
            string domainRoot = ConfigurationManager.AppSettings["domainRoot"];
            if (domainRoot.EndsWith("/") == false)
                domainRoot += "/";
            return domainRoot;
        }
        public static string GetPublic()
        {
            string publicRoot = ConfigurationManager.AppSettings["publicRoot"];
            if (publicRoot.EndsWith("/") == false)
                publicRoot += "/";
            return publicRoot;
        }
        public static string GetClient()
        {
            string clientRoot = ConfigurationManager.AppSettings["clientRoot"];
            if (clientRoot.EndsWith("/") == false)
                clientRoot += "/";
            return clientRoot;
        }
    }
}