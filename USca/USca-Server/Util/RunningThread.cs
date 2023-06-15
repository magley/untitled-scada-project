namespace USca_Server.Util
{
    public class RunningThread
    {
        public Thread Thread { get; set; }
        public bool IsRunning { get; set; } = true;

        public RunningThread(Thread thread)
        {
            Thread = thread;
        }

        public RunningThread()
        {

        }
    }
}
