using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace USca_DbManager.Tags
{
    public partial class AddTag : Window, INotifyPropertyChanged
    {
        public List<string> TagUnits { get; set; } = new();
        public TagAddDTO TagData { get; set; } = new();

        public AddTag()
        {
            InitializeComponent();
            TxtTagName.Focus();

            TagUnits.Add("mm");
            TagUnits.Add("cm");
            TagUnits.Add("°C");
            TagUnits.Add("litre");
            TagUnits.Add("kg");
            TagUnits.Add("g");
            TagData.Unit = TagUnits[0];
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
