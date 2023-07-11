using System.ComponentModel.DataAnnotations;
using USca_Server.Tags;

namespace USca_Server.TagLogs
{
    /// <summary>
    /// TagLog represents a value measured by a tag at a certain point in time.
    /// We need this to be able to report on all values of a specified tag.
    /// </summary>
    public class TagLog
    {
        [Key]
        public int Id { get; set; }
        public int TagId { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string TagName { get; set; } = "";
        public string TagDesc { get; set; } = "";
        public TagMode Mode { get; set; }
        public TagType Type { get; set; }
        public string Unit { get; set; } = "";

        public TagLog()
        {

        }

        public TagLog(Tag tag, DateTime measureTime)
        {
            TagId = tag.Id;
            Value = tag.Value;
            Timestamp = measureTime;
            this.TagName = tag.Name;
            this.TagDesc = tag.Desc;
            this.Mode = tag.Mode;
            this.Type = tag.Type;
            this.Unit = tag.Unit;
        }

        public static string LogEntry(Tag tag, DateTime timestamp)
        {
            return $"[{timestamp}] Tag {tag.Id} ({tag.Name}) logged value {tag.Value}";
        }
    }
}
