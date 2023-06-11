using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;

namespace USca_Server.Tags
{
	[Route("api/tag")]
	[ApiController]
	public class TagController : ControllerBase
	{
		private ITagService _tagService;

		public TagController(ITagService tagService)
		{
			_tagService = tagService;
		}

		[HttpGet]
		public ActionResult<List<Tag>> GetAllTags()
		{
			var res = _tagService.GetAll();
			foreach (var t in res)
			{
				Console.WriteLine(t.Name);
			}
			return StatusCode(200, res);
		}

		[HttpPost]
        public ActionResult<object> AddTag(TagAddDTO dto)
        {
			_tagService.Add(dto);
            return StatusCode(204);
        }
    }
}
