using System;
using System.Threading;
using System.Windows.Forms;


namespace MouseTrap {
    public class TrayWorker : ApplicationContext {
        private readonly ThreadStart _threadStart;
        public NotifyIcon TrayIcon { get; set; }
        public Thread Worker { get; private set; }

        static TrayWorker()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
        }

        public TrayWorker(ThreadStart threadStart)
        {
            _threadStart = threadStart ?? throw new ArgumentNullException(nameof(threadStart));
        }

        public virtual void RestartWorker()
        {
            if (Worker != null) {
                Worker.Abort();
                Worker = null;
            }

            StartWorker();
        }

        private void StartWorker()
        {
            if (Worker == null) {
                Worker = new Thread(_threadStart);
                Worker.Start();
            }
        }

        public virtual void Start()
        {
            StartWorker();
            Application.Run(this);
        }

        public virtual void Exit()
        {
            if (TrayIcon != null) {
                TrayIcon.Visible = false;
            }

            Worker?.Abort();
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                TrayIcon?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
