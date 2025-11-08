using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_TransportDepot : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Capacity);
            writer.Write(Original);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int cap);
            reader.Read(out int original);

            Enabled = enabled;
            Capacity = cap;
            Original = original;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Capacity == 0;
        }

        public bool Enabled;
        public int Capacity;
        public int Original;
    }
}
