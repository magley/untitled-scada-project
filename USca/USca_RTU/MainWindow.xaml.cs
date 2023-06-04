using System;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using USca_RTU.Processor;

namespace USca_RTU
{
    public partial class MainWindow : Window
    {
        private Simulator _simulator;
        private Reader _reader;
        private Thread _loopThread;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

			_simulator = new();
            _reader = new(_simulator);

			_loopThread = new(new ThreadStart(MainLoop));
			_loopThread.IsBackground = true;
			_loopThread.Start();
		}

        private void MainLoop()
        {
            while (true)
            {
                Thread.Sleep(1000);

                _simulator.Update();
                _reader.Update();

                string output = $"[\n\t{string.Join(",\n\t", _reader.Signals)}\n]";
				Console.WriteLine(output);
			}
        }

        private void CbSineEnabled_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            //simulator.IsSineEnabled = cb.IsChecked == true;
        }

        private void CbSimulationEnabled_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            //simulator.IsSimulationEnabled = cb.IsChecked == true;
        }
    }
}
