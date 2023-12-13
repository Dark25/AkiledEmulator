using ConsoleWriter;
using System;
namespace AkiledEmulator.Core
{
    public static class ExceptionLogger
    {
        public static void WriteLine(string line) => Writer.WriteLine(line);

        public static void LogException(string logText) => Writer.LogException(DateTime.Now.ToString() + ": " + Environment.NewLine + logText + Environment.NewLine);

        public static void LogCriticalException(string logText) => Writer.LogCriticalException(DateTime.Now.ToString() + ": " + logText);

        public static void LogDenial(string logText) => Writer.LogDDOS(DateTime.Now.ToString() + ": " + logText);

        public static void LogMessage(string logText) => Writer.LogMessage(DateTime.Now.ToString() + ": " + logText);

        public static void LogThreadException(string exception, string threadName) => Writer.LogThreadException(DateTime.Now.ToString() + ": " + exception, threadName);

        public static void LogQueryError(Exception exception, string query) => Writer.LogQueryError(exception, DateTime.Now.ToString() + ": " + query);

        public static void LogPacketException(string packet, string exception) => Writer.LogPacketException(packet, DateTime.Now.ToString() + " : " + exception);

        public static void HandleException(Exception exception, string location) => Writer.HandleException(exception, DateTime.Now.ToString() + ": " + location);

        public static void DisablePrimaryWriting(bool clearConsole) => Writer.DisablePrimaryWriting(clearConsole);
    }
}
