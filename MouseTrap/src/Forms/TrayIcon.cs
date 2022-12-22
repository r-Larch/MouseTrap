using System.ComponentModel;
using System.Reflection;


namespace MouseTrap;

public class TrayIcon : Component, IMessageFilter {
    private readonly IContainer Components = new Container();
    public NotifyIcon NotifyIcon { get; }

    public TrayIcon()
    {
        NotifyIcon = new NotifyIcon(Components);
        ContextMenu = new ContextMenuStrip(Components);

        // try to show ContextMenu on left and right click
        NotifyIcon.MouseClick += (sender, args) => {
            if (args.Button == MouseButtons.Left) {
                try {
                    var method = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                    method?.Invoke(NotifyIcon, null);
                }
                catch {
                    // ignored
                }
            }
        };
    }

    public Icon Icon {
        get => NotifyIcon.Icon;
        set => NotifyIcon.Icon = value;
    }

    public string Text {
        get => NotifyIcon.Text;
        set => NotifyIcon.Text = value;
    }

    public ContextMenuStrip ContextMenu {
        get => NotifyIcon.ContextMenuStrip;
        set => NotifyIcon.ContextMenuStrip = value;
    }

    public bool Visible {
        get => NotifyIcon.Visible;
        set => NotifyIcon.Visible = value;
    }

    public TrayApplication? Application { get; set; }

    public void Hide()
    {
        Visible = false;
    }

    public void Show()
    {
        Visible = true;
    }

    public void Close()
    {
        Dispose();
    }

    public bool PreFilterMessage(ref Message m)
    {
        WndProc(ref m);
        return false;
    }

    protected virtual void WndProc(ref Message m)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) {
            Components.Dispose();
        }

        base.Dispose(disposing);
    }
}
