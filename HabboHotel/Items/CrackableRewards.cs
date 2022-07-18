namespace Akiled.HabboHotel.Items
{
    internal class CrackableRewards
    {
        internal uint CrackableId;
        internal uint CrackableLevel;
        internal string CrackableRewardType;
        internal string CrackableReward;

        internal CrackableRewards(
          uint crackableId,
          string crackableRewardType,
          string crackableReward,
          uint crackableLevel)
        {
            this.CrackableId = crackableId;
            this.CrackableRewardType = crackableRewardType;
            this.CrackableReward = crackableReward;
            this.CrackableLevel = crackableLevel;
        }
    }
}
