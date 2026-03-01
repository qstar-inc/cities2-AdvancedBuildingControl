using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SaveStoreSystem : GameSystemBase
    {
        public PrefabSystem prefabSystem;
        public SelectedPrefabModifierSystem selectedPrefabModifierSystem;
        public BufferControlSystem bufferControlSystem;
        public static readonly HashSet<UpdateValueType> ValidUpdateValueTypes = new(
            Enum.GetValues(typeof(UpdateValueType)).Cast<UpdateValueType>().Select(v => v)
        );

        struct ModKey
        {
            public Entity Prefab;
            public UpdateValueType ValueType;

            public ModKey(Entity prefab, UpdateValueType valueType)
            {
                Prefab = prefab;
                ValueType = valueType;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = WorldHelper.PrefabSystem;
            bufferControlSystem = WorldHelper.GetSystem<BufferControlSystem>();
            selectedPrefabModifierSystem = WorldHelper.GetSystem<SelectedPrefabModifierSystem>();
            Enabled = true;
        }

        protected override void OnUpdate() { }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (!WorldHelper.IsGame)
                return;

            LoadAll();
            LogHelper.SendLog("OnLoad SaveStore done", LogLevel.DEVD);
        }

        public void LoadAll()
        {
            var bufferMap = new Dictionary<ModKey, ModifiedPrefab>();
            var dictMap = new Dictionary<ModKey, ModifiedPrefab>();

            if (bufferControlSystem.TryGetBufferCopy(out var array))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var entry = array[i];

                    if (!EntityManager.Exists(entry.ModEntity))
                        continue;

                    if (!EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef))
                        continue;

                    bufferMap[new ModKey(prefabRef.m_Prefab, entry.ValueType)] = entry;
                }

                foreach (var kv in selectedPrefabModifierSystem.localEntities)
                {
                    var prefab = kv.Key;
                    var cd = kv.Value;

                    if (cd == null || cd.PrefabChanges == null)
                        continue;

                    foreach (var change in cd.PrefabChanges)
                    {
                        dictMap[new ModKey(prefab, change.ValueType)] = change.Modifications;
                    }
                }

                foreach (var kv in bufferMap)
                {
                    var key = kv.Key;

                    if (!dictMap.ContainsKey(key))
                    {
                        selectedPrefabModifierSystem.Modify(
                            key.Prefab,
                            $"(long){kv.Value.Modified}",
                            key.ValueType
                        );
                    }
                }

                foreach (var kv in dictMap)
                {
                    var key = kv.Key;

                    if (!bufferMap.ContainsKey(key))
                    {
                        selectedPrefabModifierSystem.Modify(
                            key.Prefab,
                            $"(long){kv.Value.Original}",
                            key.ValueType,
                            true
                        );
                        continue;
                    }

                    var bufferEntry = bufferMap[key];

                    if (!bufferEntry.Equals(kv.Value))
                    {
                        selectedPrefabModifierSystem.Modify(
                            key.Prefab,
                            $"(long){bufferEntry.Modified}",
                            key.ValueType
                        );
                    }
                }
            }
        }
    }
}
