

using Akiled.Communication.Packets.Incoming.Rooms.Camera;
using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Camera
{
  public class RenderRoomMessageComposerEvent : IPacketEvent
  {
    public void Parse(GameClient Session, ClientPacket paket) => Session.SendMessage((IServerPacket) new RenderRoomMessageComposer());
  }
}
