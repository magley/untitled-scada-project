using System.ComponentModel;
using System.Windows;

namespace USca_DbManager.Tags
{
    public partial class AddTag : Window, INotifyPropertyChanged
    {
        public string TagName { get; set; } = "";

        public AddTag()
        {
            InitializeComponent();
            TxtTagName.Focus();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
