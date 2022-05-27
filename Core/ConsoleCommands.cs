using System;

namespace Akiled.Core
{
    public class ConsoleCommands
    {
        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return;

            try
            {
                #region Command parsing
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region stop
                    case "stop":
                    case "shutdown":
                        Logging.LogMessage("Server exiting at " + DateTime.Now);
                        Logging.DisablePrimaryWriting(true);
                        Console.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                        AkiledEnvironment.PreformShutDown(true);
                        break;
                    case "forceshutdown":
                        AkiledEnvironment.GetGame().gameLoopEnded = true;
                        break;
                    #endregion
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        {
                            Logging.LogMessage(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                            break;
                        }
                }
                #endregion
            }
            catch (Exception e)
            {
                Logging.LogMessage("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}