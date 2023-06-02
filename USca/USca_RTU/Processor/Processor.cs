using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace USca_RTU.Processor
{
    public class Processor
    {
        private List<Signal> SignalsLocal = new();
        private Thread _communicationThread;

        public Processor()
        {
            _communicationThread = new Thread(new ThreadStart(Communicate));
            _communicationThread.IsBackground = true;
            _communicationThread.Start();
        }

        public void WriteValue(int address, double value)
        {
            SignalsLocal.Add(new(address, value, DateTime.Now));
        }


        private void SendBatch()
        {
            Console.WriteLine($"[{string.Join(",", SignalsLocal.Select(x => x.ToString()))}]");
            SignalsLocal.Clear();
        }

        private void Communicate()
        {
            while (true)
            {
                Thread.Sleep(1 * 1000);
                SendBatch();
            }
        }
    }
}
