using Dalamud.Configuration;
using System;

namespace Puppets
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool Enabled { get; set; } = true;

        public bool DebugMode { get; set; } = false;

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private Plugin plugin;

        public void Initialize(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void Save()
        {
            this.plugin.PluginInterface.SavePluginConfig(this);
        }
    }
}
