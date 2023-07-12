using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimHeatSource : INotifyPropertyChanged
    {
        public double Rate { get; set; }

        public SimHeatSource(double rate)
        {
            Rate = rate;
        }

        public double Value { get { return Rate; } set { Rate = value; } }
    }

}
