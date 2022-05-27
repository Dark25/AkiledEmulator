using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay.Player;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Givemoney : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 3)                return;

            if (!Room.IsRoleplay)
                return;            RolePlayer Rp = UserRoom.Roleplayer;
            if (Rp == null)
                return;            if (Rp.Dead || Rp.SendPrison)                return;            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Params[1].ToString());            if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetHabbo() == null)                return;            int NumberMoney = 0;            if (!int.TryParse(Params[2].ToString(), out NumberMoney))                return;            if (NumberMoney <= 0)                return;            RolePlayer RpTwo = TargetRoomUser.Roleplayer;
            if (RpTwo == null)
                return;            if (TargetRoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)                return;            if (RpTwo.Dead || RpTwo.SendPrison)                return;            if (Rp.Money < NumberMoney)                return;            if (!((Math.Abs((TargetRoomUser.X - UserRoom.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - UserRoom.Y)) >= 2)))            {                Rp.Money -= NumberMoney;                RpTwo.Money += NumberMoney;                Rp.SendUpdate();                RpTwo.SendUpdate();                TargetRoomUser.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.receive", TargetRoomUser.GetClient().Langue), NumberMoney, UserRoom.GetUsername()));                UserRoom.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send", Session.Langue), NumberMoney, TargetRoomUser.GetUsername()));                UserRoom.OnChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send.chat", Session.Langue), TargetRoomUser.GetUsername()), 0, true);            }
        }
    }
}
