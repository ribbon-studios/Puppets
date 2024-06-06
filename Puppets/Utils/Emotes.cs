using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Puppets.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Puppets.Utils
{
    public static unsafe class Emotes
    {
        // E8 ?? ?? ?? ?? 84 C0 74 A4
        private delegate byte IsEmoteUnlockedDelegate(UIState* uiState, uint emoteId, byte unk);

        private static IsEmoteUnlockedDelegate? __isEmoteUnlocked;
        private static IsEmoteUnlockedDelegate? _isEmoteUnlocked
        {
            get
            {
                if (Emotes.__isEmoteUnlocked == null)
                {
                    if (PuppetsPlugin.TargetScanner.TryScanText("E8 ?? ?? ?? ?? 84 C0 74 A4", out var emoteUnlockedPtr))
                    {
                        PuppetsPlugin.PluginLog.Information($"emoteUnlockedPtr: {emoteUnlockedPtr:X}");
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

                    foreach (var emote in PuppetsPlugin.Data.GetExcelSheet<Emote>()!.Where(x => x.Order != 0))
                    {
                        Emotes._emotes.Add(new SearchableEmote(emote));
                    }
                }

                return Emotes._emotes;
            }
        }

        public static SearchableEmote getEmote(string emote)
        {
            string lowercaseEmote = emote.ToLower();

            return Emotes.ValidEmotes.Find(searchableEmote => searchableEmote.Equals(lowercaseEmote));
        }

        public static bool isValidEmote(string emoteName)
        {
            return Emotes.getEmote(emoteName) != null;
        }

        public static bool isInvalidEmote(string emoteName)
        {
            return !Emotes.isValidEmote(emoteName);
        }

        public static bool isUnlockedEmote(string emoteName)
        {
            SearchableEmote emote = Emotes.getEmote(emoteName);

            return emote != null && Emotes.isEmoteUnlocked(emote);
        }

        public static bool isNotUnlockedEmote(string emote)
        {
            return !Emotes.isUnlockedEmote(emote);
        }

        private static bool isEmoteUnlocked(SearchableEmote emote)
        {
            PluginLog.Information($"requested emote: {emote.Command}");
            return emote.UnlockLink == 0 || Emotes._isEmoteUnlocked != null && Emotes._isEmoteUnlocked(UIState.Instance(), emote.UnlockLink, 1) > 0;
        }
    }
}
