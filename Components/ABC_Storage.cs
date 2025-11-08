using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_Storage : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Resource);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out ulong newRes);

            Enabled = enabled;
            Resource = newRes;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Resource == 0;
        }

        public bool Enabled;
        public ulong Resource;
    }
}
