using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_Workplace : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Workplace);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out int workplace);

            Enabled = enabled;
            Workplace = workplace;
        }

        public readonly bool IsDefault()
        {
            return Enabled == false && Workplace == 0;
        }

        public bool Enabled;
        public int Workplace;
    }
}
