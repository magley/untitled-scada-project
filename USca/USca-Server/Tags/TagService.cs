﻿using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using USca_Server.Shared;
using USca_Server.TagLogs;
using USca_Server.Util;

namespace USca_Server.Tags
{
    public class TagService : ITagService
    {
        private ITagLogService _tagLogService;

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
                SocketMessageType.DELETE_TAG_READING,
            };
            SocketWorker listener = new(ws, supportedMessageTypes);
            await listener.Start();
        }
    }
}
