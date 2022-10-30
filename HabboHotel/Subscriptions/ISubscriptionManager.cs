namespace Akiled.HabboHotel.Subscriptions

{
    public interface ISubscriptionManager
    {
        void Init();
        bool TryGetSubscriptionData(int id, out SubscriptionData data);
    }
}
