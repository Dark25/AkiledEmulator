using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SetEffect : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            int.TryParse(Params[1], out int EnableNum);

            if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(EnableNum, Session.GetHabbo().HasFuse("fuse_sysadmin")))
                return;

            UserRoom.ApplyEffect(EnableNum);
        }
    }
}
