namespace Akiled.HabboHotel.Rooms.Chat.Pets.Commands
{
    public struct PetCommand
    {
        public int commandID;
        public string commandInput;

        public PetCommand(int commandID, string commandInput)
        {
            this.commandID = commandID;
            this.commandInput = commandInput;
        }
    }
}
