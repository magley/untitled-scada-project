using System.ComponentModel;
using System.Threading;
using System.Windows;
using USca_RTU.Processor;

namespace USca_RTU
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Simulator Simulator { get; set; }
        private Reader _reader;
        private Thread _loopThread;
        private Thread _sendThread;

        public MainWindow()
        {
            //CryptoUtil.SavePublicKey("C:/Users/aaa/Desktop/USca_RTU_Key.pub");

			InitializeComponent();
            DataContext = this;

            Simulator = new();
            _reader = new(Simulator);

			_loopThread = new(new ThreadStart(MainLoop));
			_loopThread.IsBackground = true;
			_loopThread.Start();

            _sendThread = new(new ThreadStart(SendData));
            _sendThread.IsBackground = true;
            _sendThread.Start();
		}

        private void MainLoop()
        {
            while (true)
            {
                Thread.Sleep(250);

                Simulator.Update();
                _reader.Update();

                string output = $"[\n\t{string.Join(",\n\t", _reader.Signals)}\n]";
			}
        }

        private async void SendData()
        {
            while (true)
            {
                Thread.Sleep(250);

                await CommService.SendSignalsBatch(_reader.Signals);
            }
        }
	}
}
