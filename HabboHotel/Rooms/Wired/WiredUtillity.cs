using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.Wired
{
    public class WiredUtillity
    {
        public static bool TypeIsWiredTrigger(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.triggercollision:
                case InteractionType.triggertimer:
                case InteractionType.triggerroomenter:
                case InteractionType.triggergameend:
                case InteractionType.triggergamestart:
                case InteractionType.triggerrepeater:
                case InteractionType.triggerrepeaterlong:
                case InteractionType.triggeronusersay:
                case InteractionType.triggercommand:
                case InteractionType.triggercollisionuser:
                case InteractionType.triggerscoreachieved:
                case InteractionType.triggerstatechanged:
                case InteractionType.triggerwalkonfurni:
                case InteractionType.triggerwalkofffurni:
                case InteractionType.wf_trg_bot_reached_avtr:
                case InteractionType.wf_trg_bot_reached_stf:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWiredAction(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.actiongivescore:
                case InteractionType.actionposreset:
                case InteractionType.actionmoverotate:
                case InteractionType.actionresettimer:
                case InteractionType.actionshowmessage:
                case InteractionType.highscore:
                case InteractionType.highscorepoints:
                case InteractionType.superwired:
                case InteractionType.actionkickuser:
                case InteractionType.actionteleportto:
                case InteractionType.wf_act_endgame_team:
                case InteractionType.actiontogglestate:
                case InteractionType.wf_act_call_stacks:
                case InteractionType.actionflee:
                case InteractionType.actionchase:
                case InteractionType.collisioncase:
                case InteractionType.collisionteam:
                case InteractionType.actiongivereward:
                case InteractionType.actionmovetodir:
                case InteractionType.wf_act_bot_clothes:
                case InteractionType.wf_act_bot_teleport:
                case InteractionType.wf_act_bot_follow_avatar:
                case InteractionType.wf_act_bot_give_handitem:
                case InteractionType.wf_act_bot_move:
                case InteractionType.wf_act_user_move:
                case InteractionType.wf_act_bot_talk_to_avatar:
                case InteractionType.wf_act_bot_talk:
                case InteractionType.wf_act_join_team:
                case InteractionType.wf_act_leave_team:
                case InteractionType.wf_act_give_score_tm:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWiredCondition(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.conditionfurnishaveusers:
                case InteractionType.conditionfurnishavenousers:
                case InteractionType.conditionstatepos:
                case InteractionType.wf_cnd_stuff_is:
                case InteractionType.wf_cnd_not_stuff_is:
                case InteractionType.conditionstateposNegative:
                case InteractionType.conditiontimelessthan:
                case InteractionType.conditiontimemorethan:
                case InteractionType.conditiontriggeronfurni:
                case InteractionType.conditiontriggeronfurniNegative:
                case InteractionType.conditionhasfurnionfurni:
                case InteractionType.conditionhasfurnionfurniNegative:
                case InteractionType.conditionactoringroup:
                case InteractionType.conditionnotingroup:
                case InteractionType.superwiredcondition:
                case InteractionType.wf_cnd_has_handitem:
                case InteractionType.wf_cnd_actor_in_team:
                case InteractionType.wf_cnd_not_in_team:
                case InteractionType.wf_cnd_not_user_count:
                case InteractionType.wf_cnd_user_count_in:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TypeIsWired(InteractionType type)
        {
            if (TypeIsWiredTrigger(type))
                return true;
            else if (TypeIsWiredAction(type))
                return true;
            else if (TypeIsWiredCondition(type))
                return true;
            else if (type == InteractionType.specialrandom)
                return true;
            else if (type == InteractionType.specialunseen)
                return true;
            else
                return false;
        }

        public static void SaveTriggerItem(IQueryAdapter dbClient, int triggerID, string triggerData2, string triggerData, bool allUsertriggerable, List<Item> itemslist)
        {
            string triggersitem = "";

            if (itemslist != null)
            {
                int i = 0;
                foreach (Item item in itemslist)
                {
                    if (i != 0)
                        triggersitem += ";";

                    triggersitem += item.Id;

                    i++;
                }
            }

            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = " + triggerID);
            dbClient.SetQuery("INSERT INTO wired_items (trigger_id,trigger_data,trigger_data_2,all_user_triggerable,triggers_item) VALUES (@id,@trigger_data,@trigger_data_2,@triggerable,@triggers_item)");
            dbClient.AddParameter("id", triggerID);
            dbClient.AddParameter("trigger_data", triggerData);
            dbClient.AddParameter("trigger_data_2", triggerData2);
            dbClient.AddParameter("triggerable", (allUsertriggerable ? 1 : 0));
            dbClient.AddParameter("triggers_item", triggersitem);
            dbClient.RunQuery();
        }
    }
}
