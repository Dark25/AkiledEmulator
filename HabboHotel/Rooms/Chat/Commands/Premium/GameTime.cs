using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class GameTime : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (AkiledEnvironment.GetGame().GetAnimationManager().IsActivate())
            {
                string Time = AkiledEnvironment.GetGame().GetAnimationManager().GetTime();
                UserRoom.SendWhisperChat("Prochaine animation de Jack & Daisy dans " + Time);
            } else
            {
                UserRoom.SendWhisperChat("Animation de Jack & Daisy désactiver");
            }
        }
    }
}
