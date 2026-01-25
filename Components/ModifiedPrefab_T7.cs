using AdvancedBuildingControl.Variables;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace AdvancedBuildingControl.Components
{
    [InternalBufferCapacity(0)]
    public struct ModifiedPrefab_T7 : IBufferElementData, ISerializable
    {
        private const int CURRENT_VERSION = 1;

        public int Version;
        public byte Enabled;
        public long Modified;
        public long Original;
        public Entity ModEntity;
        public UpdateValueType ValueType;

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(Enabled);
            writer.Write(Modified);
            writer.Write(Original);
            writer.Write(ModEntity);
            writer.Write((ushort)ValueType);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out Version);
            reader.Read(out Enabled);
            reader.Read(out Modified);
            reader.Read(out Original);
            reader.Read(out ModEntity);
            reader.Read(out ushort valueType);
            ValueType = (UpdateValueType)valueType;
        }

        public readonly bool Equals(ModifiedPrefab_T7 other)
        {
            if (ModEntity != other.ModEntity)
                return false;
            if (Modified != other.Modified)
                return false;
            if (Original != other.Original)
                return false;

            return true;
        }

        public bool IsEnabled
        {
            get => Enabled != 0;
            set => Enabled = (byte)(value ? 1 : 0);
        }
    }
}
