using System;
using System.Collections.Generic;
using System.ComponentModel;
using USca_Server.Tags;

namespace USca_RTU.Processor
{
    public partial class SimTank : INotifyPropertyChanged
    {
        public int Address { get; set; }
        public string Name { get; set; } = "";
        private double _value;
        public double Value { get { return _value; } set { _value = Math.Max(0, value); } }

        public SimTank(int address, double value)
        {
            Address = address;
            Value = value;
        }
    }

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

                    diff += o.Rate;
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
                    diff -= o.Rate;
                }
            }
            Tank.Value += diff;
        }
    }

    public partial class SimulatorNew : INotifyPropertyChanged
    {
        private int AddressCnt = 0;
        private Random r = new();

        public int NextAddress()
        {
            AddressCnt++;
            return AddressCnt;
        }

        public List<SimTank> Tanks { get; set; } = new();
        public List<SimValve> Valves { get; set; } = new();
        public List<SimValve> ExternalValves { get; set; } = new();
        public List<SimBindingTankValve> TankValveBindings { get; set; } = new();

        public SimulatorNew()
        {
            SimTank MilkTank01 = new(NextAddress(), 0);
            SimTank MilkTank02 = new(NextAddress(), 0);
            SimTank MilkTank03 = new(NextAddress(), 0);
            SimTank MilkFilterTank = new(NextAddress(), 0);
            SimTank ProcessedDairyTank = new(NextAddress(), 0);
            SimTank RawCreamStorageTank = new(NextAddress(), 0);
            SimTank PostCompressorTank = new(NextAddress(), 0);
            SimTank WaterTank = new(NextAddress(), 0);
            SimTank CoolingTank = new(NextAddress(), 0);

            SimValve MilkTank01_ExternalValve = new(-1, 4, true);
            SimValve MilkTank02_ExternalValve = new(-1, 4, true);
            SimValve MilkTank03_ExternalValve = new(-1, 4, true);
            SimValve RawCreamStorageTank_ExternalValve = new(-1, 1, true);
            SimValve MilkTank01_OutValve = new(NextAddress(), 1, true);
            SimValve MilkTank02_OutValve = new(NextAddress(), 1, true);
            SimValve MilkTank03_OutValve = new(NextAddress(), 1, true);
            SimValve MilkFilterTank_OutValve = new(NextAddress(), 2, true);
            SimValve ProcessedDairyTank_ToRawCreamValve = new(NextAddress(), 2, true);
            SimValve ProcessedDairyTank_ToCompressorValve = new(NextAddress(), 2, true);
            SimValve PostCompressorTank_Valve = new(NextAddress(), 2, true);
            SimValve PostCompressorTank_ExternalValve = new(-1, 1, true);
            SimValve WaterTank_ExternalValve = new(-1, 1, true);
            SimValve WaterTank_ExternalValve2 = new(-1, 1, true);
            SimValve CoolingTank_ExternalValve = new(NextAddress(), 3, true);

            SimBindingTankValve MilkTank01_Binding = new(MilkTank01, MilkTank01_ExternalValve, MilkTank01_OutValve);
            SimBindingTankValve MilkTank02_Binding = new(MilkTank02, MilkTank02_ExternalValve, MilkTank02_OutValve);
            SimBindingTankValve MilkTank03_Binding = new(MilkTank03, MilkTank03_ExternalValve, MilkTank03_OutValve);
            SimBindingTankValve MilkFilterTank_Binding = new(MilkFilterTank, new List<SimValve> { MilkTank01_OutValve, MilkTank02_OutValve, MilkTank03_OutValve }, new() { MilkFilterTank_OutValve });
            SimBindingTankValve ProcessedDairyTank_Binding = new(ProcessedDairyTank, new List<SimValve> { MilkFilterTank_OutValve }, new() { ProcessedDairyTank_ToRawCreamValve, ProcessedDairyTank_ToCompressorValve });
            SimBindingTankValve RawCreamStorageTank_Binding = new(RawCreamStorageTank, ProcessedDairyTank_ToRawCreamValve, RawCreamStorageTank_ExternalValve);
            SimBindingTankValve PostCompressorTank_Binding = new(PostCompressorTank, PostCompressorTank_Valve, PostCompressorTank_ExternalValve );
            SimBindingTankValve WaterTank_Binding = new(WaterTank, WaterTank_ExternalValve, WaterTank_ExternalValve2);
            SimBindingTankValve CoolingTank_Binding = new(CoolingTank, new List<SimValve> { PostCompressorTank_ExternalValve, WaterTank_ExternalValve2 }, new() { CoolingTank_ExternalValve });

            Tanks.Add(MilkTank01);
            Tanks.Add(MilkTank02);
            Tanks.Add(MilkTank03);
            Tanks.Add(MilkFilterTank);
            Tanks.Add(ProcessedDairyTank);
            Tanks.Add(RawCreamStorageTank);
            Tanks.Add(PostCompressorTank);
            Tanks.Add(WaterTank);
            Tanks.Add(CoolingTank);

            ExternalValves.Add(MilkTank01_ExternalValve);
            ExternalValves.Add(MilkTank02_ExternalValve);
            ExternalValves.Add(MilkTank03_ExternalValve);
            ExternalValves.Add(RawCreamStorageTank_ExternalValve);
            ExternalValves.Add(PostCompressorTank_ExternalValve);
            ExternalValves.Add(WaterTank_ExternalValve);
            ExternalValves.Add(WaterTank_ExternalValve2);
            ExternalValves.Add(CoolingTank_ExternalValve);
            Valves.Add(MilkTank01_OutValve);
            Valves.Add(MilkTank02_OutValve);
            Valves.Add(MilkTank03_OutValve);
            Valves.Add(MilkFilterTank_OutValve);
            Valves.Add(ProcessedDairyTank_ToRawCreamValve);
            Valves.Add(ProcessedDairyTank_ToCompressorValve);
            Valves.Add(PostCompressorTank_Valve);

            TankValveBindings.Add(MilkTank01_Binding);
            TankValveBindings.Add(MilkTank02_Binding);
            TankValveBindings.Add(MilkTank03_Binding);
            TankValveBindings.Add(MilkFilterTank_Binding);
            TankValveBindings.Add(ProcessedDairyTank_Binding);
            TankValveBindings.Add(RawCreamStorageTank_Binding);
            TankValveBindings.Add(PostCompressorTank_Binding);
            TankValveBindings.Add(WaterTank_Binding);
            TankValveBindings.Add(CoolingTank_Binding);
        }

        public void Update1()
        {
            foreach (var o in Valves)
            {
                o.Rate += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.01) * 0.01);
            }
            foreach (var o in ExternalValves)
            {
                o.Rate += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.01) * 0.01);
            }

            foreach (var o in TankValveBindings)
            {
                o.ApplyIn();
            }
        }

        public void Update2()
        {
            foreach (var o in TankValveBindings)
            {
                o.ApplyOut();
            }
        }

        internal void UpdateOutputFrom(List<OutputTagValueDTO> outputTagValues)
        {
            foreach (var o in outputTagValues)
            {
                foreach (var oo in Valves)
                {
                    if (oo.Address == o.Address)
                    {
                        oo.Open = o.Value != 0;
                    }
                }

                // Check for other objects the same way, once we add them.
            }
        }
    }
}
