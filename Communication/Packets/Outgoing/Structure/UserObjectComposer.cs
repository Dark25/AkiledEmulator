using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserObjectMessageComposer)
        {
            WriteInteger(Habbo.Id);
            WriteString(Habbo.Username);
            WriteString(Habbo.Look);
            WriteString(Habbo.Gender.ToUpper());
            WriteString(Habbo.Motto);
            WriteString("");
            WriteBoolean(false);
            WriteInteger(Habbo.Respect);
            WriteInteger(Habbo.DailyRespectPoints);
            WriteInteger(Habbo.DailyPetRespectPoints);
            WriteBoolean(false); // Friends stream active
            WriteString(Habbo.LastOnline.ToString()); // last online?
            WriteBoolean(Habbo.Rank > 20 || Habbo.CanChangeName); // Can change name
            WriteBoolean(false);
        }
    }
}
