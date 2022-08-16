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
using Dalamud.IoC;
using Dalamud.Data;
using Dalamud.Game;

namespace Puppets
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Puppets";

        private const string SettingsCommand = "/puppets";
        private const string PuppetMasterCommand = "/pm";

        [PluginService] public static DataManager Data { get; private set; }
        [PluginService] public static ClientState ClientState { get; private set; }
        [PluginService] public static PartyList PartyList { get; private set; }
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; }
        [PluginService] public static CommandManager CommandManager { get; private set; }
        [PluginService] public static ChatGui ChatGui { get; private set; }
        [PluginService] public static SigScanner TargetScanner { get; private set; }

        private static Configuration? _configuration;
        public static Configuration Configuration { 
            get
            {
                if (Plugin._configuration == null)
                {
                    Plugin._configuration = Plugin.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                }

                return Plugin._configuration;
            } 
        }
        public XivCommonBase Common { get; init; }
        private PluginUI PluginUi { get; init; }

        public Plugin()
        {
            this.Common = new XivCommonBase(Hooks.None);

            // you might normally want to embed resources and load them from the manifest stream
            this.PluginUi = new PluginUI();

            Plugin.CommandManager.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Plugin.CommandManager.AddHandler(PuppetMasterCommand, new CommandInfo(OnPuppetMasterCommand)
            {
                HelpMessage = "Use with /{emote} [delay] to sync a specific emote"
            });

            Plugin.PluginInterface.UiBuilder.Draw += DrawUI;
            Plugin.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
            Plugin.ChatGui.ChatMessage += OnChat;
        }

        private void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (
                (!Configuration.DebugMode && CharacterUtils.Owner != null && !sender.TextValue.Contains(CharacterUtils.Owner.Name.TextValue)) ||
                (Configuration.DebugMode && type != XivChatType.Echo) ||
                (!Configuration.DebugMode && type != XivChatType.Party && type != XivChatType.CrossParty) ||
                !message.TextValue.StartsWith("[PM] ")
            )
            {
                return;
            }

            string[] command = message.TextValue.Split(" ");
            string emote = command[1];

            if (Emotes.isNotUnlockedEmote(emote)) return;

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
            Plugin.CommandManager.RemoveHandler(SettingsCommand);
            Plugin.CommandManager.RemoveHandler(PuppetMasterCommand);
            Plugin.ChatGui.ChatMessage -= OnChat;
        }

        private void OnCommand(string command, string args)
        {
            this.ToggleConfigUi();
        }

        private void OnPuppetMasterCommand(string command, string args)
        {
            string[] arguments = ChatUtils.ReplaceCharacterAtStart(ChatUtils.Cleanup(args), "/").Split(" ").Where((arg) => arg != "").ToArray();
            var emote = arguments[0];

            if (CharacterUtils.IsNotOwner)
            {
                Plugin.PluginInterface.UiBuilder.AddNotification("Only a puppet master may execute emotes!", "Puppets", Dalamud.Interface.Internal.Notifications.NotificationType.Warning);
            }
            else if (Emotes.isInvalidEmote(emote))
            {
                Plugin.PluginInterface.UiBuilder.AddNotification("The provided emote does not exist (" + emote + ")", "Puppets", Dalamud.Interface.Internal.Notifications.NotificationType.Error);
            }
            else if (Emotes.isNotUnlockedEmote(emote))
            {
                Plugin.PluginInterface.UiBuilder.AddNotification("The provided emote is not unlocked (" + emote + ")", "Puppets", Dalamud.Interface.Internal.Notifications.NotificationType.Warning);
            }
            else if (!Configuration.DebugMode && CharacterUtils.NotInParty)
            {
                Plugin.PluginInterface.UiBuilder.AddNotification("You must be in a party to use puppets!", "Puppets", Dalamud.Interface.Internal.Notifications.NotificationType.Warning);
            }
            else
            {
                var delay = arguments.Length >= 2 ? arguments[1] : "1";
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
