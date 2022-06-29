using Dalamud.Game.ClientState.Party;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Numerics;

namespace SamplePlugin
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;
        private DalamudPluginInterface pluginInterface;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(
            Configuration configuration, 
            DalamudPluginInterface pluginInterface
        ) {
            this.configuration = configuration;
            this.pluginInterface = pluginInterface;
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

            ImGui.SetNextWindowSize(new Vector2(232, 110), ImGuiCond.Always);
            if (ImGui.Begin("Puppets Config", ref this.visible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                // can't ref a property, so use a local copy
                var enabled = this.configuration.Enabled;
                if (ImGui.Checkbox("Enabled", ref enabled))
                {
                    this.configuration.Enabled = enabled;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.configuration.Save();
                }

                ImGui.Text("Current Owner: " + this.configuration.Owner);

                var debugMode = this.configuration.DebugMode;
                if (ImGui.Checkbox("Debug Mode", ref debugMode))
                {
                    this.configuration.DebugMode = debugMode;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.configuration.Save();
                }
            }
            ImGui.End();
        }
    }
}
