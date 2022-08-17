using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using System;

namespace Puppets.Models
{
    public class Puppet : IEquatable<Puppet>
    {
        public string ID
        {
            get
            {
                var id = this.Name;

                if (this.HomeWorld != null)
                {
                    id += " (" + this.HomeWorld + ")";
                }

                return id;
            }
        }

        public string Name { get; init; }
        public string? HomeWorld { get; init; }

        public Puppet(PlayerCharacter character)
        {
            this.Name = character.Name.TextValue;
            this.HomeWorld = character.HomeWorld.GameData?.Name.ToString();
        }

        public Puppet(PartyMember partyMember)
        {
            this.Name = partyMember.Name.TextValue;
            this.HomeWorld = partyMember.World?.GameData?.Name.ToString();
        }

        public Puppet(string sender)
        {
            this.Name = sender;
            this.HomeWorld = PuppetsPlugin.ClientState.LocalPlayer?.CurrentWorld.GameData?.Name.ToString()
                             ?? PuppetsPlugin.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString();
        }

        public bool Equals(Puppet? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.HomeWorld == other.HomeWorld &&
                this.Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Puppet)obj);
        }

        public override int GetHashCode() => (this.HomeWorld, Name).GetHashCode();
    }
}
