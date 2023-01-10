using Akiled.Communication.Packets.Outgoing;

using System;
using System.Text;

namespace Akiled.HabboHotel.Rooms
{
    public class DynamicRoomModel
    {
        private RoomModel staticModel;

        public int DoorX;
        public int DoorY;
        public double DoorZ;
        public int DoorOrientation;
        public string Heightmap;

        public SquareState[,] SqState;
        public short[,] SqFloorHeight;
        public int MapSizeX;
        public int MapSizeY;
        private ServerPacket SerializedRelativeHeightmap;
        private bool RelativeSerialized;
        private ServerPacket SerializedHeightmap;
        private bool HeightmapSerialized;
        public int MurHeight;

        public DynamicRoomModel(RoomModel pModel)
        {
            this.staticModel = pModel;
            this.DoorX = this.staticModel.DoorX;
            this.DoorY = this.staticModel.DoorY;
            this.DoorZ = this.staticModel.DoorZ;
            this.DoorOrientation = this.staticModel.DoorOrientation;
            this.Heightmap = this.staticModel.Heightmap;
            this.MapSizeX = this.staticModel.MapSizeX;
            this.MapSizeY = this.staticModel.MapSizeY;
            this.MurHeight = this.staticModel.MurHeight;
            this.Generate();
        }

        public void Generate()
        {
            this.SqState = new SquareState[this.MapSizeX, this.MapSizeY];
            this.SqFloorHeight = new short[this.MapSizeX, this.MapSizeY];
            for (int index1 = 0; index1 < this.MapSizeY; ++index1)
            {
                for (int index2 = 0; index2 < this.MapSizeX; ++index2)
                {
                    if (index2 > this.staticModel.MapSizeX - 1 || index1 > this.staticModel.MapSizeY - 1)
                    {
                        this.SqState[index2, index1] = SquareState.BLOCKED;
                    }
                    else
                    {
                        this.SqState[index2, index1] = this.staticModel.SqState[index2, index1];
                        this.SqFloorHeight[index2, index1] = this.staticModel.SqFloorHeight[index2, index1];
                    }
                }
            }
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }

        public void SetUpdateState()
        {
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }

        public ServerPacket SerializeRelativeHeightmap()
        {
            if (!this.RelativeSerialized)
            {
                this.SerializedRelativeHeightmap = this.NewHeightMap();//this.GetRelativeHeightmap();
                this.RelativeSerialized = true;
            }

            return this.SerializedRelativeHeightmap;
        }

        public ServerPacket GetHeightmap()
        {
            if (!this.HeightmapSerialized)
            {
                this.SerializedHeightmap = this.SerializeHeightmap();
                this.HeightmapSerialized = true;
            }
            return this.SerializedHeightmap;
        }

        private ServerPacket SerializeHeightmap()
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.FloorHeightMapMessageComposer);
            Message.WriteBoolean(true);
            Message.WriteInteger((MurHeight > 0) ? MurHeight : -1);
            StringBuilder thatMessage = new StringBuilder();
            //Needs cache!
            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x == DoorX && y == DoorY)
                        thatMessage.Append(Parse(Convert.ToInt16(DoorZ)));
                    else if (SqState[x, y] == SquareState.BLOCKED)
                        thatMessage.Append("x");
                    else
                        thatMessage.Append(Parse(SqFloorHeight[x, y]));
                }

                thatMessage.Append(Convert.ToChar(13));
            }
            Message.WriteString(thatMessage.ToString());
            return Message;
        }

        private string Parse(short text)
        {
            switch (text)
            {
                case 10:
                    return "a";
                case 11:
                    return "b";
                case 12:
                    return "c";
                case 13:
                    return "d";
                case 14:
                    return "e";
                case 15:
                    return "f";
                case 16:
                    return "g";
                case 17:
                    return "h";
                case 18:
                    return "i";
                case 19:
                    return "j";
                case 20:
                    return "k";
                case 21:
                    return "l";
                case 22:
                    return "m";
                case 23:
                    return "n";
                case 24:
                    return "o";
                case 25:
                    return "p";
                case 26:
                    return "q";
                case 27:
                    return "r";
                case 28:
                    return "s";
                case 29:
                    return "t";
                case 30:
                    return "u";
                case 31:
                    return "v";
                case 32:
                    return "w";
            }
            return text.ToString();
        }

        private ServerPacket NewHeightMap()
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.HeightMapMessageComposer);
            serverMessage.WriteInteger(MapSizeX);
            serverMessage.WriteInteger(MapSizeX * MapSizeY);
            for (int i = 0; i < MapSizeY; i++)
            {
                for (int j = 0; j < MapSizeX; j++)
                {
                    if (SqState[j, i] == SquareState.BLOCKED)
                        serverMessage.WriteShort(-1);
                    else
                        serverMessage.WriteShort(SqFloorHeight[j, i] << 8);
                }
            }
            return serverMessage;
        }

        public ServerPacket setHeightMap(double Height)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.HeightMapMessageComposer);
            serverMessage.WriteInteger(MapSizeX);
            serverMessage.WriteInteger(MapSizeX * MapSizeY);
            for (int i = 0; i < MapSizeY; i++)
            {
                for (int j = 0; j < MapSizeX; j++)
                {
                    if (SqState[j, i] == SquareState.BLOCKED)
                        serverMessage.WriteShort(-1);
                    else
                        serverMessage.WriteShort((int)Math.Floor((SqFloorHeight[j, i] + Height) * 256.0));
                }
            }
            return serverMessage;
        }

        public void SetMapsize(int x, int y)
        {
            MapSizeX = x;
            MapSizeY = y;
            refreshArrays();
        }

        internal void refreshArrays()
        {
            SquareState[,] squareStateArray = new SquareState[this.MapSizeX, this.MapSizeY];
            short[,] numArray1 = new short[this.MapSizeX, this.MapSizeY];
            byte[,] numArray2 = new byte[this.MapSizeX, this.MapSizeY];
            for (int index1 = 0; index1 < this.MapSizeY; ++index1)
            {
                for (int index2 = 0; index2 < this.MapSizeX; ++index2)
                {
                    if (index2 > this.staticModel.MapSizeX - 1 || index1 > this.staticModel.MapSizeY - 1)
                    {
                        squareStateArray[index2, index1] = SquareState.BLOCKED;
                    }
                    else
                    {
                        squareStateArray[index2, index1] = this.SqState[index2, index1];
                        numArray1[index2, index1] = this.SqFloorHeight[index2, index1];
                    }
                }
            }
            this.SqState = squareStateArray;
            this.SqFloorHeight = numArray1;
            this.RelativeSerialized = false;
            this.HeightmapSerialized = false;
        }


        public void Destroy()
        {
            Array.Clear((Array)this.SqState, 0, this.SqState.Length);
            Array.Clear((Array)this.SqFloorHeight, 0, this.SqFloorHeight.Length);
            this.staticModel = (RoomModel)null;
            this.Heightmap = (string)null;
            this.SqState = (SquareState[,])null;
            this.SqFloorHeight = (short[,])null;
        }
    }
}
