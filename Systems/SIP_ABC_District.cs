using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Systems.Changers;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.UI;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;
using static Colossal.IO.AssetDatabase.AtlasFrame;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIP_ABC_District : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return nameof(AdvancedBuildingControl); }
        }

#nullable disable
        private NativeList<Entity> DistrictBuildings { get; set; }

        private PrefabSystem prefabSystem;
        private EntityQuery DistrictBuildingQuery;

        private RefChangerSystem refChangerSystem;
        private IconCommandSystem iconCommandSystem;

        private float CurrentLevel;

#nullable enable

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            prefabSystem = WorldHelper.PrefabSystem;
            iconCommandSystem = WorldHelper.IconCommandSystem;

            DistrictBuildingQuery = SystemAPI
                .QueryBuilder()
                .WithAll<Building, PrefabRef, CurrentDistrict>()
                .WithNone<Temp, Deleted>()
                .Build();

            refChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RefChangerSystem>();
            CreateTrigger<int>("ChangeLevelDistrict", ChangeLevelDistrict);
            DistrictBuildings = new NativeList<Entity>(Allocator.Persistent);
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            visible = Visible();
        }

        [Preserve]
        protected override void OnDestroy()
        {
            DistrictBuildings.Dispose();
            base.OnDestroy();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("CurrentLevel");
            writer.Write(CurrentLevel);
        }

        protected override void Reset()
        {
            CurrentLevel = 0;
            DistrictBuildings.Clear();
        }

        private bool Visible()
        {
            bool isVisible = false;
            if (
                EntityManager.HasComponent<District>(selectedEntity)
                && EntityManager.HasComponent<Area>(selectedEntity)
            )
            {
                if (!prefabSystem.TryGetPrefab(selectedPrefab, out PrefabBase _))
                    return false;

                if (EntityManager.TryGetComponent(selectedPrefab, out DistrictData _))
                    isVisible = true;

                if (!isVisible)
                    return false;
                return true;
            }
            return false;
        }

        protected override void OnProcess()
        {
            if (
                EntityManager.TryGetComponent(
                    selectedEntity,
                    out ABC_LevelDistrict altLevelDistrict
                )
            )
                CurrentLevel = altLevelDistrict.Level;
        }

        public void ChangeLevelDistrict(int level)
        {
            //Entities
            //    .WithStoreEntityQueryInField(ref DistrictBuildingQuery)
            //    .ForEach(
            //        (Entity entity, in CurrentDistrict district) =>
            //        {
            //            if (district.m_District == selectedEntity)
            //            {
            //                refChangerSystem.ReplaceEntity(
            //                    entity,
            //                    value: level.ToString(),
            //                    ProcessMode.Update,
            //                    RefChangerSystem.ValueType.Level
            //                );
            //            }
            //        }
            //    )
            //    .WithoutBurst()
            //    .Run();
            using var entities = DistrictBuildingQuery.ToEntityArray(Allocator.TempJob);
            using var districts = DistrictBuildingQuery.ToComponentDataArray<CurrentDistrict>(
                Allocator.TempJob
            );

            for (int i = 0; i < entities.Length; i++)
            {
                if (
                    districts[i].m_District == selectedEntity
                    && EntityManager.TryGetComponent(entities[i], out PrefabRef prefabRef)
                    && EntityManager.TryGetComponent(
                        prefabRef.m_Prefab,
                        out SpawnableBuildingData _
                    )
                )
                {
                    refChangerSystem.ReplaceEntity(
                        entities[i],
                        level.ToString(),
                        ProcessType.Update,
                        RefChangerSystem.UpdateValueType.Level
                    );
                }
            }
            ABC_LevelDistrict altLevelDistrict = new() { Level = level };
            CurrentLevel = level;
            EntityManager.AddComponentData(selectedEntity, altLevelDistrict);
            EntityManager.AddComponent<UpdateNextFrame>(selectedEntity);

            RequestUpdate();
        }
    }
}
