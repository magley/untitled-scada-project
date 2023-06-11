using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

        public async Task SendTagValues(WebSocket ws)
        {
            var inputTags = GetAll().Where(t => t.Mode == TagMode.Input).ToList();
            var threads = new List<Thread>();

            foreach (var tag in inputTags)
            {
                Thread t = new(new ThreadStart(() => SendTagThread(tag, ws)))
                {
                    IsBackground = false
                };
                t.Start();
                threads.Add(t);
            }

            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
                }
            }
        }

        private void SendTagThread(Tag t, WebSocket ws)
        {
            while (true)
            {
                Thread.Sleep(t.ScanTime);

                double value = 0;
                using (var db = new ServerDbContext())
                {
                    var measure = db.Measures.Find(t.Address);
                    if (measure != null)
                    {
                        value = measure.Value;

                        var message = new
                        {
                            TagID = t.Id,
                            Address = t.Address,
                            Value = value
                        };
                        var messageJson = JsonSerializer.Serialize(message);

                        ws.SendAsync(
                            new(Encoding.UTF8.GetBytes(messageJson)),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                    else
                    {
                        // The tag points to a measure that doesn't exist.
                    }
                }
            }
        }
    }
}
