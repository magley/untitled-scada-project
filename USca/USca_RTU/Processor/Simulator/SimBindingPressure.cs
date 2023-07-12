using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimBindingPressure : INotifyPropertyChanged
    {
        public SimManometer Manometer { get; set; }
        public List<SimCompressor> Compressors = new();

        public SimBindingPressure(SimManometer manometer, SimCompressor compressor)
        {
            Manometer = manometer;
            Compressors.Add(compressor);
        }

        public void ApplyCompression()
        {
            foreach (var o in Compressors)
            {
                Manometer.Pressure += o.Value;
            }
        }
    }

}
