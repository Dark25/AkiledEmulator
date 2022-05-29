

using Akiled.Communication.Packets.Outgoing;

namespace Akiled.Communication.Packets.Incoming.Camera
{
  public class RenderRoomMessageComposer : ServerPacket
  {
    public RenderRoomMessageComposer()
      : base(ServerPacketHeader.TakedRoomPhoto)
    {
    }
  }
}
