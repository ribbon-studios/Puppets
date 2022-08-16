using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Logging;
using System.Runtime.InteropServices;
using Puppets.Models;

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

        private static List<SearchableEmote>? _emotes;
        private static List<SearchableEmote> ValidEmotes
        {
            get
            {
                if (Emotes._emotes == null)
                {
                    Emotes._emotes = new List<SearchableEmote>();

                    foreach (var emote in Plugin.Data.GetExcelSheet<Emote>()!.Where(x => x.Order != 0))
                    {
                        Emotes._emotes.Add(new SearchableEmote(emote));
                    }
                }

                return _emotes;
            }
        }

        private static List<SearchableEmote> UnlockedEmotes => Emotes.ValidEmotes.Where(x => Emotes.isEmoteUnlocked(x)).ToList();

        public static bool isValidEmote(string emote)
        {
            var lowercaseEmote = emote.ToLower();

            return Emotes.ValidEmotes.Find(searchableEmote => searchableEmote.Equals(lowercaseEmote)) != null;
        }

        public static bool isInvalidEmote(string emote)
        {
            return !Emotes.isValidEmote(emote);
        }

        public static bool isUnlockedEmote(string emote)
        {
            var lowercaseEmote = emote.ToLower();

            return Emotes.UnlockedEmotes.Find(searchableEmote => searchableEmote.Equals(lowercaseEmote)) != null;
        }

        public static bool isNotUnlockedEmote(string emote)
        {
            return !Emotes.isUnlockedEmote(emote);
        }

        private static bool isEmoteUnlocked(SearchableEmote emote)
        {
            return emote.UnlockLink == 0 || Emotes._isEmoteUnlocked(UIState.Instance(), emote.UnlockLink, 1) > 0;
        }
    }
}
