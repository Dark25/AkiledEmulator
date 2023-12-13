using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms.Chat.Commands;
using Akiled.HabboHotel.Rooms.Chat.Emotions;
using Akiled.HabboHotel.Rooms.Chat.Filter;
using Akiled.HabboHotel.Rooms.Chat.Pets.Commands;
using Akiled.HabboHotel.Rooms.Chat.Styles;
using System;

namespace Akiled.HabboHotel.Rooms.Chat
{
    public sealed class ChatManager
    {
        /// <summary>
        /// Chat Emoticons.
        /// </summary>
        private readonly ChatEmotionsManager _emotions;

        /// <summary>
        /// Filter Manager.
        /// </summary>
        private readonly WordFilterManager _filter;

        /// <summary>
        /// Commands.
        /// </summary>
        private readonly CommandManager _commands;

        /// <summary>
        /// Pet Commands.
        /// </summary>
        private readonly PetCommandManager _petCommands;

        /// <summary>
        /// Chat styles.
        /// </summary>
        private readonly ChatStyleManager _chatStyles;

        /// <summary>
        /// Initializes a new instance of the ChatManager class.
        /// </summary>
        public ChatManager()
        {
            this._emotions = new ChatEmotionsManager();

            this._filter = new WordFilterManager();
            this._filter.Init();

            this._commands = new CommandManager();
            this._commands.Init();

            this._petCommands = new PetCommandManager();

            this._chatStyles = new ChatStyleManager();
            this._chatStyles.Init();
            Console.WriteLine("Chat -> Listo!");
        }

        public ChatEmotionsManager GetEmotions()
        {
            return this._emotions;
        }

        public WordFilterManager GetFilter()
        {
            return this._filter;
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }

        public PetCommandManager GetPetCommands()
        {
            return this._petCommands;
        }

        public ChatStyleManager GetChatStyles()
        {
            return this._chatStyles;
        }

    }
}
