using Akiled.Core;
using System;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Akiled
{
    public static class Program
    {
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void Main()
        {
            InitEnvironment();
            while (true)
            {
                //  \/ This                       \/ and this
                if (!Console.IsInputRedirected && Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Akiled Command> ");
                    string text = Console.ReadLine();
                    if (text.Length > 0)
                    {
                        ConsoleCommands.InvokeCommand(text.Split(' ', StringSplitOptions.None)[0]);
                    }
                }
            }
        }

        [MTAThread]
        public static async Task InitEnvironment()
        {
            Console.ForegroundColor = ConsoleColor.White;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            await AkiledEnvironment.Initialize().ConfigureAwait(true);
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Logging.DisablePrimaryWriting(true);
            Logging.LogCriticalException("SYSTEM CRITICAL EXCEPTION: " + ((Exception)args.ExceptionObject).ToString());
            AkiledEnvironment.PreformShutDown(true);
        }
    }
}