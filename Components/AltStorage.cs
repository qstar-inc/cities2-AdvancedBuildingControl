using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct AltStorage : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Resourse);
            writer.Write(Enabled);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out ulong newRes);
            reader.Read(out bool enabled);

            Resourse = newRes;
            Enabled = enabled;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Resourse == 0;
        }

        public ulong Resourse;
        public bool Enabled;
    }
}
