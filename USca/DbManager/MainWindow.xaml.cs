using USca_DbManager.User;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace USca_DbManager
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

		private async void DoLogin(string username, string password)
		{
			btnLogin.IsEnabled = false;
			Mouse.OverrideCursor = Cursors.Wait;

			try
			{
				var res = await UserService.Login(username, password);
				MessageBox.Show($"Logged in as {res.username}", "Alert.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			catch (BadCredentialsException)
			{
				MessageBox.Show("Username or Password invalid", "Could not log in.", MessageBoxButton.OK, MessageBoxImage.Error);
				tbPassword.Clear();
				tbPassword.Focus();
			}
			finally
			{
				Mouse.OverrideCursor = null;
				btnLogin.IsEnabled = true;
			}
		}
	}
}
