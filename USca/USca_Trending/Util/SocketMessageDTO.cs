﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace USca_Trending.Util
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SocketMessageType
    {
        DELETE_TAG_READING,
        UPDATE_TAG_READING,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }
}
