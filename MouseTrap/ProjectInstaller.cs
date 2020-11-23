using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;


namespace MouseTrap {
    [RunInstaller(true)]
    public class ProjectInstaller : Installer {
        public ProjectInstaller()
        {
            if (OperatingSystem.IsWindows()) {
                //var processInstaller = new ServiceProcessInstaller {
                //    // ServiceAccount.LocalService has no access to staton0 -> default desktop
                //    Account = ServiceAccount.User,
                //};

                //var serviceInstaller = new ServiceInstaller {
                //    ServiceName = MouseService.Name,
                //    StartType = ServiceStartMode.Automatic,
                //};

                var user = System.Security.Principal.WindowsIdentity.GetCurrent();

                var taskInstaller = new TaskInstaller() {
                    TaskName = $"\\{App.Name}\\{user.Name.Replace("\\", "_")}",
                    // task
                    ExecutablePath = Application.ExecutablePath,
                    Arguments = null,
                    // user
                    RunAsUser = user,
                    HighestPrivileges = true,
                };

                Installers.AddRange(new Installer[] {
                    //processInstaller,
                    //serviceInstaller,
                    taskInstaller
                });
            }
        }


        public static void Install()
        {
            if (OperatingSystem.IsWindows()) {
                ManagedInstallerClass.InstallHelper(new[] {Application.ExecutablePath});
            }
        }

        public static void Uninstall()
        {
            if (OperatingSystem.IsWindows()) {
                ManagedInstallerClass.InstallHelper(new[] {"/u", Application.ExecutablePath});
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //.Dispose();
            }

            base.Dispose(disposing);
        }
    }


    [SupportedOSPlatform("windows")]
    internal class TaskInstaller : Installer {
        public string TaskName { get; set; }
        public WindowsIdentity RunAsUser { get; set; }
        public bool HighestPrivileges { get; set; }
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }


        public override void Install(IDictionary stateSaver)
        {
            var service = TaskService.Instance;

            var task = service.NewTask();

            task.RegistrationInfo.Author = TaskName;
            task.RegistrationInfo.Date = DateTime.Now;
            task.RegistrationInfo.Description = "";

            task.Principal.UserId = RunAsUser.Name;
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
                UserId = RunAsUser.Name
            });

            task.Actions.Add(
                path: ExecutablePath,
                arguments: Arguments
            );

            // TODO move this to commit
            service.RootFolder.RegisterTaskDefinition(TaskName, task);

            stateSaver["TaskName"] = TaskName;
            stateSaver["TaskAction"] = ExecutablePath;

            base.Install(stateSaver);
        }


        public override void Uninstall(IDictionary savedState)
        {
            if (savedState == null) {
                base.Uninstall(savedState);
                return;
            }

            var taskName = (string) savedState["TaskName"];
            var executablePath = (string) savedState["TaskAction"];

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

            base.Uninstall(savedState);
        }


        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }
    }
}
