using Colossal.Serialization.Entities;
using Game.City;
using Game.Prefabs;
using Unity.Entities;

namespace AdvancedBuildingControl.Components.Effects
{
    [InternalBufferCapacity(0)]
    public struct ModifiedCityEffect : IBufferElementData, ISerializable
    {
        private const int CURRENT_VERSION = 1;

        public int Version;
        public byte Enabled;
        public Entity ModEntity;
        public CityModifierData CityModifier;

        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(Enabled);
            writer.Write(ModEntity);
            writer.Write((int)CityModifier.m_Type);
            writer.Write((int)CityModifier.m_Mode);
            writer.Write(CityModifier.m_Range.min);
            writer.Write(CityModifier.m_Range.max);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out Version);
            reader.Read(out Enabled);
            reader.Read(out ModEntity);
            reader.Read(out int type);
            reader.Read(out int mode);
            reader.Read(out float min);
            reader.Read(out float max);
            CityModifier = new CityModifierData
            {
                m_Type = (CityModifierType)type,
                m_Mode = (ModifierValueMode)mode,
                m_Range = new Colossal.Mathematics.Bounds1(min, max),
            };
        }

        public readonly bool Equals(ModifiedCityEffect other)
        {
            if (ModEntity != other.ModEntity)
                return false;
            if (CityModifier.m_Type != other.CityModifier.m_Type)
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
