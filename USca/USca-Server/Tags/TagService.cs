using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;

namespace USca_Server.Tags
{
    public class TagService : ITagService
    {
        public void Add(TagAddDTO dto)
        {
            Tag t = new(dto);

            using (var db = new ServerDbContext())
            {
                db.Tags.Load();
                db.Tags.Add(t);
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new ServerDbContext())
            {
                db.Tags.Load();
                var tag = db.Tags.Find(id);

                if (tag != null)
                {
                    db.Tags.Remove(tag);
                }
                db.SaveChanges();
            }
        }

        public List<Tag> GetAll()
        {
            using (var db = new ServerDbContext())
            {
                db.Tags.Load();
                return db.Tags.ToList();
            }
        }

        public void Update(TagDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
