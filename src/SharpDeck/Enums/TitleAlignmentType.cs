﻿namespace SharpDeck.Enums
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Provides an enumeration of alignments
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TitleAlignmentType
    {
        /// <summary>
        /// Top alignment.
        /// </summary>
        [EnumMember(Value = "top")]
        Top = 0,

        /// <summary>
        /// Bottom alignment.
        /// </summary>
        [EnumMember(Value = "middle")]
        Middle = 1,

        /// <summary>
        /// Middle alignment.
        /// </summary>
        [EnumMember(Value = "bottom")]
        Bottom = 2,
    }
}
