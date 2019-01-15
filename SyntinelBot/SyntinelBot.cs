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
using SyntinelBot.Models.Slack;

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
        private readonly ILogger<SyntinelBot> _logger;
        private IConfiguration _config;
        private RegisteredUsers _registeredUsers;
        private IStorage _botStore;
        private string _userRegistry = string.Empty;
        private string _welcomeText = string.Empty;
        private string _msteamsMention = string.Empty;
        private string _slackMention = string.Empty;
        private List<string> _notificationChannels = null;
        private string _appId = string.Empty;
        private string _password = string.Empty;
        private string _cardLocation = string.Empty;
        private static string _conversationId = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntinelBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="logger">A <see cref="ILogger"/> The logger.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        /// <param name="config">A <see cref="IConfiguration"/> Application configuration.</param>
        /// TODO: WIP
        public SyntinelBot(BotAccessors accessors, ILogger<SyntinelBot> logger, IConfiguration config)
        {
            try
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _config = config ?? throw new ArgumentNullException(nameof(config));
                _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
                _appId = _config.GetSection("MicrosoftAppId")?.Value;
                _password = _config.GetSection("MicrosoftAppPassword")?.Value;
                _registeredUsers = _config.Get<RegisteredUsers>();
                _userRegistry = _config.GetSection("UserRegistry")?.Value;
                _welcomeText = _config.GetSection("WelcomeText")?.Value;
                _msteamsMention = _config.GetSection("MsTeamsMention")?.Value;
                _slackMention = _config.GetSection("SlackMention")?.Value;
                _notificationChannels = _config.GetSection("NotificationChannels").Get<List<string>>(); // TODO: Check if it is used?
                _cardLocation = _config.GetSection("CardLocation")?.Value;

                _logger.LogInformation("Syntinel turn starts.");
                _logger.LogInformation($"Registered User Count: {_registeredUsers?.Users.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            // TODO: Remmove _botStore
            _botStore = Startup.BotStore;
        }

        /// <summary>
        /// Every conversation turn for our Bot will call this method.
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
        /// TODO: WIP
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            await SaveUserIfNewAsync(turnContext);

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            // TODO: WIP
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await LogActivityMessageAsync(turnContext);

                var activityText = turnContext.Activity.Text;

                if (!string.IsNullOrWhiteSpace(activityText) &&
                    activityText.ToLowerInvariant()
                        .Replace(_msteamsMention, string.Empty)
                        .Replace(_slackMention, string.Empty)
                        .Trim(' ', '\r', '\n') == "users")
                {
                    await ListRegisteredUsersAsync(turnContext, cancellationToken); // Done
                }
                else if (!string.IsNullOrWhiteSpace(activityText) &&
                         activityText.ToLowerInvariant()
                        .Replace(_msteamsMention, string.Empty)
                        .Replace(_slackMention, string.Empty)
                        .Trim(' ', '\r', '\n').StartsWith("notify"))
                {
                    await NotifyUserAsync(turnContext, cancellationToken);
                }
                else if (!string.IsNullOrWhiteSpace(activityText) &&
                         activityText.ToLowerInvariant()
                             .Replace(_msteamsMention, string.Empty)
                             .Replace(_slackMention, string.Empty)
                             .Trim(' ', '\r', '\n') == "notifications")
                {
                    await ListUserNotificationsAsync(turnContext); // Done
                }
                else if (!string.IsNullOrWhiteSpace(activityText) &&
                         activityText.ToLowerInvariant()
                             .Replace(_msteamsMention, string.Empty)
                             .Replace(_slackMention, string.Empty)
                             .Trim(' ', '\r', '\n') == "notifications details")
                {
                    await ListUserNotificationsAsync(turnContext, detailed: true); // TODO: Check if it is still a requirement?
                }
                else if (turnContext.Activity.ChannelId == "msteams" && turnContext.Activity.Value != null)
                {
                    var channelId = turnContext.Activity.ChannelId;
                    var userId = turnContext.Activity.From.Id;
                    var responseValue = (JObject)turnContext.Activity.Value;
                    try
                    {
                        var action = responseValue["action"].ToString();
                        string answer = string.Empty;
                        var instanceType = responseValue["instanceType"].ToString();
                        var instanceName = responseValue["instanceName"].ToString();
                        Guid.TryParse(responseValue["notificationId"].ToString(), out var notificationId);

                        switch (action)
                        {
                            case "resize":
                                var jobId = Guid.NewGuid();
                                if (AcknowledgeNotificationAsync(channelId, userId, notificationId).Result)
                                {
                                    answer = $"Job {jobId} started to {action} {instanceName} from t2.large to {instanceType}.";
                                    await NotifySyntinelAsync(turnContext, channelId, userId, notificationId, answer);
                                }
                                else
                                {
                                    answer = "Sorry, I am not able to find matching record for your request.";
                                }

                                break;
                            case "ignore":
                                answer = $"Notification to {action} {instanceName} is ignored.";
                                await NotifySyntinelAsync(turnContext, channelId, userId, notificationId, answer);
                                break;
                            default:
                                answer = "I am unable to process your request. Please contact the administrator.";
                                break;
                        }

                        await turnContext.SendActivityAsync(answer);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
                else if (turnContext.Activity.ChannelId == "slack" && turnContext.Activity.ChannelData != null)
                {
                    try
                    {
                        var channelId = turnContext.Activity.ChannelId;
                        var userId = turnContext.Activity.From.Id;
                        var channelData = (JObject)turnContext.Activity.ChannelData;
                        var slackMessage = channelData.ToObject<SlackMessage>();
                        if (slackMessage != null && slackMessage.Payload?.Type == "interactive_message")
                        {
                            var firstAction = slackMessage.Payload.Actions?.FirstOrDefault();
                            if (firstAction != null)
                            {
                                string answer = string.Empty;
                                var action = firstAction.Name;
                                var instanceType = firstAction.SelectedOptions.FirstOrDefault()?.Value;
                                var instanceName = slackMessage.Payload.CallbackId.Split('/')[0];
                                Guid.TryParse(slackMessage.Payload.CallbackId.Split('/')[1], out var notificationId);

                                switch (action)
                                {
                                    case "resize":
                                        if (instanceType != "ignore")
                                        {
                                            var jobId = Guid.NewGuid();
                                            if (AcknowledgeNotificationAsync(channelId, userId, notificationId).Result)
                                            {
                                                answer = $"Job {jobId} started to {action} {instanceName} from t2.large to {instanceType}.";
                                                await NotifySyntinelAsync(turnContext, channelId, userId, notificationId, answer);
                                            }
                                            else
                                            {
                                                answer = "Sorry, I am not able to find matching record for your request.";
                                            }
                                        }
                                        else
                                        {
                                            answer = $"Notification to {action} {instanceName} is ignored.";
                                            await NotifySyntinelAsync(turnContext, channelId, userId, notificationId, answer);
                                        }

                                        break;
                                    default:
                                        answer = "I am unable to process your request. Please contact the administrator.";
                                        break;
                                }

                                await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
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
        }

        private async Task LogActivityMessageAsync(ITurnContext turnContext)
        {
            try
            {
                // Get the conversation state from the turn context.
                var state = await _accessors.UserDataAccessor.GetAsync(turnContext, () => new UserData()
                {
                    Notifications = new List<Notification>(),
                    Jobs = new List<Job>(),
                });

                // Bump the turn count for this conversation.
                state.TurnCount++;
                state.Id = turnContext.Activity.From.Id;
                state.BotId = turnContext.Activity.Recipient.Id;
                state.BotName = turnContext.Activity.Recipient.Name;
                state.Name = turnContext.Activity.From.Name;
                state.ChannelId = turnContext.Activity.ChannelId;
                state.ServiceUrl = turnContext.Activity.ServiceUrl;
                state.ConversationId = turnContext.Activity.Conversation.Id;
                state.MessageText = turnContext.Activity.Text;
                state.MessageValue = turnContext.Activity.Value;
                state.MessageValueType = turnContext.Activity.ValueType;

                // Set the property using the accessor.
                await _accessors.UserDataAccessor.SetAsync(turnContext, state);

                // Save the new turn count into the conversation state.
                await _accessors.UserState.SaveChangesAsync(turnContext);

                var msg = $"Turn {state.TurnCount} " +
                          $"Conversation Id: {state.ConversationId} " +
                          $"Id: {state.Id} " +
                          $"Name: {state.Name} " +
                          $"BotId: {state.BotId} " +
                          $"BotName: {state.BotName} " +
                          $"ChannelId: {state.ChannelId} " +
                          $"ServiceUrl: {state.ServiceUrl} " +
                          $"Message Text: {state.MessageText} " +
                          $"Message Value: {state.MessageValue} ";
                _logger.LogInformation(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        // TODO: WIP
        private async Task<bool> AcknowledgeNotificationAsync(string channelId, string userId, Guid notificationId)
        {
            // TODO: Need to handle broadcast message later
            return true;
//            bool success = false;
//            if (!string.IsNullOrWhiteSpace(channelId) && !string.IsNullOrWhiteSpace(userId) && notificationId != Guid.Empty)
//            {
//                var storageKey = $"{channelId}/{userId.Replace(":", ";")}.UserState";
//                var userState = _botStore.ReadAsync<UserState>(new[] { storageKey }).Result?.FirstOrDefault().Value;
//                if (userState != null)
//                {
//                    foreach (var notification in userState.Notifications)
//                    {
//                        if (notification.Id == notificationId)
//                        {
//                            notification.Acknowledged = true;
//                            Dictionary<string, object>  changes = new Dictionary<string, object>();
//                            changes.Add(storageKey, userState);
//                            await _botStore.WriteAsync(changes);
//                            success = true;
//                            break;
//                        }
//                    }
//                }
//            }
//
//            return success;
        }

        // List all notifications pertaining to a user
        // TODO: WIP
        private async Task ListUserNotificationsAsync(ITurnContext turnContext, bool detailed = false)
        {
            if (turnContext == null)
            {
                _logger.LogError("TurnContext is null.");
                return;
            }

            var answer = "You do not have any unacknowledged notifications";
            var channelId = turnContext.Activity.ChannelId;

            try
            {
                // Get the user data from the turn context
                var userData = await _accessors.UserDataAccessor.GetAsync(turnContext, () => new UserData()
                {
                    Notifications = new List<Notification>(),
                    Jobs = new List<Job>(),
                });

                // Set the property using the accessor.
                await _accessors.UserDataAccessor.SetAsync(turnContext, userData);

                // Save the new turn count into the conversation state.
                await _accessors.UserState.SaveChangesAsync(turnContext);

                if (userData?.Notifications != null)
                {
                    answer = $"You have {userData.Notifications.Count} unacknowledged notifications.";

                    // TODO: Check if detailed option is still a requirement?
                    if (detailed)
                    {
                        foreach (var notification in userData.Notifications)
                        {
                            switch (channelId)
                            {
                                case "msteams":
                                    await SendTeamsInteractiveMessageAsync(turnContext, channelId, notification.Action, notification.Target, notification.ForUser, notification.Id);
                                    break;
                                case "slack":
                                    // await SendSlackInteractiveMessageAsync(turnContext, channelId, notification.Action, notification.Target, notification.ForUser, notification.Id);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                await turnContext.SendActivityAsync(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Capture new user information and writes the whole user list to a json file.
        /// </summary>
        private async Task SaveUserIfNewAsync(ITurnContext turnContext)
        {
            if (_userRegistry == null)
            {
                _logger.LogWarning("User registry json is not specified.");
                return;
            }

            try
            {
                var storageKey = $"{turnContext.Activity.ChannelId}/{turnContext.Activity.From.Id}".Replace(":", ";");

                if (_registeredUsers?.Users != null && !_registeredUsers.Users.ContainsKey(storageKey))
                {
                    var tenantId = string.Empty;
                    if (turnContext.Activity.ChannelId == "msteams")
                    {
                        var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();
                        if (channelData != null)
                        {
                            tenantId = channelData.Tenant.Id;
                        }
                    }

                    // TODO: Capture user's Slack team/channel information
                    // TODO: Store user information in a database, e.g. CosmoDB
                    var newUser = new User
                    {
                        Alias = $"{turnContext.Activity.From.Name}@{turnContext.Activity.ChannelId}".Replace(" ", "_"),
                        BotId = turnContext.Activity.Recipient.Id,
                        BotName = turnContext.Activity.Recipient.Name,
                        ChannelId = turnContext.Activity.ChannelId,
                        Id = turnContext.Activity.From.Id,
                        Name = turnContext.Activity.From.Name,
                        ServiceUrl = turnContext.Activity.ServiceUrl,
                        TenantId = tenantId,
                    };
                    _registeredUsers.Users.Add(storageKey, newUser);
                    var json = JsonConvert.SerializeObject(_registeredUsers, Formatting.Indented);
                    await File.WriteAllTextAsync(_userRegistry, json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        // Forward notification to specific user or channel (WIP)
        private async Task NotifyUserAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext == null)
            {
                _logger.LogError("TurnContext is null.");
                return;
            }

            var answer = string.Empty;
            var activityText = turnContext.Activity.Text;
            var args = Regex.Matches(activityText, @"[\""].+?[\""]|[^ ]+")
                .Select(m => m.Value)
                .ToList();
            if (args.Count != 2)
            {
                answer = "Syntax: notify <user alias>. Message content should be specified in the value field.";
            }
            else
            {
                var userAlias = args[1].ToLowerInvariant();
                var pos = userAlias.LastIndexOf('@');
                var channelId = pos != -1 ? userAlias.Substring(pos + 1) : string.Empty;

                if (!IsRegisteredUser(userAlias))
                {
                    answer = $"{userAlias} is not a registered user.";
                }
                else if (!_notificationChannels.Contains(channelId))
                {
                    answer = "Only channels 'msteams' and 'slack' are supported at the moment.";
                }
                else if (turnContext.Activity.Value == null)
                {
                    answer = "Value field is empty. There is no notification to send.";
                }
                else if (turnContext.Activity.ValueType != null &&
                         turnContext.Activity.ValueType.ToLowerInvariant() == "application/json" &&
                         !IsValidJson(turnContext.Activity.Value.ToString()))
                {
                    answer = "The content of the value field is not a valid JSON.";

                    // TODO: Add some codes to verify the Slack/Team message content
                }
                else
                {
                    var recipient = _registeredUsers.Users.Values.First(a => a.Alias == userAlias);
                    if (turnContext.Activity.Value is JObject value)
                    {
                        var messageContent = value.ToObject<SlackAPI.Message>();
                        if (messageContent != null)
                        {
                            _logger.LogInformation(messageContent.Text);
                            foreach (var attachment in messageContent.Attachments)
                            {
                                foreach (var action in attachment.Actions)
                                {
                                    _logger.LogInformation($"Action: {action.Name}");
                                }
                            }
                        }

                        switch (channelId)
                        {
                            case "msteams":
                                var notificationId = await SendTeamsInteractiveMessageAsync(null, channelId, string.Empty, string.Empty, recipient, Guid.Empty.ToString());
                                answer = $"Notification {notificationId} has been sent to {userAlias}.";
                                break;
                            case "slack":
                                var isSuccess = await SendSlackMessageAsync(recipient, messageContent);
                                answer = isSuccess ? $"Notification has been sent to {userAlias}." : $"Failed to send notification to {userAlias}";
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        answer = $"Message content is empty. There is nothing to send to {userAlias}.";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(answer))
            {
                await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
            }
        }

        private async Task<string> SendTeamsInteractiveMessageAsync(ITurnContext turnContext, string channelId, string action,
            string machineName, User recipient, string notificationId)
        {
            var filePath = string.Empty;
            notificationId = notificationId != Guid.Empty.ToString() ? notificationId : Guid.NewGuid().ToString();
            filePath = $"{_cardLocation}{action}.{channelId}.json";
            var userStateKey = $"{channelId}/{recipient.Id.Replace(":", ";")}.UserState";
            var conversationId = string.Empty;

            try
            {
                // Save the notification to the bot store
                if (_botStore != null)
                {
                    Notification notification = new Notification()
                    {
                        Id = notificationId,
                        Acknowledged = false,
                        ForUser = recipient,
                        Action = action,
                        Target = machineName,
                        From = string.Empty,
                        To = string.Empty,
                        // ConversationId = turnContext.Activity.Conversation.Id,
                        Initiator = turnContext.Activity.From,
                        NotificationTime = DateTime.Now,
                    };

                    var changes = new Dictionary<string, object>();
                    var userState = _botStore.ReadAsync<UserData>(new[] { userStateKey }).Result?.FirstOrDefault().Value;

                    if (userState == null)
                    {
                        userState = new UserData()
                        {
                            Id = recipient.Id,
                            BotId = recipient.BotId,
                            BotName = recipient.BotName,
                            Name = recipient.Name,
                            ServiceUrl = recipient.ServiceUrl,
                            ChannelId = recipient.ChannelId,
                            Jobs = new List<Job>(),
                            Notifications = new List<Notification>() { notification },
                        };
                    }
                    else if (userState.Notifications.FirstOrDefault(n => n.Id == notificationId) == null)
                    {
                        userState.Notifications.Add(notification);
                    }

                    // Store notification separately
                    var notificationKey = $"{notificationId}.Notification";
                    changes.Add(userStateKey, userState);
                    changes.Add(notificationKey, notification);
                    await _botStore.WriteAsync(changes);
                }

                var adaptiveCardJson = File.ReadAllText(filePath);
                adaptiveCardJson = adaptiveCardJson.Replace("[ResourceName]", machineName.ToUpperInvariant());
                adaptiveCardJson = adaptiveCardJson.Replace("[NotificationId]", notificationId.ToString());
                var attachment = new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(adaptiveCardJson),
                };

                var toId = recipient.Id;
                var toName = recipient.Name;
                var fromId = recipient.BotId;
                var fromName = recipient.BotName;
                var serviceUrl = recipient.ServiceUrl;
                var tenantId = recipient.TenantId;
                var userAccount = new ChannelAccount(toId, toName);
                var botAccount = new ChannelAccount(fromId, fromName);

                var message = Activity.CreateMessageActivity();
                message.From = botAccount;
                message.Recipient = userAccount;
                message.Attachments = new List<Attachment> { attachment };
                message.Locale = "en-us";

                if (turnContext != null)
                {
                    message.Conversation = turnContext.Activity.Conversation;
                    await turnContext.SendActivityAsync(message);
                }
                else
                {
                    var account = new MicrosoftAppCredentials(_appId, _password);
                    MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
                    var client = new ConnectorClient(new Uri(serviceUrl), account);
                    var conversation = await client.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);
                    conversationId = conversation.Id;
                    message.Conversation = new ConversationAccount(id: conversationId);
                    await client.Conversations.SendToConversationAsync((Activity)message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return notificationId;
        }

        // TODO: Pending review
        // Forward message to Slack
        private async Task<bool> SendSlackMessageAsync(User recipient, object messageContent)
        {
            if (recipient == null || messageContent == null)
            {
                return false;
            }

            bool isSuccess = false;

            try
            {
                var toId = recipient.Id;
                var toName = recipient.Name;
                var fromId = recipient.BotId;
                var fromName = recipient.BotName;
                var serviceUrl = recipient.ServiceUrl;
                var userAccount = new ChannelAccount(toId, toName);
                var botAccount = new ChannelAccount(fromId, fromName);

                var message = Activity.CreateMessageActivity();
                message.From = botAccount;
                message.Recipient = userAccount;
                message.ChannelData = messageContent;

                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
                var account = new MicrosoftAppCredentials(_appId, _password);
                var client = new ConnectorClient(new Uri(serviceUrl), account);

                // Reuse existing conversation  if recipient is a channel
                string conversationId;
                if (recipient.IsChannel)
                {
                    conversationId = recipient.ConversationId;
                }
                else
                {
                    // Note that Async version seems to have bug
                    var conversation = client.Conversations.CreateDirectConversation(botAccount, userAccount);
                    conversationId = conversation.Id;
                }

                message.Conversation = new ConversationAccount(id: conversationId);
                var response = await client.Conversations.SendToConversationAsync((Activity)message);
                _logger.LogInformation($"Response id: {response.Id}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return isSuccess;
        }

        private async Task NotifySyntinelAsync(ITurnContext turnContext, string channelId, string userId, Guid notificationId, string txtMessage)
        {
            if (_registeredUsers != null)
            {
                if (!string.IsNullOrWhiteSpace(channelId) && !string.IsNullOrWhiteSpace(userId) && notificationId != Guid.Empty)
                {
                    try
                    {
                        // Send notification from SyntinelBot@BP to Syntinel@BP
                        var syntinelBot = _registeredUsers.Users.Values.First(u => u.Alias == "syntinelbot@directline");
                        var syntinel = _registeredUsers.Users.Values.First(u => u.Alias == "syntinel@directline");
                        if (syntinel != null && syntinelBot != null && !string.IsNullOrWhiteSpace(_conversationId))
                        {
                            var serviceUrl = syntinel.ServiceUrl;
                            var botAccount = new ChannelAccount(syntinelBot.Id, syntinelBot.Name);
                            var userAccount = new ChannelAccount(syntinel.Id, syntinel.Name);
                            var account = new MicrosoftAppCredentials(_appId, _password);
                            var tenantId = syntinel.TenantId;
                            MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
                            var client = new ConnectorClient(new Uri(serviceUrl), account);
                            // var conversation = await client.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);

                            var message = Activity.CreateMessageActivity();
                            message.From = botAccount;
                            message.Recipient = userAccount;
                            message.Locale = "en-us";
                            message.Text = txtMessage;
                            message.Conversation = new ConversationAccount(id: _conversationId);
                            await client.Conversations.SendToConversationAsync((Activity)message);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }
            }
        }

        // DONE
        // Check if a user has been registered.
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

        // List registered users
        private async Task ListRegisteredUsersAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            string answer;
            if (_registeredUsers?.Users?.Count > 0)
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
        private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome {member.Name}. {_welcomeText}",
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
            var adaptiveCardAttachment = new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson)
            };
            return adaptiveCardAttachment;
        }

        // DONE
        // Validate if a string is a valid JSON.
        // https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        private bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();

            // For both object and array
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || (strInput.StartsWith("[") && strInput.EndsWith("]")))
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    _logger.LogError(jex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return false;
                }
            }

            return false;
        }
    }
}
