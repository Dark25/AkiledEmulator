namespace Akiled.HabboHotel.Quests
{
    public class QuestTypeUtillity
    {
        public static string GetString(QuestType type)
        {
            switch (type)
            {
                case QuestType.FURNI_MOVE:
                    return "MOVE_ITEM";
                case QuestType.FURNI_ROTATE:
                    return "ROTATE_ITEM";
                case QuestType.FURNI_PLACE:
                    return "PLACE_ITEM";
                case QuestType.FURNI_PICK:
                    return "PICKUP_ITEM";
                case QuestType.FURNI_SWITCH:
                    return "SWITCH_ITEM_STATE";
                case QuestType.FURNI_STACK:
                    return "STACK_ITEM";
                case QuestType.FURNI_DECORATION_FLOOR:
                    return "PLACE_FLOOR";
                case QuestType.FURNI_DECORATION_WALL:
                    return "PLACE_WALLPAPER";
                case QuestType.SOCIAL_VISIT:
                    return "ENTER_OTHERS_ROOM";
                case QuestType.SOCIAL_CHAT:
                    return "CHAT_WITH_SOMEONE";
                case QuestType.SOCIAL_FRIEND:
                    return "REQUEST_FRIEND";
                case QuestType.SOCIAL_RESPECT:
                    return "GIVE_RESPECT";
                case QuestType.SOCIAL_DANCE:
                    return "DANCE";
                case QuestType.SOCIAL_WAVE:
                    return "WAVE";
                case QuestType.PROFILE_CHANGE_LOOK:
                    return "CHANGE_FIGURE";
                case QuestType.PROFILE_CHANGE_MOTTO:
                    return "CHANGE_MOTTO";
                case QuestType.PROFILE_BADGE:
                    return "WEAR_BADGE";
                case QuestType.ROB_USER:
                    return "ROB_USER";
                case QuestType.PLAY_YOUTUBE:
                    return "PLAY_YOUTUBE";
                case QuestType.ROB_KAFFEEHAUS:
                    return "ROB_KAFFEEHAUS";
                case QuestType.WIN_BUCHSTABENSALAT:
                    return "WIN_BUCHSTABENSALAT";
                case QuestType.SLIME_OTHERS:
                    return "SLIME_OTHERS";
                case QuestType.FIND_POKEBALL_GOLD:
                    return "FIND_POKEBALL_GOLD";
                case QuestType.ICE_25_USERS:
                    return "ICE_25_USERS";
                case QuestType.KILL_10_USERS:
                    return "KILL_10_USERS";
                case QuestType.FISH_3_THUNFISH:
                    return "FISH_3_THUNFISH";
                default:
                    return "FIND_STUFF";
            }
        }
    }
}
