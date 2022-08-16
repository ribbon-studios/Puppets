using Dalamud.Game.Text;
using System;

namespace Puppets.Utils
{
    public static class ChatUtils
    {
        public static string AutoTranslateOpen = Char.ToString((char)SeIconChar.AutoTranslateOpen);
        public static string AutoTranslateClose = Char.ToString((char)SeIconChar.AutoTranslateClose);

        public static string Cleanup(string message)
        {
            return message.Replace(AutoTranslateOpen, String.Empty).Replace(AutoTranslateClose, String.Empty).Trim();
        }

        public static string ReplaceCharacterAtStart(string message, string character)
        {
            if (message.StartsWith(character))
            {
                return message.Remove(0, 1);
            }

            return message;
        }
    }
}
