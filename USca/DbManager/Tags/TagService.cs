using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace USca_DbManager.Tags
{
	internal class TagService
	{
		private static readonly string URL = "http://localhost:5274/api";

		public static async Task<List<TagAddDTO>> GetAllTags()
		{
			using var cli = new RestClient(new RestClientOptions(URL));
			var req = new RestRequest("tag", Method.Get);
			RestResponse response = await cli.ExecuteAsync(req);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
				var li = JsonSerializer.Deserialize<List<TagAddDTO>>(response.Content);
				return li;
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}
		}

		public static async Task<RestResponse> AddTag(TagAddDTO tag)
		{
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("tag", Method.Post);
			req.AddBody(tag);
            RestResponse response = await cli.ExecuteAsync(req);

			return response;
        }
	}
}
