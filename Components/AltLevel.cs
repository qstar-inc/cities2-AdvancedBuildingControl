using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct AltLevel : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Level);
            writer.Write(Enabled);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out int level);
            reader.Read(out bool enabled);

            Level = level;
            Enabled = enabled;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Level == 0;
        }

        public int Level;
        public bool Enabled;
    }
}
