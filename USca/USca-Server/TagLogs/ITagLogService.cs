using USca_Server.Tags;

namespace USca_Server.TagLogs
{
    public interface ITagLogService
    {
        public void AddFrom(Tag tag);
        public void AddFrom(Tag tag, DateTime measureTimestamp);
        public void AddBatch(List<Tuple<Tag, DateTime>> batch);
        public TagLog? Get(int id);
        public List<TagLog> GetAll();
        public List<TagLog> GetLatestAnalogInputs();
        public List<TagLog> GetLatestDigitalInputs();
        public List<TagLog> GetAllByTag(int tagId);
    }
}
