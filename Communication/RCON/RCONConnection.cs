using Akiled.Core;
using System;
using System.Net.Sockets;
using System.Text;

namespace Akiled.Net
{
    public class RCONConnection
    {
        private byte[] buffer = new byte[1024];
        private Socket socket;

        private readonly Encoding Encoding = Encoding.GetEncoding("Windows-1252");

        public RCONConnection(Socket _socket)
        {
            this.socket = _socket;
            try
            {
                this.socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.OnCallBack), this.socket);
            }
            catch
            {
                this.Dispose();
            }
        }

        public void OnCallBack(IAsyncResult iAr)
        {
            try
            {
                int bytes = 0;
                if (!int.TryParse(socket.EndReceive(iAr).ToString(), out bytes))
                {
                    Dispose();
                    return;
                }

                string data = Encoding.GetString(this.buffer, 0, bytes);

                if (!AkiledEnvironment.GetRCONSocket().GetCommands().Parse(data))
                {
                    Logging.WriteLine("Failed to execute a MUS command. Raw data: " + data);

                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Erreur mus: " + ex);
            }

            this.Dispose();
        }

        public void Dispose()
        {
            try
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                this.socket.Dispose();
            }
            catch
            {
            }
            this.socket = (Socket)null;
            this.buffer = (byte[])null;
        }
    }
}
