using System;
using System.Collections;
using System.IO;
using System.Text;

namespace ConsoleWriter
{
    public class Writer
    {
        private static bool mDisabled = false;

        public static bool DisabledState
        {
            get
            {
                return mDisabled;
            }
            set
            {
                mDisabled = value;
            }
        }

        public static void WriteLine(string Line)
        {
            if (mDisabled)
                return;
            Console.WriteLine(Line);
        }

        public static void LogException(string logText)
        {
            WriteToFile("Logs/exceptions.txt", logText + "\r\n\r\n");
            WriteLine("Exception has been saved");
        }

        public static void LogDDOS(string logText)
        {
            WriteToFile("Logs/ddos.txt", logText + "\r\n\r\n");
            WriteLine("Exception has been saved");
        }

        public static void LogCriticalException(string logText)
        {
            WriteToFile("Logs/criticalexceptions.txt", logText + "\r\n\r\n");
            WriteLine("CRITICAL ERROR LOGGED");
        }

        public static void LogCacheError(string logText)
        {
            WriteToFile("Logs/cacheerror.txt", logText + "\r\n\r\n");
            WriteLine("Critical error saved");
        }

        public static void LogMessage(string logText)
        {
            Console.WriteLine(logText);
        }

        public static void LogDDOSS(string logText)
        {
            WriteToFile("Logs/ddos.txt", logText + "\r\n\r\n");
            WriteLine(logText);
        }

        public static void LogThreadException(string Exception, string Threadname)
        {
            WriteToFile("Logs/threaderror.txt", "Error in thread " + Threadname + ": \r\n" + Exception + "\r\n\r\n");
            WriteLine("Error in " + Threadname + " caught");
        }

        public static void LogQueryError(Exception Exception, string query)
        {
            WriteToFile("Logs/MySQLerrors.txt", "Error in query: \r\n" + (object)query + "\r\n" + (string)Exception.ToString() + "\r\n\r\n");
            WriteLine("Error in query caught");
        }

        public static void LogPacketException(string Packet, string Exception)
        {
            WriteToFile("Logs/packeterror.txt", "Error in packet " + Packet + ": \r\n" + Exception + "\r\n\r\n");
            WriteLine("User disconnection logged: " + Exception);
        }

        public static void HandleException(Exception pException, string pLocation)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Exception logged " + DateTime.Now.ToString() + " in " + pLocation + ":");
            stringBuilder.AppendLine(((object)pException).ToString());
            if (pException.InnerException != null)
            {
                stringBuilder.AppendLine("Inner exception:");
                stringBuilder.AppendLine(((object)pException.InnerException).ToString());
            }
            if (pException.HelpLink != null)
            {
                stringBuilder.AppendLine("Help link:");
                stringBuilder.AppendLine(pException.HelpLink);
            }
            if (pException.Source != null)
            {
                stringBuilder.AppendLine("Source:");
                stringBuilder.AppendLine(pException.Source);
            }
            if (pException.Data != null)
            {
                stringBuilder.AppendLine("Data:");
                foreach (DictionaryEntry dictionaryEntry in pException.Data)
                    stringBuilder.AppendLine(string.Concat(new object[4]
                    {
                      (object) "  Key: ",
                      dictionaryEntry.Key,
                      (object) "Value: ",
                      dictionaryEntry.Value
                              }));
            }
            if (pException.Message != null)
            {
                stringBuilder.AppendLine("Message:");
                stringBuilder.AppendLine(pException.Message);
            }
            if (pException.StackTrace != null)
            {
                stringBuilder.AppendLine("Stack trace:");
                stringBuilder.AppendLine(pException.StackTrace);
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            LogException(((object)stringBuilder).ToString());
        }

        public static void DisablePrimaryWriting(bool ClearConsole)
        {
            mDisabled = true;
            if (!ClearConsole)
                return;
            Console.Clear();
        }

        public static void LogShutdown(StringBuilder builder)
        {
            WriteToFile("Logs/shutdownlog.txt", ((object)builder).ToString());
        }

        private static void WriteToFile(string path, string content)
        {
            try
            {
                FileStream fileStream = new FileStream(Akiled.AkiledEnvironment.PatchDir + path, FileMode.Append, FileAccess.Write);
                byte[] bytes = Encoding.ASCII.GetBytes(Environment.NewLine + content);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                WriteLine("Could not write to file: " + ((object)ex).ToString() + ":" + content);
            }
        }
    }
}
