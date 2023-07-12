using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_Trending.Tags
{
    public partial class InputTagReadingDTO: INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Address { get; set; }
        public double ScanTime { get; set; }
        public TagType Type { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string? Unit { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
