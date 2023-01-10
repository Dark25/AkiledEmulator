using System.Collections.Generic;
namespace Akiled.HabboHotel.Guides
{
    public class GuideManager
    {
        public Dictionary<int, bool> GuidesOnDuty = new Dictionary<int, bool>();

        public int GuidesCount
        {
            get
            {
                return this.GuidesOnDuty.Count;
            }
        }

        public int GetRandomGuide()
        {
            if (this.GuidesCount == 0) return 0;

            List<int> List = new List<int>();

            foreach (KeyValuePair<int, bool> entry in this.GuidesOnDuty)
            {
                if (entry.Value) continue;

                List.Add(entry.Key);
            }

            if (List.Count == 0) return 0;

            int RandomId = List[AkiledEnvironment.GetRandomNumber(0, List.Count - 1)];
            this.GuidesOnDuty[RandomId] = true;

            return RandomId;
        }

        public void EndService(int Id)
        {
            if (!this.GuidesOnDuty.ContainsKey(Id)) return;

            this.GuidesOnDuty[Id] = false;
        }

        public void AddGuide(int guide)
        {
            if (this.GuidesOnDuty.ContainsKey(guide)) return;

            this.GuidesOnDuty.Add(guide, false);
        }

        public void RemoveGuide(int guide)
        {
            if (!this.GuidesOnDuty.ContainsKey(guide)) return;

            this.GuidesOnDuty.Remove(guide);
        }
    }
}