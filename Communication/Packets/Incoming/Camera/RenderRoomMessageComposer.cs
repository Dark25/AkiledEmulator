using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Rooms.Camera
{
    public class RenderRoomMessageComposer : ServerPacket
    {
        public RenderRoomMessageComposer()
            : base(ServerPacketHeader.TakedRoomPhoto)
        {

        }
    }

    public class RenderRoomMessageComposerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            Session.SendMessage(new RenderRoomMessageComposer());
        }
    }
}