
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay.Player;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class EnablePvP : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!Room.IsRoleplay || !Room.Pvp || UserRoom.Freeze)
                return;
            RolePlayer roleplayer = UserRoom.Roleplayer;
            if (roleplayer == null || (roleplayer.Dead || roleplayer.SendPrison || Session.GetHabbo().is_angeln))
                return;
            roleplayer.PvpEnable = !roleplayer.PvpEnable;
            if (roleplayer.PvpEnable)
             
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("pvp_on", Session.Langue), 34);
            else
               
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("pvp_off", Session.Langue), 34);
        }
    }
}
