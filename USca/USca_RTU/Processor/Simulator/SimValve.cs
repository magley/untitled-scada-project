using System.ComponentModel;

namespace USca_RTU.Processor.Simulator
{
    public partial class SimValve : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        public double Rate { get; set; }
        public bool Open { get; set; }
        public bool External { get; set; }

        public SimValve(int address, double rate, bool open)
        {
            Address = address;
            Rate = rate;
            Open = open;
            External = address <= 0;
        }

        public double Value { get { return Open ? Rate : 0; } set { Rate = value; } }
    }

}
