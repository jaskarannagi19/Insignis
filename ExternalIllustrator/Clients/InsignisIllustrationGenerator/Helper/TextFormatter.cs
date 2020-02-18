using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Helper
{
    public class TextFormatter
    {

        public static string FormatTwoDecimal(dynamic amount)
        {

            return string.Format("{0,0:0.00}", amount);


        }


        public static string RemoveNonASCIICharacters(string pSource)
        {
            string formattedText = "";
            if (pSource != null && pSource.Length > 0)
            {
                for (int i = 0; i < pSource.Length; i++)
                {
                    if (Char.IsDigit(pSource[i]))
                        formattedText += pSource[i];
                    else if (MyIsLetter(pSource[i]))
                        formattedText += pSource[i];
                    else if (Char.IsPunctuation(pSource[i]))
                        formattedText += pSource[i];
                    else if (Char.IsSeparator(pSource[i]))
                        formattedText += pSource[i];
                    else if (Char.IsWhiteSpace(pSource[i]))
                        formattedText += pSource[i];
                    else if (Char.IsControl(pSource[i]) || Char.IsHighSurrogate(pSource[i]) || Char.IsLowSurrogate(pSource[i]))
                    {
                        // we don't copy control characters
                    }
                }
            }
            return formattedText;
        }

        public static string RemoveNonAlphaNumericCharacters(string pSource)
        {
            string formattedText = "";
            if (pSource != null && pSource.Length > 0)
            {
                for (int i = 0; i < pSource.Length; i++)
                {
                    if (Char.IsDigit(pSource[i]))
                        formattedText += pSource[i];            // copy numbers
                    else if (MyIsLetter(pSource[i]))
                        formattedText += pSource[i];            // copy letters
                    else if (Char.IsPunctuation(pSource[i]))
                    {
                        // we don't copy punctuation
                    }
                    else if (Char.IsSeparator(pSource[i]))
                    {
                        // we don't copy separators
                    }
                    else if (Char.IsWhiteSpace(pSource[i]))
                    {
                        // we don't copy white space
                    }
                    else if (Char.IsControl(pSource[i]) || Char.IsHighSurrogate(pSource[i]) || Char.IsLowSurrogate(pSource[i]))
                    {
                        // we don't copy control characters
                    }
                }
            }
            return formattedText;
        }

        public static bool MyIsLetter(char pLetter)
        {
            if (pLetter >= 'a' && pLetter <= 'z')
                return true;
            else if (pLetter >= 'A' && pLetter <= 'Z')
                return true;
            else
                return false;
        }
    }


}

