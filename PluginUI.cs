using Dalamud.Game.ClientState.Party;
using Dalamud.Plugin;
using ImGuiNET;
using Puppets.Models;
using Puppets.Utils;
using System;
using System.Numerics;

namespace Puppets
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
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
                var enabled = Plugin.Configuration.Enabled;
                if (ImGui.Checkbox("Enabled", ref enabled))
                {
                    Plugin.Configuration.Enabled = enabled;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    Plugin.Configuration.Save();
                }

                ImGui.Text("Current Owner: " + CharacterUtils.Owner?.ID);

                var debugMode = Plugin.Configuration.DebugMode;
                if (ImGui.Checkbox("Debug Mode", ref debugMode))
                {
                    Plugin.Configuration.DebugMode = debugMode;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    Plugin.Configuration.Save();
                }

                if (Plugin.Configuration.DebugMode)
                {
                    ImGui.Text("Current Player: " + Plugin.ClientState.LocalPlayer?.Name);
                    ImGui.Text("Party Length: " + Plugin.PartyList.Length);
                    ImGui.Text("Party Leader: " + Plugin.PartyList.PartyLeaderIndex);

                    for (var i = 0; i < Plugin.PartyList.Length; i++)
                    {
                        var partyMember = Plugin.PartyList[i];
                        if (partyMember == null) continue;

                        ImGui.Text("[" + i + "]: " + new Puppet(partyMember).ID);
                    }
                }
            }
            ImGui.End();
        }
    }
}
