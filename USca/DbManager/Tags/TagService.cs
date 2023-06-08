using RestSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using USca_DbManager.User;

namespace USca_DbManager.Tags
{
	internal class TagService
	{
		private static readonly string URL = "http://localhost:5274/api";

		public static async Task<string> GetAllTags()
		{
			using var cli = new RestClient(new RestClientOptions(URL));

			var req = new RestRequest("tag", Method.Get);

			RestResponse response = await cli.ExecuteAsync(req);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var res = response.Content!;
				return res;
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}
		}
	}
}
