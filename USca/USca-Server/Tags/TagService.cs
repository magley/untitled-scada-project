using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace USca_Server.Tags
{
    public class TagService : ITagService
    {
        public void Add(TagAddDTO dto)
        {
            Tag t = new(dto);

            using (var db = new ServerDbContext())
            {
                db.Tags.Add(t);
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new ServerDbContext())
            {
                var tag = db.Tags.Find(id);

                if (tag != null)
                {
                    db.Tags.Remove(tag);
                    db.SaveChanges();
                }
            }
        }

        public List<Tag> GetAll()
        {
            using (var db = new ServerDbContext())
            {
                return db.Tags.ToList();
            }
        }

        public void Update(TagDTO dto)
        {
            using (var db = new ServerDbContext())
            {
                var tag = db.Tags.Find(dto.Id);
                if (tag != null)
                {
                    db.Tags.Entry(tag).CurrentValues.SetValues(dto);
                    db.SaveChanges();
                }
            }
        }
    }
}
