using System;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Items
{
    public class ItemData
    {
        public int Id;
        public int SpriteId;
        public string ItemName;
        public string publicname;
        public char Type;
        public int Width;
        public int Length;
        public double Height;
        public bool Stackable;
        public bool Walkable;
        public bool IsSeat;
        public bool AllowEcotronRecycle;
        public bool AllowTrade;
        public bool AllowMarketplaceSell;
        public bool AllowGift;
        public bool AllowInventoryStack;
        public InteractionType InteractionType;
        public int BehaviourData;
        public int Modes;
        public List<int> VendingIds;
        public List<double> AdjustableHeights;
        public int EffectId;
        public bool IsRare;

        public ItemData(int Id, int Sprite, string Name, string Public_name, string Type, int Width, int Length, double Height, bool Stackable, bool Walkable, bool IsSeat,
            bool AllowRecycle, bool AllowTrade, bool AllowGift, bool AllowInventoryStack, InteractionType InteractionType, int behaviourData, int Modes,
            string VendingIds, string AdjustableHeights, int EffectId, bool IsRare)
        {
            this.Id = Id;
            this.SpriteId = Sprite;
            this.ItemName = Name;
            this.publicname = Public_name;
            this.Type = char.Parse(Type);
            this.Width = Width;
            this.Length = Length;
            this.Height = Height;
            this.Stackable = Stackable;
            this.Walkable = Walkable;
            this.IsSeat = IsSeat;
            this.AllowEcotronRecycle = AllowRecycle;
            this.AllowTrade = AllowTrade;
            this.AllowGift = AllowGift;
            this.AllowInventoryStack = AllowInventoryStack;
            this.InteractionType = InteractionType;
            BehaviourData = behaviourData;
            this.Modes = Modes;
            this.VendingIds = new List<int>();
            if (VendingIds.Contains(","))
            {
                foreach (string VendingId in VendingIds.Split(','))
                {
                    try
                    {
                        this.VendingIds.Add(int.Parse(VendingId));
                    }
                    catch
                    {
                        Console.WriteLine("Error with Item " + this.ItemName + " - Vending Ids");
                        continue;
                    }
                }
            }
            else if (!String.IsNullOrEmpty(VendingIds) && (int.Parse(VendingIds)) > 0)
                this.VendingIds.Add(int.Parse(VendingIds));

            this.AdjustableHeights = new List<double>();

            try
            {
                if (AdjustableHeights.Contains(","))
                {
                    foreach (string H in AdjustableHeights.Split(','))
                    {
                        this.AdjustableHeights.Add(double.Parse(H));
                    }
                }

                else if (!String.IsNullOrEmpty(AdjustableHeights) && (double.Parse(AdjustableHeights)) > 0) this.AdjustableHeights.Add(double.Parse(AdjustableHeights));

            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur ID ( " + this.Id + " ) : " + e);
            }

            this.EffectId = EffectId;

            this.IsRare = IsRare;
        }
    }
}