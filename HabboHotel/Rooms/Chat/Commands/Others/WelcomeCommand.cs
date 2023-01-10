using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class WelcomeCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            RoomUser user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            RoomUser target = Room.GetRoomUserManager().GetRoomUserByHabbo(Params[1].ToString());
            string name_hotel = (AkiledEnvironment.GetConfig().data["namehotel_text"]);
            if (target.GetClient() == null)
            {
                Session.SendWhisper("Escriba el nombre del usuario que  le quieras dar la bienvenida al hotel.");
                return;
            }
            else
            {
                Room.SendPacket(new ChatComposer(user.VirtualId, " Bienvenido(a) a " + name_hotel + ": Espero que te diviertas aquí y recuerda invitar a tus amig@s por favor. gracias <3", 0, 34));
            }
        }
    }
}
