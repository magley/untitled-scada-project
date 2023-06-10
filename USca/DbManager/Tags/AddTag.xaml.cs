using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace USca_DbManager.Tags
{
    public enum TagMode
    {
        Input,
        Output
    }

    public enum TagType
    {
        Digital,
        Analog
    }

    public partial class AddTag : Window, INotifyPropertyChanged
    {
        public List<string> TagUnits { get; set; } = new();

        public string TagName { get; set; } = "";
        public string TagDesc { get; set; } = "";
        public TagMode TagMode { get; set; }
        public TagType TagType { get; set; }
        public int TagAddress { get; set; }
        public double TagMin { get; set; } = 0;
        public double TagMax { get; set; } = 10.0;
        public string TagUnit { get; set; } = "";
        public int TagScanTime { get; set; } = 1000;

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
            TagUnit = TagUnits[0];
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
