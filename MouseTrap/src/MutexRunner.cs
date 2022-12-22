using System.Runtime.InteropServices;


namespace MouseTrap {
    internal class MutexRunner : MsgBroadcast {
        private const string Name = App.Name;
        private static readonly Mutex Mutex = new Mutex(true, Name);
        private static readonly int WmShowApp = RegisterWindowMessage("WM_SHOW_" + Name);
        private static readonly int WmCloseApp = RegisterWindowMessage("WM_CLOSE_" + Name);

        public static bool MutexRun(Action program)
        {
            try {
                if (Mutex.WaitOne(TimeSpan.Zero, true)) {
                    try {
                        program();
                        return true;
                    }
                    finally {
                        Mutex.ReleaseMutex();
                    }
                }
            }
            catch (System.Threading.AbandonedMutexException) {
                Mutex.ReleaseMutex();
                MutexRun(program);
            }

            return false;
        }

        public static void NotifyRunningInstance()
        {
            // send our Win32 message to make the currently running instance
            // jump on top of all the other windows
            PostMessage(
                (IntPtr) HWND_BROADCAST, WmShowApp,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }

        public static void CloseRunningInstance()
        {
            PostMessage(
                (IntPtr) HWND_BROADCAST, WmCloseApp,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }

        public static bool IsMutexMessageOpen(ref Message m)
        {
            if (m.Msg == WmShowApp) {
                m.Result = new IntPtr(1);
                return true;
            }

            return false;
        }

        public static bool IsMutexMessageClose(ref Message m)
        {
            if (m.Msg == WmCloseApp) {
                m.Result = new IntPtr(1);
                return true;
            }

            return false;
        }
    }

    public class MsgBroadcast {
        #region Native

        // ReSharper disable once InconsistentNaming
        protected static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("user32")]
        protected static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        protected static extern int RegisterWindowMessage(string message);

        #endregion
    }
}
