using System.Net.WebSockets;

namespace USca_Server.Tags
{
    public interface ITagService
    {
        public void Add(TagAddDTO dto);
        public Tag? Get(int id);
        public List<Tag> GetAll();
        public void Update(TagDTO dto);
        public void Update(OutputTagValueDTO dto);
        public void Delete(int id);
        public List<OutputTagValueDTO> GetOutputTagValues();

        public Task SendTagValues(WebSocket ws);
    }
}
