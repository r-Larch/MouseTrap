using System.Threading;
using System.Windows.Forms;


namespace MouseTrap {
    public class TrayWorker : ApplicationContext {
        public NotifyIcon TrayIcon { get; set; }
        public Thread Worker { get; }

        static TrayWorker()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
        }

        public TrayWorker(ThreadStart threadStart)
        {
            Worker = new Thread(threadStart);
        }

        public virtual void Start()
        {
            Worker?.Start();
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
