using System;
using System.Collections.Generic;

namespace USca_RTU.Processor
{
	public class Reader
	{
		private Simulator simulator;
		public List<Signal> Signals { get; private set; } = new();

		public Reader(Simulator simulator)
		{
			this.simulator = simulator;
		}

		public void Update()
		{
			Signals.Clear();

			foreach (var o in simulator.Thermometers)
			{
				Signals.Add(new(o.Id, o.Name, o.Value, DateTime.Now));
			}
			foreach (var o in simulator.Tanks)
			{
				Signals.Add(new(o.Id, o.Name, o.Value, DateTime.Now));
			}
			foreach (var o in simulator.Valves)
			{
				Signals.Add(new(o.Id, o.Name, o.Value, DateTime.Now));
			}
		}
	}
}
