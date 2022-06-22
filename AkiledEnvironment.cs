using Akiled.Communication.Encryption;
using Akiled.Communication.Encryption.Keys;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.WebSocket;
using Akiled.Core;
using Akiled.Core.FigureData;
using Akiled.Database;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;
using Akiled.HabboHotel.Users.UserData;
using Akiled.Net;
using Akiled.Utilities;
using ConnectionManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Akiled
{
    public class AkiledEnvironment: IAkiledEnvironment
    {
        private static ConcurrentDictionary<int, Habbo> _usersCached = new ConcurrentDictionary<int, Habbo>();
        private readonly IEnumerable<IStartable> _startableTasks;
        public static bool eventStarted = false;
        public static bool eventDisabled = false;
        public static Random Random = new Random();
        
        
        private static readonly List<char> Allowedchars = new List<char>((IEnumerable<char>)new char[82]
        {
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '0',
      '-',
      '.',
      '=',
      '?',
      '!',
      ':',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'á',
      'é',
      'í',
      'ó',
      'ú',
      'Á',
      'É',
      'Í',
      'Ó',
      'Ú',
      'ñ',
      'Ñ',
      'ü',
      'Ü'
        });

        private static Encoding _defaultEncoding;
        private static ConfigurationData _configuration;
        private static ConnectionHandeling _connectionManager;
        private static WebSocketManager _webSocketManager;
        private static Game _game;
        private static DatabaseManager _datebasemanager;
        private static RCONSocket _rcon;
        private static FigureDataManager _figureManager;
        private static LanguageManager _languageManager;
        public static DateTime ServerStarted;
        public static bool StaticEvents;
        public static string PatchDir;
        public AkiledEnvironment(
       IEnumerable<IStartable> startableTasks) => _startableTasks = startableTasks;



        public async Task<bool> Start()
        {
            Console.Clear();
            AkiledEnvironment.ServerStarted = DateTime.Now;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Clear();
            Console.WriteLine(@"");
            Console.WriteLine(@"");
            Console.WriteLine(@"");
            Console.WriteLine(@"                          _   _  _____ _    ___ ___    ___ __  __ _   _ _      _ _____ ___  ___");
            Console.WriteLine(@"                         /_\ | |/ /_ _| |  | __|   \  | __|  \/  | | | | |    /_\_   _/ _ \| _ \");
            Console.WriteLine(@"                        / _ \| ' < | || |__| _|| |) | | _|| |\/| | |_| | |__ / _ \| || (_) |   /");
            Console.WriteLine(@"                       / / \_\_|\_\___|____|___|___/  |___|_|  |_|\___/|____/_/ \_\_| \___/|_|_\");
            Console.WriteLine(@"");
            Console.WriteLine(@"                                 Basado en PlusEmulator y Wibbo Emulator - Akiled Emulator        ");
            Console.WriteLine(@"");
            Console.WriteLine(@"                           Desarrollado por Carlos Mota, --- Todos los derechos reservados   ");
            Console.WriteLine();
            _defaultEncoding = Encoding.Default;
            Console.ForegroundColor = ConsoleColor.Cyan;
            AkiledEnvironment.PatchDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/";
            Console.Title = "Cargando Emulator";

            try
            {
                AkiledEnvironment._configuration = new ConfigurationData(AkiledEnvironment.PatchDir + "Settings/configuration.ini", false);
                AkiledEnvironment._datebasemanager = new DatabaseManager(uint.Parse(AkiledEnvironment.GetConfig().data["db.pool.maxsize"]), uint.Parse(AkiledEnvironment.GetConfig().data["db.pool.minsize"]), AkiledEnvironment.GetConfig().data["db.hostname"], uint.Parse(AkiledEnvironment.GetConfig().data["db.port"]), AkiledEnvironment.GetConfig().data["db.username"], AkiledEnvironment.GetConfig().data["db.password"], AkiledEnvironment.GetConfig().data["db.name"]);
                int num = 0;
                while (!_datebasemanager.IsConnected())
                {
                    ++num;
                    Thread.Sleep(5000);
                    if (num > 10)
                    {
                        Logging.WriteLine("Error al conectar con el Mysql Server.");
                        Console.ReadKey(true);
                        Environment.Exit(1);
                        return false;
                    }
                }
                HabboEncryptionV2.Initialize(new RSAKeys());
                AkiledEnvironment._languageManager = new LanguageManager();
                AkiledEnvironment._languageManager.Init();
                AkiledEnvironment._game = new Game();
                AkiledEnvironment._game.StartGameLoop();
                AkiledEnvironment._figureManager = new FigureDataManager();
                AkiledEnvironment._figureManager.Init();
                if (AkiledEnvironment._configuration.data["Websocketenable"] == "true")
                    AkiledEnvironment._webSocketManager = new WebSocketManager(int.Parse(AkiledEnvironment.GetConfig().data["game.websocketsport"]), int.Parse(AkiledEnvironment.GetConfig().data["game.tcp.conlimit"]));
                AkiledEnvironment._connectionManager = new ConnectionHandeling(int.Parse(AkiledEnvironment.GetConfig().data["game.tcp.port"]), int.Parse(AkiledEnvironment.GetConfig().data["game.tcp.conlimit"]), int.Parse(AkiledEnvironment.GetConfig().data["game.tcp.conperip"]));
                if (AkiledEnvironment._configuration.data["Musenable"] == "true")
                    AkiledEnvironment._rcon = new RCONSocket(int.Parse(AkiledEnvironment.GetConfig().data["mus.tcp.port"]), AkiledEnvironment.GetConfig().data["mus.tcp.allowedaddr"].Split(';'));
                AkiledEnvironment.StaticEvents = AkiledEnvironment._configuration.data["static.events"] == "true";
                Logging.WriteLine("VARIABLES -> CARGADAS y LISTAS!");
                // Allow services to self initialize
                foreach (var task in _startableTasks)
                    await task.Start();

                TimeSpan TimeUsed = DateTime.Now - ServerStarted;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Logging.WriteLine("Akiled Emulador -> Activo! (" + TimeUsed.Seconds + " s, " + TimeUsed.Milliseconds + " ms)");
                if (Debugger.IsAttached)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Logging.WriteLine("Server is debugging: Activo!");
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else
                {
                    Logging.WriteLine("Server is not debugging: Inactivo");
                    Logging.DisablePrimaryWriting(false);
                }
            }
            catch (KeyNotFoundException ex)
            {
                Logging.WriteLine("Please check your configuration file - some values appear to be missing.");
                Logging.WriteLine("Press any key to shut down ...");
                Logging.WriteLine(ex.ToString());
                Console.ReadKey(true);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Logging.WriteLine("Failed to initialize AkiledEmulator: " + ex.Message);
                Logging.WriteLine("Press any key to shut down ...");
                Console.ReadKey(true);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error during startup: " + ex.ToString());
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
                Environment.Exit(1);
                return false;
            }
            return true;
        }


        public static bool EnumToBool(string Enum) => Enum == "1";

        public static Encoding GetDefaultEncoding() => _defaultEncoding;

        public static string BoolToEnum(bool Bool) => Bool ? "1" : "0";


        public static int GetRandomNumber(int Min, int Max) => Random.Next(Min, Max + 1);

        public static int GetRandomNumberMulti(int Min, int Max) => RandomNumber.GenerateLockedRandom(Min, Max);

        public static int GetUnixTimestamp() => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        internal static int GetIUnixTimestamp() => Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);

        public static FigureDataManager GetFigureManager() => AkiledEnvironment._figureManager;

        private static bool IsValid(char character) => Allowedchars.Contains(character);

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr))
                return false;
            for (int index = 0; index < inputStr.Length; ++index)
            {
                if (!AkiledEnvironment.IsValid(inputStr[index]))
                    return false;
            }
            return true;
        }

        public static bool UsernameExists(string username)
        {
            int integer;
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
                queryReactor.AddParameter(nameof(username), (object)username);
                integer = queryReactor.GetInteger();
            }
            return integer > 0;
        }

        public static string GetUsernameById(int UserId)
        {
            string str = "Unknown User";
            GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (clientByUserId != null && clientByUserId.GetHabbo() != null)
                return clientByUserId.GetHabbo().Username;
            if (AkiledEnvironment._usersCached.ContainsKey(UserId))
                return AkiledEnvironment._usersCached[UserId].Username;
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT `username` FROM `users` WHERE `id` = @id LIMIT 1");
                queryReactor.AddParameter("id", (object)UserId);
                str = queryReactor.GetString();
            }
            if (string.IsNullOrEmpty(str))
                str = "Unknown User";
            return str;
        }

        public static Habbo GetHabboByUsername(string UserName)
        {
            try
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT `id` FROM `users` WHERE `username` = @user LIMIT 1");
                    queryReactor.AddParameter("user", (object)UserName);
                    int integer = queryReactor.GetInteger();
                    if (integer > 0)
                        return AkiledEnvironment.GetHabboById(Convert.ToInt32(integer));
                }
                return (Habbo)null;
            }
            catch
            {
                return (Habbo)null;
            }
        }

        public static Habbo GetHabboById(int UserId)
        {
            try
            {
                GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (clientByUserId != null)
                {
                    Habbo habbo = clientByUserId.GetHabbo();
                    if (habbo != null && habbo.Id > 0)
                    {
                        if (AkiledEnvironment._usersCached.ContainsKey(UserId))
                            AkiledEnvironment._usersCached.TryRemove(UserId, out habbo);
                        return habbo;
                    }
                }
                else
                {
                    try
                    {
                        if (AkiledEnvironment._usersCached.ContainsKey(UserId))
                            return AkiledEnvironment._usersCached[UserId];
                        Akiled.HabboHotel.Users.UserData.UserData userData = UserDataFactory.GetUserData(UserId);
                        if (userData != null)
                        {
                            Habbo user = userData.user;
                            if (user != null)
                            {
                                AkiledEnvironment._usersCached.TryAdd(UserId, user);
                                return user;
                            }
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static LanguageManager GetLanguageManager() => AkiledEnvironment._languageManager;

        public static ConfigurationData GetConfig()
        => AkiledEnvironment._configuration;

        public static ConnectionHandeling GetConnectionManager() => AkiledEnvironment._connectionManager;

        public static RCONSocket GetRCONSocket() => AkiledEnvironment._rcon;

        public static Game GetGame() => AkiledEnvironment._game;

        public static DatabaseManager GetDatabaseManager() => AkiledEnvironment._datebasemanager;

        public static ICollection<Habbo> GetUsersCached() => AkiledEnvironment._usersCached.Values;

        public static bool RemoveFromCache(int Id, out Habbo Data) => AkiledEnvironment._usersCached.TryRemove(Id, out Data);

        internal static void PreformShutDown() => PreformShutDown(false);


        internal static void PreformRestart() => PreformShutDown(true);

        public static void PreformShutDown(bool restart)
        {
            StringBuilder builder = new StringBuilder();
            DateTime now1 = DateTime.Now;
            DateTime now2 = DateTime.Now;
            ServerPacket Packet = new ServerPacket(3801);
            Packet.WriteString("<b><font color=\"#ba3733\" size=\"14\">Hotel Apagado</font></b><br><br>El hotel se reiniciará en 10 segundos. Disculpe las molestias. <br> Gracias por visitarnos, volveremos en unos 5 minutos.</br>");
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(Packet, "");
            Thread.Sleep(10000);
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now2, "Hotel pre-warning");
            Console.Write("Game loop stopped");
            DateTime now3 = DateTime.Now;
            Console.WriteLine("Server shutting down...");
            Console.Title = "<<- SERVER SHUTDOWN ->>";
            AkiledEnvironment.GetConnectionManager().destroy();
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now3, "Socket close");
            DateTime now4 = DateTime.Now;
            Console.WriteLine("<<- SERVER SHUTDOWN ->> ROOM SAVE");
            AkiledEnvironment._game.GetRoomManager().RemoveAllRooms();
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now4, "Room destructor");
            DateTime now5 = DateTime.Now;
            AkiledEnvironment.GetGame().GetClientManager().CloseAll();
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now5, "Furni pre-save and connection close");
            DateTime now6 = DateTime.Now;
            if (AkiledEnvironment._connectionManager != null)
                AkiledEnvironment._connectionManager.destroy();
            if (AkiledEnvironment._webSocketManager != null)
                AkiledEnvironment._webSocketManager.Destroy();
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now6, "Connection shutdown");
            DateTime now7 = DateTime.Now;
            AkiledEnvironment._game.Destroy();
            AkiledEnvironment.AppendTimeStampWithComment(ref builder, now7, "Game destroy");
            DateTime now8 = DateTime.Now;
            TimeSpan span = DateTime.Now - now1;
            builder.AppendLine("Total time on shutdown " + AkiledEnvironment.TimeSpanToString(span));
            builder.AppendLine("You have reached ==> [END OF SESSION]");
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine();
            Logging.LogShutdown(builder);
            Console.WriteLine("System disposed, goodbye!");
            if (restart) Process.Start(Assembly.GetEntryAssembly().Location);
            if (!restart)
                return;
            Environment.Exit(Environment.ExitCode);
        }

        public static string TimeSpanToString(TimeSpan span) => span.Seconds.ToString() + " s, " + (object)span.Milliseconds + " ms";

        public static void AppendTimeStampWithComment(
          ref StringBuilder builder,
          DateTime time,
          string text) => builder.AppendLine(text + " =>[" + AkiledEnvironment.TimeSpanToString(DateTime.Now - time) + "]");
    }
}
