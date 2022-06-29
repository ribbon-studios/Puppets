using Dalamud.Configuration;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Plugin;
using System;

namespace SamplePlugin
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool Enabled { get; set; } = true;

        public bool DebugMode { get; set; } = false;

        public string Owner
        {
            get
            {
                if (this.Enabled)
                {
                    if (this.plugin.PartyList == null)
                    {
                        return "";
                    }

                    return this.plugin.PartyList[Convert.ToInt32(this.plugin.PartyList.PartyLeaderIndex)].Name.TextValue;
                }

                return "N/A";
            }
        }

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
