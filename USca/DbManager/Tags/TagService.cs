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

		public static async Task<List<TagDTO>> GetAllTags()
		{
			using var cli = new RestClient(new RestClientOptions(URL));
			var req = new RestRequest("tag", Method.Get);
			RestResponse response = await cli.ExecuteAsync(req);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
				var li = JsonSerializer.Deserialize<List<TagDTO>>(response.Content);
				return li;
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}
		}

        public static async Task<List<TagDTO>> GetAnalogTags()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("tag/analog", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<List<TagDTO>>(response.Content);
                return li;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static async Task<RestResponse> AddTag(TagDTO tag)
		{
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("tag", Method.Post);
			req.AddBody(tag);
            RestResponse response = await cli.ExecuteAsync(req);

			return response;
        }

        public static async Task<RestResponse> UpdateTag(TagDTO tag)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("tag", Method.Put);
            req.AddBody(tag);
            RestResponse response = await cli.ExecuteAsync(req);

            return response;
        }

        public static async Task<RestResponse> DeleteTag(TagDTO tag)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"tag/{tag.Id}", Method.Delete);
            RestResponse response = await cli.ExecuteAsync(req);

            return response;
        }
    }
}
