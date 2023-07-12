using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimThermometer : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        public double Temperature { get; set; }

        public SimThermometer(int address, double temperature)
        {
            Address = address;
            Temperature = temperature;
        }

        public double Value { get { return Temperature; } set { Temperature = value; } }
    }

}
