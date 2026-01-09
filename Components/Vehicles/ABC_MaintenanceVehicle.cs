using AdvancedBuildingControl.Interface;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components.Vehicles
{
    public struct ABC_MaintenanceVehicle
        : IABC_Component_Int,
            IComponentData,
            IQueryTypeParameter,
            ISerializable
    {
        public bool Enabled { get; set; }
        public int Modified { get; set; }
        public int Original { get; set; }

        public readonly bool IsDefault() => Enabled == false && Modified == 0;

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Modified);
            writer.Write(Original);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int modified);
            reader.Read(out int ori);

            Enabled = enabled;
            Modified = modified;
            Original = ori;
        }
    }
}
