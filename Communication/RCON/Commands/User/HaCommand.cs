using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.RCON.Commands.User
{
    class HaCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
                return false;

            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
                return false;

            string Message = parameters[2];

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
            if (Client.Antipub(Message, "<alert>"))
                return false;

            ServerPacket message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);
            message.WriteString(AkiledEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Client.Langue) + "\r\n" + Message + "\r\n- " + Client.GetHabbo().Username);
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(message);
            return true;
        }
    }
}
