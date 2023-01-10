using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.TraxMachine;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Sound
{
    internal class GetJukeboxDiscsDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int num = Packet.PopInt();
            List<TraxMusicData> Songs = new List<TraxMusicData>();
            while (num-- > 0)
            {
                TraxMusicData music = TraxSoundManager.GetMusic(Packet.PopInt());
                if (music != null)
                    Songs.Add(music);
            }
            if (Session.GetHabbo().CurrentRoom == null)
                return;
            Session.SendMessage((IServerPacket)new SetJukeboxSongMusicDataComposer((ICollection<TraxMusicData>)Songs));
        }
    }
}
