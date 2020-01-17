using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class NumberFormatter
    {
        public static decimal RoundDown(decimal pValue, int pDecimalPlaces)
        {
            decimal value = 0;
            try
            {
                string sValue = pValue.ToString();
                if (sValue.Contains("."))
                {
                    int indexOfDot = sValue.IndexOf(".");
                    string wholeNumber = sValue.Substring(0, indexOfDot);
                    string fraction = "";
                    indexOfDot++;
                    for (int i = 0; i < pDecimalPlaces; i++)
                    {
                        if (indexOfDot + i < sValue.Length)
                            fraction += sValue.Substring(indexOfDot + i, 1);
                        else
                            fraction += "0";
                    }
                    value = Convert.ToDecimal(wholeNumber + "." + fraction);
                }
                else
                    value = pValue;
            }
            catch
            {
            }
            return value;
        }
        public static decimal RoundDownPositive(decimal pValue, int pDecimalPlaces)
        {
            decimal value = 0;
            try
            {
                string sValue = pValue.ToString().Replace("-","");
                if (sValue.Contains("."))
                {
                    int indexOfDot = sValue.IndexOf(".");
                    string wholeNumber = sValue.Substring(0, indexOfDot);
                    string fraction = "";
                    indexOfDot++;
                    for (int i = 0; i < pDecimalPlaces; i++)
                    {
                        if (indexOfDot + i < sValue.Length)
                            fraction += sValue.Substring(indexOfDot + i, 1);
                        else
                            fraction += "0";
                    }
                    value = Convert.ToDecimal(wholeNumber + "." + fraction);
                }
                else
                    value = pValue;
            }
            catch
            {
            }
            return value;
        }
    }
}