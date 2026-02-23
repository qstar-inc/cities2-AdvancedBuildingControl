using System;
using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public enum PrefabType
    {
        None = 0,
        Building = 1,
        BuildingExtension = 2,
        BuildingOrExtension = 3,
        RenderPrefab = 10,
    }

    public partial class PrefabFinder : GameSystemBase
    {
        protected override void OnUpdate() { }

        public bool TryFindPrefab(string name, PrefabType prefabType, out Entity entity) =>
            TryFindPrefab(name, Colossal.Hash128.Empty, true, prefabType, out entity);

        public bool TryFindPrefab(
            string name,
            bool log,
            PrefabType prefabType,
            out Entity entity
        ) => TryFindPrefab(name, Colossal.Hash128.Empty, log, prefabType, out entity);

        public bool TryFindPrefab(
            string name,
            Colossal.Hash128 guid,
            PrefabType prefabType,
            out Entity entity
        ) => TryFindPrefab(name, guid, true, prefabType, out entity);

        public bool TryFindPrefab(
            string prefabName,
            Colossal.Hash128 guid,
            bool log,
            PrefabType prefabType,
            out Entity entity
        )
        {
            entity = Entity.Null;
            try
            {
                PrefabSystem prefabSystem = WorldHelper.PrefabSystem;
                PrefabID prefabID = new();
                if (
                    prefabType == PrefabType.Building
                    || prefabType == PrefabType.BuildingOrExtension
                )
                {
                    prefabID = new(nameof(BuildingPrefab), prefabName, guid);

                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;

                    prefabID = new(nameof(BuildingPrefab), prefabName);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;
                }

                if (
                    prefabType == PrefabType.BuildingExtension
                    || prefabType == PrefabType.BuildingOrExtension
                )
                {
                    prefabID = new(nameof(BuildingExtensionPrefab), prefabName, guid);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;

                    prefabID = new(nameof(BuildingExtensionPrefab), prefabName);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;
                }

                if (prefabType == PrefabType.RenderPrefab)
                {
                    prefabID = new(nameof(RenderPrefab), $"{prefabName} Mesh", guid);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;

                    prefabID = new(nameof(RenderPrefab), $"{prefabName} Mesh");
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;

                    prefabID = new(nameof(RenderPrefab), prefabName, guid);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;

                    prefabID = new(nameof(RenderPrefab), prefabName);
                    if (PrefabHelper.TryGetEntity(prefabID, out entity))
                        return true;
                }

                if (
                    prefabType == PrefabType.Building
                    || prefabType == PrefabType.BuildingOrExtension
                )
                    if (TryFindBuildingPrefab(prefabName, false, out entity))
                        return true;

                if (prefabType == PrefabType.RenderPrefab)
                    if (TryFindRenderPrefab(prefabName, false, out entity))
                        return true;

                if (log)
                    LogHelper.SendLog($"Unable to find {prefabName} prefab");
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
            return false;
        }

        public bool TryFindBuildingPrefab(string name, out Entity entity) =>
            TryFindBuildingPrefab(name, true, out entity);

        public bool TryFindBuildingPrefab(string name, bool log, out Entity entity)
        {
            entity = Entity.Null;
            try
            {
                PrefabSystem prefabSystem = WorldHelper.PrefabSystem;

                var eq3 = SystemAPI.QueryBuilder().WithAll<BuildingData, PrefabData>().Build();
                var ents3 = eq3.ToEntityArray(Allocator.Temp);

                if (ents3.Length <= 0)
                    return false;

                var pm = AssetDatabase.global.GetAssets<PrefabAsset>();
                Dictionary<string, PrefabBase> prefabAssets = new();
                foreach (var pmItem in pm)
                {
                    PrefabBase? prefabBase = pmItem.GetInstance<PrefabBase>();

                    if (prefabBase is not BuildingPrefab)
                        continue;

                    if (prefabBase.name == name)
                    {
                        prefabSystem.TryGetEntity(prefabBase, out entity);
                        return true;
                    }
                }
                if (log)
                    LogHelper.SendLog($"Unable to find {name} prefab");
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
            return false;
        }

        public bool TryFindRenderPrefab(string name, bool log, out Entity entity)
        {
            entity = Entity.Null;
            try
            {
                PrefabSystem prefabSystem = WorldHelper.PrefabSystem;

                var eq3 = SystemAPI.QueryBuilder().WithAll<MeshData, PrefabData>().Build();
                var ents3 = eq3.ToEntityArray(Allocator.Temp);

                if (ents3.Length <= 0)
                    return false;

                var pm = AssetDatabase.global.GetAssets<PrefabAsset>();
                Dictionary<string, PrefabBase> prefabAssets = new();
                foreach (var pmItem in pm)
                {
                    PrefabBase? prefabBase = pmItem.GetInstance<PrefabBase>();

                    if (prefabBase is not RenderPrefab)
                        continue;

                    if (prefabBase.name == name || prefabBase.name == $"{name} Mesh")
                    {
                        if (prefabSystem.TryGetEntity(prefabBase, out entity))
                            return true;
                    }
                }
                if (log)
                    LogHelper.SendLog($"Unable to find {name} prefab");
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
            return false;
        }
    }
}
