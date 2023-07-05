using Microsoft.AspNetCore.Mvc;

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

        [HttpDelete("{alarmId}")]
        public ActionResult DeleteAlarm(int alarmId)
        {
            _alarmService.Delete(alarmId);
            return StatusCode(204);
        }
    }
}