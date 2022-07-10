using Dalamud.Game.ClientState.Party;
using Dalamud.Plugin;
using ImGuiNET;
using Puppets.Models;
using System;
using System.Numerics;

namespace Puppets
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Plugin plugin;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(
            Plugin plugin
        ) {
            this.plugin = plugin;
        }

        public void Dispose()
        {

        }

        public void Draw()
        {
            DrawMainWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(232, 110), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Puppets Config", ref this.visible, ImGuiWindowFlags.NoCollapse))
            {
                // can't ref a property, so use a local copy
                var enabled = this.plugin.Configuration.Enabled;
                if (ImGui.Checkbox("Enabled", ref enabled))
                {
                    this.plugin.Configuration.Enabled = enabled;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.plugin.Configuration.Save();
                }

                ImGui.Text("Current Owner: " + this.plugin.CharacterUtils.Owner?.ID);

                var debugMode = this.plugin.Configuration.DebugMode;
                if (ImGui.Checkbox("Debug Mode", ref debugMode))
                {
                    this.plugin.Configuration.DebugMode = debugMode;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.plugin.Configuration.Save();
                }

                if (this.plugin.Configuration.DebugMode)
                {
                    ImGui.Text("Current Player: " + this.plugin.ClientState.LocalPlayer?.Name);
                    ImGui.Text("Party Length: " + this.plugin.PartyList.Length);
                    ImGui.Text("Party Leader: " + this.plugin.PartyList.PartyLeaderIndex);

                    for (var i = 0; i < this.plugin.PartyList.Length; i++)
                    {
                        var partyMember = this.plugin.PartyList[i];
                        if (partyMember == null) continue;

                        ImGui.Text("[" + i + "]: " + new Puppet(partyMember).ID);
                    }
                }
            }
            ImGui.End();
        }
    }
}
