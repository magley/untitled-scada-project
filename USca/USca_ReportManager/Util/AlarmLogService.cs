using RestSharp;
using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace USca_ReportManager.Util
{
    public class AlarmLogService
    {
        // TODO: extract from this class
        private readonly string URL = "http://localhost:5274/api";

        public async Task<AlarmLogsDTO> GetByDateRange(DateTime startTime, DateTime endTime)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"alarm/logs/range", Method.Get);
            req.AddBody(new
            {
                startTime,
                endTime
            });
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var li = JsonSerializer.Deserialize<AlarmLogsDTO>(response.Content);
                return li;
            }
            else
            {
                throw new NotFoundException();
            }
        }
    }
}
