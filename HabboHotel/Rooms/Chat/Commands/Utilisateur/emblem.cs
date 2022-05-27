using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class emblem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ADM"))
                UserRoom.CurrentEffect = 102;
            else if (Session.GetHabbo().Isguia || Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAGUIA"))
                UserRoom.CurrentEffect = 187;
            else if (Session.GetHabbo().IsEMB || Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAEMB"))
                UserRoom.CurrentEffect = 178;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAINTER"))
                UserRoom.CurrentEffect = 546;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAGM"))
                UserRoom.CurrentEffect = 570;
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAMOD"))
                UserRoom.CurrentEffect = 552;

            if (UserRoom.CurrentEffect > 0)
                Room.SendPacket(new AvatarEffectComposer(UserRoom.VirtualId, UserRoom.CurrentEffect));
        }
    }
}
