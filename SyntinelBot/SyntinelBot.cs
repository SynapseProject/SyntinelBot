// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyntinelBot.Models;

namespace SyntinelBot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class SyntinelBot : IBot
    {
        private readonly BotAccessors _accessors;
        private readonly ILogger _logger;
        private IConfiguration _config;
        private RegisteredUsers _registeredUsers;
        private IStorage _dataStore;
        private string _userDatabase = @".\UserDatabase.json";
        private const string WelcomeText = @"Welcome. Syntinel Bot is at your service.";
        private string _msteamsMention = "<at>syntinelbot</at>";
        private string _slackMention = "@syntinelbot";
        private List<string> _notificationChannels = new List<string>() { "msteams", "slack" };
        private string _appId = "b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d";
        private string _password = "pdwMACG16)^(hfdpXSE823-";

        // This array contains the file location of our adaptive cards
        string _cardLocation = @".\Resources\";
        private readonly Dictionary<string, string> _cards = new Dictionary<string, string>()
        {
            { "msteams/ec2", @".\Resources\Ec2Resize.msteams.json" },
            { "slack", @".\Resources\Ec2Resize.slack.json" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntinelBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public SyntinelBot(BotAccessors accessors, ILoggerFactory loggerFactory, IConfiguration config)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            if (config == null)
            {
                throw new System.ArgumentNullException(nameof(config));
            }

            _logger = loggerFactory.CreateLogger<SyntinelBot>();
            _logger.LogTrace("EchoBot turn start.");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            _config = config;

            _registeredUsers = _config.Get<RegisteredUsers>();
            _logger.LogInformation($"Registered User Count: {_registeredUsers?.Users.Count}");
            _dataStore = Startup.DataStore;
        }

        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var activityText = turnContext.Activity.Text;
                try
                {
                    // Get the conversation state from the turn context.
                    var state = await _accessors.UserState.GetAsync(turnContext, () => new UserState());

                    // Bump the turn count for this conversation.
                    state.TurnCount++;
                    state.Id = turnContext.Activity.From.Id;
                    state.BotId = turnContext.Activity.Recipient.Id;
                    state.BotName = turnContext.Activity.Recipient.Name;
                    state.Name = turnContext.Activity.From.Name;
                    state.ChannelId = turnContext.Activity.ChannelId;
                    state.ServiceUrl = turnContext.Activity.ServiceUrl;

                    var key = $"{turnContext.Activity.ChannelId}/{turnContext.Activity.From.Id}".Replace(":", ";");

                    if (!_registeredUsers.Users.ContainsKey(key))
                    {
                        var newUser = new User()
                        {
                            Alias = $"{turnContext.Activity.From.Name}@{turnContext.Activity.ChannelId}",
                            ChannelId = turnContext.Activity.ChannelId,
                            Id = turnContext.Activity.From.Id,
                            Name = turnContext.Activity.From.Name,
                            ServiceUrl = turnContext.Activity.ServiceUrl,
                            TenantId = string.Empty
                        };
                        _registeredUsers.Users.Add(key, newUser);
                        var json = JsonConvert.SerializeObject(_registeredUsers, Formatting.Indented);
                        File.WriteAllText(_userDatabase, json);
                    }

                    // Set the property using the accessor.
                    await _accessors.UserState.SetAsync(turnContext, state);

                    // Save the new turn count into the conversation state.
                    await _accessors.ConversationState.SaveChangesAsync(turnContext);

                    // Echo back to the user whatever they typed.

                    var msg = $"Turn {state.TurnCount} " +
                              $"Id: {state.Id} " +
                              $"BotId: {state.BotId} " +
                              $"BotName: {state.BotName} " +
                              $"Name: {state.Name} " +
                              $"ChannelId: {state.ChannelId} " +
                              $"ServiceUrl: {state.ServiceUrl} " +
                              $"Notifications: {state.Notifications?.Count} " +
                              $"Jobs: {state.Jobs?.Count}";
                    await turnContext.SendActivityAsync(msg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                if (_dataStore != null)
                {
                    var abc = await _dataStore.ReadAsync(new[] { "BotAccessors.UserState" }, cancellationToken);
                }

                if (!string.IsNullOrWhiteSpace(activityText) &&
                    activityText.ToLowerInvariant()
                        .Replace(_msteamsMention, string.Empty)
                        .Replace(_slackMention, string.Empty)
                        .Trim(' ', '\r', '\n') == "users")
                {
                    await ListRegisteredUsers(turnContext, cancellationToken);
                }
                else if (!string.IsNullOrWhiteSpace(activityText) &&
                         activityText.ToLowerInvariant()
                        .Replace(_msteamsMention, string.Empty)
                        .Replace(_slackMention, string.Empty)
                        .Trim(' ', '\r', '\n').StartsWith("notify"))
                {
                    await NotifyUser(turnContext, activityText, cancellationToken);
                }
                else if (turnContext.Activity.Value != null && turnContext.Activity.Value.GetType() == typeof(JObject))
                {
                    var responseValue = (JObject)turnContext.Activity.Value;
                    var action = responseValue["action"].ToString();
                    string responseMessage = string.Empty;
                    switch (action)
                    {
                        case "resize":
                            var instanceType = responseValue["instanceType"];
                            // var vm = responseValue["msteams"]["value"];
                            responseMessage = $"Action: {action}, New Instance Type: {instanceType}";
                            break;
                        case "ignore":
                            responseMessage = $"Action: {action}";
                            break;
                        default:
                            break;
                    }

                    await turnContext.SendActivityAsync(responseMessage);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.Typing)
            {
                // await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.DeleteUserData)
            {
                // await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else
            {
                // await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
        }

        private async Task NotifyUser(ITurnContext turnContext, string activityText, CancellationToken cancellationToken)
        {
            var answer = string.Empty;
            var args = Regex.Matches(activityText, @"[\""].+?[\""]|[^ ]+")
                .Select(m => m.Value)
                .ToList();
            if (args.Count != 4)
            {
                answer = "Syntax: notify <user alias> <action> <machine name>. Support actions: ec2resize, ec2patch, ec2snooze.";
            }
            else
            {
                var userAlias = args[1].ToLowerInvariant();
                var pos = userAlias.LastIndexOf('@');
                var channelId = pos != -1 ? userAlias.Substring(pos + 1) : string.Empty;
                var action = args[2].ToLowerInvariant();
                var machineName = args[3].ToUpperInvariant();

                if (!IsRegisteredUser(userAlias))
                {
                    answer = $"{userAlias} is not a registered user.";
                }
                else if (action != "ec2resize" && action != "ec2patch" && action != "ec2snooze")
                {
                    answer = $"Support actions: ec2resize, ec2patch, ec2snooze.";
                }
                else if (!_notificationChannels.Contains(channelId))
                {
                    answer = $"Supported channels: msteams, slack";
                }
                else
                {
                    string notificationId = await SendNotification(userAlias, channelId, action, machineName);
                    if (!string.IsNullOrWhiteSpace(notificationId))
                    {
                        answer = $"Notification {notificationId} has been sent to {userAlias}.";
                    }
                    else
                    {
                        answer = $"Unable to send notification to {userAlias}";
                    }
                }
            }

            await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
        }

        private async Task<string> SendNotification(string userAlias, string channelId, string action, string machineName)
        {
            string notificationId = string.Empty;
            string filePath = string.Empty;
            if (!string.IsNullOrWhiteSpace(userAlias) && !string.IsNullOrWhiteSpace(action) && !string.IsNullOrWhiteSpace(machineName) && _notificationChannels.Contains(channelId))
            {
                try
                {
                    var recipient = _registeredUsers.Users.Values.First(a => a.Alias == userAlias);
                    switch (channelId)
                    {
                        case "msteams":
                            notificationId = await SendTeamsInteractiveMessage(channelId, action, machineName, recipient);
                            break;
                        case "slack":
                            notificationId = await SendSlackInteractiveMessage(channelId, action, machineName, recipient);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                return notificationId;
            }

            return notificationId;
        }

        private async Task<string> SendTeamsInteractiveMessage(string channelId, string action, string machineName, User recipient)
        {
            string filePath;
            string notificationId;
            filePath = $"{_cardLocation}{action}.{channelId}.json";
            var adaptiveCardJson = File.ReadAllText(filePath);
            adaptiveCardJson = adaptiveCardJson.Replace("{ResourceName}", machineName.ToUpperInvariant());
            var attachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };

            var toId = recipient.Id;
            var toName = recipient.Name;
            var fromId = "28:b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d";
            var fromName = "SyntinelBot";
            var serviceUrl = recipient.ServiceUrl;
            var tenantId = recipient.TenantId;
            var userAccount = new ChannelAccount(toId, toName);
            var botAccount = new ChannelAccount(fromId, fromName);

            var account = new MicrosoftAppCredentials(_appId, _password);
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            var client = new ConnectorClient(new Uri(serviceUrl), account);
            var response = await client.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);
            // Create a new message.
            var message = Activity.CreateMessageActivity();
            var conversationId = response.Id;

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            message.Attachments = new List<Attachment>() {attachment};
            message.Locale = "en-us";
            await client.Conversations.SendToConversationAsync((Activity) message);
            notificationId = DateTime.Now.Ticks.ToString();
            return notificationId;
        }

        private async Task<string> SendSlackInteractiveMessage(string channelId, string action, string machineName, User recipient)
        {
            string filePath;
            string notificationId;
            filePath = $"{_cardLocation}{action}.{channelId}.json";
            var cardJson = File.ReadAllText(filePath);
            cardJson = cardJson.Replace("{ResourceName}", machineName.ToUpperInvariant());

            var toId = recipient.Id;
            var toName = recipient.Name;
            var fromId = "BEF9N1X1P:TEH9KHQG6";
            var fromName = "syntinelbot";
            var serviceUrl = recipient.ServiceUrl;
            var tenantId = recipient.TenantId;
            var userAccount = new ChannelAccount(toId, toName);
            var botAccount = new ChannelAccount(fromId, fromName);
            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
            var account = new MicrosoftAppCredentials(_appId, _password);
            var client = new ConnectorClient(new Uri(serviceUrl), account);
            var conversation = client.Conversations.CreateDirectConversation(botAccount, userAccount); // Note that Async version seems to have bug

            // Create a new message.
            var message = Activity.CreateMessageActivity();
            var conversationId = conversation.Id;

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            message.ChannelData = JsonConvert.DeserializeObject(cardJson);
            message.Locale = "en-us";
            await client.Conversations.SendToConversationAsync((Activity)message);
            notificationId = DateTime.Now.Ticks.ToString();
            return notificationId;
        }

        private bool IsRegisteredUser(string userAlias)
        {
            var foundUser = false;

            if (_registeredUsers != null && !string.IsNullOrWhiteSpace(userAlias))
            {
                foreach (var user in _registeredUsers.Users)
                {
                    if (userAlias.ToLowerInvariant() == user.Value.Alias)
                    {
                        foundUser = true;
                        break;
                    }
                }
            }

            return foundUser;
        }

        private async Task ListRegisteredUsers(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var answer = string.Empty;
            if (_registeredUsers != null && _registeredUsers.Users?.Count > 0)
            {
                answer = "Registered Users:";
                foreach (var user in _registeredUsers.Users.Values)
                {
                    answer = $"{answer}  {user.Alias}";
                }
            }
            else
            {
                answer = "There is no registered user";
            }

            await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="Attachment"/> that contains an <see cref="AdaptiveCard"/>.
        /// </summary>
        /// <param name="filePath">The path to the <see cref="AdaptiveCard"/> json file.</param>
        /// <returns>An <see cref="Attachment"/> that contains an adaptive card.</returns>
        private static Attachment CreateAdaptiveCardAttachment(string filePath)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }
    }
}
