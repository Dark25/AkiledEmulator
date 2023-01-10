namespace Akiled.HabboHotel.Rooms.Map.Movement
{
    public enum MovementState
    {
        none = 0,
        random = 1,
        leftright = 2,
        updown = 3,
        up = 4,
        right = 5,
        down = 6,
        left = 7
    }
    public enum MovementDirection
    {
        up = 0,
        upright = 1,
        right = 2,
        downright = 3,
        down = 4,
        downleft = 5,
        left = 6,
        upleft = 7,
        random = 8,
        none = 9
    }
    public enum WhenMovementBlock
    {
        none = 0,
        right45 = 1,
        right90 = 2,
        left45 = 3,
        left90 = 4,
        turnback = 5,
        turnrandom = 6
    }

}
