// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private readonly EchoBotAccessors _accessors;
        private readonly ILogger _logger;

        private const string WelcomeText = @"Welcome to Syntinel channel. Syntinel Bot is at your service.";

        // This array contains the file location of our adaptive cards
        private readonly string[] _cards =
        {
            @".\Resources\Ec2ResizeCard.json"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntinelBot"/> class.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public SyntinelBot(EchoBotAccessors accessors, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<SyntinelBot>();
            _logger.LogTrace("EchoBot turn start.");
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
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
                //// Get the conversation state from the turn context.
                //var state = await _accessors.CounterState.GetAsync(turnContext, () => new CounterState());

                //// Bump the turn count for this conversation.
                //state.TurnCount++;

                //// Set the property using the accessor.
                //await _accessors.CounterState.SetAsync(turnContext, state);

                //// Save the new turn count into the conversation state.
                //await _accessors.ConversationState.SaveChangesAsync(turnContext);

                //// Echo back to the user whatever they typed.
                //var responseMessage = $"Turn {state.TurnCount}: You sent '{turnContext.Activity.Text}'\n";
                //await turnContext.SendActivityAsync(responseMessage);
                if (!string.IsNullOrWhiteSpace(turnContext.Activity.Text) &&
                    turnContext.Activity.Text.ToLowerInvariant().Replace("<at>syntinelbot2</at>", string.Empty).Trim() == "notify")
                {
                    Random r = new Random();
                    var cardAttachment = CreateAdaptiveCardAttachment(this._cards[r.Next(this._cards.Length)]);
                    var reply = turnContext.Activity.CreateReply();
                    reply.Attachments = new List<Attachment>() { cardAttachment };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
                if (!string.IsNullOrWhiteSpace(turnContext.Activity.Text) && turnContext.Activity.Text.ToLowerInvariant().Trim() == "inform")
                {
                    // Use the data stored previously to create the required objects.
                    var toId = "29:1qabkb36ivJfq1RaTWsZKO3U4yVdk_JoipoLKU29qXXc22ZZ7qR0_Ifg4yNVKGKF-v2InhTuNLG3DU1UND1mlRw";
                    var toName = "ZHE YANG";
                    var fromId = "28:b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d";
                    var fromName = "syntinelbot2";
                    var serviceUrl = "https://smba.trafficmanager.net/apac/";
                    var userAccount = new ChannelAccount(toId, toName);
                    var botAccount = new ChannelAccount(fromId, fromName);
                    var tenantId = "a12bfa5b-4fe9-4755-83f0-a4a6ee8f2277";
                    var appId = "b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d";
                    var password = "pdwMACG16)^(hfdpXSE823-";


                    try
                    {
                        // Init connector
                        // MicrosoftAppCredentials cred = new MicrosoftAppCredentials(appId: "b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d", password: "pdwMACG16)^(hfdpXSE823-");
                        MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.Now.AddDays(7));
                        var account = new MicrosoftAppCredentials("b4e8dc4d-86fd-4675-bee0-00d4ecc35f4d", "pdwMACG16)^(hfdpXSE823-");
                        var client = new ConnectorClient(new Uri(serviceUrl), account);
                        var response = await client.Conversations.CreateOrGetDirectConversation(botAccount, userAccount, tenantId);

                        // Create a new message.
                        var message = Activity.CreateMessageActivity();
                        var conversationId = response.Id;
                        // Set the address-related properties in the message and send the message.
                        Random r = new Random();
                        var cardAttachment = CreateAdaptiveCardAttachment(this._cards[r.Next(this._cards.Length)]);
                        message.From = botAccount;
                        message.Recipient = userAccount;
                        message.Conversation = new ConversationAccount(id: conversationId);
                        message.Attachments = new List<Attachment>() { cardAttachment };
                        message.Locale = "en-us";
                        await client.Conversations.SendToConversationAsync((Activity)message);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message);
                    }


                    // https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bot-conversations/bots-conv-proactive
                    // https://stackoverflow.com/questions/48102932/microsoft-teams-bot-could-not-parse-tenant-id
//                    var parameters = new ConversationParameters
//                    {
//                        Bot = botAccount,
//                        Members = new ChannelAccount[] { userAccount },
//                        ChannelData = new TeamsChannelData
//                        {
//                            Tenant = channelData.Tenant
//                        }
//                    };
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
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.Typing)
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.DeleteUserData)
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
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
