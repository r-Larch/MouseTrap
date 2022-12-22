using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;


namespace MouseTrap.Installer {
    [SupportedOSPlatform("windows")]
    internal class TaskInstaller {
        public string? TaskName { get; set; }
        public WindowsIdentity? RunAsUser { get; set; }
        public bool HighestPrivileges { get; set; }
        public string? ExecutablePath { get; set; }
        public string? Arguments { get; set; }


        public void Install(InstallerState stateSaver)
        {
            var service = TaskService.Instance;

            var task = service.NewTask();

            task.RegistrationInfo.Author = TaskName;
            task.RegistrationInfo.Date = DateTime.Now;
            task.RegistrationInfo.Description = "";

            task.Principal.UserId = RunAsUser?.Name;
            task.Principal.LogonType = TaskLogonType.InteractiveToken;
            task.Principal.RunLevel = HighestPrivileges ? TaskRunLevel.Highest : TaskRunLevel.LUA;

            task.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.AllowHardTerminate = true;
            task.Settings.StartWhenAvailable = true;
            task.Settings.RunOnlyIfNetworkAvailable = false;
            task.Settings.AllowDemandStart = true;
            task.Settings.Enabled = true;
            task.Settings.Hidden = false;
            task.Settings.RunOnlyIfIdle = false;
            task.Settings.WakeToRun = false;
            task.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            task.Settings.Priority = ProcessPriorityClass.RealTime;
            task.Settings.RestartCount = 3;
            task.Settings.RestartInterval = TimeSpan.FromMinutes(1);

            task.Triggers.Add(new LogonTrigger {
                Enabled = true,
                UserId = RunAsUser?.Name
            });

            task.Actions.Add(
                path: ExecutablePath,
                arguments: Arguments
            );

            service.RootFolder.RegisterTaskDefinition(TaskName, task);

            stateSaver["TaskName"] = TaskName;
            stateSaver["TaskAction"] = ExecutablePath;
        }


        public void Uninstall(InstallerState savedState)
        {
            var taskName = savedState["TaskName"];
            var executablePath = savedState["TaskAction"];

            var service = TaskService.Instance;

            var tasks = service.AllTasks.Where(t =>
                t.Definition.RegistrationInfo.URI == taskName &&
                t.Definition.Actions.OfType<ExecAction>().Any(_ => _.Path == executablePath)
            );

            foreach (var task in tasks) {
                var folder = task.Folder;
                folder.DeleteTask(task.Name);
                if (folder.GetTasks().Count == 0) {
                    folder.Parent.DeleteFolder(folder.Name);
                }
            }
        }
    }
}
