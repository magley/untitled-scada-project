using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimCompressor : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        public double Rate { get; set; }

        public SimCompressor(int address, double rate, bool active)
        {
            Address = address;
            Rate = rate;
        }

        public double Value { get { return Rate; } set { Rate = value; } }
    }

}
