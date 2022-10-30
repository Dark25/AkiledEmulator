using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.ChatMessageStorage
{
    public class ChatMessageManager
    {
        private readonly List<ChatMessage> listOfMessages;

        public int messageCount
        {
            get
            {
                return this.listOfMessages.Count;
            }
        }

        public ChatMessageManager()
        {
            this.listOfMessages = new List<ChatMessage>();
        }

        public void LoadUserChatlogs(int UserId)
        {
            DataTable table;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM chatlogs WHERE user_id = " + UserId + " ORDER BY id DESC LIMIT 100");
                table = queryreactor.GetTable();
                if (table == null)
                    return;
                foreach (DataRow Row in table.Rows)
                {
                    this.AddMessage((int)Row["user_id"], Row["user_name"].ToString(), (int)Row["room_id"], Row["type"].ToString() + Row["message"].ToString());
                }
            }
        }

        public void LoadRoomChatlogs(int RoomId)
        {
            DataTable table;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM chatlogs WHERE room_id = '" + RoomId + "' ORDER BY id DESC LIMIT 100");
                table = queryreactor.GetTable();
                if (table == null)
                    return;
                foreach (DataRow Row in table.Rows)
                {
                    this.AddMessage((int)Row["user_id"], Row["user_name"].ToString(), (int)Row["room_id"], Row["type"].ToString() + Row["message"].ToString());
                }
            }
        }

        public void AddMessage(int UserId, string Username, int RoomId, string MessageText)
        {
            ChatMessage message = new ChatMessage(UserId, Username, RoomId, MessageText, DateTime.Now);

            lock (this.listOfMessages) this.listOfMessages.Add(message);

            int CountMessage = this.listOfMessages.Count;
            if (CountMessage >= 100) this.listOfMessages.RemoveRange(0, 1);
        }

        public List<ChatMessage> GetSortedMessages(int roomid)
        {
            List<ChatMessage> list = new List<ChatMessage>();

            foreach (ChatMessage chatMessage in this.listOfMessages)
            {
                if (roomid == chatMessage.roomID || roomid == 0)
                {
                    list.Add(chatMessage);
                }
            }
            list.Reverse();
            return list;
        }

        public void Serialize(ref ServerPacket message)
        {
            List<ChatMessage> ListReverse = new List<ChatMessage>();
            ListReverse.AddRange(this.listOfMessages);
            ListReverse.Reverse();
            foreach (ChatMessage chatMessage in ListReverse)
            {
                if (chatMessage != null)
                {
                    chatMessage.Serialize(ref message);
                }
                else
                {
                    message.WriteString("0"); //this.timeSpoken.Minute
                    message.WriteInteger(0); //this.timeSpoken.Minute
                    message.WriteString("Erreur");
                    message.WriteString("Erreur");
                    message.WriteBoolean(false); // Text is bold
                }
            }
        }
    }
}
