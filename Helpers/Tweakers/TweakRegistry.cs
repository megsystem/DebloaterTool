using Microsoft.Win32;

namespace DebloaterTool.Helpers
{
    public class TweakRegistry
    {
        public RegistryKey Root { get; }
        public string SubKey { get; }
        public string ValueName { get; }
        public RegistryValueKind ValueKind { get; }
        public object Value { get; }

        public TweakRegistry(RegistryKey root, string subKey, string valueName, RegistryValueKind valueKind, object value)
        {
            Root = root;
            SubKey = subKey;
            ValueName = valueName;
            ValueKind = valueKind;
            Value = value;
        }
    }
}
