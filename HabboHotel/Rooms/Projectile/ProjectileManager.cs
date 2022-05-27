using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Map.Movement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Projectile
{
    public class ProjectileManager
    {
        private List<ItemTemp> _projectile;
        private ConcurrentQueue<ItemTemp> _queueProjectile;
        private Room _room;

        private List<ServerPacket> _messages;

        public ProjectileManager(Room room)
        {
            this._projectile = new List<ItemTemp>();
            this._queueProjectile = new ConcurrentQueue<ItemTemp>();
            this._room = room;
            this._messages = new List<ServerPacket>();
        }

        public void OnCycle()
        {
            if (_projectile.Count == 0 && _queueProjectile.Count == 0) return;

            foreach (ItemTemp Item in _projectile.ToArray())
            {
                if (Item == null)
                    continue;

                bool EndProjectile = false;
                List<RoomUser> UsersTouch = new List<RoomUser>();
                Point newPoint = new Point(Item.X, Item.Y);
                int newX = Item.X;
                int newY = Item.Y;
                double newZ = Item.Z;

                if (Item.InteractionType == InteractionTypeTemp.GRENADE)
                {
                    newPoint = MovementManagement.GetMoveCoord(Item.X, Item.Y, 1, Item.Movement);
                    newX = newPoint.X;
                    newY = newPoint.Y;

                    if (Item.Distance > 2)
                        newZ += 1;
                    else
                        newZ -= 1;

                    if (Item.Distance <= 0)
                    {
                        //explosion
                        UsersTouch = _room.GetGameMap().GetNearUsers(new Point(newPoint.X, newPoint.Y), 2);

                        EndProjectile = true;
                    }

                    Item.Distance--;
                }
                else
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        newPoint = MovementManagement.GetMoveCoord(Item.X, Item.Y, i, Item.Movement);

                        UsersTouch = _room.GetGameMap().GetRoomUsers(newPoint);

                        foreach (RoomUser UserTouch in UsersTouch)
                        {
                            if (CheckUserTouch(UserTouch, Item))
                                EndProjectile = true;
                        }

                        if ((_room.GetGameMap().CanStackItem(newPoint.X, newPoint.Y, true) && (_room.GetGameMap().SqAbsoluteHeight(newPoint.X, newPoint.Y) <= Item.Z + 0.5)))
                        {
                            newX = newPoint.X;
                            newY = newPoint.Y;
                        }
                        else
                        {
                            EndProjectile = true;
                        }

                        if (EndProjectile)
                            break;

                        Item.Distance--;
                        if (Item.Distance <= 0)
                        {
                            EndProjectile = true;
                            break;
                        }
                    }
                }

                ServerPacket Message = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
                Message.WriteInteger(Item.X);
                Message.WriteInteger(Item.Y);
                Message.WriteInteger(newX);
                Message.WriteInteger(newY);
                Message.WriteInteger(1);
                Message.WriteInteger(Item.Id);
                Message.WriteString(Item.Z.ToString().Replace(',', '.'));
                Message.WriteString(newZ.ToString().Replace(',', '.'));
                Message.WriteInteger(0);
                this._messages.Add(Message);

                Item.X = newX;
                Item.Y = newY;
                Item.Z = newZ;

                if (EndProjectile)
                {
                    foreach (RoomUser UserTouch in UsersTouch)
                        this.CheckUserHit(UserTouch, Item);

                    RemoveProjectile(Item);
                }
            }

            Dictionary<int, int> BulletUser = new Dictionary<int, int>();

            if (this._queueProjectile.Count > 0)
            {
                List<ItemTemp> toAdd = new List<ItemTemp>();
                while (this._queueProjectile.Count > 0)
                {
                    ItemTemp Item = (ItemTemp)null;
                    if (this._queueProjectile.TryDequeue(out Item))
                    {
                        if (!BulletUser.ContainsKey(Item.VirtualUserId))
                        {
                            BulletUser.Add(Item.VirtualUserId, 1);
                            this._projectile.Add(Item);
                        }
                        else
                        {
                            BulletUser[Item.VirtualUserId]++;

                            //if (BulletUser[Item.VirtualUserId] > 3)
                            //this._room.GetRoomItemHandler().RemoveTempItem(Item.Id);
                            //else
                            toAdd.Add(Item);
                        }
                    }

                }
                foreach (ItemTemp Item in toAdd)
                    this._queueProjectile.Enqueue(Item);

                toAdd.Clear();
            }

            BulletUser.Clear();

            this._room.SendMessage(_messages);
            this._messages.Clear();
        }

        private void CheckUserHit(RoomUser UserTouch, ItemTemp Item)
        {
            if (UserTouch == null) return;

            if (_room.IsRoleplay)
            {
                if (UserTouch.VirtualId == Item.VirtualUserId) return;

                if (UserTouch.IsBot)
                {
                    if (UserTouch.BotData.RoleBot == null) return;

                    UserTouch.BotData.RoleBot.Hit(UserTouch, Item.Value, this._room, Item.VirtualUserId, Item.TeamId);
                }
                else
                {
                    RolePlayer Rp = UserTouch.Roleplayer;

                    if (Rp == null) return;

                    if (!Rp.PvpEnable && Rp.AggroTimer == 0) return;

                    Rp.Hit(UserTouch, Item.Value, this._room, true, (Item.InteractionType == InteractionTypeTemp.PROJECTILE_BOT));
                }
            }
            else
            {
                _room.GetWiredHandler().TriggerCollision(UserTouch, null);
            }
        }

        private bool CheckUserTouch(RoomUser UserTouch, ItemTemp Item)
        {
            if (UserTouch == null) return false;

            if (!this._room.IsRoleplay) return true;

            if (UserTouch.VirtualId == Item.VirtualUserId) return false;

            if (UserTouch.IsBot)
            {
                if (UserTouch.BotData.RoleBot == null) return false;

                if (UserTouch.BotData.RoleBot.Dead) return false;

                return true;
            }
            else
            {
                RolePlayer Rp = UserTouch.Roleplayer;

                if (Rp == null) return false;

                if ((!Rp.PvpEnable && Rp.AggroTimer == 0) || Rp.Dead || Rp.SendPrison) return false;

                return true;
            }
        }


        private void RemoveProjectile(ItemTemp Item)
        {
            if (!this._projectile.Contains(Item))
                return;

            this._projectile.Remove(Item);

            this._room.GetRoomItemHandler().RemoveTempItem(Item.Id);
        }

        public void AddProjectile(int vId, int x, int y, double z, MovementDirection movement, int Dmg = 0, int pDistance = 20, int pTeamId = -1, bool isBot = false)
        {
            ItemTemp Item = this._room.GetRoomItemHandler().AddTempItem(vId, 77151726, x, y, z, "1", Dmg, (isBot) ? InteractionTypeTemp.PROJECTILE_BOT : InteractionTypeTemp.PROJECTILE, movement, pDistance, pTeamId);
            this._queueProjectile.Enqueue(Item);
        }

        public void AddGrenade(int vId, int x, int y, double z, MovementDirection movement, int Dmg = 0, int pDistance = 10, int pTeamId = -1)
        {
            ItemTemp Item = this._room.GetRoomItemHandler().AddTempItem(vId, 3011, x, y, z, "1", Dmg, InteractionTypeTemp.GRENADE, movement, 8, pTeamId);
            this._queueProjectile.Enqueue(Item);
        }
    }
}
