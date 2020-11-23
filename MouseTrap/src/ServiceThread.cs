using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;


namespace MouseTrap {
    public class ServiceThread : MsgBroadcast {
        public Func<IService> ServiceFactory { get; set; }
        private CancellationTokenSource _cts;
        private Thread _thread;
        private volatile int _errorCount;

        public void StartService()
        {
            StartService(ServiceFactory());
        }

        public void StartService(IService service)
        {
            if (_thread == null) {
                _cts = new CancellationTokenSource();
                _thread = new Thread(Runnable(service, _cts.Token)) {
                    Name = $"{nameof(ServiceThread)}+{service.GetType().FullName}",
                    Priority = ThreadPriority.Highest,
                    IsBackground = true,
                };

                if (OperatingSystem.IsWindows()) {
                    _thread.SetApartmentState(ApartmentState.STA);
                }

                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
                _thread.Start();
            }
        }

        private ThreadStart Runnable(IService service, CancellationToken token)
        {
            _errorCount = 0;
            var lastError = DateTime.MinValue;
            return () => {
                service.OnStart();

                try {
                    if (!token.IsCancellationRequested) {
                        service.Run(token);
                    }

                    service.OnExit();
                }
                catch (Exception e) when (token.IsCancellationRequested || e is ThreadAbortException || e is ThreadInterruptedException || e is OperationCanceledException) {
                    service.OnExit();
                }
                catch (Exception e) {
                    Logger.Error(e.Message, e);

                    service.OnExit();

                    if (lastError > DateTime.Now.AddMinutes(-10) && _errorCount > 50) {
                        // throw on more as 50 errors in 10min
                        throw;
                    }
                    else if (lastError < DateTime.Now.AddMinutes(-10)) {
                        // reset count after 10min
                        _errorCount = 1;
                        lastError = DateTime.Now;
                    }
                    else {
                        Interlocked.Increment(ref _errorCount);
                        lastError = DateTime.Now;
                    }

                    // restart this current thread
                    if (!token.IsCancellationRequested) {
                        NotifyRestartWorker();
                    }
                }
            };
        }

        public virtual void StopService()
        {
            if (_thread != null) {
                _cts.Cancel(true);
                _thread.Interrupt();
                _thread.Join();
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


        public static void NotifyRestartWorker()
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
