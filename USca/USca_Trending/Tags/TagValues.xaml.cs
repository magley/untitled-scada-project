using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using USca_WebSocketUtil;

namespace USca_Trending.Tags
{
    public partial class TagValues : Window
    {
        public ObservableCollection<InputTagReadingDTO> TagReadings { get; set; } = new();
        private const string serverSocketEndpoint = "ws://localhost:5274/api/tag/ws";

        public TagValues()
        {
            InitializeComponent();
            new ClientWebSocketUtil(serverSocketEndpoint, HandleSocketMessage).WebSocketLoop();
        }

        private void HandleSocketMessage(SocketMessageType type, string? message)
        {
            switch (type)
            {
                case SocketMessageType.UPDATE_TAG_READING:
                    if (message == null)
                    {
                        throw new InvalidSocketMessageException(type, message);
                    }
                    LoadTagReading(JsonSerializer.Deserialize<InputTagReadingDTO>(message));
                    break;
                case SocketMessageType.DELETE_TAG:
                    if (message == null)
                    {
                        throw new InvalidSocketMessageException(type, message);
                    }
                    DeleteTagReading(JsonSerializer.Deserialize<int>(message));
                    break;
                default:
                    throw new UnsupportedSocketMessageTypeException(type);
            }
        }

        private void LoadTagReading(InputTagReadingDTO? dto)
        {
            if (dto == null)
            {
                return;
            }
            var item = TagReadings.FirstOrDefault(t => t.Id == dto.Id);
            int idx =  (item != null) ? TagReadings.IndexOf(item) : -1;
            if (idx == -1)
            {
                TagReadings.Add(dto);
            } else
            {
                TagReadings[idx] = dto;
            }
        }

        private void DeleteTagReading(int id)
        {
            var item = TagReadings.FirstOrDefault(t => t.Id == id);
            int idx = (item != null) ? TagReadings.IndexOf(item) : -1;
            if (idx == -1)
            {
                return;
            }
            TagReadings.RemoveAt(idx);
        }
    }
}
