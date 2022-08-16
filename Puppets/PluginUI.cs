using ImGuiNET;
using Puppets.Constants;
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
                var enabled = PuppetsPlugin.Configuration.Enabled;
                if (ImGui.Checkbox("Enabled", ref enabled))
                {
                    PuppetsPlugin.Configuration.Enabled = enabled;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    PuppetsPlugin.Configuration.Save();
                }

                ImGui.Text("Current Owner: " + CharacterUtils.Owner?.ID);

                var modes = System.Enum.GetNames(typeof(DebugMode));
                var debugMode = (int)PuppetsPlugin.Configuration.DebugMode;
                if (ImGui.Combo("Debug Mode", ref debugMode, modes, modes.Length))
                {
                    PuppetsPlugin.Configuration.DebugMode = (DebugMode)debugMode;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    PuppetsPlugin.Configuration.Save();
                }

                if (PuppetsPlugin.Configuration.DebugMode is not DebugMode.None)
                {
                    ImGui.Text("Current Player: " + PuppetsPlugin.ClientState.LocalPlayer?.Name);
                    ImGui.Text("Party Length: " + PuppetsPlugin.PartyList.Length);
                    ImGui.Text("Party Leader: " + PuppetsPlugin.PartyList.PartyLeaderIndex);

                    for (var i = 0; i < PuppetsPlugin.PartyList.Length; i++)
                    {
                        var partyMember = PuppetsPlugin.PartyList[i];
                        if (partyMember == null) continue;

                        ImGui.Text("[" + i + "]: " + new Puppet(partyMember).ID);
                    }
                }
            }
            ImGui.End();
        }
    }
}
