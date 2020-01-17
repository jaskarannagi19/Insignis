namespace Insignis.Asset.Management.Admin.Helper
{
    public class SortCode
    {
        public static bool IsValid(string pSortCode)
        {
            bool result = false;
            try
            {
                if (pSortCode != null)
                {
                    if (pSortCode.Trim().Length > 0)
                    {
                        if (pSortCode.Trim().Length == 6)
                        {
                            int numericCharacterCount = 0;

                            for (int i = 0; i < pSortCode.Length; i++)
                            {
                                if (char.IsDigit(pSortCode[i]))
                                    numericCharacterCount++;
                            }

                            if (numericCharacterCount == pSortCode.Length)
                                result = true;
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public static string DisplayFormatted(string pSortCode)
        {
            string formattedSortCode = "";
            if (IsValid(pSortCode))
            {
                formattedSortCode = pSortCode.Substring(0, 2);
                formattedSortCode += "-";
                formattedSortCode += pSortCode.Substring(2, 2);
                formattedSortCode += "-";
                formattedSortCode += pSortCode.Substring(4, 2);
            }
            else
                formattedSortCode = pSortCode;
            return formattedSortCode;
        }
    }
}