using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class Pyramide : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            foreach (Item Item in Room.GetRoomItemHandler().GetFloor.ToList())
            {
                if (Item == null || Item.GetBaseItem() == null)
                    continue;

                if (Item.GetBaseItem().ItemName != "wf_pyramid")
                    continue;

                Item.ExtraData = (Item.ExtraData == "0") ? "1" : "0";
                Item.UpdateState();
                Item.GetRoom().GetGameMap().updateMapForItem(Item);
            }            UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.pyramide", Session.Langue));        }    }}