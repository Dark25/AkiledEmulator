using System;
using System.Linq;

using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Users;
using System.Collections.Generic;
using Akiled.HabboHotel.Groups;

namespace Akiled.HabboHotel.Items
{
    static class ItemBehaviourUtility
    {
        public static void GenerateExtradata(Item Item, ServerPacket Message)
        {
            string url_youtube = (AkiledEnvironment.GetConfig().data["url_youtube"]);
            switch (Item.GetBaseItem().InteractionType)
            {
                default:
                    Message.WriteInteger(1);
                    Message.WriteInteger(0);
                    Message.WriteString((Item.GetBaseItem().InteractionType != InteractionType.TONER && Item.GetBaseItem().InteractionType != InteractionType.fbgate) ? Item.ExtraData : string.Empty);
                    break;
                case InteractionType.PURCHASABLE_CLOTHING:
                    Message.WriteInteger(0);
                    Message.WriteInteger(0);
                    Message.WriteString("0");
                    break;
                case InteractionType.WALLPAPER:
                    Message.WriteInteger(2);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);

                    break;
                case InteractionType.FLOOR:
                    Message.WriteInteger(3);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.LANDSCAPE:
                    Message.WriteInteger(4);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.GUILD_FORUM:
                    Group Group = null;
                    if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(Item.GroupId, out Group))
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString(Item.ExtraData);
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(2);
                        Message.WriteInteger(5);
                        Message.WriteString(Item.ExtraData.Split(new char[1] { ';' })[0]);
                        Message.WriteString(Group.Id.ToString());
                        Message.WriteString(Group.Badge);
                        Message.WriteString(AkiledEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                        Message.WriteString(AkiledEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
                    }
                    break;

                case InteractionType.highscore:
                case InteractionType.highscorepoints:
                    Message.WriteInteger(0);

                    Message.WriteInteger(6); //Type

                    Message.WriteString(Item.ExtraData);
                    Message.WriteInteger(2); //Type de victoire
                    Message.WriteInteger(0); //Type de duré


                    Message.WriteInteger((Item.Scores.Count > 20) ? 20 : Item.Scores.Count); //count

                    foreach (KeyValuePair<string, int> score in Item.Scores.OrderByDescending(x => x.Value).Take(20))
                    {
                        Message.WriteInteger(score.Value); //score
                        Message.WriteInteger(1); //(score.Key.Count); //count
                        //foreach(string UsernameScore in score.Key)
                        //Message.AppendString(UsernameScore);
                        Message.WriteString(score.Key);
                    }
                    break;

                case InteractionType.LOVELOCK:
                    if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                    {
                        string[] EData = Item.ExtraData.Split((char)5);
                        int I = 0;
                        Message.WriteInteger(0);
                        Message.WriteInteger(2);
                        Message.WriteInteger(EData.Length);
                        while (I < EData.Length)
                        {
                            Message.WriteString(EData[I]);
                            I++;
                        }
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString("0");
                    }
                    break;

                case InteractionType.CRACKABLE:
                    Message.WriteInteger(0);

                    Message.WriteInteger(7); //Type

                    int.TryParse(Item.ExtraData, out int ClickNumber);

                    Message.WriteString(Item.ExtraData);
                    Message.WriteInteger(ClickNumber);
                    Message.WriteInteger(Item.GetBaseItem().Modes - 1); //Type de duré
                    break;

                case InteractionType.adsbackground:
                    if (!String.IsNullOrEmpty(Item.ExtraData))
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(1);

                        string ExtraDatabackground = "state" + Convert.ToChar(9) + "0" + Convert.ToChar(9) + Item.ExtraData;

                        ExtraDatabackground = ExtraDatabackground.Replace('=', Convert.ToChar(9));
                        Message.WriteInteger(ExtraDatabackground.Split(Convert.ToChar(9)).Length / 2);

                        for (int i = 0; i <= ExtraDatabackground.Split(Convert.ToChar(9)).Length - 1; i++)
                        {
                            string Data = ExtraDatabackground.Split(Convert.ToChar(9))[i];
                            Message.WriteString(Data);
                        }
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString("");
                    }
                    break;
                    
                case InteractionType.EXTRABOX:
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(1);
                        Message.WriteInteger(4);
                        Message.WriteString("MESSAGE");
                        Message.WriteString("Bravo tu as reçu une RareBox ! Ouvre-là pour y découvrir ton lot");
                        Message.WriteString("PURCHASER_NAME");
                        Message.WriteString("Akiled");
                        Message.WriteString("PRODUCT_CODE");
                        Message.WriteString("A1 KUMIANKKA");
                        Message.WriteString("PURCHASER_FIGURE");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.DELUXEBOX:
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(1);
                        Message.WriteInteger(4);
                        Message.WriteString("MESSAGE");
                        Message.WriteString("Bravo tu as reçu une RareBox Deluxe ! Ouvre-là pour y découvrir ton lot");
                        Message.WriteString("PURCHASER_NAME");
                        Message.WriteString("Akiled");
                        Message.WriteString("PRODUCT_CODE");
                        Message.WriteString("A1 KUMIANKKA");
                        Message.WriteString("PURCHASER_FIGURE");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.BADGEBOX:
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(1);
                        Message.WriteInteger(4);
                        Message.WriteString("MESSAGE");
                        Message.WriteString("Bravo tu as reçu une BadgeBox ! Ouvre-là pour y découvrir ton lot");
                        Message.WriteString("PURCHASER_NAME");
                        Message.WriteString("Akiled");
                        Message.WriteString("PRODUCT_CODE");
                        Message.WriteString("A1 KUMIANKKA");
                        Message.WriteString("PURCHASER_FIGURE");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.LEGENDBOX:
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(1);
                        Message.WriteInteger(4);
                        Message.WriteString("MESSAGE");
                        Message.WriteString("Bravo tu as reçu une magnifique LegendBox ! Ouvre-là pour y décrouvrir tes lots !");
                        Message.WriteString("PURCHASER_NAME");
                        Message.WriteString("Akiled");
                        Message.WriteString("PRODUCT_CODE");
                        Message.WriteString("A1 KUMIANKKA");
                        Message.WriteString("PURCHASER_FIGURE");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.GIFT:
                    {
                        if (!Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                        {
                            Message.WriteInteger(0);
                            Message.WriteInteger(0);
                            Message.WriteString(Item.ExtraData);
                        }
                        else
                        {

                            string[] ExtraData = Item.ExtraData.Split(Convert.ToChar(5));
                            int Style = int.Parse(Item.ExtraData.Split(new char[1] { '\x0005' })[1]) * 1000 + int.Parse(Item.ExtraData.Split(new char[1] { '\x0005' })[2]);

                            Habbo Purchaser = AkiledEnvironment.GetHabboById(int.Parse(Item.ExtraData.Split(new char[1] { ';' })[0]));
                            Message.WriteInteger(0);
                            Message.WriteInteger(1);
                            Message.WriteInteger(6);
                            Message.WriteString("EXTRA_PARAM");
                            Message.WriteString("");
                            Message.WriteString("MESSAGE");
                            Message.WriteString(Item.ExtraData.Split(new char[1] { ';' })[1].Split(new char[1] { '\x0005' })[0]);
                            Message.WriteString("PURCHASER_NAME");
                            Message.WriteString(Purchaser == null ? "" : Purchaser.Username);
                            Message.WriteString("PURCHASER_FIGURE");
                            Message.WriteString(Purchaser == null ? "" : Purchaser.Look);
                            Message.WriteString("PRODUCT_CODE");
                            Message.WriteString("A1 KUMIANKKA");
                            Message.WriteString("state");
                            Message.WriteString(Style.ToString());
                        }
                    }
                    break;

                case InteractionType.MANNEQUIN:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    Message.WriteInteger(3);
                    if (Item.ExtraData.Contains(";"))
                    {
                        string[] Stuff = Item.ExtraData.Split(new char[1] { ';' });
                        Message.WriteString("GENDER");
                        Message.WriteString(Stuff[0].ToUpper() == "M" ? "M" : "F");
                        Message.WriteString("FIGURE");
                        Message.WriteString(Stuff[1]);
                        Message.WriteString("OUTFIT_NAME");
                        Message.WriteString(Stuff[2]);
                    }
                    else
                    {
                        Message.WriteString("GENDER");
                        Message.WriteString("M");
                        Message.WriteString("FIGURE");
                        Message.WriteString("");
                        Message.WriteString("OUTFIT_NAME");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.TONER:
                    if (Item.ExtraData.Contains(","))
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(5);
                        Message.WriteInteger(4);
                        Message.WriteInteger(Item.ExtraData.StartsWith("on") ? 1 : 0);
                        Message.WriteInteger(int.Parse(Item.ExtraData.Split(new char[1] { ',' })[1]));
                        Message.WriteInteger(int.Parse(Item.ExtraData.Split(new char[1] { ',' })[2]));
                        Message.WriteInteger(int.Parse(Item.ExtraData.Split(new char[1] { ',' })[3]));
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString(string.Empty);
                    }
                    break;

                case InteractionType.BADGE_DISPLAY:
                case InteractionType.BADGE_TROC:
                    Message.WriteInteger(0);
                    Message.WriteInteger(2);
                    Message.WriteInteger(4);
                    
                    if (Item.ExtraData.Contains(Convert.ToChar(9).ToString()))
                    {
                        string[] BadgeData = Item.ExtraData.Split(Convert.ToChar(9));

                        Message.WriteString("0");//No idea
                        Message.WriteString(BadgeData[0]);//Badge name
                        Message.WriteString(BadgeData[1]);//Owner
                        Message.WriteString(BadgeData[2]);//Date
                    }
                    else
                    {
                        Message.WriteString("0");//No idea
                        Message.WriteString(Item.ExtraData);//Badge name
                        Message.WriteString("");//Owner
                        Message.WriteString("");//Date
                    }
                    break;

                case InteractionType.tvyoutube:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    Message.WriteInteger(2);
                    Message.WriteString("THUMBNAIL_URL");
                    Message.WriteString((string.IsNullOrEmpty(Item.ExtraData)) ? "" : url_youtube + Item.ExtraData); //"https://i1.ytimg.com/vi/" + Item.ExtraData + "/default.jpg")
                    Message.WriteString("VideoId");
                    Message.WriteString(Item.ExtraData);
                    break;
            }
        }

        public static void GenerateWallExtradata(Item Item, ServerPacket Message)
        {
            switch (Item.GetBaseItem().InteractionType)
            {
                default:
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.POSTIT:
                    Message.WriteString((Item.ExtraData.Contains(' ')) ? Item.ExtraData.Split(' ')[0] : "");
                    break;
            }
        }
    }
}