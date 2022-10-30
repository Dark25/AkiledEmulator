using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AvatarEffectActivatedEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int NumEnable = Packet.PopInt();

            if (NumEnable < 0)
                return;

            if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            int CurrentEnable = User.CurrentEffect;
            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 37 || CurrentEnable == 184 || CurrentEnable == 77 || CurrentEnable == 103
                || CurrentEnable == 40 || CurrentEnable == 41 || CurrentEnable == 42 || CurrentEnable == 43
                || CurrentEnable == 49 || CurrentEnable == 50 || CurrentEnable == 51 || CurrentEnable == 52
                || CurrentEnable == 33 || CurrentEnable == 34 || CurrentEnable == 35 || CurrentEnable == 36)
                return;

            if (User.Team != Team.none || User.InGame)
                return;

            User.ApplyEffect(NumEnable);
        }
    }
}