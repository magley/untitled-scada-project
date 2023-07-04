using Microsoft.AspNetCore.Mvc;

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
			return StatusCode(200, res);
		}

		[HttpPost]
        public ActionResult<object> AddTag(TagAddDTO dto)
        {
			_tagService.Add(dto);
			return StatusCode(204);
        }

        [HttpPut]
        public ActionResult<object> UpdateTag(TagDTO dto)
        {
            _tagService.Update(dto);
            return StatusCode(204);
        }

		[HttpDelete("{id}")]
        public ActionResult<object> DeleteTag(int id)
        {
			Console.WriteLine(id);
            _tagService.Delete(id);
            return StatusCode(204);
        }

        [HttpPut("output")]
        public ActionResult<object> SetOutputTagValue(OutputTagValueDTO dto)
        {
            _tagService.Update(dto);
            return StatusCode(204);
        }

        [HttpGet("ws")]
        public async Task OpenTagWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using (var ws = await HttpContext.WebSockets.AcceptWebSocketAsync())
                {
                    await _tagService.SendTagValues(ws);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
