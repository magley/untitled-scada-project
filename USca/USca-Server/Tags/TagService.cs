using System.Net.WebSockets;
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
                db.Tags.Add(t);
                db.SaveChanges();
            }
        }

        public Tag? Get(int id)
        {
            using (var db = new ServerDbContext())
            {
                return db.Tags.Find(id);
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

                    if (tag.Type == TagType.Digital)
                    {
                        tag.Value = Convert.ToDouble(Convert.ToBoolean(dto.Value));
                    }

                    db.SaveChanges();
                }
            }
        }

        public void Update(OutputTagValueDTO dto)
        {
            // TODO: Delete this.
        }

        public List<OutputTagValueDTO> GetOutputTagValues()
        {
            using (var db = new ServerDbContext())
            {
                return db.Tags.Where(tag => tag.Mode == TagMode.Output).Select(tag => new OutputTagValueDTO()
                {
                    Id = tag.Id,
                    Address = tag.Address,
                    Value = tag.Value
                }).ToList();
            }
        }

        public async Task SendTagValues(WebSocket ws)
        {
            Console.WriteLine("Connected");

            TagTrendingWorker tagTrendingWorker = new(ws, this);
            await tagTrendingWorker.Start();

            Console.WriteLine("Disconnected");
        }
    }
}
