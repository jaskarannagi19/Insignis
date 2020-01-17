using System;

namespace Insignis.Console.Apps.Data.Loader.Config.Helper
{
    public class DateTimePattern
    {
        public static string Process(string pSource)
        {
            string destination = pSource;
            if (destination.Contains("{"))
            {
                if (destination.Contains("{yy}"))
                    destination = destination.Replace("{yy}", DateTime.Now.ToString("yy"));
                if (destination.Contains("{yyyy}"))
                    destination = destination.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));
                if (destination.Contains("{MM}"))
                    destination = destination.Replace("{MM}", DateTime.Now.ToString("MM"));
                if (destination.Contains("{MMM}"))
                    destination = destination.Replace("{MMM}", DateTime.Now.ToString("MMM"));
                if (destination.Contains("{dd}"))
                    destination = destination.Replace("{dd}", DateTime.Now.ToString("dd"));
                if (destination.Contains("{hh}"))
                    destination = destination.Replace("{hh}", DateTime.Now.ToString("hh"));
                if (destination.Contains("{HH}"))
                    destination = destination.Replace("{HH}", DateTime.Now.ToString("HH"));
                if (destination.Contains("{mm}"))
                    destination = destination.Replace("{mm}", DateTime.Now.ToString("mm"));
                if (destination.Contains("{ss}"))
                    destination = destination.Replace("{ss}", DateTime.Now.ToString("ss"));
            }
            return destination;
        }
    }
}
