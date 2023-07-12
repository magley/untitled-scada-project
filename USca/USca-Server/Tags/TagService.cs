using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text.Json;
using USca_Server.Shared;
using USca_Server.TagLogs;
using USca_Server.Util;
using USca_Server.Util.Socket;

namespace USca_Server.Tags
{
    public class TagService : ITagService, INotifySocket
    {
        private ITagLogService _tagLogService;
        public event EventHandler<SocketMessageDTO>? RaiseSocketEvent;

        private void OnRaiseSocketEvent(SocketMessageDTO e)
        {
            RaiseSocketEvent?.Invoke(this, e);
        }

        public TagService(ITagLogService tagLogService)
        {
            _tagLogService = tagLogService;
        }

        public void Add(TagAddDTO dto)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            Tag t = new(dto);

            using (var db = new ServerDbContext())
            {
                db.Tags.Add(t);
                db.SaveChanges();
            }

            // Output tags get logged immediately since their starting value is relevant.
            if (dto.Mode == TagMode.Output)
            {
                _tagLogService.AddFrom(t);
            }
        }

        public Tag? Get(int id)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.Tags.Find(id);
            }
        }

        public void Delete(int id)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                var tag = db.Tags.Find(id);

                if (tag != null)
                {
                    db.Tags.Remove(tag);
                    db.SaveChanges();
                    SocketMessageDTO message = new()
                    {
                        Type = SocketMessageType.DELETE_TAG,
                        Message = JsonSerializer.Serialize(id),
                    };
                    OnRaiseSocketEvent(message);
                }
            }
        }

        public List<Tag> GetAll()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.Tags.Include(t => t.Alarms).ToList();
            }
        }

        public List<Tag> GetAnalog()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.Tags.Where(t => t.Type == TagType.Analog).Include(t => t.Alarms).ToList();
            }
        }

        public void Update(TagDTO dto)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
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

                    // Output tag's value changed => log it.
                    if (tag.Mode == TagMode.Output)
                    {
                        _tagLogService.AddFrom(tag);
                    }

                    db.SaveChanges();
                }
            }
        }

        public List<OutputTagValueDTO> GetOutputTagValues()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
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
                SocketMessageType.DELETE_TAG,
            };
            SocketWorker listener = new(ws, supportedMessageTypes, new() { this, TagWorker.Instance });
            await listener.Start();
        }
    }
}
