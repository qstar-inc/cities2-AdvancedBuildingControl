using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_Household : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Household);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int household);

            Enabled = enabled;
            Household = household;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Household == 0;
        }

        public bool Enabled;
        public int Household;
    }
}
