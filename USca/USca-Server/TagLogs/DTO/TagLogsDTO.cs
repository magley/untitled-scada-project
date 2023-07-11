using USca_Server.Tags;

namespace USca_Server.TagLogs.DTO
{
    public class TagLogsDTO
    {
        public List<TagLog> Logs { get; set; }

        public TagLogsDTO(List<TagLog> logs)
        {
            this.Logs = logs;
        }
    }
}
