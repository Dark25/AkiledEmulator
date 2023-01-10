using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Communication.WebSocket;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.Net;
using ConnectionManager;
using SharedPacketLib;
using System;
using System.Data;

namespace Akiled.HabboHotel.WebClients
{
    public class WebClient
    {
        private ConnectionInformation _connection;
        private WebPacketParser _packetParser;

        private bool _isStaff;

        public Language Langue;

        public int UserId;

        public int ConnectionID;

        public WebClient(int id, ConnectionInformation connection)
        {
            this.ConnectionID = id;
            this._isStaff = false;
            this._connection = connection;
            this._packetParser = new WebPacketParser(this);
        }

        public void TryAuthenticate(string AuthTicket)
        {
            if (string.IsNullOrEmpty(AuthTicket))
                return;

            string ip = this.GetConnection().getIp();

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id FROM bans WHERE expire > @nowtime AND (bantype = 'ip' AND value = @IP1) LIMIT 1");
                queryreactor.AddParameter("nowtime", AkiledEnvironment.GetUnixTimestamp());
                queryreactor.AddParameter("IP1", ip);

                DataRow IsBanned = queryreactor.GetRow();
                if (IsBanned != null)
                    return;

                queryreactor.SetQuery("SELECT user_id, is_staff, langue FROM user_websocket WHERE auth_ticket = @sso");
                queryreactor.AddParameter("sso", AuthTicket);

                DataRow dUserInfo = queryreactor.GetRow();
                if (dUserInfo == null)
                    return;

                this.UserId = Convert.ToInt32(dUserInfo["user_id"]);
                this._isStaff = AkiledEnvironment.EnumToBool((string)dUserInfo["is_staff"]);
                this.Langue = LanguageManager.ParseLanguage(Convert.ToString(dUserInfo["langue"]));
                queryreactor.RunQuery("UPDATE user_websocket SET auth_ticket = '' WHERE user_id = '" + UserId + "'");

                this._sendSettingSound(queryreactor);
            }

            AkiledEnvironment.GetGame().GetClientWebManager().LogClonesOut(UserId);
            AkiledEnvironment.GetGame().GetClientWebManager().RegisterClient(this, UserId);

            this.SendPacket(new AuthOkComposer());
            this.SendPacket(new UserIsStaffComposer(this._isStaff));
            //this.SendPacket(new NotifTopInitComposer(AkiledEnvironment.GetGame().GetNotifTopManager().GetAllMessages()));
        }

        private void _sendSettingSound(IQueryAdapter queryreactor)
        {
            queryreactor.SetQuery("SELECT volume FROM users WHERE id = '" + this.UserId + "'");

            DataRow dUserVolume = queryreactor.GetRow();
            if (dUserVolume == null)
                return;

            string clientVolume = dUserVolume["volume"].ToString();

            if (clientVolume.Contains(","))
            {
                string[] Str = clientVolume.Split(',');
                if (Str.Length != 3)
                    return;

                int.TryParse(Str[0], out int systemSound);
                int.TryParse(Str[1], out int furniSound);
                int.TryParse(Str[2], out int traxSound);

                this.SendPacket(new SettingVolumeComposer(traxSound, furniSound, systemSound));
            }
        }

        private void SwitchParserRequest()
        {
            this._packetParser.onNewPacket += new WebPacketParser.HandlePacket(this.parser_onNewPacket);

            byte[] packet = (this._connection.parser as InitialPacketParser).currentData;
            this._connection.parser.Dispose();
            this._connection.parser = (IDataParser)this._packetParser;
            this._connection.parser.handlePacketData(packet);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                AkiledEnvironment.GetGame().GetPacketManager().TryExecuteWebPacket(this, Message);
            }
            catch (Exception ex)
            {
                Logging.LogPacketException(Message.ToString(), (ex).ToString());
            }
        }

        public ConnectionInformation GetConnection()
        {
            return this._connection;
        }

        public void StartConnection()
        {
            if (this._connection == null)
                return;

            (this._connection.parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this._connection.startPacketProcessing();
        }

        public void Dispose()
        {
            if (this._connection != null)
                this._connection.Dispose();
        }

        public void SendPacket(IServerPacket Message)
        {
            if (Message == null || this.GetConnection() == null) return;

            this.GetConnection().SendData(EncodeDecode.EncodeMessage(Message.GetBytes()));
        }
    }
}
