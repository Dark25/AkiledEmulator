using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class PetHorseFigureInformationComposer : ServerPacket
    {
        public PetHorseFigureInformationComposer(RoomUser PetUser)
            : base(ServerPacketHeader.PetHorseFigureInformationMessageComposer)
        {
            WriteInteger(PetUser.PetData.VirtualId);
            WriteInteger(PetUser.PetData.PetId);
            WriteInteger(PetUser.PetData.Type);
            WriteInteger(int.Parse(PetUser.PetData.Race));
            WriteString(PetUser.PetData.Color.ToLower());
            WriteInteger(1);
            if (PetUser.PetData.Saddle > 0)
            {
                WriteInteger(3); //Count

                WriteInteger(2);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);

                WriteInteger(3);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);

                WriteInteger(4);
                WriteInteger(PetUser.PetData.Saddle);
                WriteInteger(0);
            }
            else
            {

                WriteInteger(2); //Count

                WriteInteger(2);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);

                WriteInteger(3);
                WriteInteger(PetUser.PetData.PetHair);
                WriteInteger(PetUser.PetData.HairDye);
            }
            WriteBoolean(PetUser.PetData.Saddle > 0);
            WriteBoolean(PetUser.RidingHorse);
        }
    }
}
