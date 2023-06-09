using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
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

		[HttpGet("ws")]
		public async Task WebSocketGet()
		{
			if (HttpContext.WebSockets.IsWebSocketRequest)
			{
				using (var ws = await HttpContext.WebSockets.AcceptWebSocketAsync())
				{
                    Console.WriteLine($"Connect websocket with client");
                    await Echo(ws);
				}
			}
			else
			{
				HttpContext.Response.StatusCode = 400;
			}
		}

		private async Task Echo(WebSocket ws)
		{
			var buffer = new byte[1024 * 4];
			var receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
			{
				var clientMsg = Encoding.UTF8.GetString(buffer);
                var serverMsg = Encoding.UTF8.GetBytes($"I am the server. {DateTime.Now}");

				await ws.SendAsync(new(serverMsg, 0, serverMsg.Length), receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);

                receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await ws.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
        }
	}
}
