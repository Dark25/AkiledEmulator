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
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.Write("Akiled Command> ");
                    string Input = Console.ReadLine();

                    if (Input.Length > 0)
                    {
                        string s = Input.Split(' ')[0];

                        ConsoleCommands.InvokeCommand(s);
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