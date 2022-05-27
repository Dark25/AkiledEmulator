using Akiled.HabboHotel.Pets;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class PetTrainingPanelComposer : ServerPacket
    {
        public PetTrainingPanelComposer(Pet petData)
            : base(ServerPacketHeader.PetTrainingPanelMessageComposer)
        {
            WriteInteger(petData.PetId);

            List<short> AvailableCommands = new List<short>();

            WriteInteger(petData.PetCommands.Count);
            foreach (short Sh in petData.PetCommands.Keys)
            {
                WriteInteger(Sh);
                if (petData.PetCommands[Sh] == true)
                {
                    AvailableCommands.Add(Sh);
                }
            }

            WriteInteger(AvailableCommands.Count);
            foreach (short Sh in AvailableCommands)
            {
                WriteInteger(Sh);
            }
        }
    }
}
