using System;
using System.Threading;
using System.Windows.Forms;


namespace MouseTrap {
    public class TrayWorker<T> : ApplicationContext where T : IWorker, new() {
        private static IWorker Worker => new T();
        public NotifyIcon TrayIcon { get; set; }
        private Thread _thread;

        static TrayWorker()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
        }

        public virtual void StopWorker()
        {
            if (_thread != null) {
                _thread.Abort();
                _thread = null;
            }
        }

        private void StartWorker(IWorker worker)
        {
            if (_thread == null) {
                _thread = new Thread(worker.Run) {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };
                _thread.Start();
            }
        }

        public virtual ResetWorker SwapWorker(IWorker worker)
        {
            StopWorker();
            StartWorker(worker);
            return new ResetWorker(RestartWorker);
        }

        public virtual void RestartWorker()
        {
            StopWorker();
            StartWorker(Worker);
        }

        public virtual void Start()
        {
            StartWorker(Worker);
            Application.Run(this);
        }

        public virtual void Exit()
        {
            if (TrayIcon != null) {
                TrayIcon.Visible = false;
            }

            _thread?.Abort();
            Application.Exit();
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            // prevent TrayIcon form disappearing after first configuration!!
            //base.OnMainFormClosed(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                TrayIcon?.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    public class ResetWorker {
        private readonly Action _resetWorker;

        public ResetWorker(Action resetWorker)
        {
            _resetWorker = resetWorker;
        }

        public void Reset()
        {
            _resetWorker();
        }
    }
}
