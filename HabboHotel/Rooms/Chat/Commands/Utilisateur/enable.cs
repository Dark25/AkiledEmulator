using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class enable : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;
            int NumEnable;
            if (!int.TryParse(Params[1], out NumEnable))
                return;
            if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
                return;

            int CurrentEnable = UserRoom.CurrentEffect;
            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 184)
                return;

            UserRoom.ApplyEffect(NumEnable);
        }
    }
}
