using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Operations;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class DirectProducts
    {
        public static Part[] ListRelevantProducts(string pProductLineAlias, int pHubAccountTypeID, BaseAbstraction pBaseAbstraction, int pLanguageID)
        {
            List<Part> relevantProducts = new List<Part>();
            try
            {
                OperationsAbstraction operationsAbstraction = new OperationsAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);

                Part[] allDirectProducts = operationsAbstraction.ListPartByFeatureValueDescendingWithinProductLine(pProductLineAlias, "AvailableTo", pLanguageID);
                foreach (Part directProduct in allDirectProducts)
                {
                    if (directProduct.ErrorsDetected == false)
                    {
                        PartFeature partFeature = operationsAbstraction.GetPartFeatureByAlias((int)directProduct.PartID, "AvailableTo", pLanguageID);
                        if (partFeature != null && partFeature.ErrorsDetected == false && partFeature.PartFeatureID.HasValue)
                        {
                            if (partFeature.Value != null && partFeature.Value.Trim().Length > 0)
                            {
                                string pipeSeparator = "|";
                                string[] accountTypeIDs = partFeature.Value.Split(pipeSeparator.ToCharArray());
                                foreach (string accountTypeID in accountTypeIDs)
                                {
                                    if (accountTypeID.Trim().Length > 0)
                                    {
                                        if (Convert.ToInt32(accountTypeID) == pHubAccountTypeID)
                                        {
                                            relevantProducts.Add(directProduct);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        partFeature = null;
                    }
                }
                allDirectProducts = null;
            }
            catch
            {
            }
            return relevantProducts.ToArray();
        }
    }
}
