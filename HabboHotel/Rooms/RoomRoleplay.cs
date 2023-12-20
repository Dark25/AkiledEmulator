namespace Akiled.HabboHotel.Rooms;

using Akiled.HabboHotel.Items;
using System;
using System.Linq;

public class RoomRoleplay
{
    private readonly Room _roomInstance;
    public bool Pvp { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Intensity { get; set; }
    public bool CycleHourEffect { get; set; }
    public bool TimeSpeed { get; set; }

    public RoomRoleplay(Room room)
    {
        this._roomInstance = room;

        this.Pvp = true;
        this.CycleHourEffect = true;
        this.TimeSpeed = false;
        this.Hour = -1;
        this.Minute = -1;
        this.Intensity = -1;
    }

    public void OnCycle()
    {
        if (this._roomInstance.RoomData.OwnerName == "AkiledParty")
        {
            return;
        }

        if (!this.Cycle())
        {
            return;
        }

        this.UpdateRpMoodLight();
        this.UpdateRpToner();
        this.UpdateRpBlock();
    }

    private bool Cycle()
    {
        var now = DateTime.Now;

        var hourNow = (int)Math.Floor((double)(((now.Minute * 60) + now.Second) / 150)); //150sec = 2m30s = 1heure dans le rp

        var minuteNow = (int)Math.Floor(((now.Minute * 60) + now.Second - (hourNow * 150)) / 2.5);

        if (hourNow >= 16)
        {
            hourNow = hourNow + 8 - 24;
        }
        else
        {
            hourNow += 8;
        }

        if (this.TimeSpeed)
        {
            hourNow = (int)Math.Floor((double)(now.Second / 2.5));
        }

        if (this.Minute != minuteNow)
        {
            this.Minute = minuteNow;
        }

        if (this.Hour == hourNow)
        {
            return false;
        }

        this.Hour = hourNow;

        if (!this.CycleHourEffect)
        {
            return false;
        }

        if (this.Hour is >= 8 and < 20) //Journée
        {
            this.Intensity = 255;
        }
        else if (this.Hour is >= 20 and < 21)  //Crépuscule
        {
            this.Intensity = 200;
        }
        else if (this.Hour is >= 21 and < 22)  //Crépuscule
        {
            this.Intensity = 150;
        }
        else if (this.Hour is >= 22 and < 23)  //Crépuscule
        {
            this.Intensity = 100;
        }
        else if (this.Hour is >= 23 and < 24)  //Crépuscule
        {
            this.Intensity = 75;
        }
        else if (this.Hour is >= 0 and < 4)  //Nuit
        {
            this.Intensity = 50;
        }
        else if (this.Hour is >= 4 and < 5)  //Aube
        {
            this.Intensity = 75;
        }
        else if (this.Hour is >= 5 and < 6)  //Aube
        {
            this.Intensity = 100;
        }
        else if (this.Hour is >= 6 and < 7)  //Aube
        {
            this.Intensity = 150;
        }
        else if (this.Hour is >= 7 and < 8)  //Aube
        {
            this.Intensity = 200;
        }

        return true;
    }

    private void UpdateRpBlock()
    {
        var roomItems = this._roomInstance.GetRoomItemHandler().GetFloor.Where(i => i.GetBaseItem().Id == 99138022).ToList();
        if (roomItems == null)
        {
            return;
        }

        var useNum = 0;
        if (this.Intensity == 50)
        {
            useNum = 0;
        }
        else if (this.Intensity == 75)
        {
            useNum = 1;
        }
        else if (this.Intensity == 100)
        {
            useNum = 2;
        }
        else if (this.Intensity == 150)
        {
            useNum = 3;
        }
        else if (this.Intensity == 200)
        {
            useNum = 4;
        }
        else if (this.Intensity == 255)
        {
            useNum = 5;
        }

        foreach (var roomItem in roomItems)
        {
            roomItem.ExtraData = useNum.ToString();
            roomItem.UpdateState();
        }
    }

    private void UpdateRpMoodLight()
    {
        if (this._roomInstance.MoodlightData == null)
        {
            return;
        }

        var roomItem = this._roomInstance.GetRoomItemHandler().GetItem(this._roomInstance.MoodlightData.ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
        {
            return;
        }

        this._roomInstance.MoodlightData.Enabled = true;
        this._roomInstance.MoodlightData.CurrentPreset = 1;
        this._roomInstance.MoodlightData.UpdatePreset(1, "#000000", this.Intensity, false);
        roomItem.ExtraData = this._roomInstance.MoodlightData.GenerateExtraData();
        roomItem.UpdateState();
    }

    private void UpdateRpToner()
    {
        var roomItem = this._roomInstance.GetRoomItemHandler().GetFloor.FirstOrDefault(i => i.GetBaseItem().InteractionType == InteractionType.TONER);
        if (roomItem == null)
        {
            return;
        }

        var teinte = 135;
        var saturation = 180;
        var luminosite = (int)Math.Floor((double)this.Intensity / 2);
        roomItem.ExtraData = "on," + teinte + "," + saturation + "," + luminosite;
        roomItem.UpdateState();
    }
}
