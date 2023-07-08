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

        public TagLog()
        {

        }

        public TagLog(Tag tag, DateTime measureTime)
        {
            TagId = tag.Id;
            Value = tag.Value;
            Timestamp = measureTime;
        }
    }
}
