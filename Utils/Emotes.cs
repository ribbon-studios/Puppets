using System.Collections.Generic;

namespace Puppets.Utils
{
    public static class Emotes
    {
        public static readonly List<string> ValidEmotes = new List<string>()
        {
            "songbird",
            "playdead",
            "cheeron",
            "cheerwave",
            "cheerjump",
            "beesknees"
        };

        public static bool isValidEmote(string emote)
        {
            var lowercaseEmote = emote.ToLower();

            return ValidEmotes.Exists((validEmote) => validEmote.ToLower().Equals(lowercaseEmote));
        }

        public static bool isNotValidEmote(string emote)
        {
            return !isValidEmote(emote);
        }
    }
}
