using System;
using System.Windows.Forms;


namespace MouseTrap {
    public abstract class Cmd : IWin32Window, IDisposable {
        protected Cmd()
        {
            Application.EnableVisualStyles();
        }

        public abstract void Start();

        public virtual IntPtr Handle => NativeMethods.GetConsoleWindow();

        public NotifyIcon TrayIcon;

        private bool _visible = true;
        public bool Visible {
            get => _visible;
            set {
                _visible = value;
                NativeMethods.ShowWindow(Handle, _visible ? WindowShowState.Show : WindowShowState.Hide);
            }
        }

        public virtual void Exit()
        {
            if (TrayIcon != null) {
                TrayIcon.Visible = false;
                Application.DoEvents();
            }

            Dispose();
            Application.Exit();
            Environment.Exit(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing) {
                TrayIcon?.Dispose();
            }
        }
    }
}
