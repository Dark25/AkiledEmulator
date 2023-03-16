using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Items
{
    internal class CrackableManager
    {
        internal Dictionary<int, CrackableItem> Crackable;

        internal void Initialize(IQueryAdapter dbClient)
        {
            this.Crackable = new Dictionary<int, CrackableItem>();
            dbClient.SetQuery("SELECT * FROM crackable_rewards");
            foreach (DataRow row in (InternalDataCollectionBase)dbClient.GetTable().Rows)
            {
                if (!this.Crackable.ContainsKey(Convert.ToInt32(row["item_baseid"])))
                    this.Crackable.Add(Convert.ToInt32(row["item_baseid"]), new CrackableItem(row));
            }
        }

        private List<CrackableRewards> GetRewardsByLevel(int itemId, int level)
        {
            List<CrackableRewards> rewardsByLevel = new List<CrackableRewards>();
            foreach (CrackableRewards crackableRewards in this.Crackable[itemId].Rewards.Where<CrackableRewards>((Func<CrackableRewards, bool>)(furni => (long)furni.CrackableLevel == (long)level)))
                rewardsByLevel.Add(crackableRewards);
            return rewardsByLevel;
        }

        internal void ReceiveCrackableReward(RoomUser user, Room room, Item item)
        {
            if (room == null || item == null || item.GetBaseItem().InteractionType != InteractionType.PINATA && item.GetBaseItem().InteractionType != InteractionType.PINATATRIGGERED && item.GetBaseItem().InteractionType != InteractionType.CRACKABLE_EGG || !this.Crackable.ContainsKey(item.GetBaseItem().Id))
                return;
            CrackableItem crackableItem;
            this.Crackable.TryGetValue(item.GetBaseItem().Id, out crackableItem);
            if (crackableItem != null)
            {
                int getX = item.GetX;
                int getY = item.GetY;
                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                int num = new Random().Next(0, 100);
                int level = num >= 95 ? 5 : (num < 85 || num >= 95 ? (num < 65 || num >= 85 ? (num < 35 || num >= 65 ? 1 : 2) : 3) : 4);
                List<CrackableRewards> rewardsByLevel = this.GetRewardsByLevel((int)crackableItem.ItemId, level);
                CrackableRewards reward = rewardsByLevel[new Random().Next(0, rewardsByLevel.Count - 1)];
                Task.Run((Action)(() =>
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        string crackableRewardType = reward.CrackableRewardType;
                        if (crackableRewardType != "credits")
                        {
                            if (crackableRewardType != "duckets")
                            {
                                if (crackableRewardType != "diamonds")
                                {

                                    if (crackableRewardType == "badge")
                                    {
                                        if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(reward.CrackableReward))
                                            return;
                                        user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast ein Badge gezogen: " + int.Parse(reward.CrackableReward).ToString() + "."));
                                        user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                        user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(reward.CrackableReward, 0, true, user.GetClient());
                                        room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                        queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                                        return;
                                    }
                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                    queryReactor.RunQuery("UPDATE items SET base_item = " + int.Parse(reward.CrackableReward).ToString() + ", extra_data = '' WHERE id = " + item.Id.ToString());
                                    item.BaseItem = int.Parse(reward.CrackableReward);
                                    item.ResetBaseItem();
                                    item.ExtraData = string.Empty;
                                    if (!room.GetRoomItemHandler().SetFloorItem(user.GetClient(), item, item.GetX, item.GetY, item.Rotation, true, false, true))
                                    {
                                        queryReactor.RunQuery("UPDATE items SET room_id = 0 WHERE id = " + item.Id.ToString());
                                        user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
                                    }
                                }
                                else
                                {

                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                    user.GetClient().GetHabbo().AkiledPoints += int.Parse(reward.CrackableReward);
                                    user.GetClient().SendMessage((IServerPacket)new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().AkiledPoints, 0, 5));
                                    user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("diamonds", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Diamanten aus der Kiste gezogen!"));
                                    user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                    queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                                    return;
                                }
                            }
                            else
                            {
                                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                user.GetClient().GetHabbo().Duckets += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage((IServerPacket)new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Duckets, user.GetClient().GetHabbo().Duckets));
                                user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("duckets", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Duckets aus der Kiste gezogen!"));
                                user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                                return;
                            }
                        }
                        else
                        {
                            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                            user.GetClient().GetHabbo().Credits += int.Parse(reward.CrackableReward);
                            user.GetClient().SendMessage((IServerPacket)new CreditBalanceComposer(user.GetClient().GetHabbo().Credits));
                            user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("cred", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Taler aus der Kiste gezogen!"));
                            user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                            queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                            return;
                        }
                    }
                    user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                }));
            }
        }

        internal void ReceiveCrackableRewardMare(RoomUser user, Room room, Item item)
        {
            if (room == null || item == null || item.GetBaseItem().InteractionType != InteractionType.MARECRACKABLE_EGG && item.GetBaseItem().InteractionType != InteractionType.PINATA && item.GetBaseItem().InteractionType != InteractionType.PINATATRIGGERED && item.GetBaseItem().InteractionType != InteractionType.CRACKABLE_EGG || !this.Crackable.ContainsKey(item.GetBaseItem().Id))
                return;
            CrackableItem crackableItem;
            this.Crackable.TryGetValue(item.GetBaseItem().Id, out crackableItem);
            if (crackableItem != null)
            {
                int getX = item.GetX;
                int getY = item.GetY;
                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                int num = new Random().Next(0, 100);
                int level = num >= 95 ? 5 : (num < 85 || num >= 95 ? (num < 65 || num >= 85 ? (num < 35 || num >= 65 ? 1 : 2) : 3) : 4);
                List<CrackableRewards> rewardsByLevel = this.GetRewardsByLevel((int)crackableItem.ItemId, level);
                CrackableRewards reward = rewardsByLevel[new Random().Next(0, rewardsByLevel.Count - 1)];
                Task.Run((Action)(() =>
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        string crackableRewardType = reward.CrackableRewardType;
                        if (!(crackableRewardType == "credits"))
                        {
                            if (!(crackableRewardType == "duckets"))
                            {
                                if (!(crackableRewardType == "diamonds"))
                                {

                                    if (crackableRewardType == "badge")
                                    {
                                        if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(reward.CrackableReward))
                                            return;
                                        user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast ein Badge gezogen: " + int.Parse(reward.CrackableReward).ToString() + "."));
                                        user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                        user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(reward.CrackableReward, 0, true, user.GetClient());
                                        room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                        queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                                    }
                                    else
                                    {
                                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                        room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                        queryReactor.RunQuery("UPDATE items SET base_item = " + int.Parse(reward.CrackableReward).ToString() + ", extra_data = '' WHERE id = " + item.Id.ToString());
                                        item.BaseItem = int.Parse(reward.CrackableReward);
                                        item.ResetBaseItem();
                                        item.ExtraData = string.Empty;
                                        queryReactor.RunQuery("UPDATE items SET room_id = 0, user_id = " + user.UserId.ToString() + " WHERE id = " + item.Id.ToString());
                                        user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
                                    }
                                }
                                else
                                {

                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                    user.GetClient().GetHabbo().AkiledPoints += int.Parse(reward.CrackableReward);
                                    user.GetClient().SendMessage((IServerPacket)new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().AkiledPoints, 0, 5));
                                    user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("diamonds", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Diamanten aus der Kiste gezogen!"));
                                    user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                    queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                                }
                            }
                            else
                            {
                                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                                user.GetClient().GetHabbo().Duckets += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage((IServerPacket)new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Duckets, user.GetClient().GetHabbo().Duckets));
                                user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("duckets", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Duckets aus der Kiste gezogen!"));
                                user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                            }
                        }
                        else
                        {
                            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataWhacker", 1);
                            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_PinataBreaker", 1);
                            user.GetClient().GetHabbo().Credits += int.Parse(reward.CrackableReward);
                            user.GetClient().SendMessage((IServerPacket)new CreditBalanceComposer(user.GetClient().GetHabbo().Credits));
                            user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("cred", "Du hast " + int.Parse(reward.CrackableReward).ToString() + " Taler aus der Kiste gezogen!"));
                            user.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("award", "Du hast die Kiste erfolgreich geknackt!"));
                            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                            queryReactor.RunQuery("DELETE FROM items WHERE id = " + item.Id.ToString());
                        }
                    }
                }));
            }
        }
    }
}
