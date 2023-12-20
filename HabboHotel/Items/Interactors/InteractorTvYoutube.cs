using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorTvYoutube : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (Session.GetHabbo().SendWebPacket(new YoutubeTvComposer((UserHasRights) ? Item.Id : 0, Item.ExtraData)))
                return;

            if (!UserHasRights)
                return;

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;
            if (string.IsNullOrEmpty(roomUser.LoaderVideoId) && string.IsNullOrEmpty(Item.ExtraData))
            {
                roomUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("item.tpyoutubehelp", Session.Langue));
            }

            if (!string.IsNullOrEmpty(roomUser.LoaderVideoId) && roomUser.LoaderVideoId != Item.ExtraData)
            {
                Item.ExtraData = roomUser.LoaderVideoId;
                Item.UpdateState();

                roomUser.LoaderVideoId = "";
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
