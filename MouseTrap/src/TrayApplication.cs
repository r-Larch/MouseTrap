namespace MouseTrap; 

public class TrayApplication : ApplicationContext {
    public TrayIcon TrayIcon { get; }

    public TrayApplication(TrayIcon trayIcon)
    {
        TrayIcon = trayIcon ?? throw new ArgumentNullException(nameof(trayIcon));
        TrayIcon.Application = this;
        TrayIcon.Disposed += (sender, args) => {
            this.Exit();
        };
    }


    public static void Run(TrayIcon trayIcon)
    {
        new TrayApplication(trayIcon).Start();
    }


    public virtual void Start()
    {
        OnBeforeStart();
        Application.AddMessageFilter(TrayIcon);
        Application.Run(this);
    }

    public virtual void Exit()
    {
        ExitThreadCore();
    }

    protected override void ExitThreadCore()
    {
        OnBeforeExit();
        Application.RemoveMessageFilter(TrayIcon);
        base.ExitThreadCore();
    }

    protected override void OnMainFormClosed(object? sender, EventArgs e)
    {
        // prevent TrayIcon form disappearing after first configuration!!
        //base.OnMainFormClosed(sender, e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) {
            TrayIcon.Dispose();
        }

        base.Dispose(disposing);
    }


    public event EventHandler? BeforeStart;
    public event EventHandler? BeforeExit;

    protected virtual void OnBeforeStart()
    {
        BeforeStart?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnBeforeExit()
    {
        BeforeExit?.Invoke(this, EventArgs.Empty);
    }
}