using System;
using Microsoft.Win32;

public static class DeviceInfo
{
    public static string GetUUID()
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
        if (key != null)
        {
            object value = key.GetValue("MachineGuid");
            if (value != null)
            {
                return new Guid(value.ToString().Trim()).ToString("N");
            }
        }

        return Guid.Empty.ToString("N");
    }
}
