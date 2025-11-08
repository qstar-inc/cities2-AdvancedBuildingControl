using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_SewageDump : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Capacity);
            writer.Write(OriginalCap);
            writer.Write(Purification);
            writer.Write(OriginalPurification);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int capacity);
            reader.Read(out int original);
            reader.Read(out int purification);
            reader.Read(out int originalPurification);

            Enabled = enabled;
            Capacity = capacity;
            OriginalCap = original;
            Purification = purification;
            OriginalPurification = originalPurification;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Capacity == 0;
        }

        public int Capacity;
        public int OriginalCap;
        public float Purification;
        public float OriginalPurification;
        public bool Enabled;
    }
}
