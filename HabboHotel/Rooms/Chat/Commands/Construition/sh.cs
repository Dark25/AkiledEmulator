using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Akiled.Communication.Packets.Outgoing.Inventory.Furni;
using System.Globalization;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Construit
{
    class sh : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_stack_height";
            }
        }
        public string Parameters
        {
            get
            {
                return "%message%";
            }
        }
        public string Description
        {
            get
            {
                return "Establecer la altura de la pila.";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, false, false))
            {
                Session.SendNotification("No tiene permiso para el comando `stack_height`");
                return;
            }

            RoomUser user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (user == null)
            {
                Session.SendNotification("Usuario no encontrado.");
                return;
            }


            if (Params.Length < 2)
            {
                Session.SendWhisper("Ingrese un valor numérico o escriba ': sh -' para desactivarlo ");
                return;
            }

            if (Params[1] == "-")
            {
                Session.SendWhisper("Altura de la pila deshabilitada ");
                Session.GetHabbo().ForceHeight = -1;
                return;
            }

            double value;
            bool checkIfParsable = Double.TryParse(Params[1], out value);
            if (checkIfParsable == false)
            {
                Session.SendWhisper("Ingrese un valor numérico o escriba ': sh -' para desactivarlo");
                return;
            }


            double HeightValue = Convert.ToDouble(Params[1]);
            if (HeightValue < 0 || HeightValue > 100)
            {
                Session.SendWhisper("Por favor, introduzca un valor entre 0 y 100");
                return;
            }

            Session.GetHabbo().ForceHeight = HeightValue;
            Session.SendWhisper("La altura de la pila es: " + Convert.ToString(HeightValue));
        }
    }
}