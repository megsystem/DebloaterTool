using Microsoft.Win32;
using System;

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

    internal class HelperRegedit
    {
        public static void InstallRegModification(RegistryModification[] registryModifications)
        {
            if (registryModifications == null || registryModifications.Length == 0)
            {
                Logger.Log("No registry modifications to apply.", Level.WARNING);
                return;
            }

            Logger.Log("Applying registry changes...", Level.WARNING);

            foreach (var mod in registryModifications)
            {
                ApplyModification(mod);
            }

            Logger.Log("Registry changes applied successfully.", Level.SUCCESS);
        }

        public static void InstallRegModification(RegistryModification mod)
        {
            // Delegate single modification call to the array-based method.
            InstallRegModification(new[] { mod });
        }

        private static void ApplyModification(RegistryModification mod)
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