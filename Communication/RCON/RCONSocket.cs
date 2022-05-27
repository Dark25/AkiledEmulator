using Akiled.Communication.RCON.Commands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Akiled.Net
{
    public class RCONSocket
    {
        public Socket msSocket;
        public int musPort;

        public List<string> allowedIps;
        private readonly CommandManager _commands;

        public RCONSocket(int _musPort, string[] _allowedIps)
        {
            this.musPort = _musPort;
            this.allowedIps = new List<string>();
            foreach (string str in _allowedIps)
                this.allowedIps.Add(str);

            try
            {
                this.msSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.msSocket.Bind(new IPEndPoint(IPAddress.Any, this.musPort));
                this.msSocket.Listen(0);
                this.msSocket.BeginAccept(new AsyncCallback(this.onNewConnection), this.msSocket);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not set up MUS socket:\n" + (ex).ToString());
            }

            this._commands = new CommandManager();
        }

        public void onNewConnection(IAsyncResult iAr)
        {
            try
            {
                Socket _socket = ((Socket)iAr.AsyncState).EndAccept(iAr);
                string str = _socket.RemoteEndPoint.ToString().Split(new char[1] { ':' })[0];
                if (this.allowedIps.Contains(str) || str == "127.0.0.1")
                {
                    new RCONConnection(_socket);
                }
                else
                {
                    Console.WriteLine("MusSocket Ip non autorisé: " + str);
                    _socket.Close();
                }
            }
            catch
            {
            }
            this.msSocket.BeginAccept(new AsyncCallback(this.onNewConnection), this.msSocket);
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }
    }
}
