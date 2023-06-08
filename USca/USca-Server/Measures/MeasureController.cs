using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.Json;
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
			//Console.WriteLine(JsonSerializer.Serialize(data));

			Console.WriteLine(VerifySignedMessage(data.Hash, data.Signature));

			return StatusCode(204);
		}


		private static bool VerifySignedMessage(byte[] hash, byte[] signature)
		{
			CspParameters csp = new CspParameters();
			csp.KeyContainerName = "USca_RTU_Key";
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp))
			{
				var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
				deformatter.SetHashAlgorithm("SHA256");
				return deformatter.VerifySignature(hash, signature);
			}
		}
	}

}
