using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items.Interactors;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.HabboHotel.Items.Interactor
{
    class InteractorPlantSeed : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {

        }

        public override void OnRemove(GameClient Session, Item Item)
        {

        }

        public override void OnTick(Item item) => throw new NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            var tick = int.Parse(Item.ExtraData);
            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
            {
                tick++;
                Item.ExtraData = tick.ToString();
                Item.UpdateState(true, true);
                int X = Item.GetX, Y = Item.GetY, Rot = Item.Rotation;
                Double Z = Item.GetZ;
                if (tick >= 12)
                {
                    Item.ExtraData = "0";
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_AdvancedHorticulturist", 1);
                }
            }

        }



        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
