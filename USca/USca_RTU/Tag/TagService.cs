using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using USca_Server.Tags;

namespace USca_RTU.Tag
{
    public class TagService
    {
        private static readonly string URL = "http://localhost:5274/api";

        public static async Task<List<OutputTagValueDTO>?> GetOutputTagValues()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("tag/output", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<List<OutputTagValueDTO>>(response.Content);
            }
            else
            {
                return null;
            }
        }
    }
}
