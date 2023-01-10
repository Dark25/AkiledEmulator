using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.TraxMachine;

namespace Akiled.Communication.Packets.Incoming.Sound
{
    internal class RemoveDiscFromPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (!currentRoom.CheckRights(Session))
                return;
            int index = Packet.PopInt();
            RoomTraxManager traxManager = currentRoom.GetTraxManager();
            if (traxManager.Playlist.Count >= index)
            {
                Item obj = traxManager.Playlist[index];
                if (traxManager.RemoveDisc(obj))
                    return;
            }
            Session.SendMessage((IServerPacket)new RoomNotificationComposer("", "Oeps! Haal het oude item eerst uit de Jukebox.", "error", "", ""));
        }
    }
}
