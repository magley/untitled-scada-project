using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace USca_AlarmDisplay.Alarm
{
    internal class AlarmService
    {
        private static readonly string URL = "http://localhost:5274/api";
        public static async Task<List<ActiveAlarm>> GetActiveAlarms()
        {
            using var cli = new RestClient(new RestClientOptions(URL));
            var req = new RestRequest("alarm/active", Method.Get);
            RestResponse response = await cli.ExecuteAsync(req);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(response.StatusCode.ToString());
                return new();
            }
            if (response.Content == null)
            {
                return new();
            }
            var alarms = JsonSerializer.Deserialize<List<ActiveAlarm>>(response.Content);
            return alarms ?? new();
        }
    }
}
