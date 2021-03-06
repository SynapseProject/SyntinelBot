// <auto-generated /> Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// Metadata for a Task Module.
    /// </summary>
    public partial class TaskModuleTaskInfo
    {
        /// <summary>
        /// Initializes a new instance of the TaskModuleTaskInfo class.
        /// </summary>
        public TaskModuleTaskInfo() { }

        /// <summary>
        /// Initializes a new instance of the TaskModuleTaskInfo class.
        /// </summary>
        /// <param name="title">Appears below the app name and to the right of
        /// the app icon.</param>
        /// <param name="height">This can be a number, representing the task
        /// module's height in pixels, or a string, one of: small, medium,
        /// large.</param>
        /// <param name="width">This can be a number, representing the task
        /// module's width in pixels, or a string, one of: small, medium,
        /// large.</param>
        /// <param name="url">The URL of what is loaded as an iframe inside
        /// the task module. One of url or card is required.</param>
        /// <param name="card">The JSON for the Adaptive card to appear in the
        /// task module.</param>
        /// <param name="fallbackUrl">If a client does not support the task
        /// module feature, this URL is opened in a browser tab.</param>
        /// <param name="completionBotId">If a client does not support the
        /// task module feature, this URL is opened in a browser tab.</param>
        public TaskModuleTaskInfo(string title = default(string), object height = default(object), object width = default(object), string url = default(string), Attachment card = default(Attachment), string fallbackUrl = default(string), string completionBotId = default(string))
        {
            Title = title;
            Height = height;
            Width = width;
            Url = url;
            Card = card;
            FallbackUrl = fallbackUrl;
            CompletionBotId = completionBotId;
        }

        /// <summary>
        /// Gets or sets appears below the app name and to the right of the
        /// app icon.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets this can be a number, representing the task module's
        /// height in pixels, or a string, one of: small, medium, large.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "height")]
        public object Height { get; set; }

        /// <summary>
        /// Gets or sets this can be a number, representing the task module's
        /// width in pixels, or a string, one of: small, medium, large.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "width")]
        public object Width { get; set; }

        /// <summary>
        /// Gets or sets the URL of what is loaded as an iframe inside the
        /// task module. One of url or card is required.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the JSON for the Adaptive card to appear in the task
        /// module.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "card")]
        public Attachment Card { get; set; }

        /// <summary>
        /// Gets or sets if a client does not support the task module feature,
        /// this URL is opened in a browser tab.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "fallbackUrl")]
        public string FallbackUrl { get; set; }

        /// <summary>
        /// Gets or sets if a client does not support the task module feature,
        /// this URL is opened in a browser tab.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "completionBotId")]
        public string CompletionBotId { get; set; }

    }
}
