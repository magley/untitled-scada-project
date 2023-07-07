using System.Collections.Generic;
using System.ComponentModel;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimBindingTankValve : INotifyPropertyChanged
    {
        public SimTank Tank { get; set; }
        public List<SimValve> In { get; set; } = new();
        public List<SimValve> Out { get; set; } = new();

        public SimBindingTankValve(SimTank tank, SimValve? inValve = null, SimValve? outValve = null)
        {
            Tank = tank;
            if (inValve != null)
            {
                In.Add(inValve);
            }
            if (outValve != null)
            {
                Out.Add(outValve);
            }
        }

        public SimBindingTankValve(SimTank tank, List<SimValve> inValve, List<SimValve> outValve)
        {
            Tank = tank;
            foreach (var o in inValve) In.Add(o);
            foreach (var o in outValve) Out.Add(o);
        }

        public void ApplyIn()
        {
            double diff = 0;
            foreach (var o in In)
            {
                if (o.Open)
                {
                    diff += o.Value;
                }
            }
            Tank.Value += diff;
        }

        public void ApplyOut()
        {
            double diff = 0;
            foreach (var o in Out)
            {
                if (o.Open)
                {
                    diff -= o.Value;
                }
            }
            Tank.Value += diff;
        }
    }

}
