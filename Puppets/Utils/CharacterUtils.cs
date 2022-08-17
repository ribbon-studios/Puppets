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
                return PuppetsPlugin.ClientState.LocalPlayer == null ? null : new Puppet(PuppetsPlugin.ClientState.LocalPlayer);
            }
        }

        public static Puppet? Owner
        {
            get
            {
                if (!PuppetsPlugin.Configuration.Enabled) return null;

                var partyLeader = PuppetsPlugin.PartyList.Length == 0 ? null : PuppetsPlugin.PartyList[Convert.ToInt32(PuppetsPlugin.PartyList.PartyLeaderIndex)];

                if (partyLeader == null)
                {
                    return CharacterUtils.Player;
                }

                return new Puppet(partyLeader);
            }
        }

        public static bool InParty => PuppetsPlugin.PartyList.Length > 0;
        public static bool NotInParty => !CharacterUtils.InParty;
        public static bool IsOwner => CharacterUtils.Player != null && CharacterUtils.Player.Equals(CharacterUtils.Owner);
        public static bool IsNotOwner => !CharacterUtils.IsOwner;
    }
}
