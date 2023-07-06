using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace USca_DbManager.Alarms
{
    internal class AlarmService
    {
        private static readonly string URL = "http://localhost:5274/api";

        public static async Task<List<AlarmDTO>> GetAllAlarms()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("alarm", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            if (response.Content == null)
            {
                return new();
            }
            var alarms = JsonSerializer.Deserialize<List<AlarmDTO>>(response.Content);
            return alarms ?? new();
        }

        public static async Task DeleteAlarm(int alarmId)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"alarm/{alarmId}", Method.Delete);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static async Task AddAlarm(AlarmDTO alarmDTO)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"alarm", Method.Post);
            req.AddBody(alarmDTO);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public static async Task UpdateAlarm(AlarmDTO alarmDTO)
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest($"alarm", Method.Put);
            req.AddBody(alarmDTO);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
