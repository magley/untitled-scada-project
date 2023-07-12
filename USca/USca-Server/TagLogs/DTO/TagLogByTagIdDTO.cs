using USca_Server.Tags;

namespace USca_Server.TagLogs.DTO
{
    public class TagLogByTagIdDTO
    {
        public string TagName { get; set; } = "";
        public List<TagLog> Logs { get; set; }

        public TagLogByTagIdDTO(Tag tag, List<TagLog> tagLogs)
        {
            TagName = tag.Name;
            Logs = tagLogs;
        }
    }
}
