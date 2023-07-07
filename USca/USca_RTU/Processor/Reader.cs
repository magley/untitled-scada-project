using System;
using System.Collections.Generic;

namespace USca_RTU.Processor
{
	public class Reader
	{
		private SimulatorNew simulator;
		public List<Signal> Signals { get; private set; } = new();

		public Reader(SimulatorNew simulator)
		{
			this.simulator = simulator;
		}

		public void Update(object _signalsLock)
		{
			lock (_signalsLock)
			{
				Signals.Clear();

				foreach (var o in simulator.Tanks)
				{
					Signals.Add(new(o.Address, o.Name, o.Value, DateTime.Now));
				}
				foreach (var o in simulator.Valves)
				{
					if (!o.External)
					{
						Signals.Add(new(o.Address, o.Name, o.Value, DateTime.Now));
					}
				}
                foreach (var o in simulator.Thermometers)
                {
					Signals.Add(new(o.Address, o.Name, o.Value, DateTime.Now));
                }
                foreach (var o in simulator.Condensers)
                {
                    Signals.Add(new(o.Address, o.Name, o.Value, DateTime.Now));
                }
            }
		}
	}
}
