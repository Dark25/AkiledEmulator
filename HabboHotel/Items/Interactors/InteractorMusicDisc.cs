using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorMusicDisc : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Room room = Item.GetRoom();
            List<Item> avaliableSongs = room.GetTraxManager().GetAvaliableSongs();
            if (!avaliableSongs.Contains(Item) && !room.GetTraxManager().Playlist.Contains(Item))
                avaliableSongs.Add(Item);
            room.SendPacket((IServerPacket)new LoadJukeboxUserMusicItemsComposer((ICollection<Item>)avaliableSongs));
            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_JukeboxPlaceDisc", 1);
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Room room = Item.GetRoom();
            if (Item.GetRoom().GetTraxManager().GetDiscItem(Item.Id) != null)
            {
                room.GetTraxManager().StopPlayList();
                room.GetTraxManager().RemoveDisc(Item);
            }
            List<Item> avaliableSongs = room.GetTraxManager().GetAvaliableSongs();
            avaliableSongs.Remove(Item);
            room.SendPacket((IServerPacket)new LoadJukeboxUserMusicItemsComposer((ICollection<Item>)avaliableSongs));
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            Room room = Item.GetRoom();
            if (Request != 0 && Request != 1)
                return;
            room.GetTraxManager().TriggerPlaylistState();
        }

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
