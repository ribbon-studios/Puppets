using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Data;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Logging;
using System.Runtime.InteropServices;

namespace Puppets.Utils
{
    public static unsafe class Emotes
    {
        // E8 ?? ?? ?? ?? 84 C0 74 A4
        private delegate byte IsEmoteUnlockedDelegate(UIState* uiState, uint emoteId, byte unk);

        private static IsEmoteUnlockedDelegate? __isEmoteUnlocked;
        private static IsEmoteUnlockedDelegate _isEmoteUnlocked
        {
            get
            {
                if (Emotes.__isEmoteUnlocked == null)
                {
                    if (Plugin.TargetScanner.TryScanText("E8 ?? ?? ?? ?? 84 C0 74 A4", out var emoteUnlockedPtr))
                    {
                        PluginLog.Information($"emoteUnlockedPtr: {emoteUnlockedPtr:X}");
                        Emotes.__isEmoteUnlocked = Marshal.GetDelegateForFunctionPointer<IsEmoteUnlockedDelegate>(emoteUnlockedPtr);
                    }
                }

                return Emotes.__isEmoteUnlocked;
            }
        }

        private static List<string> UnlockedEmotes
        {
            get
            {
                var emotes = new List<string>();

                foreach (var emote in Plugin.Data.GetExcelSheet<Emote>()!.Where(x => x.Order != 0 && Emotes.isEmoteUnlocked(x)))
                {
                    emotes.Add(emote.TextCommand.Value!.Command.RawString.Replace("/", ""));
                }

                return emotes;
            }
        }

        public static bool isUnlockedEmote(string emote)
        {
            return UnlockedEmotes.Contains(emote.ToLower());
        }

        public static bool isNotUnlockedEmote(string emote)
        {
            return !isUnlockedEmote(emote);
        }

        private static bool isEmoteUnlocked(Emote emote)
        {
            return emote.UnlockLink == 0 || Emotes._isEmoteUnlocked(UIState.Instance(), emote.UnlockLink, 1) > 0;
        }
    }
}
