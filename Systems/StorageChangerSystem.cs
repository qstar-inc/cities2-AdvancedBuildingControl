using System.Collections.Generic;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Common;
using Game.Economy;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Systems
{
    public partial class StorageChangerSystem : GameSystemBase
    {
#nullable disable
        public PrefabSystem prefabSystem;
        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;
#nullable enable
        public EntityQuery alteredStorageComps;

        protected override void OnCreate()
        {
            //altereds = new();
            alteredStorageComps = SystemAPI.QueryBuilder().WithAll<AlteredStorage>().Build();
            prefabSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PrefabSystem>();
            createdEntitiesManagementSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
            RequireForUpdate(alteredStorageComps);
        }

        protected override void OnUpdate() { }

        public void InitOnGameStart()
        {
            var eq2 = alteredStorageComps.ToEntityArray(Allocator.Temp);
            LogHelper.SendLog($"Found {eq2.Length} entities", LogLevel.DEV);

            createdEntitiesManagementSystem.Update();

            foreach (var entity in eq2)
            {
                LogHelper.SendLog($"Initing {entity}", LogLevel.DEV);
                ReplaceEntity(entity, ProcessMode.Loading);
            }
        }

        public Resource AddToNew(
            Entity entity,
            Resource resId,
            AlteredStorage alteredStorageX = new AlteredStorage()
        )
        {
            if (entity == Entity.Null)
            {
                LogHelper.SendLog($"entity null for {entity}");
                return 0;
            }

            if (!EntityManager.TryGetComponent(entity, component: out PrefabRef prefabRef))
            {
                LogHelper.SendLog($"prefabRef null for {entity}");
                return 0;
            }

            Entity currentPrefabRef = prefabRef.m_Prefab;

            if (currentPrefabRef == Entity.Null)
            {
                LogHelper.SendLog($"currentPrefabRef null for {entity}");
                return 0;
            }

            string newName = prefabSystem.GetPrefabName(currentPrefabRef);

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", newName),
                    out PrefabBase _
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
                return 0;
            }

            //prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            Entity newPrefabEntity = EntityManager.Instantiate(currentPrefabRef);
            createdEntitiesManagementSystem.AddEntity(newName, newPrefabEntity);

            EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);

            Resource resTemp = scd.m_StoredResources;
            if (alteredStorageX.CompareTo(new AlteredStorage()))
            {
                if ((resTemp & resId) != 0)
                {
                    resTemp &= ~resId;
                }
                else
                {
                    resTemp |= resId;
                }
            }
            else
            {
                resTemp = (Resource)alteredStorageX.NewRes;
            }

            scd.m_StoredResources = resTemp;

            Resource resTempX = scd.m_StoredResources;

            RemoveCurrentResource(entity, resTempX);

            AlteredStorage alteredStorage = new() { NewRes = (ulong)resTempX, OGEntity = newName };

            EntityManager.AddComponentData(entity, alteredStorage);

            EntityManager.SetComponentData(newPrefabEntity, scd);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            //if (currentPrefabRef != ogPrefabEntity)
            //{
            //    EntityManager.DestroyEntity(currentPrefabRef);
            //}
            //EntityManager.AddComponent<Updated>(oldPrefabEntity);
            EntityManager.AddComponent<Updated>(newPrefabEntity);
            EntityManager.AddComponent<Updated>(entity);
            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);
            return resTempX;
        }

        public Resource UpdateExisting(Entity entity, AlteredStorage alteredStorage, Resource resId)
        {
            if (entity == Entity.Null)
            {
                LogHelper.SendLog($"entity null for {entity}");
                return 0;
            }

            if (!EntityManager.TryGetComponent(entity, out PrefabRef prefabRef))
            {
                LogHelper.SendLog($"prefabRef null for {entity}");
                return 0;
            }

            Entity currentPrefabRef = prefabRef.m_Prefab;

            if (currentPrefabRef == Entity.Null)
            {
                LogHelper.SendLog($"currentPrefabRef null for {entity}");
                return 0;
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", alteredStorage.OGEntity.ToString()),
                    out PrefabBase prefabBase
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {alteredStorage.OGEntity}");
                return 0;
            }

            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            Entity newPrefabEntity = EntityManager.Instantiate(ogPrefabEntity);
            createdEntitiesManagementSystem.AddEntity(
                alteredStorage.OGEntity.ToString(),
                newPrefabEntity
            );

            EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);

            Resource resTemp = (Resource)alteredStorage.NewRes; // 1116689393630

            if ((resTemp & resId) != 0)
            {
                resTemp &= ~resId;
            }
            else
            {
                resTemp |= resId;
            }

            scd.m_StoredResources = resTemp;
            alteredStorage.NewRes = (ulong)resTemp;

            Resource resTempX = scd.m_StoredResources;

            RemoveCurrentResource(entity, resTempX);

            EntityManager.AddComponentData(entity, alteredStorage);
            EntityManager.SetComponentData(newPrefabEntity, scd);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
            {
                EntityManager.DestroyEntity(currentPrefabRef);
            }
            //EntityManager.AddComponent<Updated>(oldPrefabEntity);
            EntityManager.AddComponent<Updated>(newPrefabEntity);
            EntityManager.AddComponent<Updated>(entity);
            LogHelper.SendLog(
                $"Done processing {entity}, replacing {newPrefabEntity}",
                LogLevel.DEV
            );
            return resTempX;
        }

        public Resource ResetToOG(Entity entity, AlteredStorage alteredStorage, bool keepComponent)
        {
            if (entity == Entity.Null)
            {
                LogHelper.SendLog($"entity null for {entity}");
                return 0;
            }

            if (!EntityManager.TryGetComponent(entity, out PrefabRef prefabRef))
            {
                LogHelper.SendLog($"prefabRef null for {entity}");
                return 0;
            }

            Entity currentPrefabRef = prefabRef.m_Prefab;

            if (currentPrefabRef == Entity.Null)
            {
                LogHelper.SendLog($"currentPrefabRef null for {entity}");
                return 0;
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID("BuildingPrefab", alteredStorage.OGEntity.ToString()),
                    out PrefabBase prefabBase
                )
            )
            {
                LogHelper.SendLog($"BuildingPrefab missing: {alteredStorage.OGEntity}");
                return 0;
            }

            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

            //Entity newPrefabEntity = EntityManager.Instantiate(ogPrefabEntity);

            //EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);

            //Resource resTemp = (Resource)alteredStorage.NewRes;

            //if ((resTemp & resId) != 0)
            //{
            //    resTemp &= ~resId;
            //}
            //else
            //{
            //    resTemp |= resId;
            //}

            //scd.m_StoredResources = resTemp;
            //alteredStorage.NewRes = (ulong)resTemp;

            Resource resTempX = (Resource)alteredStorage.NewRes;

            //RemoveCurrentResource(entity, resTempX);

            //EntityManager.SetComponentData(newPrefabEntity, scd);

            if (!keepComponent)
            {
                EntityManager.RemoveComponent<AlteredStorage>(entity);
            }

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
            {
                EntityManager.DestroyEntity(currentPrefabRef);
                createdEntitiesManagementSystem.RemoveEntity(
                    alteredStorage.OGEntity.ToString(),
                    currentPrefabRef
                );
            }
            //EntityManager.AddComponent<Updated>(oldPrefabEntity);
            //EntityManager.AddComponent<Updated>(newPrefabEntity);/
            EntityManager.AddComponent<UpdateNextFrame>(entity);
            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);

            return resTempX;
        }

        public enum ProcessMode
        {
            None,
            Update,
            Reset,
            Saving,
            Loading,
        }

        public Resource ReplaceEntity(
            Entity entity,
            ProcessMode processMode = ProcessMode.None,
            Resource resId = Resource.NoResource
        )
        {
            EntityManager.TryGetComponent(entity, out AlteredStorage alteredStorage);

            switch (processMode)
            {
                case ProcessMode.None:
                    return 0;
                case ProcessMode.Loading:
                    return AddToNew(entity, resId, alteredStorage);
                case ProcessMode.Update:
                    if (alteredStorage.CompareTo(new AlteredStorage()))
                        return AddToNew(entity, resId);
                    return UpdateExisting(entity, alteredStorage, resId);
                case ProcessMode.Reset:
                    return ResetToOG(entity, alteredStorage, false);
                case ProcessMode.Saving:
                    return ResetToOG(entity, alteredStorage, true);
                default:
                    return 0;
            }

            //if (
            //    entity != Entity.Null
            //    && EntityManager.TryGetComponent(entity, out PrefabRef prefabRef)
            //)
            //{
            //    Entity currentPrefabRef = prefabRef.m_Prefab;
            //    Entity oldPrefabEntity; // 12460:1
            //    Entity oldTemp = Entity.Null;

            //    bool isNotFirstTime = false;
            //    Resource resTemp = 0;
            //    //if (resId == Resource.NoResource)
            //    //{
            //    //    oldPrefabEntity = prefabRef.m_Prefab;
            //    //}
            //    //else
            //    if (EntityManager.TryGetComponent(entity, out AlteredStorage alteredStorage))
            //    {
            //        isNotFirstTime = true;

            //        if (
            //            !prefabSystem.TryGetPrefab(
            //                new PrefabID("BuildingPrefab", alteredStorage.OGEntity.ToString()),
            //                out PrefabBase prefabBase
            //            )
            //        )
            //        {
            //            LogHelper.SendLog($"BuildingPrefab missing: {alteredStorage.OGEntity}");
            //            return 0;
            //        }
            //        prefabSystem.TryGetEntity(prefabBase, out oldTemp); //alteredStorage.OGEntity;
            //        oldPrefabEntity = oldTemp;
            //        resTemp = (Resource)alteredStorage.NewRes;
            //    }
            //    else
            //    {
            //        oldPrefabEntity = prefabRef.m_Prefab;
            //    }

            //    if (oldPrefabEntity == Entity.Null)
            //    {
            //        LogHelper.SendLog("Can't find original prefab");
            //        return 0;
            //    }
            //    string oldPrefabName = prefabSystem.GetPrefabName(oldPrefabEntity);
            //    if (oldPrefabName == null || oldPrefabName == string.Empty)
            //    {
            //        LogHelper.SendLog("Prefab data missing");
            //        return 0;
            //    }

            //    Entity newPrefabEntity = EntityManager.Instantiate(oldPrefabEntity);
            //    //DataRetriever.createdEntities["as"] = new Dictionary<Entity, DataRetriever.CreatedEntitiesDetail>() { });

            //    string newEntityId = $"{newPrefabEntity.Index}:{newPrefabEntity.Version}";

            //    EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);
            //    if (!isNotFirstTime)
            //        resTemp = scd.m_StoredResources;

            //    if ((resTemp & resId) != 0)
            //    {
            //        resTemp &= ~resId;
            //    }
            //    else
            //    {
            //        resTemp |= resId;
            //    }
            //    if (resId != Resource.NoResource)
            //    {
            //        AlteredStorage alteredStorageN = new()
            //        {
            //            NewRes = (ulong)resTemp,
            //            //Guid = Mod.SessionGuid,
            //            OGEntity = prefabSystem.GetPrefabName(oldPrefabEntity),
            //            NewEntityId = newEntityId,
            //        };
            //        scd.m_StoredResources = resTemp;

            //        EntityManager.AddComponentData(entity, alteredStorageN);
            //    }

            //    if (isNotFirstTime)
            //    {
            //        alteredStorage.NewEntityId = newEntityId;
            //        if (resId != Resource.NoResource)
            //        {
            //            alteredStorage.NewRes = (ulong)resTemp;
            //        }
            //        else if (resId == Resource.NoResource)
            //        {
            //            scd.m_StoredResources = (Resource)alteredStorage.NewRes;
            //        }
            //        EntityManager.AddComponentData(entity, alteredStorage);
            //    }
            //    //else if (!alteredStorageSet && resId == Resource.NoResource)
            //    //{
            //    //    EntityManager.DestroyEntity(newPrefabEntity);
            //    //    LogHelper.SendLog($"Failed applying {resId} to {entity}", LogLevel.Error);
            //    //    return 0;
            //    //}

            //    Resource resTempX = scd.m_StoredResources;

            //    RemoveCurrentResource(entity, resTempX);

            //    EntityManager.SetComponentData(newPrefabEntity, scd);

            //    EntityManager.SetComponentData(
            //        entity,
            //        new PrefabRef() { m_Prefab = newPrefabEntity }
            //    );

            //    //if (
            //    //    oldTemp != Entity.Null
            //    //    && (currentPrefabRef != newPrefabEntity || currentPrefabRef != oldTemp)
            //    //)
            //    //{
            //    //    EntityManager.DestroyEntity(currentPrefabRef);
            //    //}
            //    EntityManager.AddComponent<Updated>(oldPrefabEntity);
            //    EntityManager.AddComponent<Updated>(newPrefabEntity);
            //    EntityManager.AddComponent<Updated>(entity);
            //    LogHelper.SendLog(
            //        $"Done processing {entity}, replacing {newPrefabEntity} with {oldPrefabEntity}",
            //        LogLevel.DEV
            //    );
            //    return resTempX;
            //}
            //LogHelper.SendLog($"Unable to continue 'ReplaceEntity' for {entity}");
            //return 0;
        }

        public void RemoveCurrentResource(Entity entity, Resource res)
        {
            EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Resources> resources);

            for (int i = resources.Length - 1; i >= 0; i--)
            {
                var r = resources[i].m_Resource;

                if (r != Resource.Money && !res.HasFlag(r))
                {
                    resources.RemoveAt(i);
                }
            }
            //var data = DataRetriever.resourceDataInfos;
            //foreach (Resource flag in Enum.GetValues(typeof(Resource)))
            //{
            //    var resData =
            //        data.Where(r => r.Resource == flag).FirstOrDefault()?.Group
            //        ?? ResourceGroup.None;
            //    if (resData == ResourceGroup.None || resData == ResourceGroup.Money)
            //        continue;

            //    if (res.HasFlag(flag) && !resources.Any(x => x.m_Resource == flag))
            //    {
            //        resources.Add(new Resources { m_Resource = flag, m_Amount = 0 });
            //    }
            //}
        }
    }
}
