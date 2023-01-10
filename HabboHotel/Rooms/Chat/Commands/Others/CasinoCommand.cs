using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class CasinoCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, usted debe especificar si quiere activar el modo casino o dar pl! Escriba :casino start o :casino pl", 34);
                return;
            }
            string query = Params[1];

            RoomUser roomUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
            {
                return;
            }

            List<Items.Item> userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
                x.Coordinate, roomUser.Coordinate) && x.Data.InteractionType == Items.InteractionType.dice).ToList();

            if (userBooth.Count != 5)
            {
                Session.SendWhisper("Usted debe tener 5 dados para activar el contador", 34);
                return;
            }

            if (query == "pl" || query == "PL")
            {
                UserRoom.SendWhisperChat("El usuari@ " + Session.GetHabbo().Username + " tiro " + Session.GetHabbo().casinoCount + " en los dados (PL Automático)");
                Session.GetHabbo().casinoEnabled = false;
                Session.GetHabbo().casinoCount = 0;
            }
            else if (query == "start" || query == "START")
            {
                Session.SendWhisper("Usted inicio el modo casino. El contador de dados esta activo", 34);
                Session.GetHabbo().casinoEnabled = true;

            }
            else
            {
                Session.SendWhisper("Oops, usted debe especificar si quiere activar el modo casino o dar pl! Escriba :casino start o :casino pl", 34);
            }

        }
    }
}
