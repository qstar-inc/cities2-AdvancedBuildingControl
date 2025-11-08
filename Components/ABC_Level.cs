using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_Level : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Level);
            writer.Write(Original);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int level);
            reader.Read(out int original);

            Enabled = enabled;
            Level = level;
            Original = original;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Level == 0;
        }

        public bool Enabled;
        public int Level;
        public int Original;
    }
}
