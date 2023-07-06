using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using USca_Server.Shared;
using USca_Server.Util;

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
                return db.Tags.Include(t => t.Alarms).ToList();
            }
        }

        public List<Tag> GetAnalog()
        {
            using (var db = new ServerDbContext())
            {
                return db.Tags.Where(t => t.Type == TagType.Analog).Include(t => t.Alarms).ToList();
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

        public async Task StartTagValuesListener(WebSocket ws)
        {
            List<SocketMessageType> supportedMessageTypes = new()
            {
                SocketMessageType.UPDATE_TAG_READING,
                SocketMessageType.DELETE_TAG_READING,
            };
            SocketWorker listener = new(ws, supportedMessageTypes);
            await listener.Start();
        }
    }
}
