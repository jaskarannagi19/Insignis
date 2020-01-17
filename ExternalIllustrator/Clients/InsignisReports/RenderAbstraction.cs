using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Entities;
using System.IO;

namespace Insignis.Asset.Management.Reports
{
    public class RenderAbstraction : BaseType
    {
        public string InternalFacingOutputFolder = "";
        public string PublicFacingOutputFolder = "";
        public string XmlFileName = "";

        public RenderAbstraction(string pInternalFacingOutputFolder, string pPublicFacingOutputFolder, string pXmlFileName)
        {
            InternalFacingOutputFolder = pInternalFacingOutputFolder;
            PublicFacingOutputFolder = pPublicFacingOutputFolder;
            XmlFileName = pXmlFileName;
        }

        public ReportContent RenderGenericReport(GenericReport pGenericReport)
        {
            string internalFacingFileName = InternalFacingOutputFolder + "\\" + XmlFileName;
            string publicFacingFileName = PublicFacingOutputFolder + "/" + XmlFileName;

            ReportContent genericContent = new ReportContent();
            try
            {
                if (File.Exists(internalFacingFileName))
                    File.Delete(internalFacingFileName);

                pGenericReport.SerialiseToDisk(internalFacingFileName);

                if (File.Exists(internalFacingFileName))
                {
                    FileInfo fi = new FileInfo(internalFacingFileName);
                    if (fi.Length > 0)
                        genericContent.URI = publicFacingFileName;
                }
            }
            catch
            {
            }
            return genericContent;
        }

        public GenericReport LoadGenericReport()
        {
            GenericReport genericReport = null;
            try
            {
                string internalFacingFileName = InternalFacingOutputFolder + "\\" + XmlFileName;
                genericReport = GenericReport.DeserialiseFromDisk(internalFacingFileName);
            }
            catch
            {
            }
            return genericReport;
        }
    }
}
