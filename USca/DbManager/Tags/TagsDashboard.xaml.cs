using System;
using System.Text.Json;
using USca_DbManager.Util;

namespace USca_DbManager.Tags
{
	public partial class TagsDashboard : MyPage
    {
		public TagsDashboard()
		{
			InitializeComponent();
		}

        private async void BtnAddTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new AddTag();
            dialog.Owner = Owner;
            if (dialog.ShowDialog() == true)
            {
                await TagService.AddTag(dialog.TagData);
            }
        }
    }
}
