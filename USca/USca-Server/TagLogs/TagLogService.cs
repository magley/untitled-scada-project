using USca_Server.Shared;
using USca_Server.Tags;

namespace USca_Server.TagLogs
{
    public class TagLogService : ITagLogService
    {
        public void AddFrom(Tag tag, DateTime measureTimestamp)
        {
            TagLog tagLog = new(tag, measureTimestamp);

            using (var db = new ServerDbContext())
            {
                db.TagLogs.Add(tagLog);
                db.SaveChanges();
            }
        }

        public void AddFrom(Tag tag)
        {
            TagLog tagLog = new(tag, DateTime.Now);

            using (var db = new ServerDbContext())
            {
                db.TagLogs.Add(tagLog);
                db.SaveChanges();
            }
        }
        

        public TagLog? Get(int id)
        {
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.Find(id);
            }
        }

        public List<TagLog> GetAll()
        {
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.ToList();
            }
        }

        public List<TagLog> GetAllByTag(int tagId)
        {
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.Where(t => t.TagId == tagId).ToList();
            }
        }
    }
}
