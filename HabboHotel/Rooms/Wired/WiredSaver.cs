using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
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
    public class WiredSaver
    {
        public static void HandleDefaultSave(int itemID, Room room, Item roomItem)
        {
            if (roomItem == null) return;

            switch (roomItem.GetBaseItem().InteractionType)
            {
                #region SaveDefaultTrigger
                case InteractionType.triggertimer:
                    int cycleCount = 0;
                    HandleTriggerSave(new Timer(roomItem, room.GetWiredHandler(), cycleCount, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerroomenter:
                    string userName = string.Empty;
                    HandleTriggerSave(new EntersRoom(roomItem, room.GetWiredHandler(), room.GetRoomUserManager(), !string.IsNullOrEmpty(userName), userName), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercollision:
                    HandleTriggerSave(new Collision(roomItem, room.GetWiredHandler(), room.GetRoomUserManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggergameend:
                    HandleTriggerSave(new GameEnds(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggergamestart:
                    HandleTriggerSave(new GameStarts(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerrepeater:
                    int cyclesRequired = 0;
                    HandleTriggerSave(new Repeater(room.GetWiredHandler(), roomItem, cyclesRequired), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerrepeaterlong:
                    int cyclesRequiredlong = 0;
                    HandleTriggerSave(new Repeaterlong(room.GetWiredHandler(), roomItem, cyclesRequiredlong), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggeronusersay:
                    bool flag = false;
                    string triggerMessage = string.Empty;
                    HandleTriggerSave(new UserSays(roomItem, room.GetWiredHandler(), !flag, triggerMessage, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercommand:
                    HandleTriggerSave(new UserCommand(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_trg_bot_reached_avtr:
                    HandleTriggerSave(new BotReadchedAvatar(roomItem, room.GetWiredHandler(), ""), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercollisionuser:
                    HandleTriggerSave(new UserCollision(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerscoreachieved:
                    int scoreLevel = 0;
                    HandleTriggerSave(new ScoreAchieved(roomItem, room.GetWiredHandler(), scoreLevel, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerstatechanged:
                    List<Item> items1 = new List<Item>();
                    int delay1 = 0;
                    HandleTriggerSave(new SateChanged(room.GetWiredHandler(), roomItem, items1, delay1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerwalkonfurni:
                    List<Item> targetItems1 = new List<Item>();
                    int requiredCycles1 = 0;
                    HandleTriggerSave(new WalksOnFurni(roomItem, room.GetWiredHandler(), targetItems1, requiredCycles1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerwalkofffurni:
                    List<Item> targetItems2 = new List<Item>();
                    int requiredCycles2 = 0;
                    HandleTriggerSave(new WalksOffFurni(roomItem, room.GetWiredHandler(), targetItems2, requiredCycles2), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region SauveDefaultAction
                case InteractionType.actiongivescore:
                    HandleTriggerSave(new GiveScore(0, 0, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_give_score_tm:
                    HandleTriggerSave(new GiveScoreTeam(0, 0, 0, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionposreset:
                    HandleTriggerSave(new PositionReset(new List<Item>(), 0, room.GetRoomItemHandler(), room.GetWiredHandler(), itemID, 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionmoverotate:
                    HandleTriggerSave(new MoveRotate(MovementState.none, RotationState.none, new List<Item>(), 0, room, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionresettimer:
                    int delay2 = 0;
                    HandleTriggerSave(new TimerReset(room, room.GetWiredHandler(), delay2, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.highscore:
                    HandleTriggerSave(new HighScore(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.highscorepoints:
                    HandleTriggerSave(new HighScorePoints(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionshowmessage:
                    HandleTriggerSave(new ShowMessage(string.Empty, room.GetWiredHandler(), itemID, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                //case InteractionType.actiongivereward:
                //WiredSaver.HandleTriggerSave(new GiveReward(string.Empty, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                //break;
                case InteractionType.superwired:
                    HandleTriggerSave(new SuperWired(string.Empty, 0, false, false, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionkickuser:
                    HandleTriggerSave(new KickUser(string.Empty, room.GetWiredHandler(), itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionteleportto:
                    List<Item> items3 = new List<Item>();
                    int delay3 = 0;
                    HandleTriggerSave(new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), items3, delay3, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_endgame_team:
                    List<Item> itemstpteam = new List<Item>();
                    HandleTriggerSave(new TeamGameOver(1, itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actiontogglestate:
                    List<Item> items4 = new List<Item>();
                    int delay4 = 0;
                    HandleTriggerSave(new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), items4, delay4, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_call_stacks:
                    HandleTriggerSave(new ExecutePile(new List<Item>(), 0, room.GetWiredHandler(), roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionflee:
                    List<Item> itemsflee = new List<Item>();
                    HandleTriggerSave(new Escape(itemsflee, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionchase:
                    List<Item> itemschase = new List<Item>();
                    HandleTriggerSave(new Chase(itemschase, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.collisionteam:
                    HandleTriggerSave(new CollisionTeam(1, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.collisioncase:
                    List<Item> itemscollision = new List<Item>();
                    HandleTriggerSave(new CollisionCase(itemscollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionmovetodir:
                    HandleTriggerSave(new MoveToDir(new List<Item>(), room, room.GetWiredHandler(), roomItem.Id, MovementDirection.none, WhenMovementBlock.none), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_clothes:
                    HandleTriggerSave(new BotClothes("", "", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_teleport:
                    HandleTriggerSave(new BotTeleport("", new List<Item>(), room.GetGameMap(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_follow_avatar:
                    HandleTriggerSave(new BotFollowAvatar("", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_give_handitem:
                    HandleTriggerSave(new BotGiveHanditem("", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_move:
                    HandleTriggerSave(new BotMove("", new List<Item>(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_user_move:
                    HandleTriggerSave(new UserMove(new List<Item>(), 0, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_talk_to_avatar:
                    HandleTriggerSave(new BotTalkToAvatar("", "", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_talk:
                    HandleTriggerSave(new BotTalk("", "", false, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_leave_team:
                    HandleTriggerSave(new TeamLeave(itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_join_team:
                    HandleTriggerSave(new TeamJoin(1, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region SaveDefaultCondition
                case InteractionType.superwiredcondition:
                    HandleTriggerSave((IWiredCondition)new SuperWiredCondition(roomItem, "", false), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionfurnishaveusers:
                    HandleTriggerSave((IWiredCondition)new FurniHasUser(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionfurnishavenousers:
                    HandleTriggerSave((IWiredCondition)new FurniHasNoUser(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionstatepos:
                    HandleTriggerSave((IWiredCondition)new FurniStatePosMatch(roomItem, new List<Item>(), 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_stuff_is:
                    HandleTriggerSave((IWiredCondition)new FurniStuffIs(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_stuff_is:
                    HandleTriggerSave((IWiredCondition)new FurniNotStuffIs(roomItem, new List<Item>()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionstateposNegative:
                    HandleTriggerSave((IWiredCondition)new FurniStatePosMatchNegative(roomItem, new List<Item>(), 0, 0, 0), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontimelessthan:
                    HandleTriggerSave((IWiredCondition)new LessThanTimer(0, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontimemorethan:
                    List<Item> items8 = new List<Item>();
                    HandleTriggerSave((IWiredCondition)new MoreThanTimer(0, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontriggeronfurni:
                    List<Item> items9 = new List<Item>();
                    HandleTriggerSave((IWiredCondition)new TriggerUserIsOnFurni(roomItem, items9), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontriggeronfurniNegative:
                    List<Item> items12 = new List<Item>();
                    HandleTriggerSave((IWiredCondition)new TriggerUserIsOnFurniNegative(roomItem, items12), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionhasfurnionfurni:
                    List<Item> items11 = new List<Item>();
                    HandleTriggerSave((IWiredCondition)new HasFurniOnFurni(roomItem, items11), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionhasfurnionfurniNegative:
                    List<Item> items14 = new List<Item>();
                    HandleTriggerSave((IWiredCondition)new HasFurniOnFurniNegative(roomItem, items14), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionactoringroup:
                    HandleTriggerSave((IWiredCondition)new HasUserInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionnotingroup:
                    HandleTriggerSave((IWiredCondition)new HasUserNotInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_actor_in_team:
                    HandleTriggerSave((IWiredCondition)new ActorInTeam(roomItem.Id, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_in_team:
                    HandleTriggerSave((IWiredCondition)new ActorNotInTeam(roomItem.Id, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_user_count:
                    HandleTriggerSave((IWiredCondition)new RoomUserNotCount(roomItem, 1, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_user_count_in:
                    HandleTriggerSave((IWiredCondition)new RoomUserCount(roomItem, 1, 1), room.GetWiredHandler(), room, roomItem);
                    break;
                    #endregion
            }
        }

        public static void HandleSave(GameClient Session, int itemID, Room room, ClientPacket clientMessage)
        {
            Item roomItem = room.GetRoomItemHandler().GetItem(itemID);
            if (roomItem == null) return;

            if (roomItem.WiredHandler != null)
            {
                roomItem.WiredHandler.Dispose();
                roomItem.WiredHandler = null;
            }

            switch (roomItem.GetBaseItem().InteractionType)
            {
                #region Trigger
                case InteractionType.triggertimer:
                    clientMessage.PopInt();
                    int cycleCount = clientMessage.PopInt();
                    HandleTriggerSave(new Timer(roomItem, room.GetWiredHandler(), cycleCount, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerroomenter:
                    clientMessage.PopInt();
                    string userName = clientMessage.PopString();
                    HandleTriggerSave(new EntersRoom(roomItem, room.GetWiredHandler(), room.GetRoomUserManager(), !string.IsNullOrEmpty(userName), userName), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercollision:
                    HandleTriggerSave(new Collision(roomItem, room.GetWiredHandler(), room.GetRoomUserManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggergameend:
                    HandleTriggerSave(new GameEnds(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggergamestart:
                    HandleTriggerSave(new GameStarts(roomItem, room.GetWiredHandler(), room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerrepeater:
                    clientMessage.PopInt();
                    int cyclesRequired = clientMessage.PopInt();
                    HandleTriggerSave(new Repeater(room.GetWiredHandler(), roomItem, cyclesRequired), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerrepeaterlong:
                    clientMessage.PopInt();
                    int cyclesRequiredlong = clientMessage.PopInt();
                    HandleTriggerSave(new Repeaterlong(room.GetWiredHandler(), roomItem, cyclesRequiredlong), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggeronusersay:
                    clientMessage.PopInt();
                    bool isOwnerOnly = clientMessage.PopInt() == 1;
                    string triggerMessage = clientMessage.PopString();
                    HandleTriggerSave(new UserSays(roomItem, room.GetWiredHandler(), isOwnerOnly, triggerMessage, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercommand:
                    HandleTriggerSave(new UserCommand(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_trg_bot_reached_avtr:
                    clientMessage.PopInt();

                    string NameBotReached = clientMessage.PopString();
                    HandleTriggerSave(new BotReadchedAvatar(roomItem, room.GetWiredHandler(), NameBotReached), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggercollisionuser:
                    HandleTriggerSave(new UserCollision(roomItem, room.GetWiredHandler(), room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerscoreachieved:
                    clientMessage.PopInt();
                    int scoreLevel = clientMessage.PopInt();
                    HandleTriggerSave(new ScoreAchieved(roomItem, room.GetWiredHandler(), scoreLevel, room.GetGameManager()), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerstatechanged:
                    clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();
                    List<Item> items1 = GetItems(clientMessage, room, itemID);
                    int delay1 = clientMessage.PopInt();
                    HandleTriggerSave(new SateChanged(room.GetWiredHandler(), roomItem, items1, delay1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerwalkonfurni:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items2 = GetItems(clientMessage, room, itemID);
                    int requiredCycles1 = clientMessage.PopInt();
                    HandleTriggerSave(new WalksOnFurni(roomItem, room.GetWiredHandler(), items2, requiredCycles1), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.triggerwalkofffurni:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items3 = GetItems(clientMessage, room, itemID);
                    int requiredCycles2 = clientMessage.PopInt();
                    HandleTriggerSave(new WalksOffFurni(roomItem, room.GetWiredHandler(), items3, requiredCycles2), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region Action
                case InteractionType.actiongivescore:
                    clientMessage.PopInt();
                    int scoreToGive = clientMessage.PopInt();
                    int maxCountPerGame = clientMessage.PopInt();
                    HandleTriggerSave(new GiveScore(maxCountPerGame, scoreToGive, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_give_score_tm:
                    clientMessage.PopInt();
                    int scoreToGive2 = clientMessage.PopInt();
                    int maxCountPerGame2 = clientMessage.PopInt();
                    int TeamId = clientMessage.PopInt();
                    HandleTriggerSave(new GiveScoreTeam(TeamId, maxCountPerGame2, scoreToGive2, room.GetGameManager(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionposreset:
                    clientMessage.PopInt();
                    int EtatActuel = clientMessage.PopInt();
                    int DirectionActuel = clientMessage.PopInt();
                    int PositionActuel = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> itemsposrest = GetItems(clientMessage, room, itemID);
                    int requiredCyclesposrest = clientMessage.PopInt();
                    HandleTriggerSave(new PositionReset(itemsposrest, requiredCyclesposrest, room.GetRoomItemHandler(), room.GetWiredHandler(), itemID, EtatActuel, DirectionActuel, PositionActuel), room.GetWiredHandler(), room, roomItem);

                    break;
                case InteractionType.actionmoverotate:
                    clientMessage.PopInt();
                    MovementState movement = (MovementState)clientMessage.PopInt();
                    RotationState rotation = (RotationState)clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();
                    List<Item> items4 = GetItems(clientMessage, room, itemID);
                    int delay2 = clientMessage.PopInt();
                    HandleTriggerSave(new MoveRotate(movement, rotation, items4, delay2, room, room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionresettimer:
                    clientMessage.PopInt();
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    int delay3 = clientMessage.PopInt();
                    HandleTriggerSave(new TimerReset(room, room.GetWiredHandler(), delay3, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.highscore:
                case InteractionType.highscorepoints:

                    break;
                case InteractionType.actionshowmessage:
                    clientMessage.PopInt();
                    string MessageWired = clientMessage.PopString();
                    int CountItemMessage = clientMessage.PopInt();
                    int DelayMessage = clientMessage.PopInt();
                    HandleTriggerSave(new ShowMessage(MessageWired, room.GetWiredHandler(), itemID, DelayMessage), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.superwired:
                    clientMessage.PopInt();
                    string MessageSuperWired = clientMessage.PopString();
                    int CountItemSuperWired = clientMessage.PopInt();
                    int DelaySuperWired = clientMessage.PopInt();
                    HandleTriggerSave(new SuperWired(MessageSuperWired, DelaySuperWired, Session.GetHabbo().HasFuse("fuse_superwired_god"), Session.GetHabbo().HasFuse("fuse_superwired_staff"), room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actiongivereward:
                    if (!Session.GetHabbo().HasFuse("fuse_superwired"))
                    {
                        return;
                    }
                    //clientMessage.PopInt();
                    //WiredSaver.HandleTriggerSave((IWiredTrigger)new GiveReward(clientMessage.PopString(), room.GetWiredHandler(), itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionkickuser:
                    clientMessage.PopInt();
                    HandleTriggerSave(new KickUser(clientMessage.PopString(), room.GetWiredHandler(), itemID, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionteleportto:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items6 = GetItems(clientMessage, room, itemID);
                    int delay4 = clientMessage.PopInt();
                    HandleTriggerSave(new TeleportToItem(room.GetGameMap(), room.GetWiredHandler(), items6, delay4, itemID), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_endgame_team:
                    clientMessage.PopInt();
                    int TeamId3 = clientMessage.PopInt();
                    HandleTriggerSave(new TeamGameOver(TeamId3, roomItem.Id, room), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actiontogglestate:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> items7 = GetItems(clientMessage, room, itemID);
                    int delay5 = clientMessage.PopInt();
                    HandleTriggerSave(new ToggleItemState(room.GetGameMap(), room.GetWiredHandler(), items7, delay5, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_call_stacks:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsExecute = GetItems(clientMessage, room, itemID);
                    int StackDeley = clientMessage.PopInt();
                    HandleTriggerSave(new ExecutePile(itemsExecute, StackDeley, room.GetWiredHandler(), roomItem), room.GetWiredHandler(), room, roomItem);

                    break;
                case InteractionType.actionflee:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsflee = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new Escape(itemsflee, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionchase:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemschase = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new Chase(itemschase, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.collisionteam:
                    clientMessage.PopInt();
                    int TeamIdCollision = clientMessage.PopInt();
                    HandleTriggerSave(new CollisionTeam(TeamIdCollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.collisioncase:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemscollision = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave(new CollisionCase(itemscollision, room, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.actionmovetodir:
                    clientMessage.PopInt();

                    MovementDirection StarDirect = (MovementDirection)clientMessage.PopInt();
                    WhenMovementBlock WhenBlock = (WhenMovementBlock)clientMessage.PopInt();

                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> itemsmovetodir = GetItems(clientMessage, room, itemID);
                    int delaymovetodir = clientMessage.PopInt();

                    HandleTriggerSave(new MoveToDir(itemsmovetodir, room, room.GetWiredHandler(), roomItem.Id, StarDirect, WhenBlock), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_clothes:
                    clientMessage.PopInt();

                    string NameAndLook = clientMessage.PopString();

                    string[] SplieNAL = NameAndLook.Split('\t');
                    if (SplieNAL.Length != 2)
                        break;

                    HandleTriggerSave(new BotClothes(SplieNAL[0], SplieNAL[1], room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_teleport:
                    clientMessage.PopInt();

                    string NameBot = clientMessage.PopString();

                    List<Item> itemsbotteleport = GetItems(clientMessage, room, itemID);

                    HandleTriggerSave(new BotTeleport(NameBot, itemsbotteleport, room.GetGameMap(), room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_follow_avatar:
                    clientMessage.PopInt();

                    bool IsFollow = (clientMessage.PopInt() == 1);
                    string NameBotFollow = clientMessage.PopString();

                    HandleTriggerSave(new BotFollowAvatar(NameBotFollow, IsFollow, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_give_handitem:
                    clientMessage.PopInt();

                    HandleTriggerSave(new BotGiveHanditem("", room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_move:
                    clientMessage.PopInt();

                    string NameBotMove = clientMessage.PopString();

                    List<Item> itemsbotMove = GetItems(clientMessage, room, itemID);

                    HandleTriggerSave(new BotMove(NameBotMove, itemsbotMove, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_user_move:
                    clientMessage.PopInt();
                    clientMessage.PopString();
                    List<Item> itemsUserMove = GetItems(clientMessage, room, itemID);

                    int delayusermove = clientMessage.PopInt();
                    HandleTriggerSave(new UserMove(itemsUserMove, delayusermove, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_talk_to_avatar:
                    clientMessage.PopInt();
                    bool IsCrier = (clientMessage.PopInt() == 1);

                    string BotNameAndMessage = clientMessage.PopString();

                    string[] SplieNAM = BotNameAndMessage.Split('\t');
                    if (SplieNAM.Length != 2)
                        break;
                    string NameBotTalk = SplieNAM[0];
                    string MessageBot = SplieNAM[1];


                    HandleTriggerSave(new BotTalkToAvatar(NameBotTalk, MessageBot, IsCrier, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_bot_talk:
                    clientMessage.PopInt();
                    bool IsCrier2 = (clientMessage.PopInt() == 1);

                    string BotNameAndMessage2 = clientMessage.PopString();

                    string[] SplieNAM2 = BotNameAndMessage2.Split('\t');
                    if (SplieNAM2.Length != 2)
                        break;
                    string NameBotTalk2 = SplieNAM2[0];
                    string MessageBot2 = SplieNAM2[1];


                    HandleTriggerSave(new BotTalk(NameBotTalk2, MessageBot2, IsCrier2, room.GetWiredHandler(), roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_leave_team:
                    HandleTriggerSave(new TeamLeave(roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_act_join_team:
                    clientMessage.PopInt();
                    int TeamId4 = clientMessage.PopInt();
                    HandleTriggerSave(new TeamJoin(TeamId4, roomItem.Id), room.GetWiredHandler(), room, roomItem);
                    break;
                #endregion
                #region Condition
                case InteractionType.superwiredcondition:
                    clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new SuperWiredCondition(roomItem, clientMessage.PopString(), Session.GetHabbo().HasFuse("fuse_superwired")), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionfurnishaveusers:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items10 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new FurniHasUser(roomItem, items10), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionfurnishavenousers:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items12 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new FurniHasNoUser(roomItem, items12), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionstatepos:
                    clientMessage.PopInt();
                    int EtatActuel2 = clientMessage.PopInt();
                    int DirectionActuel2 = clientMessage.PopInt();
                    int PositionActuel2 = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    HandleTriggerSave((IWiredCondition)new FurniStatePosMatch(roomItem, GetItems(clientMessage, room, itemID), EtatActuel2, DirectionActuel2, PositionActuel2), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_stuff_is:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    HandleTriggerSave((IWiredCondition)new FurniStuffIs(roomItem, GetItems(clientMessage, room, itemID)), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_stuff_is:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    HandleTriggerSave((IWiredCondition)new FurniNotStuffIs(roomItem, GetItems(clientMessage, room, itemID)), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionstateposNegative:
                    clientMessage.PopInt();
                    int EtatActuel3 = clientMessage.PopInt();
                    int DirectionActuel3 = clientMessage.PopInt();
                    int PositionActuel3 = clientMessage.PopInt();
                    clientMessage.PopBoolean();
                    clientMessage.PopBoolean();

                    List<Item> items17 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new FurniStatePosMatchNegative(roomItem, items17, EtatActuel3, DirectionActuel3, PositionActuel3), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontimelessthan:
                    clientMessage.PopInt();
                    int cycleCount2 = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new LessThanTimer(cycleCount2, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontimemorethan:
                    clientMessage.PopInt();
                    int cycleCount3 = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new MoreThanTimer(cycleCount3, room, roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontriggeronfurni:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items9 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new TriggerUserIsOnFurni(roomItem, items9), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditiontriggeronfurniNegative:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items14 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new TriggerUserIsOnFurniNegative(roomItem, items14), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionhasfurnionfurni:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items13 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new HasFurniOnFurni(roomItem, items13), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionhasfurnionfurniNegative:
                    clientMessage.PopInt();
                    clientMessage.PopString();

                    List<Item> items15 = GetItems(clientMessage, room, itemID);
                    HandleTriggerSave((IWiredCondition)new HasFurniOnFurniNegative(roomItem, items15), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionactoringroup:
                    HandleTriggerSave((IWiredCondition)new HasUserInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_user_count:
                    clientMessage.PopInt();
                    int UserCountMin = clientMessage.PopInt();
                    int UserCountMax = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new RoomUserNotCount(roomItem, UserCountMin, UserCountMax), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_user_count_in:
                    clientMessage.PopInt();
                    int UserCountMin2 = clientMessage.PopInt();
                    int UserCountMax2 = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new RoomUserCount(roomItem, UserCountMin2, UserCountMax2), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.conditionnotingroup:
                    HandleTriggerSave((IWiredCondition)new HasUserNotInGroup(roomItem), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_actor_in_team:
                    clientMessage.PopInt();
                    int TeamId5 = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new ActorInTeam(roomItem.Id, TeamId5), room.GetWiredHandler(), room, roomItem);
                    break;
                case InteractionType.wf_cnd_not_in_team:
                    clientMessage.PopInt();
                    int TeamId2 = clientMessage.PopInt();
                    HandleTriggerSave((IWiredCondition)new ActorNotInTeam(roomItem.Id, TeamId2), room.GetWiredHandler(), room, roomItem);
                    break;
                    #endregion
            }

            Session.SendPacket(new ServerPacket(ServerPacketHeader.WiredSavedComposer));
        }

        private static List<Item> GetItems(ClientPacket message, Room room, int itemID)
        {
            List<Item> list = new List<Item>();
            int itemCount = message.PopInt();
            for (int index = 0; index < itemCount; ++index)
            {
                Item roomItem = room.GetRoomItemHandler().GetItem(message.PopInt());
                if (roomItem != null && index < 20 && roomItem.GetBaseItem().Type == 's')
                    list.Add(roomItem);
            }

            return list;
        }

        private static void HandleTriggerSave(IWired handler, WiredHandler manager, Room room, Item roomItem)
        {
            if (roomItem == null) return;

            roomItem.WiredHandler = handler;
            manager.RemoveFurniture(roomItem);
            manager.AddFurniture(roomItem);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                handler.SaveToDatabase(queryreactor);
        }
    }
}
