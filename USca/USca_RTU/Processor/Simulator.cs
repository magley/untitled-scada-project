using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace USca_RTU.Processor
{
	public interface Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	
	public class Thermometer : Item
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
		public double Value { get; set; } = 0;
	}

	public class Tank
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
		public double Value { get; set; } = 5;
		public double ValueMax { get; set; } = 10;

		public Tank(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class Valve
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
		public double Value { get; set; } = 0;
		public bool Open { get; set; } = true;

		public Valve(int id, string name, double startValue, bool open)
		{
			Id = id;
			Name = name;
			Value = startValue;
			Open = open;
		}
	}

    class TankValve
	{
        public Tank Tank { get; set; }
		public List<Valve> In { get; private set; }
		public List<Valve> Out { get; private set; }

		public TankValve(Tank tank, List<Valve> inValves, List<Valve> outValves)
		{
			Tank = tank;
			In = inValves;
			Out = outValves;
		}

		public TankValve(Tank tank, Valve inValve, Valve outValve)
		{
			Tank = tank;
			In = new() { inValve };
			Out = new() { outValve };
		}

		public void Apply()
		{
			double diff = 0;
			foreach (var o in In)
			{
				if (o.Open)
				{
					diff += o.Value;
				}
			}
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

    public class Simulator
	{
		public List<Thermometer> Thermometers { get; set; } = new();
		public List<Tank> Tanks { get; set; } = new();
		public List<Valve> Valves { get; set; } = new();

		private List<TankValve> TankValves { get; set; } = new();
		private Random r = new();

		private int MasterId { get; set; } = 0;
		public int NextId()
		{
			MasterId++;
			return MasterId;
		}

		public Simulator()
		{
			Tanks.Add(new(NextId(), "Tank01"));

			Valves.Add(new(NextId(), "Tank01_ValveIn", 3, true));
			Valves.Add(new(NextId(), "Tank01_ValveIn_Reserve", 1.5, false));
			Valves.Add(new(NextId(), "Tank01_ValveOut", 3, true));

			TankValves.Add(new(Tanks[0], Valves.GetRange(0, 2), Valves.GetRange(2, 1)));
		}

		public void Update()
		{
			foreach (var o in Valves)
			{
				o.Value += r.NextDouble() * 0.2;
			}

			foreach (var o in TankValves)
			{
				o.Apply();
			}
		}
	}
}
