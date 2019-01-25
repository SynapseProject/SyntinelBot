using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using AwsAPI;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Bot.Schema;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SlackAPI;
using SyntinelBot.Models;
using Attachment = Microsoft.Bot.Schema.Attachment;
using File = System.IO.File;

namespace SyntinelBot
{
    /// <summary>
    ///     Represents a bot that processes incoming activities.
    ///     For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    ///     This is a Transient lifetime service.  Transient lifetime services are created
    ///     each time they're requested. For each Activity received, a new instance of this
    ///     class is created. Objects that are expensive to construct, or have a lifetime
    ///     beyond the single turn, should be carefully managed.
    ///     For example, the <see cref="MemoryStorage" /> object and associated
    ///     <see cref="IStatePropertyAccessor{T}" /> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1" />
    public class SyntinelBot : IBot
    {
        private readonly BotAccessors _accessors;
        private readonly ILogger<SyntinelBot> _logger;
        private readonly string _appId = string.Empty;
        private readonly string _cardLocation = string.Empty;
        private readonly IConfiguration _config;
        private readonly string _msteamsMention = "<at>syntinelbot</at>";
        private readonly string _password = string.Empty;
        private readonly string _slackMention = "@syntinelbot";
        private readonly string _welcomeText = "Syntinel Bot is at your service.";
        private readonly string _syntinelBaseUrl = string.Empty;
        private readonly string _syntinelSlackCueUrl = string.Empty;
        private readonly string _syntinelTeamsCueUrl = string.Empty;
        private readonly string _awsRegion = string.Empty;
        private readonly string _awsAccessKey = string.Empty;
        private readonly string _awsSecretKey = string.Empty;
        private RegisteredUsers _registeredUsers;

        // The DialogSet that contains all the Dialogs that can be used at runtime.
        private readonly DialogSet _dialogs = null;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyntinelBot" /> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}" /> used to manage state.</param>
        /// <param name="logger">A <see cref="ILogger" /> The logger.</param>
        /// <seealso
        ///     cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider" />
        /// <param name="config">A <see cref="IConfiguration" /> Application configuration.</param>
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
                _welcomeText = _config.GetSection("WelcomeText")?.Value;
                _msteamsMention = _config.GetSection("MsTeamsMention")?.Value ?? string.Empty;
                _slackMention = _config.GetSection("SlackMention")?.Value ?? string.Empty;
                _cardLocation = _config.GetSection("CardLocation")?.Value;
                _syntinelBaseUrl = _config.GetSection("SyntinelBaseUrl")?.Value;
                _syntinelSlackCueUrl = _config.GetSection("SyntinelSlackCueUrl")?.Value;
                _syntinelTeamsCueUrl = _config.GetSection("SyntinelTeamsCueUrl")?.Value;
                _awsRegion = _config.GetSection("AwsRegion")?.Value;
                _awsAccessKey = _config.GetSection("AwsAccessKey")?.Value;
                _awsSecretKey = _config.GetSection("AwsSecretKey")?.Value;
                if (_config.GetSection("UserRegistryInDatabase")?.Value != "true")
                {
                    _logger.LogInformation("Loading user registry from appsettings.json...");
                    _registeredUsers = _config.Get<RegisteredUsers>();
                }

                _logger.LogInformation("Syntinel turn starts.");

                // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
                _dialogs = new DialogSet(accessors.ConversationDialogState);

                // This array defines how the Waterfall will execute.
                var waterfallSteps = new WaterfallStep[]
                {
                    ProcessMessagesAsync,
                };
                _dialogs.Add(new WaterfallDialog("processMessages", waterfallSteps));
                _dialogs.Add(new ConfirmPrompt("confirm"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task LoadUserRegistryAsync(ITurnContext turnContext)
        {
            if (_registeredUsers == null)
            {
                if (_accessors == null)
                {
                    _logger.LogError("Database cannot be accessed as accessor is null.");
                    return;
                }

                _logger.LogInformation("Loading user registry from database...");
                _registeredUsers = await _accessors.UserRegistryAccessor.GetAsync(turnContext, () => new RegisteredUsers());

                _logger.LogInformation($"Registered User Count: {_registeredUsers?.Users?.Count}");
                var state = _registeredUsers;

                // Set the property using the accessor.
                await _accessors.UserRegistryAccessor.SetAsync(turnContext, state);

                // Save the new turn count into the conversation state.
                await _accessors.ServiceState.SaveChangesAsync(turnContext);
            }
        }

        /// <summary>
        ///     Every conversation turn for our Bot will call this method.
        ///     There are no dialogs used, since it's "single turn" processing, meaning a single
        ///     request and response.
        /// </summary>
        /// <param name="turnContext">
        ///     A <see cref="ITurnContext" /> containing all the data needed
        ///     for processing this conversation turn.
        /// </param>
        /// <param name="cancellationToken">
        ///     (Optional) A <see cref="CancellationToken" /> that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        /// <returns>A <see cref="Task" /> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet" />
        /// <seealso cref="ConversationState" />
        /// <seealso cref="IMiddleware" />
        /// TODO: WIP
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            await LoadUserRegistryAsync(turnContext);
            await SaveUserIfNewAsync(turnContext);

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            // TODO: WIP
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await LogActivityMessageAsync(turnContext);

                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync("processMessages", null, cancellationToken);
                }
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    // Check for a result.
                    if (results.Result != null)
                    {
                        // Finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                        // await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you, your response is '{results.Result}'."));
                        _logger.LogInformation($"Thank you, your response is '{results.Result}'.");
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
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

            // Save the new turn count into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private async Task RegisterUsersAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext == null)
            {
                _logger.LogError("TurnContext is null.");
                return;
            }

            if (_registeredUsers?.Users == null)
            {
                _logger.LogError("User registry is null.");
                return;
            }

            string answer;
            var activityText = turnContext.Activity.Text;
            var args = Regex.Matches(activityText, @"[\""].+?[\""]|[^ ]+")
                .Select(m => m.Value)
                .ToList();

            if (args.Count != 2 ||
                !args[1].ToLowerInvariant().EndsWith($"@{turnContext.Activity.ChannelId}"))
            {
                answer = "Syntax: '/register <alias>' to register your user alias. Alias should be suffixed with your respective channel id, '@slack', '@msteams', '@directline'.";
            }
            else
            {
                var storageKey = $"{turnContext.Activity.ChannelId}/{turnContext.Activity.From.Id}".Replace(":", ";");
                var userAlias = args[1].ToLowerInvariant();

                // User is new.
                if (!_registeredUsers.Users.ContainsKey(storageKey))
                {
                    _logger.LogInformation("Registering new user with the specified alias...");
                    var tenantId = string.Empty;
                    if (turnContext.Activity.ChannelId == "msteams")
                    {
                        var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();
                        if (channelData != null)
                        {
                            tenantId = channelData.Tenant.Id;
                        }
                    }

                    var newUser = new User
                    {
                        Alias = userAlias,
                        BotId = turnContext.Activity.Recipient.Id,
                        BotName = turnContext.Activity.Recipient.Name,
                        ChannelId = turnContext.Activity.ChannelId,
                        Id = turnContext.Activity.From.Id,
                        Name = turnContext.Activity.From.Name,
                        ServiceUrl = turnContext.Activity.ServiceUrl,
                        TenantId = tenantId,
                    };
                    _registeredUsers.Users.Add(storageKey, newUser);
                }
                else
                {
                    // Existing user.
                    _logger.LogInformation("Updating existing user with the specified alias...");
                    _registeredUsers.Users.TryGetValue(storageKey, out var existingUser);
                    if (existingUser != null)
                    {
                        existingUser.Alias = userAlias;
                        _registeredUsers.Users[storageKey] = existingUser;
                    }
                }

                var state = _registeredUsers;

                // Set the property using the accessor.
                await _accessors.UserRegistryAccessor.SetAsync(turnContext, state, cancellationToken);

                // Save the new turn count into the conversation state.
                await _accessors.ServiceState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
                answer = $"Your alias {args[1]} has been registered.";
            }

            if (!string.IsNullOrWhiteSpace(answer))
            {
                await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
            }
        }

        private async Task LogActivityMessageAsync(ITurnContext turnContext)
        {
            try
            {
                // Get the conversation state from the turn context.
                var state = await _accessors.UserDataAccessor.GetAsync(turnContext, () => new UserData
                {
                    Notifications = new List<Notification>(),
                    Jobs = new List<Job>()
                });

                // Bump the turn count for this conversation.
                state.TurnCount++;
                state.Id = turnContext.Activity.From.Id;
                state.BotId = turnContext.Activity.Recipient.Id;
                state.BotName = turnContext.Activity.Recipient.Name;
                state.Name = turnContext.Activity.From.Name;
                state.ChannelId = turnContext.Activity.ChannelId;
                state.ChannelData = turnContext.Activity.ChannelData;
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
                          $"Message Text: {state.MessageText} \n" +
                          $"Message Value: {state.MessageValue} \n" +
                          $"Channel Data: {state.ChannelData}";
                _logger.LogInformation(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        ///     Capture new user information and writes the whole user list to a json file.
        /// </summary>
        private async Task SaveUserIfNewAsync(ITurnContext turnContext)
        {
            try
            {
                var storageKey = $"{turnContext.Activity.ChannelId}/{turnContext.Activity.From.Id}".Replace(":", ";");

                if (_registeredUsers?.Users != null && !_registeredUsers.Users.ContainsKey(storageKey))
                {
                    var tenantId = string.Empty;
                    var teamId = string.Empty;
                    if (turnContext.Activity.ChannelId == "msteams")
                    {
                        var channelData = turnContext.Activity.GetChannelData<TeamsChannelData>();
                        if (channelData != null)
                        {
                            tenantId = channelData.Tenant.Id;
                            teamId = channelData.Team.Id;
                        }
                    }

                    if (turnContext.Activity.ChannelId == "slack")
                    {
                        teamId = turnContext.Activity.Id?.Split(":").Last();
                    }

                    var newUser = new User
                    {
                        Alias = $"{turnContext.Activity.From.Name}@{turnContext.Activity.ChannelId}".Replace(" ", "_"),
                        BotId = turnContext.Activity.Recipient.Id,
                        BotName = turnContext.Activity.Recipient.Name,
                        ChannelId = turnContext.Activity.ChannelId,
                        Id = turnContext.Activity.From.Id,
                        Name = turnContext.Activity.From.Name,
                        ServiceUrl = turnContext.Activity.ServiceUrl,
                        TeamId = teamId,
                        TenantId = tenantId,
                    };
                    _registeredUsers.Users.Add(storageKey, newUser);

                    _logger.LogInformation("Saving user registry to database.");
                    var state = _registeredUsers;

                    // Set the property using the accessor.
                    await _accessors.UserRegistryAccessor.SetAsync(turnContext, state);

                    // Save the new turn count into the conversation state.
                    await _accessors.ServiceState.SaveChangesAsync(turnContext);
                }
                else
                {
                    _logger.LogInformation($"'{turnContext.Activity.From.Name}' is an existing user.");
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
                else if (channelId != "slack" && channelId != "msteams")
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
                }
                else
                {
                    var recipient = _registeredUsers.Users.Values.First(a => a.Alias == userAlias);
                    if (recipient != null)
                    {
                        if (turnContext.Activity.Value is JObject value)
                        {
                            bool isSuccess = false;
                            switch (channelId)
                            {
                                case "msteams":
                                    var attachment = value.ToObject<AdaptiveCard>();
                                    if (attachment != null)
                                    {
                                        isSuccess = await SendTeamsMessageAsync(recipient, attachment);
                                    }

                                    break;
                                case "slack":
                                    var messageContent = value.ToObject<Message>();
                                    if (messageContent != null && !messageContent.IsEmpty())
                                    {
                                        _logger.LogInformation(messageContent.Text);
                                        isSuccess = await SendSlackMessageAsync(recipient, messageContent);
                                    }

                                    break;
                            }

                            answer = isSuccess ? $"Notification has been sent to {userAlias}." :
                                $"Failed to send notification to {userAlias}";
                        }
                        else
                        {
                            answer = $"{userAlias} is not a registered user.";
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
                _logger.LogInformation(answer);
                await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
            }
        }

        // Forward message to Slack
        private async Task<bool> SendTeamsMessageAsync(User recipient, object messageContent)
        {
            if (recipient == null || messageContent == null) return false;

            var isSuccess = false;

            try
            {
                var toId = recipient.Id;
                var toName = recipient.Name;
                var fromId = recipient.BotId;
                var fromName = recipient.BotName;
                var serviceUrl = recipient.ServiceUrl;
                var tenantId = recipient.TenantId; // Required for Microsoft Teams
                var userAccount = new ChannelAccount(toId, toName);
                var botAccount = new ChannelAccount(fromId, fromName);

                var attachment = new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = messageContent
                };

                var message = Activity.CreateMessageActivity();
                message.From = botAccount;
                message.Recipient = userAccount;
                message.Attachments = new List<Attachment>() { attachment };

                MicrosoftAppCredentials.TrustServiceUrl(serviceUrl);
                var account = new MicrosoftAppCredentials(_appId, _password);
                var client = new ConnectorClient(new Uri(serviceUrl), account);

                // Reuse existing conversation if recipient is a channel
                string conversationId;
                ConversationResourceResponse conversation = null;
                if (recipient.IsGroupChannel)
                {
                    var conversationParameters = new ConversationParameters
                    {
                        IsGroup = true,
                        ChannelData = new TeamsChannelData
                        {
                            Channel = new ChannelInfo(recipient.Id),
                        },
                        Activity = (Activity)message,
                    };
                    conversation = await client.Conversations.CreateConversationAsync(conversationParameters);
                }
                else
                {
                    conversation = client.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);
                    conversationId = conversation.Id;
                    message.Conversation = new ConversationAccount(id: conversationId);
                    var response = await client.Conversations.SendToConversationAsync((Activity)message);
                    _logger.LogInformation($"Response id: {response.Id}");
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return isSuccess;
        }


        // TODO: Pending review
        // Forward message to Slack
        private async Task<bool> SendSlackMessageAsync(User recipient, object messageContent)
        {
            if (recipient == null || messageContent == null) return false;

            var isSuccess = false;

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
                if (recipient.IsGroupChannel)
                {
                    conversationId = recipient.SlackConversationId;
                }
                else
                {
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
                foreach (var user in _registeredUsers.Users.Values) answer = $"{answer}  {user.Alias}";
            }
            else
            {
                answer = "There is no registered user";
            }

            await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">
        ///     A <see cref="ITurnContext" /> containing all the data needed
        ///     for processing this conversation turn.
        /// </param>
        /// <param name="cancellationToken">
        ///     (Optional) A <see cref="CancellationToken" /> that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        /// <returns>A <see cref="Task" /> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet" />
        /// <seealso cref="ConversationState" />
        /// <seealso cref="IMiddleware" />
        private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = $"Welcome {member.Name}. {_welcomeText}";
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        /// <summary>
        ///     Creates an <see cref="Attachment" /> that contains an <see cref="AdaptiveCard" />.
        /// </summary>
        /// <param name="filePath">The path to the <see cref="AdaptiveCard" /> json file.</param>
        /// <returns>An <see cref="Attachment" /> that contains an adaptive card.</returns>
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
            if (strInput.StartsWith("{") && strInput.EndsWith("}") ||
                strInput.StartsWith("[") && strInput.EndsWith("]"))
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

        private async Task<DialogTurnResult> ProcessMessagesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var turnContext = stepContext.Context;
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
                         .Trim(' ', '\r', '\n').StartsWith("/register"))
            {
                await RegisterUsersAsync(turnContext, cancellationToken);
            }
            else if ((turnContext.Activity.ChannelId == "msteams" || turnContext.Activity.ChannelId == "slack") && turnContext.Activity.Value != null)
            {
                await ForwardCueResponseAsync(turnContext, cancellationToken);
            }

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            // return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);
        }

        private async Task ForwardCueResponseAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            try
            {
                if (turnContext != null && turnContext.Activity.ChannelData is JObject channelData)
                {
                    _logger.LogInformation(channelData.ToString(Formatting.Indented));
                    string answer = "Your response is being processed.";
                    await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);

                    if (!string.IsNullOrWhiteSpace(_syntinelBaseUrl) &&
                        !string.IsNullOrWhiteSpace(_syntinelSlackCueUrl) &&
                        !string.IsNullOrWhiteSpace(_awsRegion) &&
                        !string.IsNullOrWhiteSpace(_awsAccessKey) &&
                        !string.IsNullOrWhiteSpace(_awsSecretKey))
                    {
                        var client = new RestClient(_syntinelBaseUrl);
                        AwsApiKey apiKey = new AwsApiKey()
                        {
                            Region = _awsRegion,
                            AccessKey = _awsAccessKey,
                            SecretKey = _awsSecretKey,
                        };
                        client.Authenticator = new Sig4Authenticator(apiKey);

                        var relativeUrl = string.Empty;
                        switch (turnContext.Activity.ChannelId)
                        {
                            case "slack":
                                relativeUrl = _syntinelSlackCueUrl;
                                break;
                            case "msteams":
                                relativeUrl = _syntinelTeamsCueUrl;
                                break;
                            default:
                                break;
                        }

                        if (!string.IsNullOrWhiteSpace(_syntinelBaseUrl) && !string.IsNullOrWhiteSpace(relativeUrl))
                        {
                            var jsonString = channelData.ToString(Formatting.Indented);
                            var request = new RestRequest();
                            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
                            request.Method = Method.POST;
                            request.Resource = relativeUrl;

                            // easy async support
                            client.ExecuteAsync(request, response =>
                            {
                                _logger.LogInformation(response.Content);
                            });
                        }
                        else
                        {
                            _logger.LogError("Valid Syntinel cue response url is not specified.");
                        }
                    }
                    else
                    {
                        _logger.LogError("Unable to post user response to Syntinel. Please check for missing configurations.");
                    }
                }
                else
                {
                    _logger.LogError("Unable to obtain channel data from turn context.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}