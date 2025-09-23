//using AdvancedBuildingControl.Components;
//using AdvancedBuildingControl.Variables;
//using Colossal.Entities;
//using Game;
//using Game.Prefabs;
//using StarQ.Shared.Extensions;
//using Unity.Collections;
//using Unity.Entities;

//namespace AdvancedBuildingControl.Systems
//{
//    public partial class HouseholdChangerSystem : GameSystemBase
//    {
//#nullable disable
//        public PrefabSystem prefabSystem;
//        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;
//        public PlopTheGrowableSystem plopTheGrowableSystem;
//        public Utils utils;
//#nullable enable
//        public EntityQuery alteredHouseholdComps;

//        protected override void OnCreate()
//        {
//            alteredHouseholdComps = SystemAPI.QueryBuilder().WithAll<AlteredHousehold>().Build();
//            prefabSystem = Mod.world.GetOrCreateSystemManaged<PrefabSystem>();
//            createdEntitiesManagementSystem =
//                Mod.world.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
//            utils = Mod.world.GetOrCreateSystemManaged<Utils>();
//            plopTheGrowableSystem = Mod.world.GetOrCreateSystemManaged<PlopTheGrowableSystem>();
//            RequireForUpdate(alteredHouseholdComps);
//        }

//        protected override void OnUpdate() { }

//        public void InitOnGameStart()
//        {
//            var queryArray = alteredHouseholdComps.ToEntityArray(Allocator.Temp);
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
//            int household,
//            AlteredHousehold alteredHouseholdX = new AlteredHousehold()
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
//                CreatedEntitiesManagementSystem.Arrays.Housing
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out BuildingPropertyData bpd);

//            int resiPropTemp;
//            if (alteredHouseholdX.CompareTo(new AlteredHousehold()))
//            {
//                resiPropTemp = household;
//            }
//            else
//            {
//                resiPropTemp = alteredHouseholdX.Household;
//            }

//            bpd.m_ResidentialProperties = resiPropTemp;
//            bpd.m_SpaceMultiplier = 1;

//            AlteredHousehold alteredHousehold = new()
//            {
//                Household = resiPropTemp,
//                OGEntity = newName,
//            };

//            EntityManager.AddComponentData(entity, alteredHousehold);

//            EntityManager.SetComponentData(newPrefabEntity, bpd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);

//            return resiPropTemp;
//        }

//        public int UpdateExisting(
//            Entity entity,
//            Entity currentPrefabRef,
//            int household,
//            AlteredHousehold alteredHousehold
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredHousehold.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredHousehold.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            Entity newPrefabEntity = utils.CreateEntity(currentPrefabRef);
//            createdEntitiesManagementSystem.AddEntity(
//                alteredHousehold.OGEntity.ToString(),
//                newPrefabEntity,
//                CreatedEntitiesManagementSystem.Arrays.Housing
//            );

//            EntityManager.TryGetComponent(newPrefabEntity, out BuildingPropertyData bpd);

//            bpd.m_ResidentialProperties = household;
//            bpd.m_SpaceMultiplier = 1;
//            alteredHousehold.Household = bpd.m_ResidentialProperties;

//            EntityManager.AddComponentData(entity, alteredHousehold);
//            EntityManager.SetComponentData(newPrefabEntity, bpd);

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//            }
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog(
//                $"Done processing {entity}, replacing {newPrefabEntity}",
//                LogLevel.DEV
//            );
//            return household;
//        }

//        public int ResetToOG(
//            Entity entity,
//            Entity currentPrefabRef,
//            AlteredHousehold alteredHousehold,
//            bool keepComponent
//        )
//        {
//            if (
//                !prefabSystem.TryGetPrefab(
//                    new PrefabID("BuildingPrefab", alteredHousehold.OGEntity.ToString()),
//                    out PrefabBase prefabBase
//                )
//            )
//            {
//                LogHelper.SendLog($"BuildingPrefab missing: {alteredHousehold.OGEntity}");
//                return 0;
//            }

//            prefabSystem.TryGetEntity(prefabBase, out Entity ogPrefabEntity);

//            int householdTempX = alteredHousehold.Household;

//            if (!keepComponent)
//            {
//                EntityManager.RemoveComponent<AlteredHousehold>(entity);
//            }

//            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });

//            if (currentPrefabRef != ogPrefabEntity)
//            {
//                EntityManager.DestroyEntity(currentPrefabRef);
//                createdEntitiesManagementSystem.RemoveEntity(
//                    alteredHousehold.OGEntity.ToString(),
//                    currentPrefabRef,
//                    CreatedEntitiesManagementSystem.Arrays.Housing
//                );
//            }
//            EntityManager.AddComponent<UpdateNextFrame>(entity);
//            LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);

//            return householdTempX;
//        }

//        public int ReplaceEntity(
//            Entity entity,
//            ProcessMode processMode = ProcessMode.None,
//            int household = 0
//        )
//        {
//            Entity currentPrefabRef = Entity.Null;
//            if (!utils.CheckPrefab(entity, ref currentPrefabRef))
//                return 0;

//            EntityManager.TryGetComponent(entity, out AlteredHousehold alteredHousehold);

//            switch (processMode)
//            {
//                case ProcessMode.None:
//                    return 0;
//                case ProcessMode.Loading:
//                    return AddToNew(entity, currentPrefabRef, household, alteredHousehold);
//                case ProcessMode.Update:
//                    if (alteredHousehold.CompareTo(new AlteredHousehold()))
//                        return AddToNew(entity, currentPrefabRef, household);
//                    return UpdateExisting(entity, currentPrefabRef, household, alteredHousehold);
//                case ProcessMode.Reset:
//                    return ResetToOG(entity, currentPrefabRef, alteredHousehold, false);
//                case ProcessMode.Saving:
//                    return ResetToOG(entity, currentPrefabRef, alteredHousehold, true);
//                default:
//                    return 0;
//            }
//        }
//    }
//}
