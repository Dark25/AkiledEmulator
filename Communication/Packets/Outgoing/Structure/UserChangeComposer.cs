using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            WriteInteger((Self) ? -1 : User.VirtualId);
            WriteString(User.GetClient().GetHabbo().Look);
            WriteString(User.GetClient().GetHabbo().Gender);
            WriteString(User.GetClient().GetHabbo().Motto);
            WriteInteger(User.GetClient().GetHabbo().AchievementPoints);
        }

        public UserChangeComposer(RoomUser User) //Bot
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            WriteInteger(User.VirtualId);
            WriteString(User.BotData.Look);
            WriteString(User.BotData.Gender);
            WriteString(User.BotData.Motto);
            WriteInteger(0);
        }

        public UserChangeComposer(GameClient Client)
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            WriteInteger(-1);
            WriteString(Client.GetHabbo().Look);
            WriteString(Client.GetHabbo().Gender);
            WriteString(Client.GetHabbo().Motto);
            WriteInteger(Client.GetHabbo().AchievementPoints);
        }
    }
}
