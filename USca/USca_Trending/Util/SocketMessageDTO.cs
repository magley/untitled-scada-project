using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_Trending.Util
{
    public enum SocketMessageType
    {
        DELETE_TAG_READING,
        UPDATE_TAG_READING,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }
}
