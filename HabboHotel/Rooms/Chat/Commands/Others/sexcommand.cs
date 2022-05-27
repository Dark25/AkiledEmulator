using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;
using System.Threading;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class sexcommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if ((double)Session.GetHabbo().last_sex > AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 1);
                return;
            }

            if (TargetRoom == null)
                return;

            RoomUser roomUserByHabbo1 = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null)
                return;

            if (Params.Length == 0)
            {
                Session.SendWhisper("Elige al usuari@ para coger.", 0);
            }
            if (!TargetRoom.RoomData.SexEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Disculpa, pero el dueño de la sala ha desactivado este comando.");
                return;
            }
            else
            {
                RoomUser roomUserByHabbo2 = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
                {

                    RoomUser Self = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (roomUserByHabbo1 == Self)
                    {
                        Session.SendWhisper("Oye se que tienes ganas, pero no puedes cogerte tu mism@!");
                        return;
                    }
                }
                else if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                {
                    if ((Session.GetHabbo().sexWith == null || Session.GetHabbo().sexWith == "") && (clientByUsername.GetHabbo().Username != Session.GetHabbo().sexWith && Session.GetHabbo().Username != clientByUsername.GetHabbo().sexWith))
                    {
                        Session.GetHabbo().sexWith = clientByUsername.GetHabbo().Username;
                        clientByUsername.SendNotification(Session.GetHabbo().Username + " tiene muchas ganas de hacerlo contigo aceptas su invitación? " + Session.GetHabbo().Username + ", escribe :sexo " + Session.GetHabbo().Username + " si lo acepta.");
                        Session.SendNotification("Su de deseo de sexo fue enviado, si la persona acepta ustedes tendra una rica tirada.");
                    }
                    else if (roomUserByHabbo2 != null)
                    {
                        if (clientByUsername.GetHabbo().sexWith == Session.GetHabbo().Username)
                        {
                            if (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null)
                            {
                                if (clientByUsername.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(checked(roomUserByHabbo1.X - roomUserByHabbo2.X)) < 2 && Math.Abs(checked(roomUserByHabbo1.Y - roomUserByHabbo2.Y)) < 2))
                                {
                                    clientByUsername.GetHabbo().sexWith = (string)null;
                                    Session.GetHabbo().sexWith = (string)null;
                                    if (Session.GetHabbo().Gender == "m")
                                    {
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Bajar pantie a " + Params[1] + " chuparle ese clistoris bien rico*", 0, 0), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Mojarle con la lenguita y chuparle bien rico su cosita a " + Session.GetHabbo().Username + ", mooorderleee la cabezitaa.*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Comeeerle ese toto bien rico a " + Params[1] + ", haceeerla geemir como nunca en su puta vida*", 0, 0), false);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Gritaaa mi nombre, " + Params[1] + " Pide y pidee mas penee que es lo que mas te gusta!*", 0, 0), false);
                                        roomUserByHabbo1.ApplyEffect(503);
                                        Room.SendPacket((IServerPacket)new ShoutMessageComposer(roomUserByHabbo2.VirtualId, " " + Session.GetHabbo().Username + " ", 0, 0), false);
                                        roomUserByHabbo2.ApplyEffect(501);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "aaaaaaai, aaaaah, mételoo papi..mételo.. asiiii, que ricooo mas massss*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(501);
                                        roomUserByHabbo2.ApplyEffect(503);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Metiendotelo despacito  a pasito bb, estoy apunto de explotar :oo*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Abreeeeme esas piernas y excitamee " + Session.GetHabbo().Username + ", para que me bañes la cara en lechee*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(501);
                                        roomUserByHabbo2.ApplyEffect(502);
                                        roomUserByHabbo1.ApplyEffect(503);
                                        roomUserByHabbo2.ApplyEffect(500);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Darleeee bien duro adentrooo " + Params[1] + "* *ooooh oooh oooooh*", 0, 0), false);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Morderme los labioooss, lo haces rico mi amooor sigue asi ashhh ahhhh aiii!*", 0, 0), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(503);
                                        roomUserByHabbo2.ApplyEffect(500);
                                        roomUserByHabbo1.ApplyEffect(0);
                                        roomUserByHabbo2.ApplyEffect(0);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Quitarlee el braazier a " + Params[1] + "*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Tiiraaar por la ventana*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Ayyyy amor comeme estas tettaas son todas tuyas bb " + Session.GetHabbo().Username + "*", 0, 0), false);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Mmmmmm* Quee rico mi amor esos pezonees rosaditos, MORDER", 0, 0), false);
                                        Thread.Sleep(4000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        roomUserByHabbo2.ApplyEffect(503);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Amooor tocamee la cosita mientras me chupas las tetaaas---*", 0, 0), false);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*oooh, ooooooh, ooooh, tocaar con mi dedito tu cosita mi amooor.*", 0, 0), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(501);
                                        roomUserByHabbo2.ApplyEffect(500);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Poneeerme en cuatrooo bb, " + Session.GetHabbo().Username + " meteemelo duroooo*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*OOHH que rico culo que se vee parace un corazooon <3!", 0, 0), false);
                                        Thread.Sleep(1000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        roomUserByHabbo2.ApplyEffect(503);
                                        roomUserByHabbo1.ApplyEffect(0);
                                        roomUserByHabbo2.ApplyEffect(0);
                                        Thread.Sleep(4000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Bañame la cara en lecheeee, " + Session.GetHabbo().Username + " si bb.*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo2.ApplyEffect(542);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*OOHH que rico culo que se vee parace un corazooon <3!", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Uff mi vida eres la mejoooor que rico!", 0, 0), false);
                                    }
                                    else
                                    {
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Quitarlee el pantalon a " + Params[1] + "*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(9);
                                        roomUserByHabbo2.ApplyEffect(9);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Tiiraaar por la ventana*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Ayyyy amor chupamelo ricooo " + Session.GetHabbo().Username + "*", 0, 0), false);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Mmmmmm* Quee rico mi amor, si que lo tienes grandee, MORDER", 0, 0), false);
                                        Thread.Sleep(4000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        roomUserByHabbo2.ApplyEffect(503);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Amooor tocamee la cosita mientras me chupas las tetaaas---*", 0, 0), false);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*oooh, ooooooh, ooooh, tocaar con mi dedito tu cosita mi amooor.*", 0, 0), false);
                                        Thread.Sleep(2000);
                                        roomUserByHabbo1.ApplyEffect(501);
                                        roomUserByHabbo2.ApplyEffect(500);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Poneeerme en cuatrooo bb, " + Session.GetHabbo().Username + " meteemelo duroooo*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*OOHH que rico culo que se vee parace un corazooon <3!", 0, 0), false);
                                        Thread.Sleep(1000);
                                        roomUserByHabbo1.ApplyEffect(502);
                                        roomUserByHabbo2.ApplyEffect(503);
                                        roomUserByHabbo1.ApplyEffect(0);
                                        roomUserByHabbo2.ApplyEffect(0);
                                        Thread.Sleep(4000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo1.VirtualId, "*Bañame la cara en lecheeee, " + Session.GetHabbo().Username + " si bb.*", 0, 0), false);
                                        Thread.Sleep(3000);
                                        roomUserByHabbo2.ApplyEffect(542);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*OOHH que rico culo que se vee parace un corazooon <3!", 0, 0), false);
                                        Thread.Sleep(3000);
                                        Room.SendPacket((IServerPacket)new ChatComposer(roomUserByHabbo2.VirtualId, "*Uff mi vida eres el mejoooor que rico!", 0, 0), false);
                                        Session.GetHabbo().last_sex = AkiledEnvironment.GetIUnixTimestamp();

                                    }
                                }
                                else
                                    Session.SendWhisper("Acercate mas, que no eres el negro de whatsapp.", 0);
                            }
                            else
                                Session.SendWhisper("Ocurrio un error el usuario no fue encontrado.", 0);
                        }
                        else
                            Session.SendWhisper("Marico te dejo en visto, fuiste rechazado, hazte la paja que mas te queda.", 0);
                    }
                    else
                        Session.SendWhisper("Esta persona parece que no esta en la sala.", 0);
                }
                else
                    Session.SendWhisper("Estas demasiado lejos para tener sexo!", 0);
            }

        }
    }
}
