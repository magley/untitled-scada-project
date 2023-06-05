using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace USca_Server.Measures
{
	[Route("api/measure")]
	[ApiController]
	public class MeasureController : ControllerBase
	{
		[HttpPost]
		public ActionResult<string> PutDataBatch(List<MeasureFromRtuDTO> data)
		{
			Console.WriteLine(JsonSerializer.Serialize(data));
			return StatusCode(204);
		}
	}
}
