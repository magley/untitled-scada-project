using Microsoft.AspNetCore.Mvc;
using USca_Server.TagLogs.DTO;
using USca_Server.Tags;

namespace USca_Server.TagLogs
{
    [Route("api/tag/logs")]
    [ApiController]
    public class TagLogController : ControllerBase
    {
        private ITagLogService _tagLogService;
        private ITagService _tagService;

        public TagLogController(ITagLogService tagLogService, ITagService tagService)
        {
            _tagLogService = tagLogService;
            _tagService = tagService;
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
        public ActionResult<TagLogByTagIdDTO> GetAllByTag(int tagId)
        {
            var tag = _tagService.Get(tagId);
            if (tag == null)
            {
                return StatusCode(404);
            }

            var logs = _tagLogService.GetAllByTag(tagId);

            return StatusCode(200, new TagLogByTagIdDTO(tag, logs));
        }

        [HttpGet("analog/input")]
        public ActionResult<TagLogByTagIdDTO> GetAnalogInputs()
        {
            var logs = _tagLogService.GetLatestAnalogInputs();
            return StatusCode(200, new TagLogsDTO(logs));
        }

        [HttpGet("digital/input")]
        public ActionResult<TagLogByTagIdDTO> GetDigitalInputs()
        {
            var logs = _tagLogService.GetLatestDigitalInputs();
            return StatusCode(200, new TagLogsDTO(logs));
        }

        [HttpGet("all/range")]
        public ActionResult<TagLogByTagIdDTO> GetAllByRange(DateRangeDTO dateRange)
        {
            if (dateRange.StartTime >= dateRange.EndTime)
            {
                return StatusCode(400);
            }
            var logs = _tagLogService.GetAllByRange(dateRange.StartTime, dateRange.EndTime);
            return StatusCode(200, new TagLogsDTO(logs));
        }
    }
}
