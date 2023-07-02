using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using USca_Server.Shared;
using USca_Server.Util;

namespace USca_Server.Tags
{
    /// <summary>
    /// TagTrendingWorker encapsulates all logic for sending current tag values to the Trending app.
    /// </summary>
    public class TagTrendingWorker
    {
        public WebSocket Ws { get; set; }
        private Dictionary<int, TagThreadWrapper> _threads = new();
        private LoopThread? _tagSyncThread;
        private readonly ITagService _tagService;

        public TagTrendingWorker(WebSocket ws, ITagService tagService)
        {
            Ws = ws;
            _tagService = tagService;
        }

        public async Task Start()
        {
            StartTagSync();
            await WebSocketLoop();
            EndTagSync();
        }

        /// <summary>
        /// This method runs the main WebSocket code for this Worker.
        /// <br/>
        /// It waits for a message from the other side, and if it receives a <c>WebSocketMessageType.Close</c>, it closes the WebSocket.
        /// <br/>
        /// This method returning implies that the WebSocket connection is closed.
        /// </summary>
        private async Task WebSocketLoop()
        {
            while (Ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];

                try
                {
                    var result = await Ws.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await Ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
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
        }

        private void StartTagSync()
        {
            _tagSyncThread = new(new ThreadStart(TagSync));
            _tagSyncThread.Start();
        }

        private void EndTagSync()
        {
            _tagSyncThread.Abort();
        }

        /// <summary>
        /// This method syncs the tags from the database onto _threads.
        /// This method should run in the tag sync thread.
        /// </summary>
        private void TagSync()
        {
            Thread.Sleep(1000);
            Console.WriteLine("Sync tags");

            var currentTags = _tagService.GetAll()
                .Where(t => t.Mode == TagMode.Input && t.IsScanning)
                .ToList();
            var staleTagIds = _threads.Keys.ToList().Except(currentTags.Select(t => t.Id));

            // Remove threads for tags that don't exist anymore.

            foreach (var tagId in staleTagIds)
            {
                _threads[tagId].LoopThread.Abort();
                _threads.Remove(tagId);
                Console.WriteLine($"Removed thread for tag {tagId}.");
            }

            // Create threads for new tags.

            foreach (var tag in currentTags)
            {
                if (!_threads.ContainsKey(tag.Id))
                {
                    _threads[tag.Id] = new(tag, Ws);
                    _threads[tag.Id].LoopThread.Start();
                    Console.WriteLine($"Added thread for tag {tag}.");
                } else
                {
                    _threads[tag.Id].Tag = tag;
                }
            }
        }

        /// <summary>
        /// This method tries to send current data from the tag <c>t</c> across the WebSocket.
        /// This method should run in a <c>LoopThread</c> assigned to the given tag.
        /// </summary>
        private void SendTagData(Tag t)
        {
            Thread.Sleep(t.ScanTime);

            using (var db = new ServerDbContext())
            {
                var measure = db.Measures.Find(t.Address);
                if (measure != null)
                {
                    var message = new
                    {
                        t.Id,
                        t.Name,
                        t.Type,
                        t.Min,
                        t.Max,
                        t.Unit,
                        measure.Value,
                        measure.Timestamp,
                    };
                    var messageJson = JsonSerializer.Serialize(message);

                    try
                    {
                        Ws.SendAsync(
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
        public class TagThreadWrapper
        {
            public Tag Tag { get; set; }
            public WebSocket Ws { get; set; }
            public LoopThread LoopThread { get; set; }

            public TagThreadWrapper(Tag tag, WebSocket ws)
            {
                Tag = tag;
                Ws = ws;
                LoopThread = new(() => SendTagData());
            }

            ~TagThreadWrapper()
            {
                LoopThread.Abort();
            }

            /// <summary>
            /// This method tries to send current data from the tag <c>t</c> across the WebSocket.
            /// This method should run in a <c>LoopThread</c> assigned to the given tag.
            /// </summary>
            private void SendTagData()
            {
                Thread.Sleep(Tag.ScanTime);

                using (var db = new ServerDbContext())
                {
                    var measure = db.Measures.Find(Tag.Address);
                    if (measure != null)
                    {
                        var message = new
                        {
                            Tag.Id,
                            Tag.Name,
                            Tag.Type,
                            Tag.Min,
                            Tag.Max,
                            Tag.Unit,
                            measure.Value,
                            measure.Timestamp,
                        };
                        var messageJson = JsonSerializer.Serialize(message);

                        try
                        {
                            Ws.SendAsync(
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
