namespace USca_Server.Util
{
    /// <summary>
    /// Thin wrapper around <c>Thread</c> which supports <c>Abort()</c> using a boolean flag, because
    /// <c>Thread::Abort()</c> is obsolete and not allowed in .NET Core.
    /// </summary>
    public class LoopThread
    {
        private Thread _thread { get; set; }
        public bool IsRunning { get; set; } = true;

        public LoopThread(ThreadStart threadStart)
        {
            _thread = new(new ThreadStart(() => ThinWrapper(threadStart)));
            _thread.IsBackground = false;
        }

        private void ThinWrapper(ThreadStart threadStart)
        {
            while (IsRunning)
            {
                threadStart.Invoke();
            }
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Abort()
        {
            IsRunning = false;
        }
    }
}
