using System;
using System.Collections.Generic;
using System.ComponentModel;
using USca_Server.Tags;

namespace USca_RTU.Processor
{
	public interface Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
	
	public partial class Thermometer : Item, INotifyPropertyChanged
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
		public double Value { get; set; } = 0;
	}

	public partial class Tank : Item, INotifyPropertyChanged
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
		private double _value = 5;
		public double Value
		{
			get { return _value; }
			set { _value = Math.Clamp(value, 0, ValueMax); }
		}
		public double ValueMax { get; set; } = 10;

		public Tank(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public partial class Valve : Item, INotifyPropertyChanged
	{
		public int Id { get; set; } = -1;
		public string Name { get; set; }
        private double _value = 0;
        public double Value
        {
            get { return Open ? _value : 0; }
            set { _value = Math.Clamp(value, 0, Double.MaxValue); }
        }
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

    public partial class Simulator : INotifyPropertyChanged
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
			Tanks.Add(new(NextId(), "Raw Milk Tank 01"));
            Tanks.Add(new(NextId(), "Raw Milk Tank 02"));
            Tanks.Add(new(NextId(), "Raw Milk Tank 03"));
            Tanks.Add(new(NextId(), "Milk Filter Tank"));

            Valves.Add(new(NextId(), "Infinite Valve", 0.05, true));
            Valves.Add(new(NextId(), "Raw Milk Tank 01 Valve", 0.1, true));
			Valves.Add(new(NextId(), "Raw Milk Tank 02 Valve", 0.1, true));
			Valves.Add(new(NextId(), "Raw Milk Tank 03 Valve", 0.1, true));
            Valves.Add(new(NextId(), "Milk Filter To Cream Storage Valve", 0.01, false));

            TankValves.Add(new(Tanks[0], Valves.GetRange(0, 1), Valves.GetRange(1, 1)));
            TankValves.Add(new(Tanks[1], Valves.GetRange(0, 1), Valves.GetRange(2, 1)));
            TankValves.Add(new(Tanks[2], Valves.GetRange(0, 1), Valves.GetRange(3, 1)));

            TankValves.Add(new(Tanks[3], Valves.GetRange(1, 3), Valves.GetRange(4, 1)));
		}

		public void Update()
		{
			foreach (var o in Valves)
			{
				o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.0001) * 0.0001);
			}

			foreach (var o in TankValves)
			{
				o.Apply();
			}
		}

        internal void UpdateOutputFrom(List<OutputTagValueDTO> outputTagValues)
        {
            foreach (var o in outputTagValues)
			{
				foreach (var oo in Valves)
				{
					//Console.WriteLine(o.Address);
					if (oo.Id == o.Address)
					{
						oo.Open = o.Value != 0;
					}
				}
				// Check for other objects the same way, once we add them.
            }
        }
    }
}
