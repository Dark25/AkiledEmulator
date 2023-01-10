using Akiled.HabboHotel.GameClients;using System.Linq;namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class roomenable : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            int NumEnable;            if (!int.TryParse(Params[1], out NumEnable))                return;

            if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
                return;

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())            {
                if (!User.IsBot)
                {
                    User.ApplyEffect(NumEnable);
                }            }        }    }}