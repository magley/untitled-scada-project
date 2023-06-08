using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USca_DbManager.Tags;
using USca_DbManager.Util;

namespace USca_DbManager.User
{
	public partial class Login : MyPage
	{
		public Login()
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

				Owner.Navigate(new TagsDashboard());
				var res2 = await TagService.GetAllTags();
				Console.WriteLine(res2);
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
