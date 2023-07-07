using Microsoft.AspNetCore.Mvc;
using USca_Server.Tags;

namespace USca_Server.TagLogs
{
    [Route("api/tag/logs")]
    [ApiController]
    public class TagLogController : ControllerBase
    {
        private ITagLogService _tagLogService;

        public TagLogController(ITagLogService tagLogService)
        {
            _tagLogService = tagLogService;
        }

        [HttpGet("{id}")]
        public ActionResult<TagLog> GetById(int id)
        {
            var res = _tagLogService.Get(id);
            return StatusCode(200, res);
        }

        [HttpGet("all")]
        public ActionResult<List<TagLog>> GetAll()
        {
            var res = _tagLogService.GetAll();
            return StatusCode(200, res);
        }

        [HttpGet("all/{tagId}")]
        public ActionResult<List<TagLog>> GetAllByTag(int tagId)
        {
            var res = _tagLogService.GetAllByTag(tagId);
            return StatusCode(200, res);
        }
    }
}
