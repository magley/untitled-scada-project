using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;

namespace USca_Server.Tags
{
	[Route("api/tag")]
	[ApiController]
	public class TagController : ControllerBase
	{
		[HttpGet]
		public ActionResult<List<Tag>> GetAllTags()
		{
			using (var db = new ServerDbContext())
			{
				db.Tags.Load();

				Console.WriteLine(db.Tags.Count());

				return StatusCode(200, db.Tags.ToList());
			}
		}
	}
}
