using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace USca_RTU.Processor
{
	internal class CommService
	{
		private static readonly string URL = "http://localhost:5274/api";

		public static async Task<object> SendSignalsBatch(List<Signal> signals)
		{
			var dto = signals.Select(s => new SignalDTO
			{
				Address = s.Address,
				Name = s.Name,
				Timestamp = s.Timestamp,
				Value = s.Value
			}).ToList();

			using var cli = new RestClient(new RestClientOptions(URL));

			var request = new RestRequest("measure", Method.Post).AddBody(dto);
			var response = await cli.ExecuteAsync(request);

			if (response.StatusCode == HttpStatusCode.NoContent)
			{
				return response;
			}
			else
			{
				Console.WriteLine(response.ErrorMessage);
				return new();
				//throw new Exception(response.ErrorMessage);
			}
		}
	}
}
