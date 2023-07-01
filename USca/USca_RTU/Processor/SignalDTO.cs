using System;

namespace USca_RTU.Processor
{
	internal class SignalDTO
	{
		public int Address { get; set; }
		public string Name { get; set; }
		public double Value { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
