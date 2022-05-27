using Akiled.HabboHotel.Achievements;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AchievementUnlockedMessageComposer : ServerPacket
    {
        public AchievementUnlockedMessageComposer(Achievement Achievement, int Level, int PointReward, int PixelReward)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            WriteInteger(Achievement.Id);
            WriteInteger(Level);
            WriteInteger(144);
            WriteString(Achievement.GroupName + Level);
            WriteInteger(PointReward);
            WriteInteger(PixelReward);
            WriteInteger(0);
            WriteInteger(10);
            WriteInteger(21);
            WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
            WriteString(Achievement.Category);
            WriteString(string.Empty);
        }
    }
}
