using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimBindingThermometer : INotifyPropertyChanged
    {
        public SimThermometer Thermometer { get; set; }
        public List<SimCondenser> Cooler { get; set; } = new();
        public List<SimHeatSource> Heater { get; set; } = new();

        public SimBindingThermometer(SimThermometer thermometer, SimCondenser? cooler = null, SimHeatSource? heater = null)
        {
            Thermometer = thermometer;
            if (cooler != null)
            {
                Cooler.Add(cooler);
            }
            if (heater != null)
            {
                Heater.Add(heater);
            }
        }

        public SimBindingThermometer(SimThermometer thermometer, List<SimCondenser> cooler, List<SimHeatSource> heater)
        {
            Thermometer = thermometer;
            Cooler = cooler;
            Heater = heater;
        }

        public void ApplyCooling()
        {
            foreach (var o in Cooler)
            {
                Thermometer.Temperature -= o.Value;
            }
        }

        public void ApplyHeating()
        {
            foreach (var o in Heater)
            {
                Thermometer.Temperature += o.Value;
            }
        }
    }

}
