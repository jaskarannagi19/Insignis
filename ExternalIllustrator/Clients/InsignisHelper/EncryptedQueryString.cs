using Octavo.Gate.Nabu.Encryption;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class EncryptedQueryString
    {
        private List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

        public EncryptedQueryString(System.Web.HttpRequest pRequest)
        {
            EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
            if (pRequest.QueryString["eq"] != null && pRequest.QueryString["eq"].Length > 0)
            {
                string decryptedQueryString = encryptorDecryptor.Decrypt(encryptorDecryptor.UrlDecode(pRequest.QueryString["eq"]));
                if (decryptedQueryString.Length > 0)
                {
                    if (decryptedQueryString.Contains("~"))
                    {
                        if (decryptedQueryString.Contains("|"))
                        {
                            string pipeSeparator = "|";
                            string[] queryStringVariables = decryptedQueryString.Split(pipeSeparator.ToCharArray());
                            if (queryStringVariables.Length > 0)
                            {
                                string tildaSeparator = "~";
                                foreach (string queryStringVariable in queryStringVariables)
                                {
                                    string[] parts = queryStringVariable.Split(tildaSeparator.ToCharArray());
                                    if (parts.Length == 2)
                                        parameters.Add(new KeyValuePair<string, string>(parts[0], parts[1]));
                                }
                            }
                        }
                        else
                        {
                            string tildaSeparator = "~";
                            string[] parts = decryptedQueryString.Split(tildaSeparator.ToCharArray());
                            if (parts.Length == 2)
                                parameters.Add(new KeyValuePair<string, string>(parts[0], parts[1]));
                        }
                    }
                }
            }
        }

        public string GetQueryString(string pQueryString)
        {
            string decryptedQueryString = "";
            try
            {
                if(pQueryString != null && pQueryString.Trim().Length > 0)
                {
                    if (pQueryString.StartsWith("?"))
                        decryptedQueryString = pQueryString.Substring(1);
                    else
                        decryptedQueryString = pQueryString;

                    if (decryptedQueryString.Contains("="))
                        decryptedQueryString = decryptedQueryString.Replace("=", "~");

                    if (decryptedQueryString.Contains("&"))
                        decryptedQueryString = decryptedQueryString.Replace("&", "|");
                }
            }
            catch
            {
            }
            if (decryptedQueryString.Trim().Length > 0)
            {
                EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
                return encryptorDecryptor.UrlEncode(encryptorDecryptor.Encrypt(decryptedQueryString));
            }
            else
                return decryptedQueryString;
        }

        public int GetIntegerParameter(string pKey)
        {
            int parameterValue = -1;
            try
            {
                parameterValue = Convert.ToInt32(GetStringParameter(pKey));
            }
            catch
            {
            }
            return parameterValue;
        }

        public string GetStringParameter(string pKey)
        {
            string parameterValue = "";
            try
            {
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (parameter.Key.CompareTo(pKey) == 0)
                    {
                        parameterValue = parameter.Value;
                        break;
                    }
                }
            }
            catch
            {
            }
            return parameterValue;
        }

        public string GetDecryptedQueryString()
        {
            string decryptedQueryString = "";
            try
            {
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (decryptedQueryString.Length > 0)
                        decryptedQueryString += "&";
                    decryptedQueryString += parameter.Key + "=" + parameter.Value;
                }
            }
            catch
            {
            }
            return decryptedQueryString;
        }
    }
}
