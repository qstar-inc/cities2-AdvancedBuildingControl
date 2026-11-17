using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Enums;
using AdvancedBuildingControl.Models;
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
        public ModifiedPrefabBufferSystem modifiedPrefabBufferSystem;
        public static readonly HashSet<UpdateValueType> ValidUpdateValueTypes = new(
            Enum.GetValues(typeof(UpdateValueType)).Cast<UpdateValueType>().Select(v => v)
        );

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = WorldHelper.PrefabSystem;
            selectedPrefabModifierSystem = WorldHelper.GetSystem<SelectedPrefabModifierSystem>();
            modifiedPrefabBufferSystem = WorldHelper.GetSystem<ModifiedPrefabBufferSystem>();
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
            if (!modifiedPrefabBufferSystem.TryGetBufferCopy(out var array))
                return;

            Dictionary<(Entity prefab, UpdateValueType type), ModifiedPrefab> bufferMap = new();
            Dictionary<(Entity prefab, UpdateValueType type), ModifiedPrefab> dictMap = new();

            for (int i = 0; i < array.Length; i++)
            {
                ModifiedPrefab entry = array[i];

                if (
                    !EntityManager.Exists(entry.ModEntity)
                    || !EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef)
                )
                    continue;

                bufferMap[(prefabRef.m_Prefab, entry.ValueType)] = entry;
            }

            foreach (var (k, v) in selectedPrefabModifierSystem.localEntities)
            {
                if (v?.PrefabChanges == null)
                    continue;

                foreach (PrefabChange change in v.PrefabChanges)
                {
                    dictMap[(k, change.ValueType)] = change.Modifications;
                }
            }

            foreach (var (k, v) in bufferMap)
            {
                if (!dictMap.TryGetValue(k, out ModifiedPrefab dictEntry) || !v.Equals(dictEntry))
                    selectedPrefabModifierSystem.Modify(k.prefab, $"(long){v.Modified}", k.type);
            }

            foreach (var (k, v) in dictMap)
            {
                if (!bufferMap.ContainsKey(k))
                {
                    selectedPrefabModifierSystem.Modify(
                        k.prefab,
                        $"(long){v.Original}",
                        k.type,
                        true
                    );
                    continue;
                }
            }
        }
    }
}
