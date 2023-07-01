using System.Windows;
using System.Windows.Controls;

namespace USca_DbManager.Util
{
	public partial class MyPage : Page
	{
		public MyPage(): base()
		{
			
		}
		public MainWindow Owner { get { return (MainWindow)Window.GetWindow(this); } }
	}
}
