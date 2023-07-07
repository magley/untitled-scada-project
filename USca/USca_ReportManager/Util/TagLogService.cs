using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace USca_ReportManager.Util
{
    public class TagLogService
    {
        private readonly string URL = "http://localhost:5274/api";

        public async Task<List<TagLogDTO>> GetByTag(int tagID)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"tag/logs/all/{tagID}", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<List<TagLogDTO>>(response.Content);
                return li;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
