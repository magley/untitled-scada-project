using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using USca_DbManager.Util;

namespace USca_DbManager.Tags
{
	public partial class TagsDashboard : MyPage
    {
        public ObservableCollection<TagDTO> Tags { get; set; } = new();
        public TagDTO? SelectedTag { get; set; } = null;

        public TagsDashboard()
		{
			InitializeComponent();
            LoadAllTags();
        }

        private async void BtnAddTag_Click(object sender, RoutedEventArgs e)
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
            List<TagDTO> tags = await TagService.GetAllTags();
            Tags.Clear();
            foreach (var t in tags)
            {
                Tags.Add(t);
            }
        }

        private async void TbTags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedTag == null)
            {
                return;
            }

            var dialog = new AddTag(SelectedTag);
            dialog.Owner = Owner;
            if (dialog.ShowDialog() == true)
            {
                await TagService.UpdateTag(dialog.TagData);
                LoadAllTags();
            }

        }

        private async void BtnDelTag_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTag == null)
            {
                return;
            }

            var dialog = MessageBox.Show($"Are you sure you want to delete {SelectedTag.Name}?", "Delete", MessageBoxButton.YesNo);

            if (dialog == MessageBoxResult.Yes)
            {
                await TagService.DeleteTag(SelectedTag);
                LoadAllTags();
            }
        }
    }
}
