//using AdvancedBuildingControl.Components;
//using AdvancedBuildingControl.Variables;
//using Colossal.Entities;
//using Game;
//using Game.Common;
//using Game.Economy;
//using Game.Prefabs;
//using StarQ.Shared.Extensions;
//using Unity.Collections;
//using Unity.Entities;

//namespace AdvancedBuildingControl.Systems
//{
//    public partial class StorageChangerSystem : GameSystemBase
//    {
//#nullable disable
//        public PrefabSystem prefabSystem;
//        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;
//        public Utils utils;
//#nullable enable
//        public EntityQuery alteredStorageComps;

//        protected override void OnCreate()
//        {
//            alteredStorageComps = SystemAPI.QueryBuilder().WithAll<AlteredStorage>().Build();
//            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
//            createdEntitiesManagementSystem =
//                Mod.world.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
//            utils = Mod.world.GetOrCreateSystemManaged<Utils>();
//            RequireForUpdate(alteredStorageComps);
//        }

//        protected override void OnUpdate() { }

//        public void InitOnGameStart()
//        {
//            var queryArray = alteredStorageComps.ToEntityArray(Allocator.Temp);
//            LogHelper.SendLog($"Found {queryArray.Length} entities", LogLevel.DEV);

//            foreach (var entity in queryArray)
//            {
//                LogHelper.SendLog($"Initing {entity}", LogLevel.DEV);
//                ReplaceEntity(entity, ProcessMode.Loading);
//            }
//        }

//        public Resource AddToNew(
//            Entity entity,
//            Entity currentPrefabRef,
//            Resource resId,
//            AlteredStorage alteredStorageX = new AlteredStorage()
//        )
//        {
//            string newName = prefabSystem.GetPrefabName(currentPrefabRef);

//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", newName),
//                    out PrefabBase _
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {newName}");
//                return 0;
//            }

//            Entity newPrefabEntity = utils.CreateEntity(currentPrefabRef);
//            createdEntitiesManagementSystem.AddEntity(
//                newName,
//                newPrefabEntity,
//                CreatedEntitiesManagementSystem.Arrays.Storage
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);

//            Resource resTemp = scd.m_StoredResources;
//            if (alteredStorageX.CompareTo(new AlteredStorage()))
//            {
//                if ((resTemp & resId) != 0)
//                {
//                    resTemp &= ~resId;
//                }
//                else
//                {
//                    resTemp |= resId;
//                }
//            }
//            else
//            {
//                resTemp = (Resource)alteredStorageX.NewRes;
//            }

//            scd.m_StoredResources = resTemp;

//            Resource resTempX = scd.m_StoredResources;

//            utils.RemoveCurrentResource(entity, resTempX);

//            alteredStorageX.NewRes = (ulong)resTempX;
//            alteredStorageX.OGEntity = newName;

//            EntityManager.AddComponentData(entity, alteredStorageX);

//            EntityManager.SetComponentData(newPrefabEntity, scd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            EntityManager.AddComponent<Updated>(newPrefabEntity);
//            EntityManager.AddComponent<Updated>(entity);
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);
//            return resTempX;
//        }

//        public Resource UpdateExisting(
//            Entity entity,
//            Entity currentPrefabRef,
//            Resource resId,
//            AlteredStorage alteredStorage
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredStorage.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredStorage.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            Entity newPrefabEntity = utils.CreateEntity(currentPrefabRef);
//            createdEntitiesManagementSystem.AddEntity(
//                alteredStorage.OGEntity.ToString(),
//                newPrefabEntity,
//                CreatedEntitiesManagementSystem.Arrays.Storage
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out StorageCompanyData scd);

//            Resource resTemp = (Resource)alteredStorage.NewRes;

//            if ((resTemp & resId) != 0)
//            {
//                resTemp &= ~resId;
//            }
//            else
//            {
//                resTemp |= resId;
//            }

//            scd.m_StoredResources = resTemp;
//            alteredStorage.NewRes = (ulong)resTemp;

//            Resource resTempX = scd.m_StoredResources;

//            utils.RemoveCurrentResource(entity, resTempX);

//            EntityManager.AddComponentData(entity, alteredStorage);
//            EntityManager.SetComponentData(newPrefabEntity, scd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//            }
//            EntityManager.AddComponent<Updated>(newPrefabEntity);
//            EntityManager.AddComponent<Updated>(entity);
//            LogHelper.SendLog(
//                $"Done processing {entity}, replacing {newPrefabEntity}",
//                LogLevel.DEV
//            );
//            return resTempX;
//        }

//        public Resource ResetToOG(
//            Entity entity,
//            Entity currentPrefabRef,
//            AlteredStorage alteredStorage,
//            bool keepComponent
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredStorage.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredStorage.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            Resource resTempX = (Resource)alteredStorage.NewRes;

//            if (!keepComponent)
//            {
//                EntityManager.RemoveComponent<AlteredStorage>(entity);
//            }

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//                createdEntitiesManagementSystem.RemoveEntity(
//                    alteredStorage.OGEntity.ToString(),
//                    currentPrefabRef,
//                    CreatedEntitiesManagementSystem.Arrays.Storage
//                );
//            }
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);

//            return resTempX;
//        }

//        public Resource ReplaceEntity(
//            Entity entity,
//            ProcessMode processMode = ProcessMode.None,
//            Resource resId = Resource.NoResource
//        )
//        {
//            Entity currentPrefabRef = Entity.Null;
//            if (!utils.CheckPrefab(entity, ref currentPrefabRef))
//                return 0;

//            EntityManager.TryGetComponent(entity, out AlteredStorage alteredStorage);

//            switch (processMode)
//            {
//                case ProcessMode.None:
//                    return 0;
//                case ProcessMode.Loading:
//                    return AddToNew(entity, currentPrefabRef, resId, alteredStorage);
//                case ProcessMode.Update:
//                    if (alteredStorage.CompareTo(new AlteredStorage()))
//                        return AddToNew(entity, currentPrefabRef, resId);
//                    return UpdateExisting(entity, currentPrefabRef, resId, alteredStorage);
//                case ProcessMode.Reset:
//                    return ResetToOG(entity, currentPrefabRef, alteredStorage, false);
//                case ProcessMode.Saving:
//                    return ResetToOG(entity, currentPrefabRef, alteredStorage, true);
//                default:
//                    return 0;
//            }
//        }

//    }
//}
