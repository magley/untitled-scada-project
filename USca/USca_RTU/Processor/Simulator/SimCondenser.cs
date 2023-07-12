using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimCondenser : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        public double Rate { get; set; }
        public bool Open { get; set; } // If not open, the 'rate' slowly goes down to 0.

        public SimCondenser(int address, double rate, bool open)
        {
            Address = address;
            Rate = rate;
            Open = open;
        }

        public double Value { get { return Rate; } set { Rate = value; } }

        public void Update()
        {
            if (!Open)
            {
                if (Rate > 0)
                {
                    Rate -= Math.Abs(Rate / 10);
                }
                if (Rate < 0)
                {
                    Rate += Math.Abs(Rate / 10);
                }
                if (Math.Abs(Rate) < 0.001)
                {
                    Rate = 0;
                }
            }
        }
    }

}
