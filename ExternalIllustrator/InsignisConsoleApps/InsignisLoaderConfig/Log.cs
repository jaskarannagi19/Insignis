using System;
using System.IO;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class Log
    {
        private string logFolder = "";
        public Log(string pLogFolder)
        {
            logFolder = pLogFolder;
            if (logFolder.EndsWith("\\") == false)
                logFolder += "\\";
        }
        public void LogActivity(string pMessage)
        {
            string logFilename = logFolder + "IAMDataLoader" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            using (StreamWriter sw = new StreamWriter(logFilename, true))
            {
                sw.WriteLine("<logEntry at=\"" + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "\" message=\"" + pMessage + "\"/>");
                sw.Close();
            }
        }
    }
}
