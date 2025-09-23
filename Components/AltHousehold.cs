using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct AltHousehold : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Household);
            writer.Write(Enabled);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out int household);
            reader.Read(out bool enabled);

            Household = household;
            Enabled = enabled;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Household == 0;
        }

        public int Household;
        public bool Enabled;
    }
}
