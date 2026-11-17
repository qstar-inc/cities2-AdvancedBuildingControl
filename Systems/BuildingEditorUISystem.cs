using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Bindings;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.UI.Editor;
using StarQ.Shared.Extensions;
using StarQ.Shared.Extensions.UI;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class BuildingEditorUISystem : ExtendedUISystemBase
    {
        public override GameMode gameMode => GameMode.Game;

        private PrefabSystem prefabSystem;
        private EffectControlSystem effectControlSystem;

        private ValueBindingHelper<EditorBuilding[]> EditorBuildings;
        private ValueBindingHelper<string[]> EditorBuildingSources;
        private ValueBindingHelper<EditorBuildingAssetPack[]> EditorBuildingAssetPacks;
        private ValueBindingHelper<EditorBuildingData> EditorBuildingData;

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = WorldHelper.PrefabSystem;
            effectControlSystem = WorldHelper.GetSystem<EffectControlSystem>();

            EditorBuildings = CreateBinding("EditorBuildings", Array.Empty<EditorBuilding>());
            EditorBuildingSources = CreateBinding("EditorBuildingSources", Array.Empty<string>());
            EditorBuildingAssetPacks = CreateBinding(
                "EditorBuildingAssetPacks",
                Array.Empty<EditorBuildingAssetPack>()
            );
            EditorBuildingData = CreateBinding("EditorBuildingData", new EditorBuildingData());

            CreateTrigger<Entity>("GetSelectedBuildingsData", GetSelectedBuildingsData);
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            LoadEditorData();
        }

        private void LoadEditorData()
        {
            NativeArray<Entity> entities = SystemAPI
                .QueryBuilder()
                .WithAll<PrefabData>()
                .WithAny<BuildingData, BuildingExtensionData>()
                .Build()
                .ToEntityArray(Allocator.Temp);
            List<EditorBuilding> editorBuildings = new();
            HashSet<string> sources = new();
            Dictionary<Entity, EditorBuildingAssetPack> packLookup = new();

            foreach (Entity entity in entities)
            {
                if (
                    !prefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase)
                    || prefabBase == null
                )
                    continue;

                string source = "Unknown";
                if (prefabBase.asset != null)
                    source = prefabBase.asset.database.name;

                List<Entity> buildingPacks = new();
                if (
                    EntityManager.TryGetBuffer(
                        entity,
                        true,
                        out DynamicBuffer<AssetPackElement> packBuffer
                    ) && (packBuffer.Length > 0)
                )
                {
                    foreach (AssetPackElement pack in packBuffer)
                    {
                        if (pack.m_Pack == null)
                            continue;
                        Entity packEntity = pack.m_Pack;

                        if (!packLookup.ContainsKey(packEntity))
                            packLookup[packEntity] = new EditorBuildingAssetPack
                            {
                                Entity = packEntity,
                                PrefabName = PrefabHelper.GetPrefabName(packEntity),
                                UIIcon = WorldHelper.ImageSystem.GetThumbnail(packEntity),
                            };

                        buildingPacks.Add(packEntity);
                    }
                }

                editorBuildings.Add(
                    new EditorBuilding()
                    {
                        Entity = entity,
                        PrefabName = PrefabHelper.GetPrefabName(entity),
                        UIIcon = WorldHelper.ImageSystem.GetThumbnail(entity),
                        Source = source,
                        AssetPacks = buildingPacks.ToArray(),
                        IsUpgrade = EntityManager.HasComponent<ServiceUpgradeData>(entity),
                    }
                );
            }

            EditorBuildings.Value = editorBuildings.ToArray();
            EditorBuildingSources.Value = sources.ToArray();
            EditorBuildingAssetPacks.Value = packLookup.Values.ToArray();
        }

        private void GetSelectedBuildingsData(Entity item)
        {
            effectControlSystem.CheckEffects(item, out BldgEffectInfo bldgEffectInfo);

            EditorBuildingData data = new()
            {
                Entity = item,
                PrefabName = PrefabHelper.GetPrefabName(item),
                BldgEffectInfo = bldgEffectInfo,
            };

            EditorBuildingData.Value = data;
        }
    }
}
