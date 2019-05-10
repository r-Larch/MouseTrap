using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;


namespace MouseTrap {
    public class ServiceThread : MsgBroadcast {
        public Func<IService> ServiceFactory { get; set; }
        private Thread _thread;

        public void StartService()
        {
            StartService(ServiceFactory());
        }

        public void StartService(IService service)
        {
            if (_thread == null) {
                _thread = new Thread(Runnable(service)) {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                _thread.Start();
            }
        }

        private ThreadStart Runnable(IService service)
        {
            var errorCount = 0;
            var lastError = DateTime.MinValue;
            return () => {
                service.OnStart();

                try {
                    service.Run();
                }
                catch (Exception e) {
                    if (e is ThreadAbortException || e is ThreadInterruptedException) {
                        service.OnExit();
                        throw;
                    }

                    Logger.Error(e.Message, e);

                    service.OnExit();

                    if (lastError > DateTime.Now.AddMinutes(-10) && errorCount > 5) {
                        // throw on more as five errors in 10min
                        throw;
                    }
                    else if (lastError < DateTime.Now.AddMinutes(-10)) {
                        // reset count after 10min
                        errorCount = 1;
                        lastError = DateTime.Now;
                    }
                    else {
                        errorCount++;
                        lastError = DateTime.Now;
                    }

                    // restart this current thread
                    NotifyRestartWorker();
                }
            };
        }

        public virtual void StopService()
        {
            if (_thread != null) {
                _thread.Abort();
                _thread = null;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
            }
        }

        public virtual void RestartService()
        {
            StopService();
            StartService(ServiceFactory());
        }


        #region WndProc

        private static readonly int WmRestartWorker = RegisterWindowMessage("WM_RESTART_WORKER_" + App.Name);

        private static void NotifyRestartWorker()
        {
            PostMessage(
                (IntPtr) HWND_BROADCAST, WmRestartWorker,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }

        public void WndProc(ref Message m)
        {
            if (m.Msg == WmRestartWorker) {
                m.Result = new IntPtr(1);
                RestartService();
            }
        }

        #endregion
    }
}
