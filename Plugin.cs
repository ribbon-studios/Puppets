using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using System;
using System.Linq;
using XivCommon;
using System.Threading.Tasks;
using Puppets.Utils;
using Dalamud.Game.ClientState;
using Newtonsoft.Json;

namespace Puppets
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Puppets";

        private const string SettingsCommand = "/puppets";
        private const string PuppetMasterCommand = "/pm";

        public ClientState ClientState { get; init; }
        public PartyList PartyList { get; init; }
        public DalamudPluginInterface PluginInterface { get; init; }
        public CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public ChatGui ChatGui { get; init; }
        public XivCommonBase Common { get; init; }
        private PluginUI PluginUi { get; init; }
        public CharacterUtils CharacterUtils { get; init; }

        public Plugin(
            ClientState clientState,
            PartyList partyList,
            DalamudPluginInterface pluginInterface,
            CommandManager commandManager,
            ChatGui chatGui
        )
        {
            this.ClientState = clientState;
            this.PartyList = partyList;
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.ChatGui = chatGui;
            this.Common = new XivCommonBase(Hooks.None);
            
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this);

            // you might normally want to embed resources and load them from the manifest stream
            this.PluginUi = new PluginUI(this);
            this.CharacterUtils = new CharacterUtils(this);

            this.CommandManager.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            this.CommandManager.AddHandler(PuppetMasterCommand, new CommandInfo(OnPuppetMasterCommand)
            {
                HelpMessage = "Use with /{emote} [delay] to sync a specific emote"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
            this.ChatGui.ChatMessage += OnChat;
        }

        private void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (
                (!Configuration.DebugMode && this.CharacterUtils.Owner != null && !sender.TextValue.Contains(this.CharacterUtils.Owner.Name.TextValue)) ||
                (Configuration.DebugMode && type != XivChatType.Echo) ||
                (!Configuration.DebugMode && type != XivChatType.Party && type != XivChatType.CrossParty) ||
                !message.TextValue.StartsWith("[PM] ")
            )
            {
                return;
            }

            string[] command = message.TextValue.Split(" ");
            string emote = command[1];

            if (Emotes.isNotValidEmote(emote)) return;

            var when = command.Length > 3 ? DateTime.Parse(string.Join(" ", command.Skip(3).ToArray())) : DateTime.Now.ToUniversalTime();

            if (Configuration.Enabled)
            {
                this.Emote(emote, when);
            }
        }

        public void Dispose()
        {
            this.Common.Dispose();
            this.PluginUi.Dispose();
            this.CommandManager.RemoveHandler(SettingsCommand);
            this.CommandManager.RemoveHandler(PuppetMasterCommand);
            this.ChatGui.ChatMessage -= OnChat;
        }

        private void OnCommand(string command, string args)
        {
            this.ToggleConfigUi();
        }

        private void OnPuppetMasterCommand(string command, string args)
        {
            string[] arguments = args.Split(" ").Where((arg) => arg != "").ToArray();
            var emote = arguments[0];

            if (Emotes.isValidEmote(emote)) 
            {
                var delay = arguments[1];
                var when = DateTime.Now.AddSeconds(double.Parse(delay)).ToUniversalTime();

                if (Configuration.DebugMode)
                {
                    this.Common.Functions.Chat.SendMessage("/echo [PM] " + emote + " @ " + when.ToString());
                }
                else
                {
                    this.Common.Functions.Chat.SendMessage("/p [PM] " + emote + " @ " + when.ToString());
                }
            } 
            else
            {
                this.PluginInterface.UiBuilder.AddNotification("The provided emote is not allowed (" + emote + ")", "Puppet Control Error", Dalamud.Interface.Internal.Notifications.NotificationType.Error);
            }

        }

        private void Emote(string emote, DateTime when)
        {
            Task.Run(async () =>
            {
                TimeSpan delay = when - DateTime.Now.ToUniversalTime();

                this.Common.Functions.Chat.SendMessage("/echo " + delay.TotalMilliseconds.ToString());

                if (delay.TotalMilliseconds > 0) await Task.Delay((int)delay.TotalMilliseconds);

                this.Common.Functions.Chat.SendMessage("/" + emote);
            });
        }

        private void ToggleConfigUi()
        {
            // in response to the slash command, just display our main ui
            this.PluginUi.Visible = !this.PluginUi.Visible;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }
    }
}
