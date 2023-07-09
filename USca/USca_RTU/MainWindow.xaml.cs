using System.ComponentModel;
using System.Threading;
using System.Windows;
using USca_RTU.Processor;
using USca_RTU.Tag;

namespace USca_RTU
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public SimulatorNew Simulator { get; set; }
        private Reader _reader;
        private Thread _loopThread;
        private Thread _sendThread;
        private object _signalsLock = new();

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

            _sendThread = new(new ThreadStart(SyncOutputTagValues));
            _sendThread.IsBackground = true;
            _sendThread.Start();
        }

        private void MainLoop()
        {
            int alternator = 0; // We don't want to apply in-valve and out-valve in the
            // same tick, because the tank's value might not change (or will change slightly).
            // This is just for demonstration purposes, to show that stuff is happening.

            while (true)
            {
                Thread.Sleep(100);

                if (alternator == 0)
                {
                    Simulator.Update1();
                    alternator = 1;
                } else
                {
                    Simulator.Update2();
                    alternator = 0;
                }
        
                _reader.Update(_signalsLock);

                string output = $"[\n\t{string.Join(",\n\t", _reader.Signals)}\n]";
            }
        }

        private async void SendData()
        {
            while (true)
            {
                Thread.Sleep(250);

                await CommService.SendSignalsBatch(_reader.Signals, _signalsLock);
            }
        }

        private void SyncOutputTagValues()
        {
            while (true)
            {
                Thread.Sleep(250);
                FetchOutputTagValues();
            }
        }

        private async void FetchOutputTagValues()
        {
            var outputTagValues = await TagService.GetOutputTagValues();
            if (outputTagValues != null)
            {
                Simulator.UpdateOutputFrom(outputTagValues);
            }
        }
    }
}
