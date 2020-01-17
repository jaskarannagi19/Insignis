using Octavo.Gate.Nabu.Entities.Financial;
using Octavo.Gate.Nabu.Entities.Operations;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class ProductPoolingRequest
    {
        public Part product = null;
        public List<Client> clients = new List<Client>();
    }
}
