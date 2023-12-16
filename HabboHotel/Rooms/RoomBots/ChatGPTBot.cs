namespace Akiled.HabboHotel.Rooms.RoomBots;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core.OpenIA;
using Akiled.HabboHotel.GameClients;

public partial class ChatGPTBot : BotAI
{
    private const int MAX_HISTORY = 10;
    private readonly Dictionary<int, List<ChatCompletionMessage>> _userMessages;
    private RoomUser _lastMessageUser;
    private readonly List<string> _listUserName;
    private readonly Dictionary<int, string> _listActions;
    private int _resetBotTimer;
    private int _resetDanseTimer;
    private int _timeoutTimer = 10;

    public ChatGPTBot(int virtualId)
    {
        this.VirtualId = virtualId;
        this._userMessages = new();
        this._listUserName = new();

        this._listActions = new Dictionary<int, string>()
        {
            { 0, "Do nothing" },
            { 1, "Greet" },
            { 2, "Give the user a drink" },
            { 3, "Give the user an ice cream" },
            { 4, "Sit down" },
            { 5, "Move to user" },
            { 6, "Love or like" },
            { 7, "Sad or bad or angry" },
            { 8, "Laugther" },
            { 9, "Kiss or embrace" },
            { 10, "Danse" }
        };
    }

    public override void OnSelfEnterRoom()
    {
    }

    private void StackMessages(int userId, params ChatCompletionMessage[] message)
    {
        if (this._userMessages.TryGetValue(userId, out var userMessages))
        {
            userMessages.AddRange(message);
            this._userMessages[userId] = userMessages.TakeLast(MAX_HISTORY).ToList();
        }
        else
        {
            this._userMessages.Add(userId, new List<ChatCompletionMessage>(message));
        }
    }

    public void RemoveUserMessages(int userId) => _ = this._userMessages.Remove(userId);

    public override void OnSelfLeaveRoom(bool kicked)
    {
    }

    public override void OnUserEnterRoom(RoomUser user)
    {
        if (this._listUserName.Contains(user.GetUsername()) == false)
        {
            this._listUserName.Add(user.GetUsername());
        }
    }

    public override void OnUserLeaveRoom(GameClient client)
    {
        if (this._listUserName.Contains(client.GetHabbo().Username) == false)
        {
            this._listUserName.Add(client.GetHabbo().Username);
        }

        this.RemoveUserMessages(client.GetHabbo().Id);
    }

    public override void OnUserSay(RoomUser user, string message)
    {
        if (this.GetBotData() == null)
        {
            return;
        }

        var botName = this.GetBotData().Name;

        if (!message.Contains("@" + botName) && !message.StartsWith(": " + botName) && !message.StartsWith(botName))
        {
            return;
        }

        message = message.Replace(": " + botName, "");
        message = message.Trim();

        var chatMsg = new ChatCompletionMessage()
        {
            Content = message,
            Role = "user"
        };

        this.StackMessages(user.UserId, chatMsg);
        this._lastMessageUser = user;
    }

    public override void OnUserShout(RoomUser user, string message)
    {
    }

    private void ParseActionId(string messageText, int userId)
    {
        var match = MyRegex().Match(messageText);
        if (!match.Success || int.TryParse(match.Groups[1].Value, out var actionId) == false)
        {
            return;
        }

        var targetUser = this.GetRoom().GetRoomUserManager.GetRoomUserByUserId(userId);
        if (targetUser == null || actionId == 0)
        {
            return;
        }

        switch (actionId)
        {
            case 1: //Greet
            {
                this.GetRoom().SendPacket(new ActionMessageComposer(this.GetRoomUser().VirtualId, 1));
                break;
            }
            case 2: //Give the user a drink
            {
                const int drinkId = 6;
                if (targetUser.CarryItemID != drinkId)
                {
                    targetUser.CarryItem(drinkId, true);
                }
                break;
            }
            case 3: //Give the user an ice cream
            {
                const int iceCreamId = 4;
                if (targetUser.CarryItemID != iceCreamId)
                {
                    targetUser.CarryItem(iceCreamId, true);
                }
                break;
            }
            case 4: //Sit down
            {
                if (this.GetRoomUser().ContainStatus("sit") || this.GetRoomUser().ContainStatus("lay"))
                {
                    break;
                }

                if (this.GetRoomUser().RotBody % 2 == 0)
                {
                    this.GetRoomUser().SetStatus("sit", "0.5");
                    this.GetRoomUser().IsSit = true;
                    this.GetRoomUser().UpdateNeeded = true;
                }
                break;
            }
            case 5: //Move to user
            {
                var numberCars = new List<int> { 22, 21, 17, 15, 6, 3, 2 };
                this.GetRoomUser().ApplyEffect(numberCars[AkiledEnvironment.GetRandomNumber(0, numberCars.Count)], true);

                this.GetRoomUser().TimerResetEffect = 6;
                this.GetRoomUser().MoveTo(targetUser.X, targetUser.Y, true);

                this._resetBotTimer = 600;
                break;
            }
            case 6: //Love or like
            {
                this.GetRoomUser().ApplyEffect(168, true);
                this.GetRoomUser().TimerResetEffect = 6;
                break;
            }
            case 7: //Sad or bad or angry
            {
                this.GetRoomUser().ApplyEffect(113, true);
                this.GetRoomUser().TimerResetEffect = 6;
                break;
            }
            case 8: //Laugther
            {
                this.GetRoom().SendPacket(new ActionMessageComposer(this.GetRoomUser().VirtualId, 3));
                break;
            }
            case 9: //Kiss or embrace
            {
                this.GetRoom().SendPacket(new ActionMessageComposer(this.GetRoomUser().VirtualId, 2));
                break;
            }
            case 10: //Danse
            {
                var danceId = AkiledEnvironment.GetRandomNumber(1, 4);
                if (danceId > 0 && this.GetRoomUser().CarryItemID > 0)
                {
                    this.GetRoomUser().CarryItem(0);
                }

                this.GetRoomUser().DanceId = danceId;
                this.GetRoom().SendPacket(new DanceComposer(this.GetRoomUser().VirtualId, danceId));
                this._resetDanseTimer = 12;
                break;
            }
        }

        _ = this.GetRoom().AllowsShous(this.GetRoomUser(), this.GetBotData().Name + "_" + actionId);
    }

    public override void OnTimerTick()
    {
        if (this.GetBotData() == null)
        {
            return;
        }

        if (this._resetDanseTimer > 0)
        {
            this._resetDanseTimer--;

            if (this._resetDanseTimer <= 0)
            {
                if (this.GetRoomUser().DanceId > 0)
                {
                    this.GetRoomUser().DanceId = 0;
                    this.GetRoom().SendPacket(new DanceComposer(this.GetRoomUser().VirtualId, 0));
                }
            }
        }

        if (this._resetBotTimer > 0)
        {
            this._resetBotTimer--;

            if (this._resetBotTimer <= 0)
            {
                var bot = this.GetRoomUser();

                bot.RotHead = bot.BotData.Rot;
                bot.RotBody = bot.BotData.Rot;
                this.GetRoom().SendPacket(RoomItemHandling.TeleportUser(bot, new Point(bot.BotData.X, bot.BotData.Y), 0, this.GetRoom().gamemap.SqAbsoluteHeight(bot.BotData.X, bot.BotData.Y)));
            }
        }

        if (!AkiledEnvironment.GetChatOpenAI().IsReadyToSend() || this._lastMessageUser == null)
        {
            return;
        }

        if (this._timeoutTimer > 0)
        {
            this._timeoutTimer--;

            return;
        }

        var botName = this.GetBotData().Name;
        var userId = this._lastMessageUser.UserId;
        var userName = this._lastMessageUser.GetUsername();
        var userGender = this._lastMessageUser.Client?.GetHabbo()?.Gender;

        this._lastMessageUser = null;
        this._timeoutTimer = 10;

        _ = this.GetRoom().RunTask(async () =>
        {
            try
            {
                if (!AkiledEnvironment.GetChatOpenAI().IsReadyToSend())
                {
                    return;
                }

                this.GetRoom().SendPacket(new UserTypingMessageComposer(this.VirtualId, true));

                var listActions = "";
                foreach (var kvp in this._listActions)
                {
                    listActions += $"{kvp.Key}: {kvp.Value}. ";
                }

                var firstPrompt = !string.IsNullOrWhiteSpace(this.GetBotData().ChatText) ? this.GetBotData().ChatText : AkiledEnvironment.GetSettingsManager().TryGetValue("openia.prompt");
                firstPrompt = firstPrompt.Replace("{{botname}}", botName);
                firstPrompt = firstPrompt.Replace("{{username}}", userName);
                firstPrompt = firstPrompt.Replace("{{usergender}}", userGender == "M" ? "man" : "women");
                firstPrompt = firstPrompt.Replace("{{listusername}}", string.Join(",", this._listUserName));
                firstPrompt = firstPrompt.Replace("{{currentdate}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                firstPrompt = firstPrompt.Replace("{{listaction}}", listActions[..^1]);

                var prePrompt = new ChatCompletionMessage()
                {
                    Content = firstPrompt,
                    Role = "system"
                };
                var messagesSend = new List<ChatCompletionMessage>(new[] { prePrompt });
                messagesSend.AddRange(this._userMessages.TryGetValue(userId, out var userMessages) ? userMessages : new List<ChatCompletionMessage>());

                var messagesGtp = await AkiledEnvironment.GetChatOpenAI().SendChatMessage(messagesSend);

                if (messagesGtp != null)
                {
                    var message = messagesGtp.Content;
                    if (message.Contains("(Action: "))
                    {
                        message = message.Split("(Action: ")[0];
                        this.ParseActionId(messagesGtp.Content, userId);
                    }

                    var chatTexts = SplitSentence(message, 20);

                    foreach (var chatText in chatTexts.Take(4))
                    {
                        if (string.IsNullOrWhiteSpace(chatText) == false)
                        {
                            this.GetRoomUser().OnChat(chatText.Length > 150 ? chatText[..150] + "..." : chatText);
                        }
                    }

                    //stack the response as well - everything is context to Open AI
                    this.StackMessages(userId, messagesGtp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                this.GetRoom().SendPacket(new UserTypingMessageComposer(this.VirtualId, false));
            }
        });
    }

    private static List<string> SplitSentence(string sentence, int chunkSize)
    {
        var words = sentence.Split(' '); // Divise la phrase en mots individuels
        var chunks = new List<string>(); // Liste pour stocker les morceaux de la phrase

        var startIndex = 0;
        while (startIndex < words.Length)
        {
            var endIndex = Math.Min(startIndex + chunkSize, words.Length);
            var chunk = string.Join(" ", words, startIndex, endIndex - startIndex);
            chunks.Add(chunk);
            startIndex = endIndex;
        }

        return chunks;
    }

    [GeneratedRegex("\\(Action: (\\d+)\\)", options: RegexOptions.IgnoreCase, "fr-BE")]
    private static partial Regex MyRegex();
}
