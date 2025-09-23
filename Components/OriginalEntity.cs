using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct OriginalEntity : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(OGEntity.ToString());
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out string oGEntity);

            OGEntity = oGEntity;
        }

        public readonly bool CompareTo(OriginalEntity other)
        {
            return OGEntity == other.OGEntity;
        }

        public FixedString128Bytes OGEntity;
    }
}
