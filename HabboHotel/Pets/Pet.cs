using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Pets
{
    public class Pet
    {
        public int[] ExperienceLevels = new int[19]
        {
            100,
            200,
            400,
            600,
            1000,
            1300,
            1800,
            2400,
            3200,
            4300,
            7200,
            8500,
            10100,
            13300,
            17500,
            23000,
            51900,
            120000,
            240000
        };
        public int PetId;
        public int OwnerId;
        public int VirtualId;
        public int Type;
        public string Name;
        public string Race;
        public string Color;
        public int Expirience;
        public int Energy;
        public int Nutrition;
        public int RoomId;
        public int X;
        public int Y;
        public double Z;
        public int Respect;
        public double CreationStamp;
        public bool PlacedInRoom;
        public DatabaseUpdateState DBState;
        public int Saddle;
        public int HairDye;
        public int PetHair;
        public bool AnyoneCanRide;
        public Dictionary<short, bool> PetCommands;

        public Room Room
        {
            get
            {
                if (!this.IsInRoom)
                    return null;
                else
                    return AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            }
        }

        public bool IsInRoom
        {
            get
            {
                return this.RoomId > 0;
            }
        }

        public int Level
        {
            get
            {
                for (int index = 0; index < this.ExperienceLevels.Length; ++index)
                {
                    if (this.Expirience < this.ExperienceLevels[index])
                        return index + 1;
                }
                return this.ExperienceLevels.Length + 1;
            }
        }

        public static int MaxLevel
        {
            get
            {
                return 20;
            }
        }

        public int ExpirienceGoal
        {
            get
            {
                if (this.Level < 19)
                    return this.ExperienceLevels[this.Level - 1];
                else
                    return this.ExperienceLevels[18];
            }
        }

        public static int MaxEnergy
        {
            get
            {
                return 100;
            }
        }

        public static int MaxNutrition
        {
            get
            {
                return 150;
            }
        }

        public int Age
        {
            get
            {
                return (int)Math.Floor(((double)AkiledEnvironment.GetUnixTimestamp() - this.CreationStamp) / 86400.0);
            }
        }

        public string Look
        {
            get
            {
                return this.Type + " " + this.Race + " " + this.Color;
            }
        }

        public string OwnerName
        {
            get
            {
                return AkiledEnvironment.GetGame().GetClientManager().GetNameById(this.OwnerId);
            }
        }

        public Pet(int PetId, int OwnerId, int RoomId, string Name, int Type, string Race, string Color, int Expirience, int Energy, int Nutrition, int Respect, double CreationStamp, int X, int Y, double Z, int havesaddle, int hairdye, int PetHair, bool CanMountAllPeople)
        {
            this.PetId = PetId;
            this.OwnerId = OwnerId;
            this.RoomId = RoomId;
            this.Name = Name;
            this.Type = Type;
            this.Race = Race;
            this.Color = Color;
            this.Expirience = Expirience;
            this.Energy = Energy;
            this.Nutrition = Nutrition;
            this.Respect = Respect;
            this.CreationStamp = CreationStamp;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.PlacedInRoom = false;
            this.DBState = DatabaseUpdateState.Updated;
            this.Saddle = (havesaddle == 1) ? 9 : (havesaddle == 2) ? 10 : 0;
            this.HairDye = hairdye;
            this.PetHair = PetHair;
            this.AnyoneCanRide = CanMountAllPeople;
            this.PetCommands = GetPetCommands();
        }

        public bool HasCommand(short Command)
        {
            if (!PetCommands.ContainsKey(Command)) return false;

            return PetCommands[Command];
        }

        public Dictionary<short, bool> GetPetCommands()
        {
            Dictionary<short, bool> Output = new Dictionary<short, bool>();
            short qLevel = (short)this.Level;

            switch (this.Type)
            {
                default:
                    {

                        Output.Add(0, true); // Libre
                        Output.Add(1, true); // Assis
                        Output.Add(13, true); // Panier
                        Output.Add(2, qLevel >= 2); // Couché
                        Output.Add(4, qLevel >= 3); // Demande
                        Output.Add(3, qLevel >= 4); // Viens ici
                        Output.Add(5, qLevel >= 4); // Fais le mort
                        Output.Add(43, qLevel >= 5); // mange
                        Output.Add(14, qLevel >= 5); // Bois
                        Output.Add(6, qLevel >= 6); // Reste
                        Output.Add(17, qLevel >= 6); // Joue au Foot
                        Output.Add(8, qLevel >= 8); // Debout
                        Output.Add(7, qLevel >= 9); // Suis moi
                        Output.Add(9, qLevel >= 11); // Saute
                        Output.Add(11, qLevel >= 11); // Joue
                        Output.Add(12, qLevel >= 12); // Silence
                        Output.Add(10, qLevel >= 12); // Parle
                        Output.Add(15, qLevel >= 16); // Suis à gauche
                        Output.Add(16, qLevel >= 16); // Suis à droite
                        Output.Add(24, qLevel >= 17); // Avance

                        if (this.Type == 3 || this.Type == 4)
                        {
                            Output.Add(46, true); //Reproduire
                        }
                    }
                    break;

                case 8: // Araña
                    Output.Add(1, true); // Assis
                    Output.Add(2, true); // Couché
                    Output.Add(3, qLevel >= 2); // Viens ici
                    Output.Add(17, qLevel >= 3); // Joue au Foot
                    Output.Add(6, qLevel >= 4); // Reste
                    Output.Add(5, qLevel >= 4); // Fais le mort
                    Output.Add(7, qLevel >= 5); // Suis moi
                    Output.Add(23, qLevel >= 6); // Allume la télé
                    Output.Add(9, qLevel >= 7); // Saute
                    Output.Add(10, qLevel >= 8); // Parle
                    Output.Add(11, qLevel >= 8); // Joue
                    Output.Add(24, qLevel >= 9); // Avance
                    Output.Add(15, qLevel >= 10); // Suis à gauche
                    Output.Add(16, qLevel >= 10); // Suis à droite
                    Output.Add(13, qLevel >= 12); // Panier
                    Output.Add(14, qLevel >= 13); // Bois
                    Output.Add(19, qLevel >= 14); // Rebondis
                    Output.Add(20, qLevel >= 14); // Aplatis toi
                    Output.Add(22, qLevel >= 15); // Tourne
                    Output.Add(21, qLevel >= 16); // Danse
                    break;

                case 16:
                    break;
            }

            return Output;
        }

        public void OnRespect()
        {
            ++this.Respect;
            if (this.DBState != DatabaseUpdateState.NeedsInsert)
                this.DBState = DatabaseUpdateState.NeedsUpdate;
            if (this.Expirience > 51900)
                return;
            this.AddExpirience(10);
        }

        public void AddExpirience(int Amount)
        {
            this.Expirience = this.Expirience + Amount;
            if (this.Expirience >= 51900)
                return;

            if (this.DBState != DatabaseUpdateState.NeedsInsert)
                this.DBState = DatabaseUpdateState.NeedsUpdate;

            if (this.Room == null)
                return;

            ServerPacket Message1 = new ServerPacket(ServerPacketHeader.AddExperiencePointsMessageComposer);
            Message1.WriteInteger(this.PetId);
            Message1.WriteInteger(this.VirtualId);
            Message1.WriteInteger(Amount);
            this.Room.SendPacket(Message1);

            if (this.Expirience <= this.ExpirienceGoal)
                return;

            ServerPacket LevelNotify = new ServerPacket(ServerPacketHeader.PetLevelUpComposer);
            LevelNotify.WriteInteger(PetId);
            LevelNotify.WriteString(Name);
            LevelNotify.WriteInteger(Level);
            this.Room.SendPacket(LevelNotify);

            PetCommands.Clear();
            PetCommands = GetPetCommands();
        }

        public void PetEnergy(bool addEnergy)
        {
            if (this.Energy >= 100)
                return;

            int randomUsage = AkiledEnvironment.GetRandomNumber(4, 15);

            if (!addEnergy)
            {
                this.Energy -= randomUsage;

                if (this.Energy < 0)
                {
                    this.Energy = 1;
                    randomUsage = 1;
                }
            }
            else
            {
                this.Energy = (this.Energy + randomUsage) % 100;
            }

            if (DBState != DatabaseUpdateState.NeedsInsert)
                DBState = DatabaseUpdateState.NeedsUpdate;
        }
    }
}
