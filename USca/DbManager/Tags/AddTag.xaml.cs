using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;

namespace USca_DbManager.Tags
{
    public partial class AddTag : Window, INotifyPropertyChanged
    {
        public List<string> TagUnits { get; set; } = new();
        public TagDTO TagData { get; set; } = new();

        public AddTag(TagDTO original)
        {
            InitializeComponent();
            OnCtor();
            TagData = JsonSerializer.Deserialize<TagDTO>(JsonSerializer.Serialize(original));
        }

        public AddTag()
        {
            InitializeComponent();
            OnCtor();
            TagData.Unit = TagUnits[0];
        }

        private void OnCtor()
        {
            TxtTagName.Focus();

            TagUnits.Add("mm");
            TagUnits.Add("cm");
            TagUnits.Add("°C");
            TagUnits.Add("litre");
            TagUnits.Add("kg");
            TagUnits.Add("g");   
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
