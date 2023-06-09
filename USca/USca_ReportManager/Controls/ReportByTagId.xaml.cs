﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    public partial class ReportByTagId : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<TagLogDTO> TagLogs { get; set; } = new();
        private TagLogService _tagLogService = new();
        public string TagName { get; set; } = "";

        public ReportByTagId()
        {
            InitializeComponent();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            BtnSearch.IsEnabled = false;
            string searchQuery = TxtTagID.Text;
            if (!int.TryParse(searchQuery,  out int id))
            {
                MessageBox.Show("Please enter an integer!", "Failure", MessageBoxButton.OK);
                return;
            }

            try
            {
                var res = await _tagLogService.GetByTag(id);
                TagLogs.Clear();
                foreach (var o in res.Logs.OrderByDescending(l => l.Value))
                {
                    TagLogs.Add(o);
                }
                TagName = res.TagName;
            }
            catch (NotFoundException)
            {
                TagLogs.Clear();
                TagName = "";
                MessageBox.Show("Tag not found!", "Failure", MessageBoxButton.OK);
            }
            BtnSearch.IsEnabled = true;
        }
    }
}
