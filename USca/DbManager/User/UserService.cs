using RestSharp;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace USca_DbManager.User
{
    class UserService
    {
		private static readonly string URL = "http://localhost:5274/api";

		public static async Task<LoginResultDTO> Login(string username, string password)
        {
			var dto = new
			{
				username,
				password
			};
			
			using var cli = new RestClient(new RestClientOptions(URL));

			var req = new RestRequest("User", Method.Put);
			req.AddBody(dto);

			RestResponse response = await cli.ExecuteAsync(req);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var res = JsonSerializer.Deserialize<LoginResultDTO>(response.Content!)!;
				return res;
			}
			else
			{
				throw new BadCredentialsException();
			}
		}
    }
}
