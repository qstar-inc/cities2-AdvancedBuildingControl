using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Entities;

namespace AdvancedBuildingControl.Components.Effects
{
    [InternalBufferCapacity(0)]
    public struct ModifiedLocalEffect : IBufferElementData, ISerializable
    {
        private const int CURRENT_VERSION = 1;

        public int Version;
        public byte Enabled;
        public Entity ModEntity;
        public LocalModifierData LocalModifier;

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(Enabled);
            writer.Write(ModEntity);
            writer.Write(LocalModifier);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out Version);
            reader.Read(out Enabled);
            reader.Read(out ModEntity);
            reader.Read(out LocalModifier);
        }

        public readonly bool Equals(ModifiedLocalEffect other)
        {
            if (ModEntity != other.ModEntity)
                return false;
            if (LocalModifier.m_Type != other.LocalModifier.m_Type)
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
