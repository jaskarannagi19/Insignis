using Octavo.Gate.Nabu.Entities;
using System.Collections.Generic;
using System.Web;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Form : BaseType
    {
        public Form()
        {
        }

        public string GetInput(string pControlID, HttpRequest pRequest)
        {
            string responseValue = "";
            foreach (string parameter in pRequest.Params.Keys)
            {
                if (parameter.EndsWith(pControlID))
                {
                    responseValue = pRequest.Form[parameter];
                    break;
                }
            }
            return responseValue;
        }

        public bool ControlExists(string pControlID, HttpRequest pRequest)
        {
            bool controlExists = false;
            foreach (string parameter in pRequest.Params.Keys)
            {
                if (parameter.EndsWith(pControlID))
                {
                    controlExists = true;
                    break;
                }
            }
            return controlExists;
        }

        public bool ControlExistsContaining(string pControlID, HttpRequest pRequest)
        {
            bool controlExists = false;
            foreach (string parameter in pRequest.Params.Keys)
            {
                if (parameter.Contains(pControlID))
                {
                    controlExists = true;
                    break;
                }
            }
            return controlExists;
        }

        public List<string> ListElementsContaining(string pControlID, HttpRequest pRequest)
        {
            List<string> elementIDs = new List<string>();
            foreach (string parameter in pRequest.Params.Keys)
            {
                if (parameter.Contains(pControlID))
                    elementIDs.Add(parameter);
            }
            return elementIDs;
        }
    }
}