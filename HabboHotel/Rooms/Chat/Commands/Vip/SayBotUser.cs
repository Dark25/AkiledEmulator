
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class SayBotUser : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
                Session.SendWhisper("Oops, introduce el nombre del bot, seguido del mensaje que quieres que diga.", 34);
            else if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, solo el dueño de la sala puede ejecutar este comando.", 34);
            }
            else
            {
                string name = Params[1];
                RoomUser botOrPetByName = Room.GetRoomUserManager().GetBotOrPetByName(name);
                if (botOrPetByName == null)
                    return;
                string MessageText = CommandManager.MergeParams(Params, 2);
                if (string.IsNullOrEmpty(MessageText))
                    return;
                botOrPetByName.OnChat(MessageText, botOrPetByName.IsPet ? 0 : 2);
            }
        }
    }
}
