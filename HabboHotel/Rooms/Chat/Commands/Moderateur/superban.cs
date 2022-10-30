using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Users;
namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class Superban : IChatCommand    {        public async void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length < 2)                return;            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            Habbo Habbo = AkiledEnvironment.GetHabboByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }            if (Session.Langue != clientByUsername.Langue)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                return;
            }            if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)            {                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));                await AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(Session, "Robot", (double)788922000, "Su cuenta ha sido prohibida por seguridad", false, false);            }            else            {                int num = 788922000;
                if (Params.Length == 3)
                    int.TryParse(Params[2], out num);                if (num <= 600)                    Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));                else                {                    string Raison = CommandManager.MergeParams(Params, 3);
                    string Username = Habbo.Username;
                    await AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(clientByUsername, Session.GetHabbo().Username, (double)num, Raison, false, false).ConfigureAwait(false);                    Session.Antipub(Raison, "<CMD>");
                    AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("baneo", "El usuario: " + clientByUsername.GetHabbo().Username + " ha sido baneado, por favor verifiquen la razon del baneo, para evitar malos entendidos"));
                    Session.SendWhisper("Excelente has baneado la ip del usuario '" + Username + "' por la razon: '" + Raison + "'!");                }                if (clientByUsername.GetHabbo().Rank > 8 && Session.GetHabbo().Rank < 12)
                {
                    await AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(Session, "Robot", (double)788922000, "Su cuenta ha sido prohibida por seguridad", false, false).ConfigureAwait(false);
                }            }        }    }}