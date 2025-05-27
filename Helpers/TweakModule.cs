using System;

namespace DebloaterTool.Helpers
{
    public class TweakModule
    {
        public Action Action { get; }
        public string Description { get; }
        public bool DefaultEnabled { get; }

        public TweakModule(Action action, string description, bool defaultEnabled)
        {
            Action = action;
            Description = description;
            DefaultEnabled = defaultEnabled;
        }
    }
}
