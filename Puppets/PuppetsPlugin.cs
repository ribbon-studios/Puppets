using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Puppets.Constants;
using Puppets.Models;
using Puppets.SeFunctions;
using Puppets.Time;
using Puppets.Utils;
using System.Linq;
using System.Threading.Tasks;
using XivCommon;

namespace Puppets
{
    public sealed class PuppetsPlugin : IDalamudPlugin
    {
        public string Name => "Puppets";

        private const string SettingsCommand = "/puppets";
        private const string PuppetMasterCommand = "/pm";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [PluginService] public static IDataManager Data { get; private set; }
        [PluginService] public static IClientState ClientState { get; private set; } 
        [PluginService] public static IPartyList PartyList { get; private set; }
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; }
        [PluginService] public static ICommandManager CommandManager { get; private set; }
        [PluginService] public static IChatGui ChatGui { get; private set; }
        [PluginService] public static SigScanner TargetScanner { get; private set; }
        [PluginService] public static IPluginLog PluginLog { get; private set; }
        [PluginService] public static INotificationManager NotificationManager { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private static Configuration? _configuration;
        public static Configuration Configuration
        {
            get
            {
                if (PuppetsPlugin._configuration == null)
                {
                    PuppetsPlugin._configuration = PuppetsPlugin.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                }

                return PuppetsPlugin._configuration;
            }
        }
        public XivCommonBase Common { get; init; }
        private PluginUI PluginUi { get; init; }

        public PuppetsPlugin()
        {
            this.Common = new XivCommonBase(PluginInterface, Hooks.None);

            // you might normally want to embed resources and load them from the manifest stream
            this.PluginUi = new PluginUI();

            PuppetsPlugin.CommandManager.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            PuppetsPlugin.CommandManager.AddHandler(PuppetMasterCommand, new CommandInfo(OnPuppetMasterCommand)
            {
                HelpMessage = "Use with /{emote} [delay] to sync a specific emote"
            });

            PuppetsPlugin.PluginInterface.UiBuilder.Draw += DrawUI;
            PuppetsPlugin.PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
            PuppetsPlugin.ChatGui.ChatMessage += OnChat;
        }

        private void OnChat(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            var senderPuppet = new Puppet(ChatUtils.CleanupSender(sender.TextValue));

            if (
                (Configuration.GetDebugMode() is not DebugMode.Echo and not DebugMode.EchoNoEmote && !senderPuppet.Equals(CharacterUtils.Owner)) ||
                (Configuration.GetDebugMode() is DebugMode.Echo or DebugMode.EchoNoEmote && type != XivChatType.Echo) ||
                (Configuration.GetDebugMode() is not DebugMode.Echo and not DebugMode.EchoNoEmote && type != XivChatType.Party && type != XivChatType.CrossParty) ||
                !message.TextValue.StartsWith("[PM] ")
            )
            {
                return;
            }

            string[] command = message.TextValue.Split(" ");
            string emote = command[1];

            if (Emotes.isNotUnlockedEmote(emote)) return;

            var dateString = string.Join(" ", command.Skip(3).ToArray());
            var when = command.Length > 3 ? new(dateString) : SeTime.GetServerTime();

            if (Configuration.Enabled)
            {
                this.Emote(emote, when);
            }
        }

        public void Dispose()
        {
            this.Common.Dispose();
            this.PluginUi.Dispose();
            PuppetsPlugin.CommandManager.RemoveHandler(SettingsCommand);
            PuppetsPlugin.CommandManager.RemoveHandler(PuppetMasterCommand);
            PuppetsPlugin.ChatGui.ChatMessage -= OnChat;
        }

        private void OnCommand(string command, string args)
        {
            this.ToggleConfigUi();
        }

        private void OnPuppetMasterCommand(string command, string args)
        {
            string[] arguments = ChatUtils.CleanupCommand(args).Split(" ").Where((arg) => arg != "").ToArray();
            var emote = arguments[0];

            if (Configuration.GetDebugMode() is not DebugMode.Echo and not DebugMode.EchoNoEmote && CharacterUtils.IsNotOwner)
            {
                NotificationManager.AddNotification(new Notification {
                    Title = "Puppets",
                    Content = "Only a puppet master may execute emotes!",
                    Type = NotificationType.Warning
                });
            }
            else if (Emotes.isInvalidEmote(emote))
            {
                NotificationManager.AddNotification(new Notification {
                    Title = "Puppets",
                    Content = $"The provided emote does not exist ({emote})",
                    Type = NotificationType.Error
                });
            }
            else if (Emotes.isNotUnlockedEmote(emote))
            {
                NotificationManager.AddNotification(new Notification {
                    Title = "Puppets",
                    Content = $"The provided emote is not unlocked ({emote})",
                    Type = NotificationType.Warning
                });
            }
            else if (Configuration.GetDebugMode() is not DebugMode.Echo and not DebugMode.EchoNoEmote && CharacterUtils.NotInParty)
            {
                NotificationManager.AddNotification(new Notification {
                    Title = "Puppets",
                    Content = "You must be in a party to use puppets!",
                    Type = NotificationType.Warning
                });
            }
            else
            {
                var delay = arguments.Length >= 2 ? arguments[1] : "1";
                var when = SeTime.GetServerTime().AddMilliseconds((long) double.Parse(delay) * 1000);
                var message = $"[PM] {emote} @ {when}";

                if (Configuration.GetDebugMode() is DebugMode.Echo or DebugMode.EchoNoEmote)
                {
                    this.Common.Functions.Chat.SendMessage($"/e {message}");
                }
                else
                {
                    this.Common.Functions.Chat.SendMessage($"/p {message}");
                }
            }

        }

        private void Emote(string emote, TimeStamp when)
        {
            Task.Run(async () =>
            {
                long delay = when - SeTime.GetServerTime();

                #if DEBUG
                #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await Task.Delay(1000);

                    this.Common.Functions.Chat.SendMessage($"/e '{when}': '{delay}'");
                });
                #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                #endif

                if (delay > 0) await Task.Delay((int)delay);

                if (Configuration.GetDebugMode() is DebugMode.None or DebugMode.Echo)
                {
                    this.Common.Functions.Chat.SendMessage("/" + emote);
                }
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
