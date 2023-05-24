using RestSharp;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace DbManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			string username = tbUsername.Text;
			string password = tbPassword.Password;
			DoLogin(username, password);
		}

		private void DoLogin(string username, string password)
		{
			var dto = new
			{
				username,
				password
			};

			var URL = "http://localhost:5274/api";
			using var cli = new RestClient(new RestClientOptions(URL));

			var req = new RestRequest("User", Method.Put);
			req.AddBody(dto);
			Console.WriteLine(dto);

			RestResponse response = cli.Execute(req);
			Console.WriteLine(response.Content);

			if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				MessageBox.Show("Username or Password invalid", "Could not log in.", MessageBoxButton.OK, MessageBoxImage.Error);
				tbPassword.Clear();
				tbPassword.Focus();
			} 
			else if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				LoginResultDTO result = JsonSerializer.Deserialize<LoginResultDTO>(response.Content!)!;
				MessageBox.Show($"Logged in as {result.username}", "Alert.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
	}

	class LoginResultDTO
	{
		public string username { get; set; }
	}
}
