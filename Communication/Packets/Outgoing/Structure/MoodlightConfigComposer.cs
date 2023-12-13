using Akiled.HabboHotel.Items;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MoodlightConfigComposer : ServerPacket
    {
        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.MoodlightConfigMessageComposer)
        {
            WriteInteger(MoodlightData.Presets.Count);
            WriteInteger(MoodlightData.CurrentPreset);

            var i = 0;
            foreach (var moodlightPreset in MoodlightData.Presets)
            {
                i++;
                this.WriteInteger(i);
                this.WriteInteger((moodlightPreset.BackgroundOnly ? 1 : 0) + 1);
                this.WriteString(moodlightPreset.ColorCode);
                this.WriteInteger(moodlightPreset.ColorIntensity);
            }
        }
    }
}
