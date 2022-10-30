using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;


namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SpamCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            var ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;
            string spam_hotel = (AkiledEnvironment.GetConfig().data["spamhotel_text"]);

            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ »   " + spam_hotel + " » << Nuevos Look, Muchos $ HC infinito, Participa De Eventos!Seras Premiado, Uneteƒ Te Esperamos!", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ Unete a nosotros ahora y obten beneficios, Crea Tu Mansion, Hospital>>   " + spam_hotel + " <<", 0, ThisUser.LastBubble));
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@  " + spam_hotel + " /Miillones de Creditos Disponibles Para Ti, Crea  Salas A Tu Gustoƒ Te Esperamos", 0, ThisUser.LastBubble));
        }
    }
}
