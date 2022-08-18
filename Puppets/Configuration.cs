using Dalamud.Configuration;
using Puppets.Constants;
using System;

namespace Puppets
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool Enabled { get; set; } = true;

        public DebugMode DebugMode { private get; set; } = DebugMode.None;

        public DebugMode GetDebugMode()
        {
            #if DEBUG
            return this.DebugMode;
            #else
            return DebugMode.None;
            #endif
        }

        public void Save()
        {
            PuppetsPlugin.PluginInterface.SavePluginConfig(this);
        }
    }
}
