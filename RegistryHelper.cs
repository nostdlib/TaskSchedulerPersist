using System;
using System.Management;
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

    public static string GetSerialNumber()
    {
        Guid biosId = Guid.Empty;

        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");

            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["UUID"] != null)
                {
                    var uuid = obj["UUID"].ToString().Trim();
                    biosId = new Guid(uuid);
                    break;
                }
            }
        }
        catch { }

        return biosId.ToString("N");
    }
}
