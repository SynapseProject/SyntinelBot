using System;
using Newtonsoft.Json;

namespace SlackAPI
{
    //TODO: See below:
    /// <summary>
    ///     Please do a full, thorough, review of this.
    /// </summary>
    public class File
    {
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Channels;

        [JsonProperty("comments_count", NullValueHandling = NullValueHandling.Ignore)]
        public int CommentsCount;

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Created;

        [JsonProperty("display_as_bot", NullValueHandling = NullValueHandling.Ignore)]
        public bool DisplayAsBot;

        [JsonProperty("edit_link", NullValueHandling = NullValueHandling.Ignore)]
        public string EditLink;

        [JsonProperty("editable", NullValueHandling = NullValueHandling.Ignore)]
        public bool Editable;

        [JsonProperty("external_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalType;

        [JsonProperty("filetype", NullValueHandling = NullValueHandling.Ignore)]
        public string Filetype;

        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Groups;

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id;

        [JsonProperty("ims", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Ims;

        [JsonProperty("initial_comment", NullValueHandling = NullValueHandling.Ignore)]
        public FileComment InitialComment;

        [JsonProperty("is_external", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsExternal;

        [JsonProperty("is_public", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPublic;

        [JsonProperty("is_starred", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsStarred;

        [JsonProperty("lines", NullValueHandling = NullValueHandling.Ignore)]
        public int Lines;

        [JsonProperty("lines_more", NullValueHandling = NullValueHandling.Ignore)]
        public int LinesMore;

        [JsonProperty("mimetype", NullValueHandling = NullValueHandling.Ignore)]
        public string Mimetype;

        [JsonProperty("mode", NullValueHandling = NullValueHandling.Ignore)]
        public string Mode;

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name;

        [JsonProperty("num_stars", NullValueHandling = NullValueHandling.Ignore)]
        public int NumStars;

        [JsonProperty("permalink", NullValueHandling = NullValueHandling.Ignore)]
        public string Permalink;

        [JsonProperty("permalink_public", NullValueHandling = NullValueHandling.Ignore)]
        public string PermalinkPublic;

        [JsonProperty("pinned_to", NullValueHandling = NullValueHandling.Ignore)]
        public string[] PinnedTo;

        [JsonProperty("pretty_type", NullValueHandling = NullValueHandling.Ignore)]
        public string PrettyType;

        [JsonProperty("preview", NullValueHandling = NullValueHandling.Ignore)]
        public string Preview;

        [JsonProperty("preview_highlight", NullValueHandling = NullValueHandling.Ignore)]
        public string PreviewHighlight;

        [JsonProperty("public_url_shared", NullValueHandling = NullValueHandling.Ignore)]
        public bool PublicUrlShared;

        [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
        public Reaction[] Reactions;

        /// <summary>
        ///     File size in bytes
        /// </summary>
        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int Size;

        [JsonProperty("thumb_160", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb160;

        [JsonProperty("thumb_360", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb360;

        [JsonProperty("thumb_360_gif", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb360Gif;

        [JsonProperty("thumb_360_h", NullValueHandling = NullValueHandling.Ignore)]
        public int Thumb360H;

        [JsonProperty("thumb_360_w", NullValueHandling = NullValueHandling.Ignore)]
        public int Thumb360W;

        [JsonProperty("thumb_480", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb480;

        [JsonProperty("thumb_480_h", NullValueHandling = NullValueHandling.Ignore)]
        public int Thumb480H;

        [JsonProperty("thumb_480_w", NullValueHandling = NullValueHandling.Ignore)]
        public int Thumb480W;

        [JsonProperty("thumb_64", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb64;

        [JsonProperty("thumb_80", NullValueHandling = NullValueHandling.Ignore)]
        public string Thumb80;

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title;

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url;

        [JsonProperty("url_download", NullValueHandling = NullValueHandling.Ignore)]
        public string UrlDownload;

        [JsonProperty("url_private", NullValueHandling = NullValueHandling.Ignore)]
        public string UrlPrivate;

        [JsonProperty("url_private_download", NullValueHandling = NullValueHandling.Ignore)]
        public string UrlPrivateDownload;

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User;

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username;
    }
}