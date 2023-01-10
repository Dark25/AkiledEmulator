using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Roleplay.Weapon;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Roleplay.Enemy
{
    public class RoleBot
    {
        public int Health;
        public RPWeapon WeaponGun;
        public RPWeapon WeaponCac;
        public bool Dead;
        public int DeadTimer;
        public int AggroVirtuelId;
        public bool ResetBot;
        public int ResetBotTimer;
        public int SlowTimer;
        public bool Dodge;
        public int DodgeTimer;
        public int GunCharger;
        public int GunLoadTimer;
        public int HitCount;
        public int ActionTimer;
        public int AggroTimer;
        public int DodgeStartCount;
        public RPEnemy Config;

        public RoleBot(RPEnemy EnemyConfig)
        {
            this.SetConfig(EnemyConfig);

            this.Dead = false;
            this.AggroVirtuelId = 0;
            this.AggroTimer = 0;
            this.ResetBot = false;
            this.ResetBotTimer = 0;
            this.HitCount = 0;
            this.Dodge = false;
            this.DodgeTimer = 0;
            this.GunCharger = 6;
            this.GunLoadTimer = 0;
            this.DodgeStartCount = AkiledEnvironment.GetRandomNumber(2, 4);
            this.ActionTimer = AkiledEnvironment.GetRandomNumber(10, 30);
        }

        public void SetConfig(RPEnemy EnemyConfig)
        {
            this.Config = EnemyConfig;

            this.Health = this.Config.Health;
            this.WeaponGun = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(this.Config.WeaponGunId);
            this.WeaponCac = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(this.Config.WeaponCacId);
        }

        private bool IsAllowZone(RoomUser Bot)
        {
            int BotX = (Bot.SetStep) ? Bot.SetX : Bot.X;
            int BotY = (Bot.SetStep) ? Bot.SetY : Bot.Y;

            if (Math.Abs(BotX - Bot.BotData.X) > this.Config.ZoneDistance || Math.Abs(BotY - Bot.BotData.Y) > this.Config.ZoneDistance)
                return false;

            return true;
        }

        private void ReloadGunCycle(RoomUser Bot)
        {
            if (this.GunLoadTimer > 0)
            {
                this.GunLoadTimer--;
                if (this.GunLoadTimer == 0)
                {
                    this.GunCharger = 6;
                }
            }
            else
            {
                if (this.GunCharger == 0)
                {
                    this.GunLoadTimer = 6;
                    Bot.OnChat("*Recharge mon arme*");
                }
            }
        }

        public void OnCycle(RoomUser Bot, Room Room)
        {
            if (this.SlowTimer > 0 || this.Config.ZombieMode)
            {
                if (this.SlowTimer > 0)
                    this.SlowTimer--;

                if (!Bot.BreakWalkEnable)
                    Bot.BreakWalkEnable = true;
            }
            else
            {
                if (Bot.BreakWalkEnable)
                    Bot.BreakWalkEnable = false;
            }

            this.ReloadGunCycle(Bot);

            if (this.AggroVirtuelId > 0)
                this.AggroCycle(Bot, Room);

            if (this.Config.AggroDistance > 0 && !this.Dead) // && this.AggroVirtuelId == 0
                this.AggroSearch(Bot, Room);

            if (!this.ResetBot && !this.Dead && this.AggroVirtuelId == 0 && !Bot.Freeze)
            {
                this.FreeTimeCycle(Bot);
            }

            if (this.ResetBot && !this.Dead && this.AggroVirtuelId == 0)
            {
                this.CheckResetBot(Bot, Room);
            }

            if (this.Dead)
            {
                this.DeadTimer--;
                if (this.DeadTimer <= 0)
                {
                    this.Dead = false;
                    this.Health = this.Config.Health;


                    Bot.RemoveStatus("lay");
                    Bot.Freeze = false;
                    Bot.FreezeEndCounter = 0;
                    Bot.IsLay = false;
                    Bot.UpdateNeeded = true;
                }
            }
        }

        public void Hit(RoomUser Bot, int Dmg, Room Room, int pAggroVId, int pTeamId)
        {
            if (this.Dead)
                return;

            if (this.Health <= Dmg)
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByVirtualId(this.AggroVirtuelId);
                if (User != null && !User.IsBot)
                {
                    RolePlayer Rp = User.Roleplayer;
                    if (Rp != null)
                    {
                        Rp.AddExp(this.Config.Health);
                    }
                }

                this.Health = 0;
                this.Dead = true;
                this.DeadTimer = this.Config.DeadTimer;
                this.AggroVirtuelId = 0;
                this.AggroTimer = 0;
                this.ResetBot = false;
                this.ResetBotTimer = 0;
                this.HitCount = 0;
                this.Dodge = false;
                this.DodgeTimer = 0;

                Bot.SetStatus("lay", (Bot.IsPet) ? "" : "0.7");
                Bot.Freeze = true;
                Bot.FreezeEndCounter = 0;
                Bot.IsLay = true;
                Bot.UpdateNeeded = true;

                if (this.Config.MoneyDrop > 0)
                    Room.GetRoomItemHandler().AddTempItem(Bot.VirtualId, this.Config.DropScriptId, Bot.SetX, Bot.SetY, Bot.Z, "1", this.Config.MoneyDrop, InteractionTypeTemp.MONEY);
                if (this.Config.LootItemId > 0)
                {
                    RPItem Item = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(this.Config.LootItemId);
                    if (Item != null)
                        Room.GetRoomItemHandler().AddTempItem(Bot.VirtualId, 3996, Bot.SetX, Bot.SetY, Bot.Z, Item.Name, this.Config.LootItemId, InteractionTypeTemp.RPITEM);
                }

                Bot.OnChat("Esto ha sido muy fuerte para mi :( ! [" + this.Health + "/" + this.Config.Health + "]", (Bot.IsPet) ? 0 : 2, true);
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("UPDATE user_rp SET killed = killed + 1 WHERE user_id = " + User.GetClient().GetHabbo().Id + " LIMIT 1");
            }
            else
            {
                this.Health -= Dmg;
                this.SlowTimer = 6;

                this.ResetBot = false;
                this.ResetBotTimer = 60;

                this.AggroTimer = 0;

                if (pTeamId == -1 || pTeamId != this.Config.TeamId)
                    this.AggroVirtuelId = pAggroVId;

                if (!this.Dodge)
                {
                    this.HitCount += 1;
                    if (this.HitCount % this.DodgeStartCount == 0)
                    {
                        this.Dodge = true;
                        this.DodgeTimer = 3;
                        this.DodgeStartCount = AkiledEnvironment.GetRandomNumber(2, 4);
                    }
                }

                Bot.OnChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.hit", Room.RoomData.Langue), this.Health, this.Config.Health, Dmg), (Bot.IsPet) ? 0 : 2, true);
            }
        }

        private void Pan(RoomUser Bot, Room Room)
        {
            MovementDirection movement = MovementManagement.GetMovementByDirection(Bot.RotBody);

            int WeaponEanble = this.WeaponGun.Enable;

            Bot.ApplyEffect(WeaponEanble, true);
            Bot.TimerResetEffect = this.WeaponGun.FreezeTime + 1;

            if (Bot.FreezeEndCounter <= this.WeaponGun.FreezeTime)
            {
                Bot.Freeze = true;
                Bot.FreezeEndCounter = this.WeaponGun.FreezeTime;
            }

            for (int i = 0; i < this.WeaponGun.FreezeTime; i++)
            {
                if (this.GunCharger <= 0)
                    return;

                this.GunCharger--;
                int Dmg = AkiledEnvironment.GetRandomNumber(this.WeaponGun.DmgMin, this.WeaponGun.DmgMax);

                Room.GetProjectileManager().AddProjectile(Bot.VirtualId, Bot.SetX, Bot.SetY, Bot.SetZ, movement, Dmg, this.WeaponGun.Distance, this.Config.TeamId, true);
            }
        }

        private void Cac(RoomUser Bot, Room Room, RoomUser User)
        {
            int Dmg = AkiledEnvironment.GetRandomNumber(this.WeaponCac.DmgMin, this.WeaponCac.DmgMax);

            if (!User.IsBot)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp != null)
                    Rp.Hit(User, Dmg, Room, false, true);
            }
            else
            {
                if (User.BotData.RoleBot != null)
                    User.BotData.RoleBot.Hit(User, Dmg, Room, Bot.VirtualId, User.BotData.RoleBot.Config.TeamId);
            }

            int WeaponEanble = this.WeaponCac.Enable;

            Bot.ApplyEffect(WeaponEanble, true);
            Bot.TimerResetEffect = this.WeaponCac.FreezeTime + 1;

            if (Bot.FreezeEndCounter <= this.WeaponCac.FreezeTime + 1)
            {
                Bot.Freeze = true;
                Bot.FreezeEndCounter = this.WeaponCac.FreezeTime + 1;
            }
        }

        public void ResetAggro()
        {
            this.AggroVirtuelId = 0;
            this.AggroTimer = 0;
        }

        private void AggroCycle(RoomUser Bot, Room Room)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByVirtualId(this.AggroVirtuelId);
            if (User == null || this.Dead)
            {
                this.ResetAggro();
                return;
            }

            if (this.AggroTimer > 120)
            {
                this.ResetAggro();
                return;
            }
            else
                this.AggroTimer++;

            if (!User.IsBot)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp == null)
                {
                    this.ResetAggro();
                    return;
                }

                if (Rp.Dead || (!Rp.PvpEnable && Rp.AggroTimer <= 0) || Rp.SendPrison || !Rp.PvpEnable)
                {
                    this.ResetAggro();
                    return;
                }
            }
            else
            {
                if (User.BotData.RoleBot == null || User.BotData.RoleBot.Dead)
                {
                    this.ResetAggro();
                    return;
                }
            }

            int BotX = (Bot.SetStep) ? Bot.SetX : Bot.X;
            int BotY = (Bot.SetStep) ? Bot.SetY : Bot.Y;

            int UserX = (User.SetStep) ? User.SetX : User.X;
            int UserY = (User.SetStep) ? User.SetY : User.Y;

            int DistanceX = Math.Abs(UserX - BotX);
            int DistanceY = Math.Abs(UserY - BotY);

            if (DistanceX > this.Config.LostAggroDistance || DistanceY > this.Config.LostAggroDistance)
            {
                this.ResetAggro();
                return;
            }

            if (DistanceX > this.Config.LostAggroDistance || DistanceY > this.Config.LostAggroDistance)
            {
                this.ResetAggro();
                return;
            }

            if (DistanceX > this.Config.LostAggroDistance || DistanceY > this.Config.LostAggroDistance)
            {
                this.ResetAggro();
                return;
            }

            if (Math.Abs(BotX - Bot.BotData.X) > this.Config.ZoneDistance + 10 || Math.Abs(BotY - Bot.BotData.Y) > this.Config.ZoneDistance + 10)
            {
                this.ResetAggro();
                return;
            }

            if (Bot.Freeze)
                return;


            if (!this.BotPathFind(Bot, Room, User))
                return;

            int Rot = Rotation.Calculate(BotX, BotY, UserX, UserY);

            Bot.RotHead = Rot;
            Bot.RotBody = Rot;
            Bot.UpdateNeeded = true;

            this.AggroTimer = 0;

            if (this.WeaponCac.Id != 0 && (DistanceX < 2 && DistanceY < 2))
            {
                this.Cac(Bot, Room, User);
            }
            else if (this.WeaponGun.Id != 0)
            {
                this.Pan(Bot, Room);
            }
        }

        private bool BotPathFind(RoomUser Bot, Room Room, RoomUser User)
        {
            int BotX = (Bot.SetStep) ? Bot.SetX : Bot.X;
            int BotY = (Bot.SetStep) ? Bot.SetY : Bot.Y;

            int UserX = (User.SetStep) ? User.SetX : User.X;
            int UserY = (User.SetStep) ? User.SetY : User.Y;

            int DistanceX = Math.Abs(UserX - BotX);
            int DistanceY = Math.Abs(UserY - BotY);

            if (this.Dodge)
            {
                this.DodgeTimer--;
                if (this.DodgeTimer <= 0)
                {
                    this.Dodge = false;
                    this.HitCount = 0;
                }

                if (!Bot.IsWalking)
                {
                    if (DistanceX < DistanceY)
                    {
                        Bot.MoveTo(UserX + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? 1 : -1), BotY, true);
                    }
                    else
                    {
                        Bot.MoveTo(BotX, UserY + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? 1 : -1), true);
                    }
                }
                return false;
            }

            if (this.WeaponCac.Id == 0 && (this.WeaponGun.Id == 0 || this.GunCharger == 0)) //Fuite
            {
                if (Bot.IsWalking)
                    return false;

                if (DistanceX > DistanceY)
                {
                    if (User.X > BotX)
                        Bot.MoveTo(BotX, BotY + AkiledEnvironment.GetRandomNumber(1, 3), true);
                    else
                        Bot.MoveTo(BotX, BotY - AkiledEnvironment.GetRandomNumber(1, 3), true);
                }
                else
                {
                    if (User.Y > BotY)
                        Bot.MoveTo(BotX - AkiledEnvironment.GetRandomNumber(1, 3), BotY, true);
                    else
                        Bot.MoveTo(BotX + AkiledEnvironment.GetRandomNumber(1, 3), BotY, true);
                }
                return false;
            }

            if ((DistanceX >= 2 || DistanceY >= 2) || this.WeaponCac.Id == 0) //Distance
            {
                if ((this.WeaponGun.Id == 0 || this.GunCharger == 0) && this.WeaponCac.Id != 0) //Déplace le bot au cac si il est uniquement cac
                {
                    Bot.MoveTo(UserX + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), UserY + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
                }
                else if (this.WeaponGun.Id != 0 && this.GunCharger != 0) //Si le bot a une arme distance
                {
                    if ((this.WeaponCac.Id == 0 || this.GunCharger == 0) && ((BotX == User.X && BotY == User.Y) || (BotX == UserX && BotY == UserY))) //Eloigné le bot si l'utilisateur est sur sa case et que le bot n'a pas d'arme cac
                    {
                        Bot.MoveTo(UserX + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -5 : 5), UserY + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -5 : 5), true);
                        return false;
                    }

                    if ((BotX + BotY) == (UserX + UserY) ||
                        (BotX - BotY) == (UserX - UserY) ||
                        (BotX + BotY) == (UserX + UserY) ||
                        (BotX - BotY) == (UserX - UserY) ||
                        (UserX == BotX || UserY == BotY) ||
                        (UserX == BotX || UserY == BotY)) //Bot en position de tirer
                    {
                        int Rot = Rotation.Calculate(BotX, BotY, UserX, UserY);

                        if (this.CheckCollisionDir(Bot, Room, Rot, DistanceX, DistanceY)) //Check si la balle peut passer
                        {
                            Bot.MoveTo(UserX + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), UserY + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
                            return false;
                        }
                    }
                    else
                    {
                        if (DistanceX < 3 && DistanceY < 3)
                        {
                            Bot.MoveTo(UserX + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), UserY + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -1 : 1), true);
                        }
                        else
                        {
                            if (Bot.IsWalking)
                                return false;

                            if (DistanceX < DistanceY)
                            {
                                Bot.MoveTo(UserX, (DistanceY > 5) ? BotY - AkiledEnvironment.GetRandomNumber(1, 2) : BotY + AkiledEnvironment.GetRandomNumber(1, 2), true);
                            }
                            else
                            {
                                Bot.MoveTo((DistanceX > 5) ? BotX - AkiledEnvironment.GetRandomNumber(1, 2) : BotX + AkiledEnvironment.GetRandomNumber(1, 2), UserY, true);
                            }
                        }
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckCollisionDir(RoomUser Bot, Room Room, int Rot, int DistanceX, int DistanceY)
        {
            int BotX = (Bot.SetStep) ? Bot.SetX : Bot.X;
            int BotY = (Bot.SetStep) ? Bot.SetY : Bot.Y;
            double BotZ = (Bot.SetStep) ? Bot.SetZ : Bot.Z;

            if (this.WeaponGun.Distance < DistanceX || this.WeaponGun.Distance < DistanceY)
                return true;

            if (Rot == 2)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX + i, BotY, true) || Room.GetGameMap().SqAbsoluteHeight(BotX + i, BotY) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 6)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX - i, BotY, true) || Room.GetGameMap().SqAbsoluteHeight(BotX - i, BotY) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 4)
            {
                for (int i = 1; i < DistanceY; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX, BotY + i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX, BotY + i) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 0)
            {
                for (int i = 1; i < DistanceY; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX, BotY - i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX, BotY - i) > (BotZ + 0.5))
                        return true;
                }
            }
            //diago
            else if (Rot == 7)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX - i, BotY - i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX - i, BotY - i) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 1)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX + i, BotY - i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX + i, BotY - i) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 3)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX + i, BotY + i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX + i, BotY + i) > (BotZ + 0.5))
                        return true;
                }
            }
            else if (Rot == 5)
            {
                for (int i = 1; i < DistanceX; i++)
                {
                    if (!Room.GetGameMap().CanStackItem(BotX - i, BotY + i, true) || Room.GetGameMap().SqAbsoluteHeight(BotX - i, BotY + i) > (BotZ + 0.5))
                        return true;
                }
            }

            return false;
        }

        private void CheckResetBot(RoomUser Bot, Room Room)
        {
            if ((this.Config.ResetPosition && (this.IsAllowZone(Bot) || !Bot.IsWalking) && this.Health == this.Config.Health) || (!this.Config.ResetPosition && this.Health == this.Config.Health))
                this.ResetBotTimer = 0;
            else
                this.ResetBotTimer--;

            this.Health = (this.Health + 25 >= this.Config.Health) ? this.Config.Health : this.Health + 25;

            if (this.ResetBotTimer <= 0)
            {
                this.ResetBot = false;
                this.ResetBotTimer = 0;

                this.Health = this.Config.Health;

                if (this.Config.ResetPosition && !this.IsAllowZone(Bot))
                {
                    Bot.RotHead = Bot.BotData.Rot;
                    Bot.RotBody = Bot.BotData.Rot;
                    Room.SendPacket(Room.GetRoomItemHandler().TeleportUser(Bot, new Point(Bot.BotData.X, Bot.BotData.Y), 0, Room.GetGameMap().SqAbsoluteHeight(Bot.BotData.X, Bot.BotData.Y)));
                }
            }
        }

        private void FreeTimeCycle(RoomUser Bot)
        {
            if (!this.IsAllowZone(Bot) || this.Health != this.Config.Health)
            {
                this.ResetBot = true;
                this.ResetBotTimer = 60;

                if (this.Config.ResetPosition && !this.IsAllowZone(Bot))
                    Bot.MoveTo(Bot.BotData.X, Bot.BotData.Y);


                if (this.Health != this.Config.Health)
                {
                    Bot.ApplyEffect(4, true);
                    Bot.TimerResetEffect = 4;
                }

                return;
            }

            //Action bot
            if (this.ActionTimer > 0)
            {
                this.ActionTimer--;
                return;
            }

            if (Bot.IsWalking)
                return;

            this.ActionTimer = AkiledEnvironment.GetRandomNumber(15, 30);
            if (this.ActionTimer >= 25 && !this.Config.ZombieMode)
            {
                if (this.ActionTimer == 30)
                {
                    if (Bot.RotBody != Bot.BotData.Rot)
                    {
                        Bot.RotHead = Bot.BotData.Rot;
                        Bot.RotBody = Bot.BotData.Rot;
                        Bot.UpdateNeeded = true;
                    }
                }
                else
                {
                    if (Bot.IsSit)
                    {
                        Bot.RemoveStatus("sit");
                        Bot.IsSit = false;
                        Bot.UpdateNeeded = true;
                    }
                    else
                    {
                        if (Bot.RotBody % 2 == 0)
                        {
                            if (Bot.IsPet)
                                Bot.SetStatus("sit", "");
                            else
                                Bot.SetStatus("sit", "0.5");
                            Bot.IsSit = true;
                            Bot.UpdateNeeded = true;
                        }
                    }
                }
            }
            else
            {
                if (this.Config.ZoneDistance > 0)
                {
                    //Bouge le bot aléatoirement dans sa zone
                    int LenghtX = AkiledEnvironment.GetRandomNumber(0, this.Config.ZoneDistance);
                    int LenghtY = AkiledEnvironment.GetRandomNumber(0, this.Config.ZoneDistance);
                    Bot.MoveTo(Bot.BotData.X + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -LenghtX : LenghtX), Bot.BotData.Y + ((AkiledEnvironment.GetRandomNumber(1, 2) == 2) ? -LenghtY : LenghtY), true);
                }
            }
        }

        private void AggroSearch(RoomUser Bot, Room Room)
        {
            List<RoomUser> Users = Room.GetGameMap().GetNearUsers(new Point(Bot.X, Bot.Y), this.Config.AggroDistance);
            if (Users == null)
                return;

            int BotX = (Bot.SetStep) ? Bot.SetX : Bot.X;
            int BotY = (Bot.SetStep) ? Bot.SetY : Bot.Y;

            int DistanceUserNow = 99;

            foreach (RoomUser User in Users)
            {
                if (User == Bot)
                    continue;

                int RotationDistance = Math.Abs(Rotation.Calculate(BotX, BotY, User.X, User.Y) - Bot.RotBody);
                if (RotationDistance >= 2 && !(User.X == BotX && User.Y == BotY))
                    continue;

                if (!User.IsBot)
                {
                    RolePlayer Rp = User.Roleplayer;
                    if (Rp == null)
                        continue;

                    if (Rp.Dead || (!Rp.PvpEnable && Rp.AggroTimer <= 0) || Rp.SendPrison)
                        continue;
                }
                else
                {
                    if (User.BotData.RoleBot == null || (User.BotData.RoleBot.Dead || this.Config.TeamId == User.BotData.RoleBot.Config.TeamId))
                        continue;
                }

                int UserX = (User.SetStep) ? User.SetX : User.X;
                int UserY = (User.SetStep) ? User.SetY : User.Y;

                int DistanceX = Math.Abs(UserX - BotX);
                int DistanceY = Math.Abs(UserY - BotY);

                int DistanceUser = DistanceX + DistanceY;

                if (DistanceUser >= DistanceUserNow)
                    continue;

                DistanceUserNow = DistanceUser;

                this.ResetBot = false;
                this.ResetBotTimer = 60;
                this.AggroVirtuelId = User.VirtualId;
                this.AggroTimer = 0;
            }
        }
    }
}
