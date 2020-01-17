using System.IO;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class Echo
    {
        private static Log log = null;
        public bool ToConsole = false;

        public Echo(string pStoreLogsInFolder)
        {
            if (pStoreLogsInFolder.Trim().Length > 0)
            {
                if (Directory.Exists(pStoreLogsInFolder.Trim()))
                {
                    log = new Log(pStoreLogsInFolder);
                }
                else
                {
                    WriteLine("Warning - log folder does not exist.");
                }
            }
        }

        public void Write(string pMessage)
        {
            if (ToConsole)
            {
                System.Console.Write(pMessage);
            }
            if (log != null)
                log.LogActivity(pMessage);
        }

        public void WriteLine(string pMessage)
        {
            if (ToConsole)
            {
                System.Console.WriteLine(pMessage);
            }
            if (log != null)
                log.LogActivity(pMessage);
        }
    }
}
