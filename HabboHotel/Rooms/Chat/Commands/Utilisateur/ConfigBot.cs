using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ConfigBot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
                return;

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
                return;

            switch (Params[2])
            {
                case "enable":
                    {
                        if (Params.Length < 4)
                            break;

                        int.TryParse(Params[3], out int IntValue);

                        if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(IntValue, false))
                            return;

                        if (Bot.CurrentEffect != IntValue)
                            Bot.ApplyEffect(IntValue);

                        if (Bot.BotData.Enable != IntValue)
                        {
                            Bot.BotData.Enable = IntValue;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET enable = '" + IntValue + "' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
                case "handitem":
                    {
                        if (Params.Length < 4)
                            break;

                        int.TryParse(Params[3], out int IntValue);

                        if (Bot.CarryItemID != IntValue)
                            Bot.CarryItem(IntValue, true);

                        if (Bot.BotData.Handitem != IntValue)
                        {
                            Bot.BotData.Handitem = IntValue;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET handitem = '" + IntValue + "' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
                case "rot":
                    {
                        if (Params.Length < 4)
                            break;

                        int.TryParse(Params[3], out int IntValue);
                        IntValue = (IntValue > 7 || IntValue < 0) ? 0 : IntValue;

                        if (Bot.RotBody != IntValue)
                        {
                            Bot.RotBody = IntValue;
                            Bot.RotHead = IntValue;
                            Bot.UpdateNeeded = true;
                        }

                        if (Bot.BotData.Rot != IntValue)
                        {
                            Bot.BotData.Rot = IntValue;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET rotation = '" + Bot.RotBody + "' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
                case "sit":
                    {
                        if (Bot.BotData.Status == 1)
                        {
                            Bot.BotData.Status = 0;

                            Bot.RemoveStatus("sit");
                            Bot.IsSit = false;
                            Bot.UpdateNeeded = true;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET status = '0' WHERE id = " + Bot.BotData.Id);
                        }
                        else
                        {
                            if (!Bot.IsSit)
                            {
                                Bot.SetStatus("sit", (Bot.IsPet) ? "" : "0.5");
                                Bot.IsSit = true;
                                Bot.UpdateNeeded = true;
                            }

                            Bot.BotData.Status = 1;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET status = '1' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
                case "lay":
                    {
                        if (Bot.BotData.Status == 2)
                        {
                            Bot.BotData.Status = 0;

                            Bot.RemoveStatus("lay");
                            Bot.IsSit = false;
                            Bot.UpdateNeeded = true;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET status = '0' WHERE id = " + Bot.BotData.Id);
                        }
                        else
                        {
                            if (!Bot.IsLay)
                            {
                                Bot.SetStatus("lay", (Bot.IsPet) ? "" : "0.7");
                                Bot.IsLay = true;
                                Bot.UpdateNeeded = true;
                            }

                            Bot.BotData.Status = 2;

                            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryreactor.RunQuery("UPDATE bots SET status = '2' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
            }
        }
    }
}
