using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorJukebox : FurniInteractor
  {
    public override void OnPlace(GameClient Session, Item Item)
    {
    }

    public override void OnRemove(GameClient Session, Item Item)
    {
    }

    public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
    {
      if (!UserHasRights || Session == null || Item == null)
        return;


    }
  }
}
