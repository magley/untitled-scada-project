using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;

namespace USca_Server.Measures
{
	public class MeasureService : IMeasureService
	{
		public void PutBatch(List<MeasureFromRtuDTO> batch)
		{
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
	}
}
