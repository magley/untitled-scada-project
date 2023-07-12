using System;

namespace USca_RTU.Processor
{
    public class Signal
    {
        public int Address { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }

        public Signal(int address, string name, double value, DateTime timestamp)
        {
            Address = address;
            Name = name;
            Timestamp = timestamp;
            Value = value;
        }

        public Signal()
        {
        }

        public override string ToString()
        {
            return $"[{Timestamp}] {Name} ({Address}): {Value}";
        }
    }
}
