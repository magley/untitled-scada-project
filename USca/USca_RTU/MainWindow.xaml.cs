using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using USca_RTU.Processor;

namespace USca_RTU
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Simulator _simulator;
        public Simulator Simulator { get { return _simulator; } private set { _simulator = value; } }
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
                Thread.Sleep(500);

                _simulator.Update();
                _reader.Update();

                string output = $"[\n\t{string.Join(",\n\t", _reader.Signals)}\n]";
				Console.WriteLine(output);
			}
        }
	}
}
