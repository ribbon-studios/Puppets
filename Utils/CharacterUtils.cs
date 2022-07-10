using Puppets.Models;
using System;

namespace Puppets.Utils
{
    public class CharacterUtils
    {
        public Puppet? Player
        {
            get
            {
                return this.plugin.ClientState.LocalPlayer == null ? null : new Puppet(this.plugin.ClientState.LocalPlayer);
            }
        }

        public Puppet? Owner
        {
            get
            {
                if (!this.plugin.Configuration.Enabled) return null;

                var partyLeader = this.plugin.PartyList[Convert.ToInt32(this.plugin.PartyList.PartyLeaderIndex)];

                if (partyLeader == null)
                {
                    return this.Player;
                }

                return new Puppet(partyLeader);
            }
        }

        private Plugin plugin;

        public CharacterUtils(Plugin plugin)
        {
            this.plugin = plugin;
        }
    }
}
