using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using USca_DbManager.Util;

namespace USca_DbManager.Tags
{
	public partial class TagsDashboard : MyPage
    {
        public ObservableCollection<TagAddDTO> Tags { get; set; } = new();

		public TagsDashboard()
		{
			InitializeComponent();
            LoadAllTags();
        }

        private async void BtnAddTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new AddTag();
            dialog.Owner = Owner;
            if (dialog.ShowDialog() == true)
            {
                await TagService.AddTag(dialog.TagData);
                LoadAllTags();
            }
        }

        private async void LoadAllTags()
        {
            List<TagAddDTO> tags = await TagService.GetAllTags();
            Tags.Clear();
            foreach (var t in tags)
            {
                Tags.Add(t);
            }
        }
    }
}
