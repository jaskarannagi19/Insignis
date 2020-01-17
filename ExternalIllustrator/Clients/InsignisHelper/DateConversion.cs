using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class DateConversion
    {
        public static double GetDifferenceBetweenDatesIgnoringTime(DateTime pDate1, DateTime pDate2)
        {
            double difference = -1;
            try
            {
                DateTime date1 = DateTime.ParseExact(pDate1.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime date2 = DateTime.ParseExact(pDate2.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                difference = date1.Subtract(date2).TotalDays;
            }
            catch
            {
            }
            return difference;
        }
    }
}