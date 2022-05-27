using Akiled.HabboHotel.Groups;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ManageGroupComposer : ServerPacket
    {
        public ManageGroupComposer(Group Group, string[] BadgeParts)
            : base(ServerPacketHeader.ManageGroupMessageComposer)
        {
            WriteInteger(0);
            WriteBoolean(true);
            WriteInteger(Group.Id);
            WriteString(Group.Name);
            WriteString(Group.Description);
            WriteInteger(1);
            WriteInteger(Group.Colour1);
            WriteInteger(Group.Colour2);
            WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            WriteInteger(Group.AdminOnlyDeco);
            WriteBoolean(false);
            WriteString("");

            WriteInteger(5);

            for (int x = 0; x < BadgeParts.Length; x++)
            {
                string symbol = BadgeParts[x];
                int symbolInt = 0;

                this.WriteInteger((symbol.Length >= 6) ? (int.TryParse(symbol.Substring(0, 3), out symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(0, 2), out symbolInt)) ? symbolInt : 0);
                this.WriteInteger((symbol.Length >= 6) ? (int.TryParse(symbol.Substring(3, 2), out symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(2, 2), out symbolInt)) ? symbolInt : 0);
                this.WriteInteger(symbol.Length < 5 ? 0 : symbol.Length >= 6 ? (int.TryParse(symbol.Substring(5, 1), out symbolInt)) ? symbolInt : 0 : (int.TryParse(symbol.Substring(4, 1), out symbolInt)) ? symbolInt : 0);
            }

            int i = 0;
            while (i < (5 - BadgeParts.Length))
            {
                WriteInteger(0);
                WriteInteger(0);
                WriteInteger(0);
                i++;
            }

            WriteString(Group.Badge);
            WriteInteger(Group.MemberCount);
        }
    }
}
