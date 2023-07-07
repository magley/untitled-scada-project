using System;
using System.Collections.Generic;
using System.ComponentModel;
using USca_RTU.Processor.Simulator;
using USca_Server.Tags;

namespace USca_RTU.Processor
{
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

        public List<SimThermometer> Thermometers { get; set; } = new();
        public List<SimCondenser> Condensers { get; set; } = new();
        public List<SimHeatSource> HeatSources { get; set; } = new();
        public List<SimBindingThermometer> ThermometerBindings { get; set; } = new();

        public List<SimManometer> Manometers { get; set; } = new();
        public List<SimCompressor> Compressors { get; set; } = new();
        public List<SimBindingPressure> PressureBindings { get; set; } = new();

        public SimulatorNew()
        {
            SimTank MilkTank01 = new(NextAddress(), 0);
            SimTank MilkTank02 = new(NextAddress(), 0);
            SimTank MilkTank03 = new(NextAddress(), 0);

            SimValve MilkTank01_ExternalValve = new(-1, 4, true);
            SimValve MilkTank02_ExternalValve = new(-1, 4, true);
            SimValve MilkTank03_ExternalValve = new(-1, 4, true);
            SimValve RawCreamStorageTank_ExternalValve = new(-1, 1, true);
            SimValve MilkTank01_OutValve = new(NextAddress(), 1, true);
            SimValve MilkTank02_OutValve = new(NextAddress(), 1, true);
            SimValve MilkTank03_OutValve = new(NextAddress(), 1, true);

            SimTank MilkFilterTank = new(NextAddress(), 0);
            SimValve MilkFilterTank_OutValve = new(NextAddress(), 2, true);
            
            SimTank ProcessedDairyTank = new(NextAddress(), 0);
            SimValve ProcessedDairyTank_ToRawCreamValve = new(NextAddress(), 2, true);
            SimTank RawCreamStorageTank = new(NextAddress(), 0);
            
            SimValve ProcessedDairyTank_ToCompressorValve = new(NextAddress(), 2, true);
            SimManometer Manometer = new(NextAddress(), 9000); // desired pressure: ~14000
            SimCompressor Compressor = new(NextAddress(), 10, true);
            SimValve PostCompressorTank_Valve = new(NextAddress(), 2, true);
            SimTank PostCompressorTank = new(NextAddress(), 0);
            SimValve PostCompressorTank_ExternalValve = new(-1, 1, true);

            SimValve WaterTank_ExternalValve = new(-1, 1, true);
            SimTank WaterTank = new(NextAddress(), 0);
            SimValve WaterTank_ExternalValve2 = new(-1, 1, true);

            SimTank CoolingTank = new(NextAddress(), 0);
            SimValve CoolingTank_ExternalValve = new(-1, 3, true);

            SimBindingTankValve MilkTank01_Binding = new(MilkTank01, MilkTank01_ExternalValve, MilkTank01_OutValve);
            SimBindingTankValve MilkTank02_Binding = new(MilkTank02, MilkTank02_ExternalValve, MilkTank02_OutValve);
            SimBindingTankValve MilkTank03_Binding = new(MilkTank03, MilkTank03_ExternalValve, MilkTank03_OutValve);
            SimBindingTankValve MilkFilterTank_Binding = new(MilkFilterTank, new List<SimValve> { MilkTank01_OutValve, MilkTank02_OutValve, MilkTank03_OutValve }, new() { MilkFilterTank_OutValve });
            SimBindingTankValve ProcessedDairyTank_Binding = new(ProcessedDairyTank, new List<SimValve> { MilkFilterTank_OutValve }, new() { ProcessedDairyTank_ToRawCreamValve, ProcessedDairyTank_ToCompressorValve });
            SimBindingTankValve RawCreamStorageTank_Binding = new(RawCreamStorageTank, ProcessedDairyTank_ToRawCreamValve, RawCreamStorageTank_ExternalValve);
            SimBindingTankValve PostCompressorTank_Binding = new(PostCompressorTank, PostCompressorTank_Valve, PostCompressorTank_ExternalValve );
            SimBindingTankValve WaterTank_Binding = new(WaterTank, WaterTank_ExternalValve, WaterTank_ExternalValve2);
            SimBindingTankValve CoolingTank_Binding = new(CoolingTank, new List<SimValve> { PostCompressorTank_ExternalValve, WaterTank_ExternalValve2 }, new() { CoolingTank_ExternalValve });

            SimThermometer CoolingTankThermometer = new(NextAddress(), 4);
            SimCondenser CoolingTankCondenser1 = new(NextAddress(), 0.001, true);
            SimCondenser CoolingTankCondenser2 = new(NextAddress(), 0.01, false);
            SimCondenser CoolingTankCondenser3 = new(NextAddress(), 0.01, false);
            SimCondenser CoolingTankCondenser4 = new(NextAddress(), 0.01, false);
            SimHeatSource CoolingTankStartingTemp = new(0.0025);

            SimBindingThermometer CoolingTankThermometer_Binding = new(CoolingTankThermometer, new List<SimCondenser> { CoolingTankCondenser1, CoolingTankCondenser2, CoolingTankCondenser3, CoolingTankCondenser4 }, new() { CoolingTankStartingTemp });
            SimBindingPressure PressureBinding = new(Manometer, Compressor);

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

            Thermometers.Add(CoolingTankThermometer);

            Condensers.Add(CoolingTankCondenser1);
            Condensers.Add(CoolingTankCondenser2);
            Condensers.Add(CoolingTankCondenser3);
            Condensers.Add(CoolingTankCondenser4);

            HeatSources.Add(CoolingTankStartingTemp);

            ThermometerBindings.Add(CoolingTankThermometer_Binding);

            Manometers.Add(Manometer);
            Compressors.Add(Compressor);
            PressureBindings.Add(PressureBinding);
        }

        public void PertubateRates()
        {
            foreach (var o in Valves)
            {
                o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.01) * 0.01);
            }

            foreach (var o in ExternalValves)
            {
                o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.01) * 0.01);
            }

            foreach (var o in Condensers)
            {
                o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.00001) * 0.00001);
            }

            foreach (var o in HeatSources)
            {
                o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.00001) * 0.00001);
            }

            foreach (var o in Compressors)
            {
                o.Value += Math.Abs(r.NextDouble() * Math.Sin(DateTime.Now.Ticks / 5.0 + r.NextDouble() * 0.001) * 0.001);
            }

            foreach (var o in PressureBindings)
            {
                o.ApplyCompression();
            }
        }

        public void Update1()
        {
            PertubateRates();

            foreach (var o in TankValveBindings)
            {
                o.ApplyIn();
            }

            foreach (var o in ThermometerBindings)
            {
                o.ApplyCooling();
            }

            foreach (var o in Condensers)
            {
                o.Update();
            }
        }

        public void Update2()
        {
            foreach (var o in TankValveBindings)
            {
                o.ApplyOut();
            }

            foreach (var o in ThermometerBindings)
            {
                o.ApplyHeating();
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

                foreach (var oo in Condensers)
                {
                    if (oo.Address == o.Address)
                    {
                        oo.Open = o.Value != 0;
                    }
                }

                foreach (var oo in Compressors)
                {
                    if (oo.Address == o.Address)
                    {
                        oo.Rate = o.Value;
                    }
                }
            }
        }
    }
}
