using System.ComponentModel;
using System.Xml.Serialization;

public partial class Program
{
    enum TASK_CREATION : int
    {
        TASK_VALIDATE_ONLY = 0x1,
        TASK_CREATE = 0x2,
        TASK_UPDATE = 0x4,
        TASK_CREATE_OR_UPDATE = 0x6,
        TASK_DISABLE = 0x8,
        TASK_DONT_ADD_PRINCIPAL_ACE = 0x10,
        TASK_IGNORE_REGISTRATION_TRIGGERS = 0x20
    }

    public enum TaskLogonType
    {
        None,
        Password,
        S4U,
        InteractiveToken,
        Group,
        ServiceAccount,
        InteractiveTokenOrPassword
    }

    [DefaultValue(IgnoreNew)]
    public enum TaskInstancesPolicy
    {
        Parallel,
        Queue,
        IgnoreNew,
        StopExisting
    }

    public enum TaskCompatibility
    {
        AT,
        V1,
        V2,
        V2_1,
        V2_2,
        V2_3
    }

    public enum TaskRunLevel
    {
        [XmlEnum("LeastPrivilege")]
        LUA,

        [XmlEnum("HighestAvailable")]
        Highest
    }

    public enum TaskTriggerType
    {
        Event = 0,
        Time = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4,
        MonthlyDOW = 5,
        Idle = 6,
        Registration = 7,
        Boot = 8,
        Logon = 9,
        SessionStateChange = 11,
        Custom = 12
    }

    public enum TaskActionType
    {
        Execute = 0,
        ComHandler = 5,
        SendEmail = 6,
        ShowMessage = 7
    }

    public enum TaskState
    {
        Unknown,
        Disabled,
        Queued,
        Ready,
        Running
    }

    public enum TaskSessionStateChangeType
    {
        ConsoleConnect = 1,
        ConsoleDisconnect = 2,
        RemoteConnect = 3,
        RemoteDisconnect = 4,
        SessionLock = 7,
        SessionUnlock = 8
    }
}
