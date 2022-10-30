using System;

namespace Akiled.HabboHotel.Rooms
{
    public class RoomModel
    {
        public int DoorX;
        public int DoorY;
        public double DoorZ;
        public int DoorOrientation;
        public int MurHeight;
        public string Heightmap;

        public SquareState[,] SqState;
        public short[,] SqFloorHeight;
        public int MapSizeX;
        public int MapSizeY;

        public RoomModel(string id, int DoorX, int DoorY, double DoorZ, int DoorOrientation, string Heightmap, int Murheight)
        {
            try
            {
                this.DoorX = DoorX;
                this.DoorY = DoorY;
                this.DoorZ = DoorZ;
                this.DoorOrientation = DoorOrientation;
                this.Heightmap = Heightmap.ToLower();
                string[] tmpHeightmap = this.Heightmap.Split(new char[1] { Convert.ToChar(13) });
                this.MapSizeX = tmpHeightmap[0].Length;
                this.MapSizeY = tmpHeightmap.Length;
                this.SqState = new SquareState[this.MapSizeX, this.MapSizeY];
                this.SqFloorHeight = new short[this.MapSizeX, this.MapSizeY];
                this.MurHeight = Murheight;

                for (int y = 0; y < MapSizeY; y++)
                {
                    string line = tmpHeightmap[y];
                    line = line.Replace("\r", "");
                    line = line.Replace("\n", "");
                    line = line.Replace(" ", "");

                    int x = 0;
                    foreach (char square in line)
                    {
                        if (square == 'x')
                        {
                            SqState[x, y] = SquareState.BLOCKED;
                        }
                        else
                        {
                            SqState[x, y] = SquareState.OPEN;
                            SqFloorHeight[x, y] = Parse(square);
                        }
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during room modeldata loading for model " + id + ": " + ex);
                //throw ex;
            }
        }

        public static short Parse(char input)
        {
            switch (input)
            {
                case 'y':
                case 'x':
                case 'z':
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                    return 10;
                case 'b':
                    return 11;
                case 'c':
                    return 12;
                case 'd':
                    return 13;
                case 'e':
                    return 14;
                case 'f':
                    return 15;
                case 'g':
                    return 16;
                case 'h':
                    return 17;
                case 'i':
                    return 18;
                case 'j':
                    return 19;
                case 'k':
                    return 20;
                case 'l':
                    return 21;
                case 'm':
                    return 22;
                case 'n':
                    return 23;
                case 'o':
                    return 24;
                case 'p':
                    return 25;
                case 'q':
                    return 26;
                case 'r':
                    return 27;
                case 's':
                    return 28;
                case 't':
                    return 29;
                case 'u':
                    return 30;
                case 'v':
                    return 31;
                case 'w':
                    return 32;
                default:
                    Console.WriteLine("The input was not in a correct format: input must be between (0-k) : " + input);
                    return 0;
            }
        }
    }
}
