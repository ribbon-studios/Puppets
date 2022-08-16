using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;
using System;

namespace Puppets.Models
{
    public class Puppet : IEquatable<Puppet>
    {
        public string ID
        {
            get
            {
                var id = this.Name.TextValue;

                if (this.HomeWorld.GameData != null)
                {
                    id += " (" + this.HomeWorld.GameData.Name + ")";
                }

                return id;
            }
        }

        public SeString Name { get; init; }
        public ExcelResolver<World> HomeWorld { get; init; }

        public Puppet(PlayerCharacter character)
        {
            this.Name = character.Name;
            this.HomeWorld = character.HomeWorld;
        }

        public Puppet(PartyMember partyMember)
        {
            this.Name = partyMember.Name;
            this.HomeWorld = partyMember.World;
        }

        public Puppet(string sender)
        {
            this.Name = sender;
        }

        public bool Equals(Puppet? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.HomeWorld.Id == other.HomeWorld.Id &&
                this.Name.TextValue == other.Name.TextValue;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Puppet) obj);
        }
    }
}
