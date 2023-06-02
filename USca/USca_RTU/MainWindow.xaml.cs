using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace USca_RTU
{
    public class Simulator
    {
        private Processor.Processor processor;
        private Thread _simulationThread;

        public bool IsSineEnabled { get; set; } = false;
        public bool IsSimulationEnabled { get; set; } = false;

        public Simulator()
        {
            processor = new();

            _simulationThread = new(new ThreadStart(Simulation));
            _simulationThread.IsBackground = true;
            _simulationThread.Start();
        }

        private void Simulation()
        {
            while (true)
            {
                Thread.Sleep(500);

                if (!IsSimulationEnabled)
                {
                    continue;
                }

                if (IsSineEnabled)
                {
                    processor.WriteValue(1, Math.Sin(DateTime.Now.Ticks / 1000.0) * 5.0);
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private Simulator simulator;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            simulator = new();
        }

        private void CbSineEnabled_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            simulator.IsSineEnabled = cb.IsChecked == true;
        }

        private void CbSimulationEnabled_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            simulator.IsSimulationEnabled = cb.IsChecked == true;
        }
    }
}
