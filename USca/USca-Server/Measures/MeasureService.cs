using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;
using USca_Server.Util;

namespace USca_Server.Measures
{
	public class MeasureService : IMeasureService
	{
        public void PutBatch(List<MeasureFromRtuDTO> batch)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            var data = batch.Select(b => new Measure()
			{
				Id = b.Address,
				Name = b.Name,
				Timestamp = b.Timestamp,
				Value = b.Value,
			});

			using (var db = new ServerDbContext())
			{
				foreach (var o in data)
				{
					if (db.Measures.AsNoTracking().Where(oo => oo.Id == o.Id).FirstOrDefault() == null)
					{
						db.Measures.Add(o);
					}
					else
					{
						db.Measures.Update(o); 
					}
				}

				db.SaveChanges();
			}
		}

        public List<Measure> Get()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            using (var db = new ServerDbContext())
			{
				return db.Measures.ToList();
			}
        }

        public List<int> GetAddresses()
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
            return (from m in Get() select m.Id).ToList();
        }
    }
}
