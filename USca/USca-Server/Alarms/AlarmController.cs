using Microsoft.AspNetCore.Mvc;
using USca_Server.TagLogs.DTO;

namespace USca_Server.Alarms
{
    [Route("api/alarm")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private IAlarmService _alarmService;

        public AlarmController(IAlarmService alarmService)
        {
            _alarmService = alarmService;
        }

        [HttpGet]
        public ActionResult<List<Alarm>> GetAllAlarms()
        {
            return StatusCode(200, _alarmService.GetAll());
        }

        [HttpGet("active")]
        public ActionResult<List<ActiveAlarmDTO>> GetActiveAlarms()
        {
            return StatusCode(200, _alarmService.GetActive());
        }

        [HttpGet("logs/range")]
        public ActionResult<List<ActiveAlarmDTO>> GetLogsByRange(DateRangeDTO dateRange)
        {
            if (dateRange.StartTime >= dateRange.EndTime)
            {
                return StatusCode(400);
            }
            var logs = _alarmService.GetLogsByRange(dateRange.StartTime, dateRange.EndTime);
            return StatusCode(200, new AlarmLogsDTO(logs));
        }

        [HttpDelete("{alarmId}")]
        public ActionResult DeleteAlarm(int alarmId)
        {
            _alarmService.Delete(alarmId);
            return StatusCode(204);
        }

        [HttpPost]
        public ActionResult AddAlarm(AlarmAddDTO alarmAddDTO)
        {
            _alarmService.Add(alarmAddDTO);
            return StatusCode(204);
        }

        [HttpPut]
        public ActionResult UpdateAlarm(AlarmUpdateDTO alarmUpdateDTO)
        {
            _alarmService.Update(alarmUpdateDTO);
            return StatusCode(204);
        }

        [HttpGet("ws")]
        public async Task OpenAlarmWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _alarmService.StartAlarmValuesListener(ws);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}