﻿using System;

namespace USca_ReportManager.Util
{
    public class TagLogDTO
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string TagName { get; set; } = "";
        public string TagDesc { get; set; } = "";
        public string Unit { get; set; } = "";
    }
}
