using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Incoming.Catalog;
using Akiled.Communication.Packets.Incoming.LandingView;
using Akiled.Communication.Packets.Incoming.Marketplace;
using Akiled.Communication.Packets.Incoming.Navigator;
using Akiled.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using Akiled.Communication.Packets.Incoming.Rooms.Engine;
using Akiled.Communication.Packets.Incoming.Rooms.Furni;
using Akiled.Communication.Packets.Incoming.Rooms.Furni.Moodlight;
using Akiled.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions;
using Akiled.Communication.Packets.Incoming.Sound;
using Akiled.Communication.Packets.Incoming.Structure;
using Akiled.Communication.Packets.Incoming.Users;
using Akiled.Communication.Packets.Incoming.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.WebClients;
using System;
using System.Collections.Generic;

namespace Akiled.Communication.Packets
{
    public sealed class PacketManager
    {
        private readonly Dictionary<int, IPacketEvent> _incomingPackets;
        private readonly Dictionary<int, IPacketWebEvent> _incomingWebPackets;

        public PacketManager()
        {
            _incomingPackets = new Dictionary<int, IPacketEvent>();
            _incomingWebPackets = new Dictionary<int, IPacketWebEvent>();

            this.RegisterHandshake();
            this.RegisterLandingView();
            this.RegisterCatalog();
            this.RegisterNavigator();
            this.RegisterMarketplace();
            this.RegisterNewNavigator();
            this.RegisterRoomAction();
            this.RegisterQuests();
            this.RegisterRoomConnection();
            this.RegisterRoomChat();
            this.RegisterRoomEngine();
            this.RegisterFurni();
            this.RegisterUsers();
            this.RegisterSound();
            this.RegisterMisc();
            this.RegisterInventory();
            this.RegisterPurse();
            this.RegisterRoomAvatar();
            this.RegisterAvatar();
            this.RegisterMessenger();
            this.RegisterGroups();
            this.RegisterRoomSettings();
            this.RegisterPets();
            this.RegisterBots();
            this.FloorPlanEditor();
            this.RegisterModeration();
            this.RegisterGuide();
            this.RegisterNux();
            this.RegisterRoomCamera();

            this.RegisterWebPacket();

            Console.WriteLine("Logged " + _incomingPackets.Count + " packet handler(s)!");
        }

        public void TryExecutePacket(GameClient Session, ClientPacket Packet)
        {
            IPacketEvent Pak = null;

            if (!_incomingPackets.TryGetValue(Packet.Id, out Pak))
            {
                if (AkiledEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Packet.ToString());
                    Console.ResetColor();
                }
                return;
            }

            if (AkiledEnvironment.StaticEvents)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Packet.ToString());
                Console.ResetColor();
            }

            Pak.Parse(Session, Packet);
        }

        public void TryExecuteWebPacket(WebClient Session, ClientPacket Packet)
        {
            if (!_incomingWebPackets.TryGetValue(Packet.Id, out IPacketWebEvent Pak))
            {
                if (AkiledEnvironment.StaticEvents)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Packet.ToString());
                    Console.ResetColor();
                }
                return;
            }

            if (AkiledEnvironment.StaticEvents)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Packet.ToString());
                Console.ResetColor();
            }

            Pak.Parse(Session, Packet);
        }

        private void RegisterWebPacket()
        {
            _incomingWebPackets.Add(1, new SSOTicketWebEvent());
            _incomingWebPackets.Add(2, new SendHotelAlertEvent());
            _incomingWebPackets.Add(3, new EditTvYoutubeEvent());
            _incomingWebPackets.Add(4, new PingWebEvent());
            _incomingWebPackets.Add(6, new MoveUserEvent());
            _incomingWebPackets.Add(7, new FollowUserIdEvent());
            _incomingWebPackets.Add(8, new RpBuyItemsEvent());
            _incomingWebPackets.Add(9, new RpUseItemsEvent());
            _incomingWebPackets.Add(10, new JoinRoomIdEvent());
            _incomingWebPackets.Add(11, new RpTrocAddItemEvent());
            _incomingWebPackets.Add(12, new RpTrocRemoveItemEvent());
            _incomingWebPackets.Add(13, new RpTrocAccepteEvent());
            _incomingWebPackets.Add(14, new RpTrocConfirmeEvent());
            _incomingWebPackets.Add(15, new RpTrocStopEvent());
            _incomingWebPackets.Add(16, new RpBotChooseEvent());
        }

        private void RegisterHandshake()
        {
            _incomingPackets.Add(ClientPacketHeader.GetClientVersionMessageEvent, new GetClientVersionEvent());
            //_incomingPackets.Add(ClientPacketHeader.InitCryptoMessageEvent, new InitCryptoEvent());
            _incomingPackets.Add(ClientPacketHeader.GenerateSecretKeyMessageEvent, new GenerateSecretKeyEvent());
            _incomingPackets.Add(ClientPacketHeader.UniqueIDMessageEvent, new UniqueIDEvent());
            _incomingPackets.Add(ClientPacketHeader.SSOTicketMessageEvent, new SSOTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.InfoRetrieveMessageEvent, new InfoRetrieveEvent());
            _incomingPackets.Add(ClientPacketHeader.PingMessageEvent, new PingEvent());
            _incomingPackets.Add(ClientPacketHeader.ClientActionsMessageEvent, new ClientActionsEvent());
        }

        private void RegisterLandingView()
        {
            _incomingPackets.Add(ClientPacketHeader.RefreshCampaignMessageEvent, new RefreshCampaignEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPromoArticlesMessageEvent, new GetPromoArticlesEvent());
            _incomingPackets.Add(ClientPacketHeader.GET_COMMUNITY_GOAL_HALL_OF_FAME, new GetCommunityGoalHallOfFameEvent());
        }

        private void RegisterNux()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetNuxPresentEvent, new GetNuxPresentEvent());
            this._incomingPackets.Add(ClientPacketHeader.UserNuxEvent, new RoomNuxAlert());
        }

        private void RegisterCatalog()
        {
            _incomingPackets.Add(ClientPacketHeader.GetCatalogIndexMessageEvent, new GetCatalogIndexEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogPageMessageEvent, new GetCatalogPageEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCatalogOfferMessageEvent, new GetCatalogOfferEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogMessageEvent, new PurchaseFromCatalogEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseFromCatalogAsGiftMessageEvent, new PurchaseFromCatalogAsGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent, new GetGiftWrappingConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckPetNameMessageEvent, new CheckPetNameEvent());
            _incomingPackets.Add(ClientPacketHeader.RedeemVoucherMessageEvent, new RedeemVoucherEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSellablePetBreedsMessageEvent, new GetSellablePetBreedsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniConfigMessageEvent, new GetGroupFurniConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMarketplaceConfigurationMessageEvent, new GetMarketplaceConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.CameraPurchaseMessageEvent, new CameraPurchaseEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestCameraConfigurationMessageEvent, new RequestCameraConfigurationEvent());
            _incomingPackets.Add(ClientPacketHeader.GetClubGiftsMessageEvent, new GetClubGiftsEvent());


        }

        private void RegisterMarketplace()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetOffersMessageEvent, new GetOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetOwnOffersMessageEvent, new GetOwnOffersEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent, new GetMarketplaceCanMakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetMarketplaceItemStatsMessageEvent, new GetMarketplaceItemStatsEvent());
            this._incomingPackets.Add(ClientPacketHeader.MakeOfferMessageEvent, new MakeOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.CancelOfferMessageEvent, new CancelOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.BuyOfferMessageEvent, new BuyOfferEvent());
            this._incomingPackets.Add(ClientPacketHeader.RedeemOfferCreditsMessageEvent, new RedeemOfferCreditsEvent());
        }

        private void RegisterNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.AddFavouriteRoomMessageEvent, new AddFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GetUserFlatCatsMessageEvent, new GetUserFlatCatsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteFavouriteRoomMessageEvent, new RemoveFavouriteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToHotelViewMessageEvent, new GoToHotelViewEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateNavigatorSettingsMessageEvent, new UpdateNavigatorSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.CanCreateRoomMessageEvent, new CanCreateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.CreateFlatMessageEvent, new CreateFlatEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGuestRoomMessageEvent, new GetGuestRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.GetEventCategoriesMessageEvent, new GetNavigatorFlatsEvent());
            _incomingPackets.Add(1918, (IPacketEvent)new ToggleStaffPickEvent());

        }

        private void RegisterRoomCamera()
        {
            this._incomingPackets.Add(ClientPacketHeader.SetRoomThumbnailMessageEvent, new CameraRoomThumbnailEvent());
            this._incomingPackets.Add(ClientPacketHeader.RenderRoomEvent, new CameraRoomPictureEvent());
        }

        private void RegisterNewNavigator()
        {
            _incomingPackets.Add(ClientPacketHeader.InitializeNewNavigatorMessageEvent, new InitializeNewNavigatorEvent());
            _incomingPackets.Add(ClientPacketHeader.NavigatorSearchMessageEvent, new NavigatorSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.FindRandomFriendingRoomMessageEvent, new FindRandomFriendingRoomEvent());
        }
        private void RegisterRoomConnection()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenFlatConnectionMessageEvent, new OpenFlatConnectionEvent());
            _incomingPackets.Add(ClientPacketHeader.GoToFlatMessageEvent, new GoToFlatEvent());
        }

        private void RegisterRoomSettings()
        {
            _incomingPackets.Add(ClientPacketHeader.GetRoomSettingsMessageEvent, new GetRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveRoomSettingsMessageEvent, new SaveRoomSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteRoomMessageEvent, new DeleteRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMuteToolMessageEvent, new ToggleMuteToolEvent());


            _incomingPackets.Add(ClientPacketHeader.GetRoomRightsMessageEvent, new GetRoomRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRoomBannedUsersMessageEvent, new GetRoomBannedUsersEvent());
            _incomingPackets.Add(ClientPacketHeader.UnbanUserFromRoomMessageEvent, new UnbanUserFromRoomEvent());

        }

        private void FloorPlanEditor()
        {
            _incomingPackets.Add(ClientPacketHeader.SaveFloorPlanModelMessageEvent, new SaveFloorPlanModelEvent());
            _incomingPackets.Add(ClientPacketHeader.InitializeFloorPlanSessionMessageEvent, new InitializeFloorPlanSessionEvent());
        }

        private void RegisterAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.GetWardrobeMessageEvent, new GetWardrobeEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWardrobeOutfitMessageEvent, new SaveWardrobeOutfitEvent());

        }

        private void RegisterRoomAction()
        {
            _incomingPackets.Add(ClientPacketHeader.LetUserInMessageEvent, new LetUserInEvent());
            _incomingPackets.Add(ClientPacketHeader.BanUserMessageEvent, new BanUserEvent());
            _incomingPackets.Add(ClientPacketHeader.KickUserMessageEvent, new KickUserEvent());
            _incomingPackets.Add(ClientPacketHeader.AssignRightsMessageEvent, new AssignRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveRightsMessageEvent, new RemoveRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveAllRightsMessageEvent, new RemoveAllRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.MuteUserMessageEvent, new MuteUserEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveHandItemMessageEvent, new GiveHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveMyRightsMessageEvent, new RemoveMyRightsEvent());

        }

        private void RegisterRoomEngine()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetRoomEntryDataMessageEvent, new GetRoomEntryDataEvent());
            this._incomingPackets.Add(ClientPacketHeader.GetFurnitureAliasesMessageEvent, new GetFurnitureAliasesMessageEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveAvatarMessageEvent, new MoveAvatarEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveObjectMessageEvent, new MoveObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.PickupObjectMessageEvent, new PickupObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.MoveWallItemMessageEvent, new MoveWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.ApplyDecorationMessageEvent, new ApplyDecorationEvent());
            this._incomingPackets.Add(ClientPacketHeader.PlaceObjectMessageEvent, new PlaceObjectEvent());
            this._incomingPackets.Add(ClientPacketHeader.UseFurnitureMessageEvent, new UseFurnitureEvent());
            this._incomingPackets.Add(ClientPacketHeader.ToggleWallItemEvent, new UseWallItemEvent());
            this._incomingPackets.Add(ClientPacketHeader.AnswerPollMessageEvent, new AnswerPollEvent());
        }

        private void RegisterRoomChat()
        {
            _incomingPackets.Add(ClientPacketHeader.ChatMessageEvent, new ChatEvent());
            _incomingPackets.Add(ClientPacketHeader.ShoutMessageEvent, new ShoutEvent());
            _incomingPackets.Add(ClientPacketHeader.WhisperMessageEvent, new WhisperEvent());
            _incomingPackets.Add(ClientPacketHeader.StartTypingMessageEvent, new StartTypingEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelTypingMessageEvent, new CancelTypingEvent());
        }

        private void RegisterInventory()
        {
            _incomingPackets.Add(ClientPacketHeader.InitTradeMessageEvent, new InitTradeEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingOfferItemMessageEvent, new TradingOfferItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradeOfferMultipleItemsMessageEvent, new TradeOfferMultipleItemsEvent());

            _incomingPackets.Add(ClientPacketHeader.TradingRemoveItemMessageEvent, new TradingRemoveItemEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingAcceptMessageEvent, new TradingAcceptEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelMessageEvent, new TradingCancelEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingConfirmMessageEvent, new TradingConfirmEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingModifyMessageEvent, new TradingModifyEvent());
            _incomingPackets.Add(ClientPacketHeader.TradingCancelConfirmMessageEvent, new TradingCancelConfirmEvent());

            _incomingPackets.Add(ClientPacketHeader.RequestFurniInventoryMessageEvent, new RequestFurniInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgesMessageEvent, new GetBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetAchievementsMessageEvent, new GetAchievementsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetActivatedBadgesMessageEvent, new SetActivatedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBotInventoryMessageEvent, new GetBotInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInventoryMessageEvent, new GetPetInventoryEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectActivatedMessageEvent, new AvatarEffectActivatedEvent());
            _incomingPackets.Add(ClientPacketHeader.AvatarEffectSelectedMessageEvent, new AvatarEffectSelectedEvent());
        }

        private void RegisterPurse()
        {
            this._incomingPackets.Add(ClientPacketHeader.GetCreditsInfoMessageEvent, new GetCreditsInfoEvent());
        }

        private void RegisterMessenger()
        {
            _incomingPackets.Add(ClientPacketHeader.MessengerInitMessageEvent, new MessengerInitEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBuddyRequestsMessageEvent, new GetBuddyRequestsEvent());
            _incomingPackets.Add(ClientPacketHeader.FollowFriendMessageEvent, new FollowFriendEvent());
            _incomingPackets.Add(ClientPacketHeader.FindNewFriendsMessageEvent, new FindNewFriendsEvent());

            _incomingPackets.Add(ClientPacketHeader.RemoveBuddyMessageEvent, new RemoveBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.RequestBuddyMessageEvent, new RequestBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.SendMsgMessageEvent, new SendMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.SendRoomInviteMessageEvent, new SendRoomInviteEvent());
            _incomingPackets.Add(ClientPacketHeader.HabboSearchMessageEvent, new HabboSearchEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptBuddyMessageEvent, new AcceptBuddyEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineBuddyMessageEvent, new DeclineBuddyEvent());
        }

        private void RegisterGroups()
        {
            _incomingPackets.Add(ClientPacketHeader.JoinGroupMessageEvent, new JoinGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupFavouriteMessageEvent, new RemoveGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.SetGroupFavouriteMessageEvent, new SetGroupFavouriteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupInfoMessageEvent, new GetGroupInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupMembersMessageEvent, new GetGroupMembersEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupCreationWindowMessageEvent, new GetGroupCreationWindowEvent());
            _incomingPackets.Add(ClientPacketHeader.GetBadgeEditorPartsMessageEvent, new GetBadgeEditorPartsEvent());
            _incomingPackets.Add(ClientPacketHeader.PurchaseGroupMessageEvent, new PurchaseGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupIdentityMessageEvent, new UpdateGroupIdentityEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupBadgeMessageEvent, new UpdateGroupBadgeEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupColoursMessageEvent, new UpdateGroupColoursEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateGroupSettingsMessageEvent, new UpdateGroupSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.ManageGroupMessageEvent, new ManageGroupEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveAdminRightsMessageEvent, new GiveAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.TakeAdminRightsMessageEvent, new TakeAdminRightsEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveGroupMemberMessageEvent, new RemoveGroupMemberEvent());
            _incomingPackets.Add(ClientPacketHeader.AcceptGroupMembershipMessageEvent, new AcceptGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeclineGroupMembershipMessageEvent, new DeclineGroupMembershipEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteGroupMessageEvent, new DeleteGroupEvent());
        }

        private void RegisterPets()
        {
            _incomingPackets.Add(ClientPacketHeader.RespectPetMessageEvent, new RespectPetEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetInformationMessageEvent, new GetPetInformationEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpPetMessageEvent, new PickUpPetEvent());
            _incomingPackets.Add(ClientPacketHeader.PlacePetMessageEvent, new PlacePetEvent());
            _incomingPackets.Add(ClientPacketHeader.RideHorseMessageEvent, new RideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplyHorseEffectMessageEvent, new ApplyHorseEffectEvent());
            _incomingPackets.Add(ClientPacketHeader.RemoveSaddleFromHorseMessageEvent, new RemoveSaddleFromHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent, new ModifyWhoCanRideHorseEvent());
            _incomingPackets.Add(ClientPacketHeader.GetPetTrainingPanelMessageEvent, new GetPetTrainingPanelEvent());
            _incomingPackets.Add(ClientPacketHeader.MoveMonsterPlanteMessageEvent, new MoveMonsterPlanteEvent());

        }

        private void RegisterQuests()
        {
            _incomingPackets.Add(ClientPacketHeader.GetQuestListMessageEvent, new GetQuestListEvent());
            _incomingPackets.Add(ClientPacketHeader.StartQuestMessageEvent, new StartQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.CancelQuestMessageEvent, new CancelQuestEvent());
            _incomingPackets.Add(ClientPacketHeader.GetCurrentQuestMessageEvent, new GetCurrentQuestEvent());
        }

        private void RegisterFurni()
        {
            _incomingPackets.Add(ClientPacketHeader.UpdateMagicTileMessageEvent, new UpdateMagicTileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetYouTubeTelevisionMessageEvent, new GetYouTubeTelevisionEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredTriggerConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredEffectConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveWiredConditionConfigMessageEvent, new SaveWiredConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBrandingItemMessageEvent, new SaveBrandingItemEvent());
            _incomingPackets.Add(ClientPacketHeader.SetTonerMessageEvent, new SetTonerEvent());
            _incomingPackets.Add(ClientPacketHeader.DiceOffMessageEvent, new DiceOffEvent());
            _incomingPackets.Add(ClientPacketHeader.ThrowDiceMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinNameMessageEvent, new SetMannequinNameEvent());
            _incomingPackets.Add(ClientPacketHeader.SetMannequinFigureMessageEvent, new SetMannequinFigureEvent());
            _incomingPackets.Add(ClientPacketHeader.CreditFurniRedeemMessageEvent, new CreditFurniRedeemEvent());
            _incomingPackets.Add(ClientPacketHeader.GetStickyNoteMessageEvent, new GetStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.AddStickyNoteMessageEvent, new AddStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateStickyNoteMessageEvent, new UpdateStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.DeleteStickyNoteMessageEvent, new DeleteStickyNoteEvent());
            _incomingPackets.Add(ClientPacketHeader.GetMoodlightConfigMessageEvent, new GetMoodlightConfigEvent());
            _incomingPackets.Add(ClientPacketHeader.MoodlightUpdateMessageEvent, new MoodlightUpdateEvent());
            _incomingPackets.Add(ClientPacketHeader.ToggleMoodlightMessageEvent, new ToggleMoodlightEvent());
            _incomingPackets.Add(ClientPacketHeader.UseOneWayGateMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.UseHabboWheelMessageEvent, new UseFurnitureEvent());
            _incomingPackets.Add(ClientPacketHeader.FootballGateSaveLookEvent, new ChangeFootGate());
            _incomingPackets.Add(ClientPacketHeader.OpenGiftMessageEvent, new OpenGiftEvent());
            _incomingPackets.Add(ClientPacketHeader.GetGroupFurniSettingsMessageEvent, new GetGroupFurniSettingsEvent());
            _incomingPackets.Add(ClientPacketHeader.UseSellableClothingMessageEvent, new UseSellableClothingEvent());
            _incomingPackets.Add(ClientPacketHeader.ConfirmLoveLockMessageEvent, new ConfirmLoveLockEvent());
            _incomingPackets.Add(1251, (IPacketEvent)new CraftingSecretEvent());
            _incomingPackets.Add(1173, (IPacketEvent)new ExecuteCraftingRecipeEvent());
            _incomingPackets.Add(633, (IPacketEvent)new GetCraftingItemEvent());
            _incomingPackets.Add(3086, (IPacketEvent)new GetCraftingRecipesAvailableEvent());
            _incomingPackets.Add(1245, (IPacketEvent)new SetCraftingRecipeEvent());
        }

        private void RegisterUsers()
        {
            _incomingPackets.Add(ClientPacketHeader.ScrGetUserInfoMessageEvent, new ScrGetUserInfoMessageEvent());
            _incomingPackets.Add(ClientPacketHeader.SetChatPreferenceMessageEvent, new SetChatPreference());


            _incomingPackets.Add(ClientPacketHeader.RespectUserMessageEvent, new RespectUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UpdateFigureDataMessageEvent, new UpdateFigureDataEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenPlayerProfileMessageEvent, new OpenPlayerProfileEvent());
            _incomingPackets.Add(ClientPacketHeader.GetSelectedBadgesMessageEvent, new GetSelectedBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetRelationshipsMessageEvent, new GetRelationshipsEvent());
            _incomingPackets.Add(ClientPacketHeader.SetRelationshipMessageEvent, new SetRelationshipEvent());
            _incomingPackets.Add(ClientPacketHeader.CheckValidNameMessageEvent, new CheckValidNameEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeNameMessageEvent, new ChangeNameEvent());
            _incomingPackets.Add(ClientPacketHeader.GetHabboGroupBadgesMessageEvent, new GetHabboGroupBadgesEvent());
            _incomingPackets.Add(ClientPacketHeader.GetIgnoredUsersMessageEvent, new GetIgnoredUsersEvent());
        }

        private void RegisterSound()
        {
            _incomingPackets.Add(ClientPacketHeader.SetSoundSettingsMessageEvent, new SetSoundSettingsEvent());
            _incomingPackets.Add(3189, (IPacketEvent)new GetSongInfoEvent());
            _incomingPackets.Add(1435, (IPacketEvent)new GetJukeboxPlayListEvent());
            _incomingPackets.Add(2304, (IPacketEvent)new LoadJukeboxDiscsEvent());
            _incomingPackets.Add(3082, (IPacketEvent)new GetJukeboxDiscsDataEvent());
            _incomingPackets.Add(753, (IPacketEvent)new AddDiscToPlayListEvent());
            _incomingPackets.Add(3050, (IPacketEvent)new RemoveDiscFromPlayListEvent());
        }

        private void RegisterMisc()
        {
            _incomingPackets.Add(ClientPacketHeader.LatencyTestMessageEvent, new LatencyTestEvent());
            _incomingPackets.Add(ClientPacketHeader.SetFriendBarStateMessageEvent, new SetFriendBarStateEvent());
        }

        private void RegisterRoomAvatar()
        {
            _incomingPackets.Add(ClientPacketHeader.ActionMessageEvent, new ActionEvent());
            _incomingPackets.Add(ClientPacketHeader.ApplySignMessageEvent, new ApplySignEvent());
            _incomingPackets.Add(ClientPacketHeader.DanceMessageEvent, new DanceEvent());
            _incomingPackets.Add(ClientPacketHeader.SitMessageEvent, new SitEvent());
            _incomingPackets.Add(ClientPacketHeader.ChangeMottoMessageEvent, new ChangeMottoEvent());
            _incomingPackets.Add(ClientPacketHeader.LookToMessageEvent, new LookToEvent());
            _incomingPackets.Add(ClientPacketHeader.DropHandItemMessageEvent, new DropHandItemEvent());
            _incomingPackets.Add(ClientPacketHeader.GiveRoomScoreMessageEvent, new GiveRoomScoreEvent());
            _incomingPackets.Add(ClientPacketHeader.IgnoreUserMessageEvent, new IgnoreUserEvent());
            _incomingPackets.Add(ClientPacketHeader.UnIgnoreUserMessageEvent, new UnIgnoreUserEvent());
            _incomingPackets.Add(ClientPacketHeader.WhiperGroupMessageEvent, new WhiperGroupEvent());
        }

        private void RegisterBots()
        {
            _incomingPackets.Add(ClientPacketHeader.PlaceBotMessageEvent, new PlaceBotEvent());
            _incomingPackets.Add(ClientPacketHeader.PickUpBotMessageEvent, new PickUpBotEvent());
            _incomingPackets.Add(ClientPacketHeader.OpenBotActionMessageEvent, new OpenBotActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SaveBotActionMessageEvent, new SaveBotActionEvent());
        }

        private void RegisterModeration()
        {
            _incomingPackets.Add(ClientPacketHeader.OpenHelpToolMessageEvent, new OpenHelpToolEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomInfoMessageEvent, new GetModeratorRoomInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserInfoMessageEvent, new GetModeratorUserInfoEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserRoomVisitsMessageEvent, new GetModeratorUserRoomVisitsEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerateRoomMessageEvent, new ModerateRoomEvent());
            _incomingPackets.Add(ClientPacketHeader.ModeratorActionMessageEvent, new ModeratorActionEvent());
            _incomingPackets.Add(ClientPacketHeader.SubmitNewTicketMessageEvent, new SubmitNewTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorRoomChatlogMessageEvent, new GetModeratorRoomChatlogEvent());
            _incomingPackets.Add(ClientPacketHeader.GetModeratorUserChatlogMessageEvent, new GetModeratorUserChatlogEvent());

            _incomingPackets.Add(ClientPacketHeader.PickTicketMessageEvent, new PickTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.ReleaseTicketMessageEvent, new ReleaseTicketEvent());
            _incomingPackets.Add(ClientPacketHeader.CloseTicketMesageEvent, new CloseTicketEvent());

            _incomingPackets.Add(ClientPacketHeader.ModerationMuteMessageEvent, new ModerationMuteEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationKickMessageEvent, new ModerationMuteEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationBanMessageEvent, new ModerationBanEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationMsgMessageEvent, new ModerationMsgEvent());
            _incomingPackets.Add(ClientPacketHeader.ModerationCautionMessageEvent, new ModerationMsgEvent());
        }

        private void RegisterGuide()
        {
            _incomingPackets.Add(ClientPacketHeader.RequestGuideToolEvent, new GetHelperToolConfiguration());
            _incomingPackets.Add(ClientPacketHeader.OnGuideSessionDetached, new OnGuideSessionDetached());
            _incomingPackets.Add(ClientPacketHeader.OnGuide, new OnGuide());
            _incomingPackets.Add(ClientPacketHeader.GuideRecommendHelperEvent, new RecomendHelpers());
            _incomingPackets.Add(ClientPacketHeader.GuideToolMessageNew, new GuideToolMessageNew());
            _incomingPackets.Add(ClientPacketHeader.GuideInviteUserEvent, new GuideInviteToRoom());
            _incomingPackets.Add(ClientPacketHeader.GuideVisitUserEvent, new VisitRoomGuides());
            _incomingPackets.Add(ClientPacketHeader.GuideEndSession, new GuideEndSession());
            _incomingPackets.Add(ClientPacketHeader.CancellInviteGuide, new CancellInviteGuide());
        }
    }
}