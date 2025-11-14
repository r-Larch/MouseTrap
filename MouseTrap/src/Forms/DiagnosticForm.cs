using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MouseTrap.Models;
using MouseTrap.Service;
using TextCopy;


// ReSharper disable LocalizableElement
namespace MouseTrap.Forms;

public partial class DiagnosticForm : Form {
    public ServiceThread Service { get; }

    public DiagnosticForm(ServiceThread service)
    {
        Service = service;
        InitializeComponent();

        var diagnostic = false;
        this.BtnStartDiagnostic.Click += delegate {
            if (!diagnostic) {
                ConsoleBox.Text = string.Empty;
                var config = ScreenConfigCollection.Load();
                Service.StopService();
                Service.StartService(new MouseBridgeDiagnosticService(config, RealtimeLog));
                this.BtnStartDiagnostic.Text = "Stop Diagnostic";
                diagnostic = true;
            }
            else {
                Service.RestoreOriginalState();
                this.BtnStartDiagnostic.Text = "Start Diagnostic";
                diagnostic = false;
            }
        };

        this.BtnCopy.Click += delegate {
            var sb = new StringBuilder();

            // update log infos
            LogfileBox.Text = string.Empty;
            InitLogFileInfos();

            sb.AppendLine(InfosBox.Text);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(ConsoleBox.Text);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(LogfileBox.Text);

            ClipboardService.SetText(sb.ToString());

            MessageBox.Show("Diagnostic data copied to clipboard.");
        };

        this.FormClosing += delegate {
            Service.RestoreOriginalState();
        };

        ConsoleBox.Text = "\r\n  INFO: Start Diagnostic to see Realtime output..";

        InitSystemInfos();
        InitLogFileInfos();
    }


    private void InitLogFileInfos()
    {
        try {
            if (File.Exists(Logger.Log.LogFilename)) {
                var log = File.ReadAllText(Logger.Log.LogFilename);
                LogfileBox.Text = log;
            }
        }
        catch (Exception e) {
            LogfileBox.Text = $"Failed at reading logfile:\r\n\r\n{e}";
        }
    }


    private void InitSystemInfos()
    {
        LogInfos($"Config: {Json(ScreenConfigCollection.Load())}");
        LogInfos($"Screens: {Json(Screen.AllScreens)}");
        LogInfos($"SystemInformation: {Json(StaticProperties(typeof(SystemInformation)))}");
        LogInfos($"Environment: {Json(StaticProperties(typeof(Environment)))}");

        void LogInfos(string msg)
        {
            this.InfosBox.AppendText($"{msg}\r\n");
        }

        static IDictionary<string, object?> StaticProperties(Type type)
        {
            var data = new Dictionary<string, object?>();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Static)) {
                try {
                    data.Add(property.Name, property.GetValue(null));
                }
                catch (Exception e) {
                    data.Add(property.Name, $"[Failed To Read] {e}");
                }
            }

            return data;
        }

        static string Json(object obj)
        {
            try {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                });
            }
            catch (Exception e) {
                return $"[Failed To Serialize] <{obj?.GetType()}> {e}";
            }
        }
    }


    public void RealtimeLog(string msg)
    {
        ConsoleBox.Invoke(() => {
            this.ConsoleBox.AppendText($"{msg}\r\n");
        });
    }
}
