using System.Collections.Generic;
using Unity.Entities;

namespace AdvancedBuildingControl.Models
{
    public class CreatedEntitiesData
    {
        public Entity CreatedEntity { get; set; }
        public List<PrefabChange> PrefabChanges { get; set; }
    }
}
