using USca_Server.Shared;
using USca_Server.Tags;
using USca_Server.Util;

namespace USca_Server.TagLogs
{
    public class TagLogService : ITagLogService
    {
        public void AddBatch(List<Tuple<Tag, DateTime>> batch)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                foreach (var o in batch)
                {
                    TagLog tagLog = new(o.Item1, o.Item2);
                    db.TagLogs.Add(tagLog);
                }

                db.SaveChanges();
            }
        }

        public void AddFrom(Tag tag, DateTime measureTimestamp)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            TagLog tagLog = new(tag, measureTimestamp);

            using (var db = new ServerDbContext())
            {
                db.TagLogs.Add(tagLog);
                db.SaveChanges();
            }
        }

        public void AddFrom(Tag tag)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            TagLog tagLog = new(tag, DateTime.Now);

            using (var db = new ServerDbContext())
            {
                db.TagLogs.Add(tagLog);
                db.SaveChanges();
            }
        }
        

        public TagLog? Get(int id)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.Find(id);
            }
        }

        public List<TagLog> GetAll()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.ToList();
            }
        }

        public List<TagLog> GetAllByTag(int tagId)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.TagLogs.Where(t => t.TagId == tagId).ToList();
            }
        }

        public List<TagLog> GetLatestAnalogInputs()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.TagLogs
                    .Where(t => t.Type == TagType.Analog && t.Mode == TagMode.Input)
                    .GroupBy(t => t.TagId)
                    .Select(g => g.OrderByDescending(t => t.Timestamp).First()).ToList();
            }
        }

        public List<TagLog> GetLatestDigitalInputs()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
            {
                return db.TagLogs
                    .Where(t => t.Type == TagType.Digital && t.Mode == TagMode.Input)
                    .GroupBy(t => t.TagId)
                    .Select(g => g.OrderByDescending(t => t.Timestamp).First()).ToList();
            }
        }
    }
}
