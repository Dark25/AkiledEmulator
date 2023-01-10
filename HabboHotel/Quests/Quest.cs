namespace Akiled.HabboHotel.Quests
{
    public class Quest
    {
        public readonly int Id;
        public readonly string Category;
        public readonly int Number;
        public readonly QuestType GoalType;
        public readonly int GoalData;
        public readonly string Name;
        public readonly int Reward;
        public readonly string DataBit;

        public string ActionName
        {
            get
            {
                return QuestTypeUtillity.GetString(this.GoalType);
            }
        }

        public Quest(int Id, string Category, int Number, QuestType GoalType, int GoalData, string Name, int Reward, string DataBit)
        {
            this.Id = Id;
            this.Category = Category;
            this.Number = Number;
            this.GoalType = GoalType;
            this.GoalData = GoalData;
            this.Name = Name;
            this.Reward = Reward;
            this.DataBit = DataBit;
        }

        public bool IsCompleted(int UserProgress)
        {
            if (this.GoalType != QuestType.EXPLORE_FIND_ITEM)
                return (long)UserProgress >= (long)this.GoalData;
            else
                return UserProgress >= 1;
        }
    }
}
