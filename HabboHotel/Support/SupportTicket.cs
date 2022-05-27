using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;

using System;

namespace Akiled.HabboHotel.Support
{
    public class SupportTicket
    {
        private readonly int Id;
        public int Score;
        public int Type;
        public TicketStatus Status;
        public int SenderId;
        public int ReportedId;
        public int ModeratorId;
        public string Message;
        public int RoomId;
        public string RoomName;
        public double Timestamp;
        private readonly string SenderName;
        private readonly string ReportedName;
        private readonly string ModName;

        public int TabId
        {
            get
            {
                if (this.Status == TicketStatus.OPEN)
                    return 1;
                if (this.Status == TicketStatus.PICKED || this.Status == TicketStatus.ABUSIVE || (this.Status == TicketStatus.INVALID || this.Status == TicketStatus.RESOLVED))
                    return 2;
                return this.Status == TicketStatus.DELETED ? 3 : 0;
            }
        }

        public int TicketId
        {
            get
            {
                return this.Id;
            }
        }

        public SupportTicket(int Id, int Score, int Type, int SenderId, int ReportedId, string Message, int RoomId, string RoomName, double Timestamp)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            this.Status = TicketStatus.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            this.ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;
            this.SenderName = AkiledEnvironment.GetGame().GetClientManager().GetNameById(SenderId);
            this.ReportedName = AkiledEnvironment.GetGame().GetClientManager().GetNameById(ReportedId);
            this.ModName = AkiledEnvironment.GetGame().GetClientManager().GetNameById(this.ModeratorId);
        }

        public SupportTicket(int Id, int Score, int Type, int SenderId, int ReportedId, string Message, int RoomId, string RoomName, double Timestamp, object senderName, object reportedName, object modName)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            this.Status = TicketStatus.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            this.ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;
            this.SenderName = senderName != DBNull.Value ? (string)senderName : string.Empty;
            this.ReportedName = reportedName != DBNull.Value ? (string)reportedName : string.Empty;
            if (modName == DBNull.Value)
                this.ModName = string.Empty;
            else
                this.ModName = (string)modName;
        }

        public ServerPacket Serialize(ServerPacket message)
        {
            message.WriteInteger(Id); // id
            message.WriteInteger(TabId); // state
            message.WriteInteger(4); // type (3 or 4 for new style)
            message.WriteInteger(Type); // priority
            message.WriteInteger((int)(AkiledEnvironment.GetUnixTimestamp() - Timestamp) * 1000); // -->> timestamp
            message.WriteInteger(Score); // priority
            message.WriteInteger(SenderId);
            message.WriteInteger(SenderId); // sender id 8 ints
            message.WriteString(SenderName); // sender name
            message.WriteInteger(ReportedId);
            message.WriteString(ReportedName);
            message.WriteInteger((Status == TicketStatus.PICKED) ? ModeratorId : 0); // mod id
            message.WriteString(ModName); // mod name
            message.WriteString(this.Message); // issue message
            message.WriteInteger(0);
            message.WriteInteger(0);

            return message;
        }

        public void Pick(int pModeratorId, bool UpdateInDb)
        {
            this.Status = TicketStatus.PICKED;
            this.ModeratorId = pModeratorId;
            this.Timestamp = (double)AkiledEnvironment.GetUnixTimestamp();
            if (!UpdateInDb)
                return;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = " + pModeratorId + ", timestamp = '" + AkiledEnvironment.GetUnixTimestamp() + "' WHERE id = " + this.Id);
        }

        public void Close(TicketStatus NewStatus, bool UpdateInDb)
        {
            this.Status = NewStatus;
            if (!UpdateInDb)
                return;
            string str;
            switch (NewStatus)
            {
                case TicketStatus.ABUSIVE:
                    str = "abusive";
                    break;
                case TicketStatus.INVALID:
                    str = "invalid";
                    break;
                default:
                    str = "resolved";
                    break;
            }
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4]
                {
           "UPDATE moderation_tickets SET status = '",
           str,
           "' WHERE id = ",
           this.Id
                }));
        }

        public void Release(bool UpdateInDb)
        {
            this.Status = TicketStatus.OPEN;
            if (!UpdateInDb)
                return;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = " + this.Id);
        }

        public void Delete(bool UpdateInDb)
        {
            this.Status = TicketStatus.DELETED;
            if (!UpdateInDb)
                return;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = " + this.Id);
        }

        public ServerPacket Serialize()
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.ModeratorSupportTicketMessageComposer);
            serverMessage.WriteInteger(this.Id);
            serverMessage.WriteInteger(this.TabId);
            serverMessage.WriteInteger(3);
            serverMessage.WriteInteger(this.Type);
            serverMessage.WriteInteger((int)(AkiledEnvironment.GetUnixTimestamp() - this.Timestamp) * 1000);
            serverMessage.WriteInteger(this.Score);
            serverMessage.WriteInteger(this.SenderId);
            serverMessage.WriteInteger(this.SenderId);
            if (AkiledEnvironment.GetHabboById(this.SenderId) != null)
                serverMessage.WriteString(this.SenderName.Equals("") ? AkiledEnvironment.GetHabboById(this.SenderId).Username : this.SenderName);
            else
                serverMessage.WriteString(this.SenderName);
            serverMessage.WriteInteger(this.ReportedId);
            if (AkiledEnvironment.GetHabboById(this.ReportedId) != null)
                serverMessage.WriteString(this.ReportedName.Equals("") ? AkiledEnvironment.GetHabboById(this.ReportedId).Username : this.ReportedName);
            else
                serverMessage.WriteString(this.ReportedName);
            serverMessage.WriteInteger(this.Status == TicketStatus.PICKED ? this.ModeratorId : 0);
            if (AkiledEnvironment.GetHabboById(this.ModeratorId) != null)
                serverMessage.WriteString(this.Status == TicketStatus.PICKED ? (this.ModName.Equals("") ? AkiledEnvironment.GetHabboById(this.ModeratorId).Username : this.ModName) : "");
            else
                serverMessage.WriteString(this.ModName);
            serverMessage.WriteString(this.Message);
            serverMessage.WriteInteger(0);
            serverMessage.WriteInteger(0);

            return serverMessage;
        }
    }
}
