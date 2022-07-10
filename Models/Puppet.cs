using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.GeneratedSheets;

namespace Puppets.Models
{
    public class Puppet
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

        public bool Equals(Puppet other)
        {
            return this.ID.Equals(other.ID);
        }
    }
}
