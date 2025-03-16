using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace DebloaterTool
{
    public class RegistryModification
    {
        public RegistryKey Root { get; }
        public string SubKey { get; }
        public string ValueName { get; }
        public RegistryValueKind ValueKind { get; }
        public object Value { get; }

        public RegistryModification(RegistryKey root, string subKey, string valueName, RegistryValueKind valueKind, object value)
        {
            Root = root;
            SubKey = subKey;
            ValueName = valueName;
            ValueKind = valueKind;
            Value = value;
        }
    }

    internal class ComRegedit
    {
        public static void InstallRegModification(RegistryModification[] registryModifications)
        {
            Logger.Log("Applying registry changes...", Level.WARNING);
            try
            {
                // Apply each registry modification.
                foreach (RegistryModification mod in registryModifications)
                {
                    try
                    {
                        using (RegistryKey key = mod.Root.CreateSubKey(mod.SubKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            if (key != null)
                            {
                                key.SetValue(mod.ValueName, mod.Value, mod.ValueKind);
                                Logger.Log($"Updated {mod.Root}\\{mod.SubKey} -> {mod.ValueName} = {mod.Value}", Level.INFO);
                            }
                            else
                            {
                                Logger.Log($"Failed to open registry key: {mod.SubKey}", Level.ERROR);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to modify {mod.ValueName} in {mod.SubKey}: {ex.Message}", Level.ERROR);
                    }
                }
                Logger.Log("Registry changes applied successfully.", Level.SUCCESS);

                // Kill Explorer and restart it.
                ProcessStartInfo psiKill = new ProcessStartInfo("taskkill", "/F /IM explorer.exe")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psiKill)?.WaitForExit();
                Process.Start("explorer.exe");
                Logger.Log("Explorer restarted to apply registry changes.", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error applying registry changes: {ex.Message}", Level.ERROR);
            }
        }

        public static void DeleteRegistryKey(RegistryKey root, string subKeyPath)
        {
            try
            {
                root.DeleteSubKeyTree(subKeyPath, false);
                Logger.Log($"Successfully deleted registry key: {root.Name}\\{subKeyPath}", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to delete registry key: {root.Name}\\{subKeyPath} - {ex.Message}", Level.ERROR);
            }
        }
    }
}
