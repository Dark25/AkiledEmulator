using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Refresh : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            if (Params.Length != 2)
                return;

            string Cmd = Params[1];

            if (string.IsNullOrEmpty(Cmd))
                return;

            switch (Cmd)
            {
                case "view":
                case "vue":
                    {
                        AkiledEnvironment.GetGame().GetHotelView().InitHotelViewPromo();
                        break;
                    }
                case "text":
                case "texte":
                case "locale":
                    {
                        AkiledEnvironment.GetLanguageManager().InitLocalValues();
                        break;
                    }

                case "AkiledGames":
                case "habbogame":
                    {
                        AkiledEnvironment.GetGame().GetAnimationManager().Init();
                        break;
                    }
                case "autogame":
                    {
                        
                        if(!AkiledEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
                        {
                            UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", Session.Langue));
                        } else
                        {
                            UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", Session.Langue));
                        }
                         
                        break;
                    }
                case "rpitems":
                    {
                        AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().Init();
                        break;
                    }
                case "rpweapon":
                    {
                        AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init();
                        break;
                    }
                case "rpenemy":
                    {
                        AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init();
                        break;
                    }
                case "cmd":
                case "commands":
                case "comandos":
                    {
                        AkiledEnvironment.GetGame().GetChatManager().GetCommands().Init();
                        break;
                    }
                case "role":
                case "permisos":
                case "permissions":
                    {
                        AkiledEnvironment.GetGame().GetRoleManager().Init();
                        break;
                    }
                case "effet":
                    {
                        AkiledEnvironment.GetGame().GetEffectsInventoryManager().init();
                        break;
                    }
                case "rp":
                case "roleplay":
                    {
                        AkiledEnvironment.GetGame().GetRoleplayManager().Init();
                        break;
                    }
                case "modo":
                case "moderation":
                    {
                        AkiledEnvironment.GetGame().GetModerationManager().Init();
                        break;
                    }
                case "catalogue":
                case "cata":
                    {
                        AkiledEnvironment.GetGame().GetItemManager().Init();
                        AkiledEnvironment.GetGame().GetCatalog().Init(AkiledEnvironment.GetGame().GetItemManager());
                        AkiledEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        break;
                    }
                case "navigateur":
                case "navi":
                    {
                        AkiledEnvironment.GetGame().GetNavigator().Init();
                        break;
                    }
                case "filter":
                case "filtre":
                    {
                        AkiledEnvironment.GetGame().GetChatManager().GetFilter().Init();
                        break;
                    }
                case "items":
                    {
                        AkiledEnvironment.GetGame().GetItemManager().Init();
                        break;
                    }
                case "model":
                    AkiledEnvironment.GetGame().GetRoomManager().LoadModels();
                    break;
                case "mutant":
                case "figure":
                    {
                        AkiledEnvironment.GetFigureManager().Init();
                        break;
                    }
                case "notiftop":
                case "pushtop":
                    {
                        AkiledEnvironment.GetGame().GetNotifTopManager().Init();
                        break;
                    }
                default:
                    {
                        UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.notfound", Session.Langue));
                        return;
                    }
            }
            UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.refresh", Session.Langue));
        }
    }
}
