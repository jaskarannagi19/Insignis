using Octavo.Gate.Nabu.Entities;

namespace Insignis.Asset.Management.Action.Logging.Entities
{
    public class ActionOutputDocument : BaseType
    {
        public int? ActionOutputDocumentID = null;
        public string DocumentType = null;
        public string PublicFacingFolder = null;
        public string InternalFacingFolder = null;
        public string Filename = null;

        public ActionOutputDocument()
        {
        }
        public ActionOutputDocument(int pActionOutputDocumentID)
        {
            ActionOutputDocumentID = pActionOutputDocumentID;
        }
        public ActionOutputDocument(int? pActionOutputDocumentID)
        {
            ActionOutputDocumentID = pActionOutputDocumentID;
        }
        public ActionOutputDocument(string pDocumentType,string pPublicFacingFolder, string pInternalFacingFolder, string pFilename)
        {
            DocumentType = pDocumentType;
            PublicFacingFolder = pPublicFacingFolder;
            InternalFacingFolder = pInternalFacingFolder;
            Filename = pFilename;
        }
    }
}
