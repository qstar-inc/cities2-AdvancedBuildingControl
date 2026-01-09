using AdvancedBuildingControl.Interface;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    public struct ABC_SewagePurification
        : IABC_Component_Float,
            IComponentData,
            IQueryTypeParameter,
            ISerializable
    {
        public bool Enabled { get; set; }
        public float Modified { get; set; }
        public float Original { get; set; }

        public readonly bool IsDefault() => Enabled == false && Modified == 0;

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Enabled);
            writer.Write(Modified);
            writer.Write(Original);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out bool enabled);
            reader.Read(out float modified);
            reader.Read(out float ori);

            Enabled = enabled;
            Modified = modified;
            Original = ori;
        }
    }
}
