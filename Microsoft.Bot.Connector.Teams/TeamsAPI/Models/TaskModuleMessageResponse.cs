// <auto-generated /> Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Bot.Connector.Teams.Models
{
    using System.Linq;

    /// <summary>
    /// Task Module response with message action.
    /// </summary>
    public partial class TaskModuleMessageResponse : TaskModuleResponseBase
    {
        /// <summary>
        /// Initializes a new instance of the TaskModuleMessageResponse class.
        /// </summary>
        public TaskModuleMessageResponse() { }

        /// <summary>
        /// Initializes a new instance of the TaskModuleMessageResponse class.
        /// </summary>
        /// <param name="type">Choice of action options when responding to the
        /// task/submit message. Possible values include: 'message',
        /// 'continue'</param>
        /// <param name="value">Teams will display the value of value in a
        /// popup message box.</param>
        public TaskModuleMessageResponse(string type = default(string), string value = default(string))
            : base(type)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets teams will display the value of value in a popup
        /// message box.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
