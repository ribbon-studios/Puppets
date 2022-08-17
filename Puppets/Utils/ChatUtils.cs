using Dalamud.Game.Text;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Puppets.Utils
{
    public static class ChatUtils
    {
        public static string AutoTranslateOpen = Char.ToString((char)SeIconChar.AutoTranslateOpen);
        public static string AutoTranslateClose = Char.ToString((char)SeIconChar.AutoTranslateClose);
        public static string Number1 = Char.ToString((char)SeIconChar.BoxedNumber1);
        public static string Number2 = Char.ToString((char)SeIconChar.BoxedNumber2);
        public static string Number3 = Char.ToString((char)SeIconChar.BoxedNumber3);
        public static string Number4 = Char.ToString((char)SeIconChar.BoxedNumber4);
        public static string Number5 = Char.ToString((char)SeIconChar.BoxedNumber5);
        public static string Number6 = Char.ToString((char)SeIconChar.BoxedNumber6);
        public static string Number7 = Char.ToString((char)SeIconChar.BoxedNumber7);
        public static string Number8 = Char.ToString((char)SeIconChar.BoxedNumber8);

        public static string Star = "\u2605";
        public static string Circle = "\u25CF";
        public static string Triangle = "\u25B2";
        public static string Diamond = "\u2666";
        public static string Heart = "\u2665";
        public static string Ace = "\u2660";
        public static string Spade = "\u2663";

        public static string[] PartyListNumbers = { Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8 };
        public static string[] GroupIndicators = { Star, Circle, Triangle, Diamond, Heart, Ace, Spade };

        public static string CleanupMessage(string message)
        {
            return message.Replace(AutoTranslateOpen, String.Empty).Replace(AutoTranslateClose, String.Empty).Trim();
        }

        public static string CleanupCommand(string command)
        {
            return ChatUtils.ReplaceGroup(ChatUtils.CleanupMessage(command), "/");
        }

        public static string CleanupSender(string sender)
        {
            return ReplaceGroups(
                sender,
                PartyListNumbers,
                GroupIndicators
            ).Trim();
        }

        private static string ReplaceGroups(string message, params string[][] characterGroups)
        {
            var finalMessage = message;

            foreach (var characters in characterGroups)
            {
                finalMessage = ReplaceGroup(finalMessage, characters);
            }

            return finalMessage;
        }

        private static string ReplaceGroup(string message, params string[] characters)
        {
            if (Regex.Match(message, $@"(?:{string.Join('|', characters)})").Success)
            {
                return message.Remove(0, 1);
            }

            return message;
        }
    }
}
