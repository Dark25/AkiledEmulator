using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.RoomBots;
using Akiled.HabboHotel.Users;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomUser> Users)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            WriteInteger(Users.Count);
            foreach (RoomUser User in Users.ToList())
            {
                WriteUser(User);
            }
        }

        public UsersComposer(RoomUser User)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            WriteInteger(1);//1 avatar
            WriteUser(User);
        }

        private void WriteUser(RoomUser User)
        {
            if (User.IsBot)
            {
                WriteInteger(User.BotAI.BaseId);
                WriteString(User.BotData.Name);
                WriteString(User.BotData.Motto);
                if (User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet)
                {
                    WriteString(User.BotData.Look.ToLower() + ((User.PetData.Saddle > 0) ? " 3 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 4 " + User.PetData.Saddle + " 0" : " 2 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + ""));

                }
                else
                    WriteString(User.BotData.Look);
                WriteInteger(User.VirtualId);
                WriteInteger(User.X);
                WriteInteger(User.Y);
                WriteString(User.Z.ToString());
                WriteInteger(2);
                WriteInteger(User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet ? 2 : 4);
                if (User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet)
                {
                    WriteInteger(User.PetData.Type);
                    WriteInteger(User.PetData.OwnerId);
                    WriteString(User.PetData.OwnerName);
                    WriteInteger(1);
                    WriteBoolean(User.PetData.Saddle > 0);
                    WriteBoolean(User.RidingHorse);
                    WriteInteger(0);
                    WriteInteger(0);
                    WriteString("");
                }
                else
                {
                    WriteString(User.BotData.Gender);
                    WriteInteger(User.BotData.OwnerId);
                    WriteString(User.BotData.OwnerName);
                    WriteInteger(6);
                    WriteShort(1);
                    WriteShort(2);
                    WriteShort(3);
                    WriteShort(4);
                    WriteShort(5);
                    WriteShort(6);
                }
            }
            else
            {
                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                {
                    WriteInteger(0);
                    WriteString("");
                    WriteString("");
                    WriteString("");
                    WriteInteger(User.VirtualId);
                    WriteInteger(User.X);
                    WriteInteger(User.Y);
                    WriteString(User.Z.ToString());
                    WriteInteger(0);
                    WriteInteger(1);
                    WriteString("M");
                    WriteInteger(0);
                    WriteInteger(0);
                    WriteString("");

                    WriteString("");//Whats this?
                    WriteInteger(0);
                    WriteBoolean(false);
                }
                else
                {
                    Habbo Habbo = User.GetClient().GetHabbo();

                    Group Group = null;
                    if (Habbo != null)
                    {
                        if (Habbo.FavouriteGroupId > 0)
                        {
                            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(Habbo.FavouriteGroupId, out Group))
                                Group = null;
                        }
                    }

                    if (User.transfbot)
                    {
                        WriteInteger(Habbo.Id);
                        WriteString(Habbo.Username);
                        WriteString("Beep beep.");
                        WriteString(Habbo.Look);
                        WriteInteger(User.VirtualId);
                        WriteInteger(User.X);
                        WriteInteger(User.Y);
                        WriteString(User.Z.ToString());
                        WriteInteger(0);
                        WriteInteger(4);

                        WriteString(Habbo.Gender);
                        WriteInteger(Habbo.Id);
                        WriteString(Habbo.Username);
                        WriteInteger(0);
                    }
                    else if (User.transformation)
                    {
                        WriteInteger(Habbo.Id);
                        WriteString(Habbo.Username);
                        WriteString(Habbo.Motto);
                        WriteString(User.transformationrace + " 2 2 -1 0 3 4 -1 0");

                        WriteInteger(User.VirtualId);
                        WriteInteger(User.X);
                        WriteInteger(User.Y);
                        WriteString(User.Z.ToString());
                        WriteInteger(4);
                        WriteInteger(2);
                        WriteInteger(0);
                        WriteInteger(Habbo.Id);
                        WriteString(Habbo.Username);
                        WriteInteger(1);
                        WriteBoolean(false);
                        WriteBoolean(false);
                        WriteInteger(0);
                        WriteInteger(0);
                        WriteString("");
                    }
                    else
                    {
                        WriteInteger(Habbo.Id);
                        WriteString(Habbo.Username);
                        WriteString(Habbo.Motto);
                        WriteString(Habbo.Look);
                        WriteInteger(User.VirtualId);
                        WriteInteger(User.X);
                        WriteInteger(User.Y);
                        WriteString(User.Z.ToString());
                        WriteInteger(0);
                        WriteInteger(1);
                        WriteString(Habbo.Gender.ToLower());

                        if (Group != null)
                        {
                            WriteInteger(Group.Id);
                            WriteInteger(0);
                            WriteString(Group.Name);
                        }
                        else
                        {
                            WriteInteger(0);
                            WriteInteger(0);
                            WriteString("");
                        }

                        WriteString("");//Whats this?
                        WriteInteger(Habbo.AchievementPoints);
                        WriteBoolean(false);
                    }
                }
            }
        }
    }
}
