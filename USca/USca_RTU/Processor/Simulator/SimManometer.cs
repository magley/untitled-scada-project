using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimManometer : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        public double Pressure { get; set; }

        public SimManometer(int address, double pressure)
        {
            Address = address;
            Pressure = pressure;
        }

        public double Value { get { return Pressure; } set { Pressure = value; } }
    }

}
