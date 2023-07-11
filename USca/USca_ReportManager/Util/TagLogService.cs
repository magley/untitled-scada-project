using RestSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace USca_ReportManager.Util
{
    public class NotFoundException : Exception
    {

    }

    public class TagLogService
    {
        private readonly string URL = "http://localhost:5274/api";

        public async Task<TagLogByTagIdDTO> GetByTag(int tagID)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"tag/logs/all/{tagID}", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<TagLogByTagIdDTO>(response.Content);
                return li;
            }
            else
            {
                throw new NotFoundException();
            }
        }

        public async Task<TagLogsDTO> GetLatestAnalogInputs()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"tag/logs/analog/input", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<TagLogsDTO>(response.Content);
                return li;
            }
            else
            {
                throw new NotFoundException();
            }
        }

        public async Task<TagLogsDTO> GetLatestDigitalInputs()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"tag/logs/digital/input", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<TagLogsDTO>(response.Content);
                return li;
            }
            else
            {
                throw new NotFoundException();
            }
        }
    }
}
