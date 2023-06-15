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
            Console.WriteLine("Established connection to web socket.");
            var tagThreads = new Dictionary<int, RunningThread>();
            bool isRunning = true;

            Thread syncThread = new(new ThreadStart(() => SyncTagThreadsWithTagsInSystem(tagThreads, ws, ref isRunning)));
            syncThread.IsBackground = true;
            syncThread.Start();

            while (ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];

                try
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
                catch (WebSocketException)
                {
                    break;
                }
            }

            isRunning = false;
            Console.WriteLine("Ended connection to web socket.");
        }

        private void SyncTagThreadsWithTagsInSystem(Dictionary<int, RunningThread> tagThreads, WebSocket ws, ref bool isRunning)
        {
            const int syncTimeMs = 1000;

            while (isRunning)
            {
                Thread.Sleep(syncTimeMs);
                Console.WriteLine("Sync tag threads...");

                var currentTags = GetAll()
                    .Where(t => t.Mode == TagMode.Input)
                    .Select(t => t.Id)
                    .ToList();
                var staleTags = tagThreads.Keys.ToList().Except(currentTags);

                // Remove threads for tags that don't exist anymore.

                foreach (var tagId in staleTags)
                {
                    tagThreads[tagId].IsRunning = false;
                    tagThreads.Remove(tagId);
                    Console.WriteLine($"Removed thread for tag {tagId}.");
                }

                // Create threads for new tags.

                foreach (var tagId in currentTags)
                {
                    if (!tagThreads.ContainsKey(tagId))
                    {
                        Tag? tag = Get(tagId);
                        if (tag == null)
                        {
                            // Should never happen, but VS complains about null checks.
                            continue;
                        }

                        RunningThread rt = new();
                        Thread t = new(new ThreadStart(() => SendTagThread(rt, tag, ws)))
                        {
                            IsBackground = false
                        };
                        t.Start();
                        rt.Thread = t;
                        tagThreads[tagId] = rt;

                        Console.WriteLine($"Added thread for tag {tagId}.");
                    }
                }
            }

            Console.WriteLine("Sync tag threads STOP.");
        }

        private void SendTagThread(RunningThread self, Tag t, WebSocket ws)
        {
            while (self.IsRunning)
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

                        try
                        {
                            ws.SendAsync(
                                new(Encoding.UTF8.GetBytes(messageJson)),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            );
                        }
                        catch (WebSocketException)
                        {
                            // Client forcibly closed the socket.
                        }
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
