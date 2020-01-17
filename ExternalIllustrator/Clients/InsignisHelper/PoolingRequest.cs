using Octavo.Gate.Nabu.Entities.Financial;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class PoolingRequest
    {
        public Institution institution = null;
        public List<ProductPoolingRequest> productPoolingRequests = new List<ProductPoolingRequest>();
        public int? optionalID = null;
    }
}
