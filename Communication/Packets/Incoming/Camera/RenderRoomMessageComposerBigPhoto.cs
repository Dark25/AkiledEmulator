
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.Communication.Packets.Incoming.Camera
{
  public class RenderRoomMessageComposerBigPhoto : IPacketEvent
  {
    public void Parse(GameClient Session, ClientPacket paket)
    {
      Console.WriteLine("packet recv");
            string json = Akiled.Communication.Packets.Incoming.Camera.Camera.Decompiler(paket.ReadBytes(paket.PopInt()));
            string dataFromJson = URLPost.GetDataFromJSON(json, "roomid");
      double num = double.Parse(URLPost.GetDataFromJSON(json, "timestamp"));
      string str = (num - num % 100.0).ToString();
      Session.GetHabbo().lastPhotoPreview = dataFromJson + "-" + str;
      Console.WriteLine("DEBUG: " + URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview));
      Session.SendMessage((IServerPacket) new CameraPhotoPreviewComposer("photos/" + URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview) + ".png"));
    }
  }
}
