﻿using System.ComponentModel;
using System.Text.Json.Serialization;

namespace USca_DbManager.Tags
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TagMode
    {
        Input,
        Output
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TagType
    {
        Digital,
        Analog
    }

    public partial class TagAddDTO : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public string Desc { get; set; } = "";
        public TagMode Mode { get; set; }
        public TagType Type { get; set; }
        public int Address { get; set; }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 10.0;
        public string Unit { get; set; } = "";
        public int ScanTime { get; set; } = 1000;
        public bool IsScanning { get; set; } = true;
        public double Value { get; set; }
    }
}
