using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserPerksComposer : ServerPacket
    {
        public UserPerksComposer(Habbo Habbo)
            : base(ServerPacketHeader.UserPerksMessageComposer)
        {
            WriteInteger(16); // Count
            WriteString("USE_GUIDE_TOOL");
            WriteString("");
            WriteBoolean(Habbo.HasFuse("fuse_helptool"));

            WriteString("GIVE_GUIDE_TOURS");
            WriteString("requirement.unfulfilled.helper_le");
            WriteBoolean(false);

            WriteString("JUDGE_CHAT_REVIEWS");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("VOTE_IN_COMPETITIONS");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("CALL_ON_HELPERS");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("CITIZEN");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("TRADE");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("HEIGHTMAP_EDITOR_BETA");
            WriteString(""); // ??
            WriteBoolean(false);

            WriteString("EXPERIMENTAL_CHAT_BETA");
            WriteString("requirement.unfulfilled.helper_level_2");
            WriteBoolean(true);

            WriteString("EXPERIMENTAL_TOOLBAR");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("BUILDER_AT_WORK");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("NAVIGATOR_PHASE_ONE_2014");
            WriteString(""); // ??
            WriteBoolean(false);

            WriteString("CAMERA");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("NAVIGATOR_PHASE_TWO_2014");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("MOUSE_ZOOM");
            WriteString(""); // ??
            WriteBoolean(true);

            WriteString("NAVIGATOR_ROOM_THUMBNAIL_CAMERA");
            WriteString(""); // ??
            WriteBoolean(true);
        }
    }
}
