using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.HabboHotel.Items.Interactor;
using Akiled.HabboHotel.Items.Interactors;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items
{
    public delegate void OnItemTrigger(object sender, ItemTriggeredArgs e);

    public class Item : IEquatable<Item>
    {
        public int Id;
        public int RoomId;
        public int BaseItem;

        public string ExtraData;
        public int GroupId;
        public int LimitedNo;
        public int LimitedTot;
        public Team team;
        public int interactionCountHelper;
        public int value;
        public FreezePowerUp freezePowerUp;
        public int Rotation;
        public string wallCoord;
        private bool updateNeeded;
        public int UpdateCounter;
        public int OwnerId;
        public string Username;
        public int InteractingUser;
        public int InteractingUser2;
        private Room mRoom;
        public bool pendingReset;
        public int Fx;

        public bool ChronoStarter;
        public MovementState movement;
        public MovementDirection MovementDir;

        public Dictionary<string, int> Scores;

        public IWired WiredHandler;
        public event OnItemTrigger ItemTriggerEventHandler;
        public event UserAndItemDelegate OnUserWalksOffFurni;
        public event UserAndItemDelegate OnUserWalksOnFurni;

        public Dictionary<int, ThreeDCoord> GetAffectedTiles { get; private set; }

        public int GetX { get; private set; }

        public int GetY { get; private set; }

        public double GetZ { get; private set; }

        public bool IsRoller { get; private set; }

        public Point Coordinate
        {
            get
            {
                return new Point(this.GetX, this.GetY);
            }
        }

        public bool UpdateNeeded
        {
            get { return updateNeeded; }
            set
            {
                if (value && GetRoom() != null)
                    GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
                updateNeeded = value;
            }
        }

        public List<Point> GetCoords
        {
            get
            {
                List<Point> list = new List<Point>();
                list.Add(this.Coordinate);
                foreach (ThreeDCoord threeDcoord in this.GetAffectedTiles.Values)
                    list.Add(new Point(threeDcoord.X, threeDcoord.Y));
                return list;
            }
        }

        public double TotalHeight
        {
            get
            {
                return GetZ + Height;
            }
        }

        public double Height
        {
            get
            {
                if (this.GetBaseItem().AdjustableHeights.Count > 1 && this.ExtraData != "")
                {
                    int index;
                    if (int.TryParse(this.ExtraData, out index))
                    {
                        if (index < this.GetBaseItem().AdjustableHeights.Count && index >= 0)
                        {
                            if (index < this.GetBaseItem().AdjustableHeights.Count && index >= 0)
                                return this.GetBaseItem().AdjustableHeights[index];
                        }
                    }
                }

                return Data.Height;
            }
        }

        public bool IsWallItem;

        public bool IsFloorItem;

        public Point SquareInFront
        {
            get
            {
                Point point = new Point(this.GetX, this.GetY);
                if (this.Rotation == 0)
                    --point.Y;
                else if (this.Rotation == 2)
                    ++point.X;
                else if (this.Rotation == 4)
                    ++point.Y;
                else if (this.Rotation == 6)
                    --point.X;
                return point;
            }
        }

        public Point SquareBehind
        {
            get
            {
                Point point = new Point(this.GetX, this.GetY);
                if (this.Rotation == 0)
                    ++point.Y;
                else if (this.Rotation == 2)
                    --point.X;
                else if (this.Rotation == 4)
                    --point.Y;
                else if (this.Rotation == 6)
                    ++point.X;
                return point;
            }
        }

        public ItemData Data;
        public GameClient ballMover;


        public int ExtradataInt
        {
            get
            {
                int result;
                return int.TryParse(this.ExtraData, out result) ? result : 0;
            }

        }
        public FurniInteractor Interactor
        {
            get
            {
                switch (this.GetBaseItem().InteractionType)
                {
                    case InteractionType.GATE:
                    case InteractionType.banzaipyramid:
                        return new InteractorGate();
                    case InteractionType.scoreboard:
                        return new InteractorScoreboard();
                    case InteractionType.vendingmachine:
                        return new InteractorVendor();
                    case InteractionType.vendingenablemachine:
                        return new InteractorVendorEnable();
                    case InteractionType.alert:
                        return new InteractorAlert();
                    case InteractionType.onewaygate:
                        return new InteractorOneWayGate();
                    case InteractionType.loveshuffler:
                        return new InteractorLoveShuffler();
                    case InteractionType.habbowheel:
                        return new InteractorHabboWheel();
                    case InteractionType.dice:
                        return new InteractorDice();
                    case InteractionType.bottle:
                        return new InteractorSpinningBottle();
                    case InteractionType.TELEPORT:
                        return new InteractorTeleport();
                    case InteractionType.football:
                        return new InteractorFootball();
                    case InteractionType.footballcountergreen:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.footballcounterblue:
                    case InteractionType.footballcounterred:
                        return new InteractorScoreCounter();
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.banzaiscoregreen:
                        return new InteractorBanzaiScoreCounter();
                    case InteractionType.ChronoTimer:
                        return new InteractorTimer();
                    case InteractionType.banzaiblo:
                    case InteractionType.banzaiblob:
                        return new InteractorBlob();
                    case InteractionType.banzaipuck:
                        return new InteractorBanzaiPuck();
                    case InteractionType.freezetileblock:
                    case InteractionType.freezetile:
                        return new InteractorFreezeTile();
                    case InteractionType.JUKEBOX:
                        return new InteractorJukebox();
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
                    case InteractionType.triggercollision:
                    case InteractionType.actiongivescore:
                    case InteractionType.actionposreset:
                    case InteractionType.actionmoverotate:
                    case InteractionType.actionresettimer:
                    case InteractionType.actionshowmessage:
                    case InteractionType.highscore:
                    case InteractionType.highscorepoints:
                    case InteractionType.actiongivereward:
                    case InteractionType.superwired:
                    case InteractionType.superwiredcondition:
                    case InteractionType.actionteleportto:
                    case InteractionType.wf_act_endgame_team:
                    case InteractionType.wf_act_call_stacks:
                    case InteractionType.actiontogglestate:
                    case InteractionType.actionkickuser:
                    case InteractionType.actionflee:
                    case InteractionType.actionchase:
                    case InteractionType.collisioncase:
                    case InteractionType.collisionteam:
                    case InteractionType.actionmovetodir:
                    case InteractionType.conditionfurnishaveusers:
                    case InteractionType.conditionfurnishavenousers:
                    case InteractionType.conditionhasfurnionfurni:
                    case InteractionType.conditionhasfurnionfurniNegative:
                    case InteractionType.conditionstatepos:
                    case InteractionType.wf_cnd_stuff_is:
                    case InteractionType.wf_cnd_not_stuff_is:
                    case InteractionType.conditionstateposNegative:
                    case InteractionType.conditiontimelessthan:
                    case InteractionType.conditiontimemorethan:
                    case InteractionType.conditiontriggeronfurni:
                    case InteractionType.conditiontriggeronfurniNegative:
                    case InteractionType.conditionactoringroup:
                    case InteractionType.conditionnotingroup:
                    case InteractionType.wf_trg_bot_reached_stf:
                    case InteractionType.wf_trg_bot_reached_avtr:
                    case InteractionType.wf_act_bot_clothes:
                    case InteractionType.wf_act_bot_teleport:
                    case InteractionType.wf_act_bot_follow_avatar:
                    case InteractionType.wf_act_bot_give_handitem:
                    case InteractionType.wf_act_bot_move:
                    case InteractionType.wf_act_user_move:
                    case InteractionType.wf_act_bot_talk_to_avatar:
                    case InteractionType.wf_act_bot_talk:
                    case InteractionType.wf_cnd_has_handitem:
                    case InteractionType.wf_act_join_team:
                    case InteractionType.wf_act_leave_team:
                    case InteractionType.wf_act_give_score_tm:
                    case InteractionType.wf_cnd_actor_in_team:
                    case InteractionType.wf_cnd_not_in_team:
                    case InteractionType.wf_cnd_not_user_count:
                    case InteractionType.wf_cnd_user_count_in:
                        return new WiredInteractor();
                    case InteractionType.MANNEQUIN:
                        return new InteractorManiqui();
                    case InteractionType.TONER:
                        return new InteractorChangeBackgrounds();
                    case InteractionType.puzzlebox:
                        return new InteractorPuzzleBox();
                    case InteractionType.floorswitch1:
                        return new InteractorSwitch1(this.GetBaseItem().Modes);
                    case InteractionType.CRACKABLE:
                        return new InteractorCrackable(this.GetBaseItem().Modes);
                    case InteractionType.tvyoutube:
                        return new InteractorTvYoutube();
                    case InteractionType.LOVELOCK:
                        return new InteractorLoveLock();
                    case InteractionType.tronco:
                        return (FurniInteractor)new Interactortronco();
                    case InteractionType.mineria:
                        return (FurniInteractor)new InteractorMineria();
                    case InteractionType.PHOTO:
                        return new InteractorIgnore();
                    case InteractionType.PLANT_SEED:
                        return new InteractorPlantSeed();
                    default:
                        return new InteractorGenericSwitch(this.GetBaseItem().Modes);
                }
            }
        }

        public bool IsWired
        {
            get
            {
                switch (GetBaseItem().InteractionType)
                {
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
                    case InteractionType.triggercollision:
                    case InteractionType.actiongivescore:
                    case InteractionType.actionposreset:
                    case InteractionType.actionmoverotate:
                    case InteractionType.actionresettimer:
                    case InteractionType.actionshowmessage:
                    case InteractionType.highscore:
                    case InteractionType.highscorepoints:
                    case InteractionType.actiongivereward:
                    case InteractionType.superwired:
                    case InteractionType.superwiredcondition:
                    case InteractionType.actionteleportto:
                    case InteractionType.wf_act_endgame_team:
                    case InteractionType.wf_act_call_stacks:
                    case InteractionType.actiontogglestate:
                    case InteractionType.actionkickuser:
                    case InteractionType.actionflee:
                    case InteractionType.actionchase:
                    case InteractionType.collisioncase:
                    case InteractionType.collisionteam:
                    case InteractionType.actionmovetodir:
                    case InteractionType.conditionfurnishaveusers:
                    case InteractionType.conditionfurnishavenousers:
                    case InteractionType.conditionhasfurnionfurni:
                    case InteractionType.conditionhasfurnionfurniNegative:
                    case InteractionType.conditionstatepos:
                    case InteractionType.wf_cnd_stuff_is:
                    case InteractionType.wf_cnd_not_stuff_is:
                    case InteractionType.conditionstateposNegative:
                    case InteractionType.conditiontimelessthan:
                    case InteractionType.conditiontimemorethan:
                    case InteractionType.conditiontriggeronfurni:
                    case InteractionType.conditiontriggeronfurniNegative:
                    case InteractionType.conditionactoringroup:
                    case InteractionType.conditionnotingroup:
                    case InteractionType.wf_trg_bot_reached_stf:
                    case InteractionType.wf_trg_bot_reached_avtr:
                    case InteractionType.wf_act_bot_clothes:
                    case InteractionType.wf_act_bot_teleport:
                    case InteractionType.wf_act_bot_follow_avatar:
                    case InteractionType.wf_act_bot_give_handitem:
                    case InteractionType.wf_act_bot_move:
                    case InteractionType.wf_act_user_move:
                    case InteractionType.wf_act_bot_talk_to_avatar:
                    case InteractionType.wf_act_bot_talk:
                    case InteractionType.wf_cnd_has_handitem:
                    case InteractionType.wf_act_join_team:
                    case InteractionType.wf_act_leave_team:
                    case InteractionType.wf_act_give_score_tm:
                    case InteractionType.wf_cnd_actor_in_team:
                    case InteractionType.wf_cnd_not_in_team:
                    case InteractionType.wf_cnd_not_user_count:
                    case InteractionType.wf_cnd_user_count_in:
                        return true;
                }

                return false;
            }
        }

        public bool BallIsMoving { get; set; }
        public int BallValue { get; set; }

        public Item(int mId, int fOwnerId, int RoomId, int mBaseItem, string ExtraData, int limitedNumber, int limitedStack, int X, int Y, double Z, int Rot, string wallCoord, Room pRoom = null)
        {
            ItemData Data = null;
            if (AkiledEnvironment.GetGame().GetItemManager().GetItem(mBaseItem, out Data))
            {
                this.Id = mId;
                this.OwnerId = fOwnerId;
                this.Username = AkiledEnvironment.GetUsernameById(OwnerId);
                this.RoomId = RoomId;
                this.BaseItem = mBaseItem;
                this.ExtraData = ExtraData;
                this.GetX = X;
                this.GetY = Y;
                if (!double.IsInfinity(Z))
                    this.GetZ = Z;
                this.Rotation = Rot;
                this.UpdateCounter = 0;
                this.InteractingUser = 0;
                this.InteractingUser2 = 0;
                this.interactionCountHelper = (byte)0;
                this.value = 0;
                this.LimitedNo = limitedNumber;
                this.LimitedTot = limitedStack;
                this.Data = Data;
                this.wallCoord = wallCoord;

                this.Scores = new Dictionary<string, int>();

                this.Fx = this.Data.EffectId;

                this.mRoom = pRoom;
                if (this.GetBaseItem() == null)
                    Logging.LogException("Unknown baseID: " + mBaseItem);
                switch (this.GetBaseItem().InteractionType)
                {
                    case InteractionType.roller:
                        this.IsRoller = true;
                        break;
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.freezegreengate:
                        this.team = Team.green;
                        break;
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.freezeyellowgate:
                        this.team = Team.yellow;
                        break;
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.banzaiscoreblue:
                    case InteractionType.freezebluecounter:
                    case InteractionType.freezebluegate:
                        this.team = Team.blue;
                        break;
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaigatered:
                    case InteractionType.banzaiscorered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.freezeredgate:
                        this.team = Team.red;
                        break;
                    case InteractionType.banzaitele:
                        this.ExtraData = "";
                        break;
                    case InteractionType.GUILD_ITEM:
                    case InteractionType.GUILD_GATE:
                        if (!string.IsNullOrEmpty(ExtraData))
                        {
                            if (ExtraData.Contains(";"))
                            {
                                int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out this.GroupId);
                            }
                        }
                        break;
                }
                this.IsWallItem = this.GetBaseItem().Type.ToString().ToLower() == "i";
                this.IsFloorItem = this.GetBaseItem().Type.ToString().ToLower() == "s";

                this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.GetX, this.GetY, Rot);
            }
        }

        public void SetState(int pX, int pY, double pZ, Dictionary<int, ThreeDCoord> Tiles)
        {
            this.GetX = pX;
            this.GetY = pY;
            if (!double.IsInfinity(pZ))
                this.GetZ = pZ;
            this.GetAffectedTiles = Tiles;
        }

        public void OnTrigger(RoomUser user)
        {
            if (this.ItemTriggerEventHandler == null)
                return;
            this.ItemTriggerEventHandler(null, new ItemTriggeredArgs(user, this));
        }

        public void Destroy()
        {
            this.mRoom = (Room)null;
            this.GetAffectedTiles.Clear();

            if (this.WiredHandler != null)
                this.WiredHandler.Dispose();
            this.WiredHandler = null;

            this.ItemTriggerEventHandler = (OnItemTrigger)null;
            this.OnUserWalksOffFurni = (UserAndItemDelegate)null;
            this.OnUserWalksOnFurni = (UserAndItemDelegate)null;
        }

        public bool Equals(Item comparedItem)
        {
            return comparedItem.Id == this.Id;
        }

        public Point GetMoveCoord(int X, int Y, int i)
        {

            switch (this.MovementDir)
            {
                case MovementDirection.up:
                    {
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.upright:
                    {
                        X = X + i;
                        Y = Y - i;
                        break;
                    }
                case MovementDirection.right:
                    {
                        X = X + i;
                        break;
                    }
                case MovementDirection.downright:
                    {
                        X = X + i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.down:
                    {
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.downleft:
                    {
                        X = X - i;
                        Y = Y + i;
                        break;
                    }
                case MovementDirection.left:
                    {
                        X = X - i;
                        break;
                    }
                case MovementDirection.upleft:
                    {
                        X = X - i;
                        Y = Y - i;
                        break;
                    }
            }

            return new Point(X, Y);
        }

        public void GetNewDir(int X, int Y)
        {
            switch (this.MovementDir)
            {
                case MovementDirection.up:
                    this.MovementDir = MovementDirection.down;
                    break;
                case MovementDirection.down:
                    this.MovementDir = MovementDirection.up;
                    break;
                case MovementDirection.right:
                    this.MovementDir = MovementDirection.left;
                    break;
                case MovementDirection.left:
                    this.MovementDir = MovementDirection.right;
                    break;

                case MovementDirection.upright:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.downleft;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                        {
                            if (AkiledEnvironment.GetRandomNumber(1, 2) == 1)
                                this.MovementDir = MovementDirection.downright;
                            else
                                this.MovementDir = MovementDirection.upleft;
                        }
                        else
                        {
                            if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true))
                                this.MovementDir = MovementDirection.upleft;
                            else if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                                this.MovementDir = MovementDirection.downright;
                        }
                    }
                    break;
                case MovementDirection.upleft:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.downright;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                        {
                            if (AkiledEnvironment.GetRandomNumber(1, 2) == 1)
                                this.MovementDir = MovementDirection.downleft;
                            else
                                this.MovementDir = MovementDirection.upright;
                        }
                        else
                        {
                            if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true))
                                this.MovementDir = MovementDirection.upright;
                            else if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                                this.MovementDir = MovementDirection.downleft;
                        }
                    }
                    break;
                case MovementDirection.downright:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.upleft;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                        {
                            if (AkiledEnvironment.GetRandomNumber(1, 2) == 1)
                                this.MovementDir = MovementDirection.downleft;
                            else
                                this.MovementDir = MovementDirection.upright;
                        }
                        else
                        {
                            if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true))
                                this.MovementDir = MovementDirection.upright;
                            else if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y - 1, true))
                                this.MovementDir = MovementDirection.downleft;
                        }
                    }
                    break;
                case MovementDirection.downleft:
                    if (!this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                    {
                        this.MovementDir = MovementDirection.upright;
                    }
                    else
                    {
                        if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                        {
                            if (AkiledEnvironment.GetRandomNumber(1, 2) == 1)
                                this.MovementDir = MovementDirection.downright;
                            else
                                this.MovementDir = MovementDirection.upleft;
                        }
                        else
                        {
                            if (this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true))
                                this.MovementDir = MovementDirection.upleft;
                            else if (this.GetRoom().GetGameMap().CanStackItem(X + 1, Y + 1, true) && !this.GetRoom().GetGameMap().CanStackItem(X - 1, Y - 1, true))
                                this.MovementDir = MovementDirection.downright;
                        }
                    }
                    break;
            }
        }

        public void ProcessUpdates()
        {
            this.UpdateCounter--;
            if (this.UpdateCounter > 0)
            {
                return;
            }

            this.UpdateCounter = 0;

            this.Interactor.OnTick(this);
        }

        public void ReqUpdate(int Cycles)
        {
            if (this.UpdateCounter > 0)
                return;
            this.UpdateCounter = Cycles;
            this.GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
        }

        public void UpdateState() => this.UpdateState(true, true);

        public void RequestUpdate(int Cycles, bool setUpdate)
        {
            UpdateCounter = Cycles;
            if (setUpdate)
                UpdateNeeded = true;
        }

        public void UpdateState(bool inDb, bool inRoom)
        {
            if (this.GetRoom() == null)
                return;
            if (inDb)
                this.GetRoom().GetRoomItemHandler().UpdateItem(this);
            if (!inRoom)
                return;
            if (this.IsFloorItem)
            {
                this.GetRoom().SendPacket(new ObjectUpdateComposer(this, OwnerId));
            }
            else
            {
                this.GetRoom().SendPacket(new ItemUpdateComposer(this, OwnerId));
            }
        }

        public void ResetBaseItem()
        {
            this.Data = (ItemData)null;
            this.Data = this.GetBaseItem();

            switch (this.GetBaseItem().InteractionType)
            {
                case InteractionType.roller:
                    this.IsRoller = true;
                    break;
                case InteractionType.footballcountergreen:
                case InteractionType.banzaigategreen:
                case InteractionType.banzaiscoregreen:
                case InteractionType.freezegreencounter:
                case InteractionType.freezegreengate:
                    this.team = Team.green;
                    break;
                case InteractionType.footballcounteryellow:
                case InteractionType.banzaigateyellow:
                case InteractionType.banzaiscoreyellow:
                case InteractionType.freezeyellowcounter:
                case InteractionType.freezeyellowgate:
                    this.team = Team.yellow;
                    break;
                case InteractionType.footballcounterblue:
                case InteractionType.banzaigateblue:
                case InteractionType.banzaiscoreblue:
                case InteractionType.freezebluecounter:
                case InteractionType.freezebluegate:
                    this.team = Team.blue;
                    break;
                case InteractionType.footballcounterred:
                case InteractionType.banzaigatered:
                case InteractionType.banzaiscorered:
                case InteractionType.freezeredcounter:
                case InteractionType.freezeredgate:
                    this.team = Team.red;
                    break;
                case InteractionType.banzaitele:
                    this.ExtraData = "";
                    break;
                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    if (!string.IsNullOrEmpty(ExtraData))
                    {
                        if (ExtraData.Contains(";"))
                        {
                            int.TryParse(this.ExtraData.Split(new char[1] { ';' })[1], out this.GroupId);
                        }
                    }
                    break;
            }

            this.GetAffectedTiles = Gamemap.GetAffectedTiles(this.GetBaseItem().Length, this.GetBaseItem().Width, this.GetX, this.GetY, this.Rotation);
        }

        public static bool InteractionsAllowed(Item Item)
        {
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.NONE:
                case InteractionType.GATE:
                    return true;
            }

            return false;
        }

        public ItemData GetBaseItem()
        {
            if (this.Data == null)
            {
                ItemData I = null;
                if (AkiledEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out I))
                    this.Data = I;
            }

            return this.Data;
        }

        public Room GetRoom()
        {
            if (this.mRoom == null)
                this.mRoom = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            return this.mRoom;
        }

        public void UserWalksOnFurni(RoomUser user, Item item)
        {
            if (this.OnUserWalksOnFurni == null)
                return;
            this.OnUserWalksOnFurni(user, item);
        }
        public override bool Equals(object obj) => this.Equals(obj as Item);

        public void UserWalksOffFurni(RoomUser user, Item item)
        {
            if (this.OnUserWalksOffFurni == null)
                return;
            this.OnUserWalksOffFurni(user, item);
        }

    }
}
