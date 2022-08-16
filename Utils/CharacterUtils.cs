using Dalamud.Logging;
using Puppets.Models;
using System;

namespace Puppets.Utils
{
    public class CharacterUtils
    {
        public static Puppet? Player
        {
            get
            {
                return Plugin.ClientState.LocalPlayer == null ? null : new Puppet(Plugin.ClientState.LocalPlayer);
            }
        }

        public static Puppet? Owner
        {
            get
            {
                if (!Plugin.Configuration.Enabled) return null;

                var partyLeader = Plugin.PartyList.Length == 0 ? null : Plugin.PartyList[Convert.ToInt32(Plugin.PartyList.PartyLeaderIndex)];

                if (partyLeader == null)
                {
                    return CharacterUtils.Player;
                }

                return new Puppet(partyLeader);
            }
        }

        public static bool InParty => Plugin.PartyList.Length > 0;
        public static bool NotInParty => !CharacterUtils.InParty;
        public static bool IsOwner => CharacterUtils.Player.Equals(CharacterUtils.Owner);
        public static bool IsNotOwner => !CharacterUtils.IsOwner;
    }
}
