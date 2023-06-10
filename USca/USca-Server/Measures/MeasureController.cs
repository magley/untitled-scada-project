using Microsoft.AspNetCore.Mvc;
using USca_Server.Util;

namespace USca_Server.Measures
{
	[Route("api/measure")]
	[ApiController]
	public class MeasureController : ControllerBase
	{
		private readonly IMeasureService _measureService;

		public MeasureController(IMeasureService measureService)
		{
			_measureService = measureService;
		}

		[HttpPost]
		public ActionResult<string> PutDataBatch(SignedDTO<List<MeasureFromRtuDTO>> data)
		{
			bool isValid = CryptoUtil.VerifySignedMessage(data.Payload, data.Signature);
			if (!isValid)
			{
				return StatusCode(400, "Invalid signature");
			}

			_measureService.PutBatch(data.Payload);

			return StatusCode(204);
		}

		[HttpGet("address")]
		public ActionResult<List<int>> GetAvailableAddresses()
		{
			return StatusCode(200, _measureService.GetAddresses());
		}
	}
}
