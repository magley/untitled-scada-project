using System.Windows;
using USca_DbManager.Alarms;
using USca_DbManager.User;
using USca_DbManager.Util;

namespace USca_DbManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Navigate(new AlarmsDashboard());
		}

		public void Navigate(MyPage page)
		{
			FrameContent.Navigate(page);
		}
	}
}
