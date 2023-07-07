using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimTank : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        private double _value;
        public double Value { get { return _value; } set { _value = Math.Max(0, value); } }

        public SimTank(int address, double value)
        {
            Address = address;
            Value = value;
        }
    }

}
