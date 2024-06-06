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
            PuppetsPlugin.PluginLog.Information($"requested emote: {emote.Command}");
            return emote.UnlockLink == 0 || UIState.Instance() -> IsUnlockLinkUnlockedOrQuestCompleted(emote.UnlockLink);
        }
    }
}
