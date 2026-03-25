using Microsoft.Win32;

public static class RegistryHelper
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public static void SetRunKey(string name, string command)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
        {
            key.SetValue(name, command);
        }
    }
}
