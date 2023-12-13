using Akiled.Database.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace Akiled.HabboHotel.Items
{
    public class MoodlightData
    {
        public int ItemId { get; set; }
        public int CurrentPreset { get; set; }
        public bool Enabled { get; set; }

        public List<MoodlightPreset> Presets { get; set; }


        public MoodlightData(int itemId, bool enabled, int currentPreset, string presetOne, string presetTwo, string presetThree)
        {
            this.ItemId = itemId;

            this.Enabled = enabled;
            this.CurrentPreset = currentPreset;
            this.Presets = new List<MoodlightPreset>
        {
            GeneratePreset(presetOne),
            GeneratePreset(presetTwo),
            GeneratePreset(presetThree)
        };
        }

        public void Enable()
        {
            Enabled = true;

            using IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.RunQuery("UPDATE `room_items_moodlight` SET enabled = '" + 1 + "' WHERE item_id = '" + ItemId + "' LIMIT 1");
        }

        public void Disable()
        {
            Enabled = false;

            using IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.RunQuery("UPDATE room_items_moodlight SET enabled = 0 WHERE item_id = '" + ItemId + "' LIMIT 1");
        }

        public void UpdatePreset(int Preset, string color, int Intensity, bool BgOnly, bool Hax = false)
        {
            if (!IsValidColor(color) || !IsValidIntensity(Intensity) && !Hax)
            {
                return;
            }

            string Pr = Preset switch
            {
                3 => "three",
                2 => "two",
                _ => "one",
            };



            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `room_items_moodlight` SET `preset_" + Pr + "` = @color WHERE item_id = '" + ItemId + "' LIMIT 1");
                dbClient.AddParameter("color", color + "," + Intensity + "," + (BgOnly ? "1" : "0"));
                dbClient.RunQuery();

            }

            GetPreset(Preset).ColorCode = color;
            GetPreset(Preset).ColorIntensity = Intensity;
            GetPreset(Preset).BackgroundOnly = BgOnly;
        }

        public static MoodlightPreset GeneratePreset(string Data)
        {
            var bits = Data.Split(',');

            if (!IsValidColor(bits[0]))
            {
                bits[0] = "#000000";
            }

            return new MoodlightPreset(bits[0], int.Parse(bits[1]), bits[2] == "1");
        }

        public MoodlightPreset GetPreset(int i)
        {
            i--;

            if (this.Presets[i] != null)
            {
                return this.Presets[i];
            }

            return new MoodlightPreset("#000000", 255, false);
        }

        public static bool IsValidColor(string ColorCode) => ColorCode switch
        {
            "#000000" or "#0053F7" or "#EA4532" or "#82F349" or "#74F5F5" or "#E759DE" or "#F2F851" => true,
            _ => false,
        };

        public static bool IsValidIntensity(int Intensity)
        {
            if (Intensity is < 0 or > 255)
            {
                return false;
            }

            return true;
        }

        public string GenerateExtraData()
        {
            var preset = this.GetPreset(this.CurrentPreset);
            var sb = new StringBuilder()
            .Append(this.Enabled ? 2 : 1)
            .Append(',')
            .Append(this.CurrentPreset)
            .Append(',')
            .Append(preset.BackgroundOnly ? 2 : 1)
            .Append(',')
            .Append(preset.ColorCode)
            .Append(',')
            .Append(preset.ColorIntensity);

            return sb.ToString();
        }
    }
}
