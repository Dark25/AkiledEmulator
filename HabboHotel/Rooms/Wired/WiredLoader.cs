using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Conditions;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.Wired
{
    public class WiredLoader
    {
        public static void LoadWiredItem(Item item, Room room, IQueryAdapter dbClient)
        {
            IWired handler = null;
            switch (item.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.triggertimer:
                    handler = new Timer(item, room.GetWiredHandler(), 2, room.GetGameManager());
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggerroomenter:
                    handler = new EntersRoom(item, room.GetWiredHandler(), room.GetRoomUserManager(), false, string.Empty);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggercollision:
                    handler = new Collision(item, room.GetWiredHandler(), room.GetRoomUserManager());
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggergameend:
                    HandleItemLoad(new GameEnds(item, room.GetWiredHandler(), room.GetGameManager()), item);
                    break;
                case InteractionType.triggergamestart:
                    HandleItemLoad(new GameStarts(item, room.GetWiredHandler(), room.GetGameManager()), item);
                    break;
                case InteractionType.triggerrepeater:
                    handler = new Repeater(room.GetWiredHandler(), item, 0);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggerrepeaterlong:
                    handler = new Repeaterlong(room.GetWiredHandler(), item, 0);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggeronusersay:
                    handler = new UserSays(item, room.GetWiredHandler(), false, string.Empty, room);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.triggercommand:
                    handler = new UserCommand(item, room.GetWiredHandler(), room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_trg_bot_reached_avtr:
                    handler = new BotReadchedAvatar(item, room.GetWiredHandler(), "");
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.triggercollisionuser:
                    handler = new UserCollision(item, room.GetWiredHandler(), room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.triggerscoreachieved:
                    handler = new ScoreAchieved(item, room.GetWiredHandler(), 0, room.GetGameManager());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.triggerstatechanged:
                    handler = new SateChanged(room.GetWiredHandler(), item, new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.triggerwalkonfurni:
                    handler = new WalksOnFurni(item, room.GetWiredHandler(), new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.triggerwalkofffurni:
                    handler = new WalksOffFurni(item, room.GetWiredHandler(), new List<Item>(), 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                #endregion
                #region Action
                case InteractionType.actiongivescore:
                    handler = new GiveScore(0, 0, room.GetGameManager(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_give_score_tm:
                    handler = new GiveScoreTeam(0, 0, 0, room.GetGameManager(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionposreset:
                    handler = new PositionReset(new List<Item>(), 0, room.GetRoomItemHandler(), room.GetWiredHandler(), item.Id, 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionmoverotate:
                    handler = new MoveRotate(MovementState.none, RotationState.none, new List<Item>(), 0, room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionresettimer:
                    handler = new TimerReset(room, room.GetWiredHandler(), 1, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.highscore:
                    handler = new HighScore(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.highscorepoints:
                    handler = new HighScorePoints(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionshowmessage:
                    handler = new ShowMessage(string.Empty, room.GetWiredHandler(), item.Id, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actiongivereward:
                    //handlergr = (IWiredTrigger) new GiveReward(string.Empty, room.GetWiredHandler(), item.Id);
                    //handlergr.LoadFromDatabase(dbClient, room);
                    //WiredLoader.HandleItemLoad(handlergr, item);
                    break;
                case InteractionType.superwired:
                    handler = new SuperWired(string.Empty, 0, false, false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionteleportto:
                    handler = new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_endgame_team:
                    handler = new TeamGameOver(1, item.Id, room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actiontogglestate:
                    handler = new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), new List<Item>(), 0, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_call_stacks:
                    handler = new ExecutePile(new List<Item>(), 0, room.GetWiredHandler(), item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionkickuser:
                    handler = new KickUser(string.Empty, room.GetWiredHandler(), item.Id, room);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionflee:
                    handler = new Escape(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionchase:
                    handler = new Chase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.collisionteam:
                    handler = new CollisionTeam(1, room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.collisioncase:
                    handler = new CollisionCase(new List<Item>(), room, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.actionmovetodir:
                    handler = new MoveToDir(new List<Item>(), room, room.GetWiredHandler(), item.Id, MovementDirection.up, WhenMovementBlock.none);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_clothes:
                    handler = new BotClothes("", "", room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_teleport:
                    handler = new BotTeleport("", new List<Item>(), room.GetGameMap(), room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_follow_avatar:
                    handler = new BotFollowAvatar("", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_give_handitem:
                    handler = new BotGiveHanditem("", room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_move:
                    handler = new BotMove("", new List<Item>(), room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;

                case InteractionType.wf_act_user_move:
                    handler = new UserMove(new List<Item>(), 0, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_talk_to_avatar:
                    handler = new BotTalkToAvatar("", "", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_bot_talk:
                    handler = new BotTalk("", "", false, room.GetWiredHandler(), item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_leave_team:
                    handler = new TeamLeave(item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_act_join_team:
                    handler = new TeamJoin(1, item.Id);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                #endregion
                #region Condition
                case InteractionType.superwiredcondition:
                    handler = (IWiredCondition)new SuperWiredCondition(item, string.Empty, false);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionfurnishaveusers:
                    handler = (IWiredCondition)new FurniHasUser(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionfurnishavenousers:
                    handler = (IWiredCondition)new FurniHasNoUser(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionstatepos:
                    handler = (IWiredCondition)new FurniStatePosMatch(item, new List<Item>(), 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_cnd_stuff_is:
                    handler = (IWiredCondition)new FurniStuffIs(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_cnd_not_stuff_is:
                    handler = (IWiredCondition)new FurniNotStuffIs(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionstateposNegative:
                    handler = (IWiredCondition)new FurniStatePosMatchNegative(item, new List<Item>(), 0, 0, 0);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditiontimelessthan:
                    handler = (IWiredCondition)new LessThanTimer(0, room, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditiontimemorethan:
                    handler = (IWiredCondition)new MoreThanTimer(0, room, item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditiontriggeronfurni:
                    handler = (IWiredCondition)new TriggerUserIsOnFurni(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditiontriggeronfurniNegative:
                    handler = (IWiredCondition)new TriggerUserIsOnFurniNegative(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionhasfurnionfurni:
                    handler = (IWiredCondition)new HasFurniOnFurni(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionhasfurnionfurniNegative:
                    handler = (IWiredCondition)new HasFurniOnFurniNegative(item, new List<Item>());
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionactoringroup:
                    handler = (IWiredCondition)new HasUserInGroup(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.conditionnotingroup:
                    handler = (IWiredCondition)new HasUserNotInGroup(item);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_cnd_actor_in_team:
                    handler = (IWiredCondition)new ActorInTeam(item.Id, 1);
                    handler.LoadFromDatabase(dbClient, room);

                    break;
                case InteractionType.wf_cnd_not_in_team:
                    handler = (IWiredCondition)new ActorNotInTeam(item.Id, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.wf_cnd_not_user_count:
                    handler = (IWiredCondition)new RoomUserNotCount(item, 1, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                case InteractionType.wf_cnd_user_count_in:
                    handler = (IWiredCondition)new RoomUserCount(item, 1, 1);
                    handler.LoadFromDatabase(dbClient, room);
                    break;
                    #endregion
            }

            if (handler != null) HandleItemLoad(handler, item);
        }

        private static void HandleItemLoad(IWired handler, Item item)
        {
            if (item.WiredHandler != null) item.WiredHandler.Dispose();

            item.WiredHandler = handler;
        }
    }
}
