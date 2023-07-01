using System.Windows;
using USca_DbManager.User;
using USca_DbManager.Util;

namespace USca_DbManager
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Navigate(new Login());
		}

		public void Navigate(MyPage page)
		{
			FrameContent.Navigate(page);
		}
	}
}
