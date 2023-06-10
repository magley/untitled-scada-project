using System;
using USca_DbManager.Util;

namespace USca_DbManager.Tags
{
	public partial class TagsDashboard : MyPage
    {
		public TagsDashboard()
		{
			InitializeComponent();
		}

        private void BtnAddTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new AddTag();
            dialog.Owner = this.Owner;
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine(dialog.TagName);
            }
        }
    }
}
