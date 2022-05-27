using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(RoomData Room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            WriteInteger(Room.Id);
            WriteString(Room.Name);
            WriteString(Room.Description);
            WriteInteger(Room.State);
            WriteInteger(Room.Category);
            WriteInteger(Room.UsersMax);
            WriteInteger(((Room.Model.MapSizeX * Room.Model.MapSizeY) > 100) ? 50 : 25);

            WriteInteger(Room.Tags.Count);
            foreach (string Tag in Room.Tags.ToArray())
            {
                WriteString(Tag);
            }

            WriteInteger(Room.TrocStatus); //Trade
            WriteInteger(Room.AllowPets ? 1 : 0); // allows pets in room - pet system lacking, so always off
            WriteInteger(Room.AllowPetsEating ? 1 : 0);// allows pets to eat your food - pet system lacking, so always off
            WriteInteger(Room.AllowWalkthrough ? 1 : 0);
            WriteInteger(Room.Hidewall ? 1 : 0);
            WriteInteger(Room.WallThickness);
            WriteInteger(Room.FloorThickness);

            WriteInteger(Room.ChatType);//Chat mode
            WriteInteger(Room.ChatBalloon);//Chat size
            WriteInteger(Room.ChatSpeed);//Chat speed
            WriteInteger(Room.ChatMaxDistance);//Hearing Distance
            WriteInteger(Room.ChatFloodProtection);//Additional Flood

            WriteBoolean(true);

            WriteInteger(Room.MuteFuse); // who can mute
            WriteInteger(Room.WhoCanKick); // who can kick
            WriteInteger(Room.BanFuse); // who can ban
        }
    }
}
