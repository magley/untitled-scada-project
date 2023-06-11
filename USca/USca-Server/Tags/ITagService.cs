namespace USca_Server.Tags
{
    public interface ITagService
    {
        public void Add(TagAddDTO dto);
        public List<Tag> GetAll();
        public void Update(TagDTO dto);
        public void Delete(int id);
    }
}
