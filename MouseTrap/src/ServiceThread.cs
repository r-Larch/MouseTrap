using System;
using System.Diagnostics;
using System.Threading;


namespace MouseTrap {
    public class ServiceThread {
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
                    throw;
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
    }
}
