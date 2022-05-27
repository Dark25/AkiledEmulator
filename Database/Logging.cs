using ConsoleWriter;
using System;
using System.Text;

namespace Akiled.Database
{
    public static class LoggingMySql
    {
        public static bool DisabledState
        {
            get
            {
                return Writer.DisabledState;
            }
            set
            {
                Writer.DisabledState = value;
            }
        }

        public static void WriteLine(string Line)
        {
            Writer.WriteLine(Line);
        }

        public static void LogException(string logText)
        {
            Writer.LogException(DateTime.Now.ToString() + ": " + Environment.NewLine + logText + Environment.NewLine);
        }

        public static void LogCriticalException(string logText)
        {
            Writer.LogCriticalException(DateTime.Now.ToString() + ": " + logText);
        }

        public static void LogCacheError(string logText)
        {
            Writer.LogCacheError(DateTime.Now.ToString() + ": " + logText);
        }

        public static void LogDenial(string logText)
        {
            Writer.LogDDOSS(DateTime.Now.ToString() + ": " + logText);
        }

        public static void LogMessage(string logText)
        {
            Writer.LogMessage(DateTime.Now.ToString() + ": " + logText);
        }

        public static void LogThreadException(string Exception, string Threadname)
        {
            Writer.LogThreadException(DateTime.Now.ToString() + ": " + Exception, Threadname);
        }

        public static void LogQueryError(Exception Exception, string query)
        {
            Writer.LogQueryError(Exception, DateTime.Now.ToString() + ": " + query);
        }

        public static void LogPacketException(string Packet, string Exception)
        {
            Writer.LogPacketException(Packet, DateTime.Now.ToString() + " : " + Exception);
        }

        public static void HandleException(Exception pException, string pLocation)
        {
            Writer.HandleException(pException, DateTime.Now.ToString() + ": " + pLocation);
        }

        public static void DisablePrimaryWriting(bool ClearConsole)
        {
            Writer.DisablePrimaryWriting(ClearConsole);
        }

        public static void LogShutdown(StringBuilder builder)
        {
            Writer.LogShutdown(builder);
        }
    }
}
