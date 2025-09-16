using System;
using System.Data.SqlTypes;
using System.Linq;
using AdvancedBuildingControl.Systems;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Components
{
    public struct AlteredStorage : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            //writer.Write(Name.ToString());
            //writer.Write(OGName.ToString());
            //writer.Write(VariantIndex);
            //writer.Write(Guid.ToString());
            //writer.Write(OldRes);
            writer.Write(NewRes);

            writer.Write(OGEntity.ToString());
            //writer.Write(NewEntityId.ToString());
            //writer.Write(NewPrefabEntity);
            //LogHelper.SendLog($"Serializing AlteredStorage: {NewRes}, {OGEntity}", LogLevel.DEV);
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            //reader.Read(out string name);
            //reader.Read(out string ogName);
            //reader.Read(out int index);
            //reader.Read(out string guid);
            //reader.Read(out ulong oldRes);
            reader.Read(out ulong newRes);
            reader.Read(out string oGEntity);
            //reader.Read(out string newEntityId);
            //reader.Read(out Entity newPrefabEntity);

            //Name = name;
            //OGName = ogName;
            //VariantIndex = index;
            //Guid = guid;
            //OldRes = oldRes;
            NewRes = newRes;
            OGEntity = oGEntity;
            //NewEntityId = newEntityId;
            //NewPrefabEntity = newPrefabEntity;
            //LogHelper.SendLog($"Deserializing AlteredStorage: {NewRes}, {OGEntity}", LogLevel.DEV);
        }

        public readonly bool CompareTo(AlteredStorage other)
        {
            return OGEntity == other.OGEntity;
        }

        //public FixedString64Bytes Name;
        //public FixedString64Bytes OGName;
        //public int VariantIndex;
        //public FixedString32Bytes Guid;

        //public ulong OldRes;
        public ulong NewRes;

        public FixedString128Bytes OGEntity;
        //public FixedString32Bytes NewEntityId;
        //public Entity NewPrefabEntity;
    }
}
