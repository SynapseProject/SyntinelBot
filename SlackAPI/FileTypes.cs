using System;

namespace SlackAPI
{
    [Flags]
    public enum FileTypes
    {
        All = 63,
        Posts = 1,
        Snippets = 2,
        Images = 4,
        Gdocs = 8,
        Zips = 16,
        Pdfs = 32
    }
}