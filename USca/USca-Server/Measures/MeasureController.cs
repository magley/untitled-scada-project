using Microsoft.AspNetCore.Mvc;
using USca_Server.Util;

namespace USca_Server.Measures
{
	[Route("api/measure")]
	[ApiController]
	public class MeasureController : ControllerBase
	{
		[HttpPost]
		public ActionResult<string> PutDataBatch(SignedDTO<List<MeasureFromRtuDTO>> data)
		{
			Console.WriteLine(CryptoUtil.VerifySignedMessage(data.Hash, data.Signature));
			return StatusCode(204);
		}
	}
}
