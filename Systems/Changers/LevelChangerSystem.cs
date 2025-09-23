//using System;
//using AdvancedBuildingControl.Components;
//using AdvancedBuildingControl.Variables;
//using Colossal.Entities;
//using Game;
//using Game.Prefabs;
//using StarQ.Shared.Extensions;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;

//namespace AdvancedBuildingControl.Systems
//{
//    public partial class LevelChangerSystem : GameSystemBase
//    {
//#nullable disable
//        public PrefabSystem prefabSystem;
//        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;
//        public PlopTheGrowableSystem plopTheGrowableSystem;
//        public Utils utils;
//#nullable enable
//        public EntityQuery alteredLevelComps;

//        protected override void OnCreate()
//        {
//            alteredLevelComps = SystemAPI.QueryBuilder().WithAll<AlteredLevel>().Build();
//            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
//            createdEntitiesManagementSystem =
//                Mod.world.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
//            utils = Mod.world.GetOrCreateSystemManaged<Utils>();
//            plopTheGrowableSystem = Mod.world.GetOrCreateSystemManaged<PlopTheGrowableSystem>();
//            RequireForUpdate(alteredLevelComps);
//        }

//        protected override void OnUpdate() { }

//        public void InitOnGameStart()
//        {
//            var queryArray = alteredLevelComps.ToEntityArray(Allocator.Temp);
//            LogHelper.SendLog($"Found {queryArray.Length} entities", LogLevel.DEV);

//            foreach (var entity in queryArray)
//            {
//                LogHelper.SendLog($"Initing {entity}", LogLevel.DEV);
//                ReplaceEntity(entity, ProcessMode.Loading);
//            }
//        }

//        public int AddToNew(
//            Entity entity,
//            Entity currentPrefabRef,
//            int level,
//            AlteredLevel alteredLevelX = new AlteredLevel()
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
//                CreatedEntitiesManagementSystem.Arrays.Level
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out SpawnableBuildingData sbd);

//            byte levelTemp;
//            if (alteredLevelX.CompareTo(new AlteredLevel()))
//            {
//                levelTemp = (byte)level;
//            }
//            else
//            {
//                levelTemp = (byte)alteredLevelX.Level;
//            }

//            sbd.m_Level = levelTemp;

//            AlteredLevel alteredLevel = new() { Level = (int)levelTemp, OGEntity = newName };

//            EntityManager.AddComponentData(entity, alteredLevel);

//            EntityManager.SetComponentData(newPrefabEntity, sbd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            plopTheGrowableSystem.LockLevelWithPTG(entity);
//            utils.UpdateUpkeep(newPrefabEntity, level, sbd.m_ZonePrefab);

//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);

//            return levelTemp;
//        }

//        public int UpdateExisting(
//            Entity entity,
//            Entity currentPrefabRef,
//            int level,
//            AlteredLevel alteredLevel
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredLevel.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredLevel.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            Entity newPrefabEntity = utils.CreateEntity(currentPrefabRef);
//            createdEntitiesManagementSystem.AddEntity(
//                alteredLevel.OGEntity.ToString(),
//                newPrefabEntity,
//                CreatedEntitiesManagementSystem.Arrays.Level
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out SpawnableBuildingData sbd);

//            sbd.m_Level = (byte)level;
//            alteredLevel.Level = sbd.m_Level;

//            EntityManager.AddComponentData(entity, alteredLevel);
//            EntityManager.SetComponentData(newPrefabEntity, sbd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            plopTheGrowableSystem.LockLevelWithPTG(entity);
//            utils.UpdateUpkeep(newPrefabEntity, level, sbd.m_ZonePrefab);

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//            }
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog(
//                $"Done processing {entity}, replacing {newPrefabEntity}",
//                LogLevel.DEV
//            );
//            return level;
//        }

//        public int ResetToOG(
//            Entity entity,
//            Entity currentPrefabRef,
//            AlteredLevel alteredLevel,
//            bool keepComponent
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredLevel.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredLevel.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            byte levelTempX = (byte)alteredLevel.Level;

//            if (!keepComponent)
//            {
//                EntityManager.RemoveComponent<AlteredLevel>(entity);
//            }
//            EntityManager.TryGetComponent(ogPrefabEntity, out SpawnableBuildingData sbd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });
//            utils.UpdateUpkeep(ogPrefabEntity, sbd.m_Level, sbd.m_ZonePrefab);

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//                createdEntitiesManagementSystem.RemoveEntity(
//                    alteredLevel.OGEntity.ToString(),
//                    currentPrefabRef,
//                    CreatedEntitiesManagementSystem.Arrays.Level
//                );
//            }
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);

//            return levelTempX;
//        }

//        public int ReplaceEntity(
//            Entity entity,
//            ProcessMode processMode = ProcessMode.None,
//            int level = 0
//        )
//        {
//            Entity currentPrefabRef = Entity.Null;
//            if (!utils.CheckPrefab(entity, ref currentPrefabRef))
//                return 0;

//            EntityManager.TryGetComponent(entity, out AlteredLevel alteredLevel);

//            switch (processMode)
//            {
//                case ProcessMode.None:
//                    return 0;
//                case ProcessMode.Loading:
//                    return AddToNew(entity, currentPrefabRef, level, alteredLevel);
//                case ProcessMode.Update:
//                    if (alteredLevel.CompareTo(new AlteredLevel()))
//                        return AddToNew(entity, currentPrefabRef, level);
//                    return UpdateExisting(entity, currentPrefabRef, level, alteredLevel);
//                case ProcessMode.Reset:
//                    return ResetToOG(entity, currentPrefabRef, alteredLevel, false);
//                case ProcessMode.Saving:
//                    return ResetToOG(entity, currentPrefabRef, alteredLevel, true);
//                default:
//                    return 0;
//            }
//        }
//    }
//}
