using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.Tools;
using Game.UI;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AdvancedBuildingControl.Systems
{
    public partial class StaticPloppableBuilder : GameSystemBase
    {
        public static bool hasSP = false;

        public static RenderPrefab baseProp = null;
        public static AssetPackPrefab assetPackPrefab = null;

        public PrefabSystem prefabSystem;
        public ToolSystem toolSystem;

        public PrefabFinder prefabFinder;
        public StaticPloppableData staticPloppableData;

        protected override void OnCreate()
        {
            base.OnCreate();
            prefabSystem = WorldHelper.PrefabSystem;
            toolSystem = WorldHelper.ToolSystem;
            prefabFinder = WorldHelper.GetSystem<PrefabFinder>();
            staticPloppableData = WorldHelper.GetSystem<StaticPloppableData>();

            Enabled = true;
            ModHelper.AddAfterActivePlaysetOrModStatusChanged(CheckSP);
        }

        protected override void OnUpdate() { }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            base.OnGamePreload(purpose, mode);
            CheckSP();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (WorldHelper.IsGame)
                FindAndCreateMissingSP();
        }

        public void CheckSP()
        {
            hasSP = false;
            baseProp = null;
            assetPackPrefab = null;

            if (prefabSystem == null)
                return;

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID(
                        nameof(AssetPackPrefab),
                        "StarQ SP01-BaseGame Pack",
                        Colossal.Hash128.Parse("34790457835307a80af82aa68c67a333")
                    ),
                    out PrefabBase _
                )
            )
            {
                LogHelper.SendLog("Static Ploppables UI not found.");
                return;
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID(nameof(RenderPrefab), "WindowBoardsSmall01 Mesh"),
                    out PrefabBase basePropRP
                )
            )
            {
                LogHelper.SendLog("Failed to find a required vanilla asset");
                return;
            }

            if (
                !prefabSystem.TryGetPrefab(
                    new PrefabID(
                        nameof(AssetPackPrefab),
                        "StarQ SP00-ABC Pack",
                        Colossal.Hash128.Parse("b013d44fa0b11e4c2da89043f3a7aaae")
                    ),
                    out PrefabBase ap
                )
            )
            {
                LogHelper.SendLog("Failed to find prefab: StarQ SP00-ABC Pack");
                return;
            }
            baseProp = basePropRP as RenderPrefab;
            assetPackPrefab = ap as AssetPackPrefab;
            hasSP = true;

            LogHelper.SendLog("Successfully initialized SP_Builder", LogLevel.DEVD);
            staticPloppableData.CreateSPFromFile();
        }

        public void MakeSP(Entity selectedPrefab)
        {
            if (!hasSP)
                return;
            string prefabName = PrefabHelper.GetPrefabName(selectedPrefab);

            LogHelper.SendLog($"Building SP for {prefabName}");

            if (TryMakeSP(selectedPrefab, true, out PrefabBase prefabBase))
            {
                if (prefabBase != null)
                    toolSystem.ActivatePrefabTool(prefabBase);
            }
            LocaleHelper.OnActiveDictionaryChanged();
        }

        public void MakeSP(SPData spData)
        {
            if (!hasSP)
                return;

            if (
                !prefabFinder.TryFindPrefab(
                    spData.prefabName,
                    Colossal.Hash128.Parse(spData.guid),
                    false,
                    PrefabType.BuildingOrExtension,
                    out Entity foundEntity
                )
            )
            {
                LogHelper.SendLog($"{spData.prefabName} not found, skipping");
                return;
            }

            TryMakeSP(foundEntity, false, out PrefabBase _);
        }

        public bool TryMakeSP(Entity entity, bool log, out PrefabBase createdPrefab)
        {
            createdPrefab = null;
            try
            {
                string entityName = prefabSystem.GetPrefabName(entity);

                if (!prefabSystem.TryGetPrefab(entity, out PrefabBase bldgPrefab))
                    return false;

                if (bldgPrefab is not BuildingPrefab && bldgPrefab is not BuildingExtensionPrefab)
                    return false;

                BuildingPrefab buildingPrefab =
                    bldgPrefab is BuildingPrefab ? bldgPrefab as BuildingPrefab : new();

                if (bldgPrefab is BuildingExtensionPrefab)
                {
                    if (
                        !bldgPrefab.TryGet(out ServiceUpgrade sUpgrade)
                        || sUpgrade.m_Buildings.Length <= 0
                    )
                        return false;

                    BuildingPrefab main = sUpgrade.m_Buildings[0];

                    buildingPrefab.m_Circular = main.m_Circular;
                    buildingPrefab.m_LotDepth = main.m_LotDepth;
                    buildingPrefab.m_LotWidth = main.m_LotWidth;
                }

                //if (bldgPrefab.asset == null)
                //    return false;

                //string assetPath = bldgPrefab.asset.path;
                //string[] assetPathSplit = assetPath.Split('/');

                if (entityName.StartsWith("StarQ_SP"))
                {
                    LogHelper.SendLog($"{entityName} is already a Static Ploppable");
                    createdPrefab = buildingPrefab;
                    return true;
                }

                prefabSystem.TryGetEntity(bldgPrefab, out Entity newBldgEntity);
                if (
                    !prefabSystem.TryGetPrefab(
                        newBldgEntity,
                        out ObjectGeometryPrefab objectGeometryPrefab
                    )
                )
                    return false;

                ObjectMeshInfo[] subMeshesNew = new ObjectMeshInfo[0];
                ObjectMeshInfo[] subMeshes = objectGeometryPrefab.m_Meshes;

                if (subMeshes.Length <= 0)
                    return false;

                for (int i = 0; i < subMeshes.Length; i++)
                {
                    RenderPrefabBase rpb = subMeshes[i].m_Mesh;
                    prefabSystem.TryGetEntity(rpb, out Entity mesh);
                    var mn = prefabSystem.GetPrefabName(mesh);
                    var mn_x = mn.Replace(" Mesh", "");

                    //                    if (!meshList.ContainsKey(mn_x) && !ignoredList.Contains(mn_x))
                    //{
                    //meshList.Add(mn_x, mesh);

                    //Mod.log.Info($"Adding {mn_x} to meshList");
                    prefabSystem.TryGetPrefab(mesh, out PrefabBase renderPrefab);

                    //BldgData bldgData = GetBldgData(mn_x);

                    List<string> renders = new() { mn };
                    if (renderPrefab.Has<ProceduralAnimationProperties>()) // && !bldgData.SkipAnimated)
                        renders.Add($"{mn}_Animated");

                    foreach (string meshName in renders)
                    {
                        bool animated = false;
                        if (meshName.EndsWith("_Animated"))
                        {
                            animated = true;
                            //if (!anim.Contains(meshName))
                            //    anim.Add(meshName);
                        }

                        string dupedName = $"StarQ_SP {meshName}";
                        string nonMeshName = $"StarQ_SP {meshName.Replace(" Mesh", "")}";

                        if (
                            prefabFinder.TryFindBuildingPrefab(
                                nonMeshName,
                                log,
                                out Entity existing
                            ) && prefabSystem.TryGetPrefab(existing, out createdPrefab)
                        )
                        {
                            LogHelper.SendLog($"Found existing: {nonMeshName}", LogLevel.DEVD);
                            return true;
                        }
                        LogHelper.SendLog(
                            $"Existing not found, creating: {nonMeshName}",
                            LogLevel.DEVD
                        );

                        PrefabBase newRenderPrefab = prefabSystem.DuplicatePrefab(
                            renderPrefab,
                            dupedName
                        );
                        BaseProperties bp = newRenderPrefab.AddOrGetComponent<BaseProperties>();
                        bp.m_BaseType = baseProp;
                        bp.m_UseMinBounds = false;

                        BuildingPrefab newBuildingCreated =
                            ScriptableObject.CreateInstance<BuildingPrefab>();
                        newBuildingCreated.name = nonMeshName;

                        //bldgDataDone.Add(meshName);

                        //PrefabBase assetPack = assetPackPrefab;

                        //PrefabSystem.TryGetEntity(assetPack, out Entity assetPackEntity);
                        //AssetPackElement app = new() { m_Pack = assetPackEntity };
                        //EntityManager.TryGetComponent(assetPackEntity, out PrefabData apData);
                        //PrefabSystem.TryGetPrefab(apData, out AssetPackPrefab apPrefab);
                        AssetPackItem ap = ScriptableObject.CreateInstance<AssetPackItem>();
                        ap.m_Packs = new AssetPackPrefab[] { assetPackPrefab };
                        newBuildingCreated.AddComponentFrom(ap);
                        //folder = apName.Replace(" Pack", "");

                        //if (selected == "vanilla" && !folder.Contains("BaseGame"))
                        //    continue;
                        //if (!string.IsNullOrEmpty(selected) && !folder.Contains("-RP_"))
                        //    if (!folder.Contains(selected))
                        //        continue;

                        //if (bldgData.DLC == "CS1TH" || bldgData.DLC == "SF" || bldgData.DLC == "LM")
                        //    folder = apName + "_" + GetDLCName(bldgData.DLC);

                        LodProperties lodProperties = newRenderPrefab.GetComponent<LodProperties>();
                        if (lodProperties != null && lodProperties.m_LodMeshes != null)
                        {
                            for (int l = 0; l < lodProperties.m_LodMeshes.Length; l++)
                            {
                                RenderPrefab lod = lodProperties.m_LodMeshes[l];
                                prefabSystem.TryGetEntity(lod, out Entity lodEntity);
                                string animSuff = animated ? "_Animated" : "";
                                string lodName =
                                    $"StarQ_SP {prefabSystem.GetPrefabName(lodEntity)}{animSuff}";
                                //if (!meshList.ContainsKey(lodName))
                                //{
                                //meshList.Add(lodName, lodEntity);
                                PrefabBase newlod = prefabSystem.DuplicatePrefab(lod, lodName);
                                lodProperties.m_LodMeshes[l] = (RenderPrefab)newlod;

                                BaseProperties bpLod = newlod.AddOrGetComponent<BaseProperties>();
                                bpLod.m_BaseType = baseProp;
                                bpLod.m_UseMinBounds = false;

                                if (!animated)
                                    newlod.Remove<ProceduralAnimationProperties>();

                                //AssetDataPath adp_lod = AssetDataPath.Create(
                                //    $"ImportedData/SP/{folder}/{nonMeshName.Replace(".", "_")}",
                                //    lodName.Replace(".", "_"),
                                //    EscapeStrategy.None
                                //);
                                //AssetDatabase
                                //    .user.AddAsset(adp_lod, newlod)
                                //    .Save(ct, false, true);
                                prefabSystem.AddOrUpdatePrefab(newlod);
                                //}
                            }
                        }

                        if (!animated)
                            newRenderPrefab.Remove<ProceduralAnimationProperties>();

                        //AssetDataPath adp_rp = AssetDataPath.Create(
                        //    $"ImportedData/SP/{folder}/{nonMeshName.Replace(".", "_")}",
                        //    dupedName.Replace(".", "_"),
                        //    EscapeStrategy.None
                        //);
                        //AssetDatabase.user.AddAsset(adp_rp, newRenderPrefab).Save(ct, false, true);
                        prefabSystem.AddOrUpdatePrefab(newRenderPrefab);

                        BuildingTerraformOverride bto =
                            newBuildingCreated.AddOrGetComponent<BuildingTerraformOverride>();
                        bto.m_LevelMinOffset = new float2(1600f, 1600f);
                        bto.m_LevelMaxOffset = new float2(0, 0);
                        bto.m_LevelFrontLeft = new float2(1f, 1f);
                        bto.m_LevelFrontRight = new float2(1f, 1f);
                        bto.m_LevelBackLeft = new float2(1f, 1f);
                        bto.m_LevelBackRight = new float2(1f, 1f);
                        bto.m_SmoothMinOffset = new float2(0, 0);
                        bto.m_SmoothMaxOffset = new float2(0, 0);
                        bto.m_HeightOffset = 0;
                        bto.m_AdditionalSmoothAreas = new BuildingTerraformOverride.SubLot[0];
                        bto.m_DontRaise = true;
                        bto.m_DontLower = true;

                        newBuildingCreated.m_Meshes = new ObjectMeshInfo[0];
                        Array.Resize(
                            ref newBuildingCreated.m_Meshes,
                            newBuildingCreated.m_Meshes.Length + 1
                        );
                        newBuildingCreated.m_Meshes[^1] = new()
                        {
                            m_Mesh = (RenderPrefab)newRenderPrefab,
                            //m_Position = bldgData.Position,
                            //m_Rotation = bldgData.Rotation,
                        };

                        var regexName = Regex.Match(nonMeshName, "/.+_(\\d)x(\\d)$/");
                        //if (bldgData.Width > 0)
                        //    newBuildingCreated.m_LotWidth = bldgData.Width;
                        //else
                        //{
                        //    newBuildingCreated.m_LotWidth = 4;
                        //    if (regexName.Success)
                        //        newBuildingCreated.m_LotWidth = regexName.Value[0];
                        //}
                        //if (bldgData.Depth > 0)
                        //    newBuildingCreated.m_LotDepth = bldgData.Depth;
                        //else
                        //{
                        //    newBuildingCreated.m_LotDepth = 4;
                        //    if (regexName.Success)
                        //        newBuildingCreated.m_LotDepth = regexName.Value[1];
                        //}
                        //newBuildingCreated.m_Circular = bldgData.Circle;
                        newBuildingCreated.m_LotWidth = buildingPrefab.m_LotWidth;
                        newBuildingCreated.m_LotDepth = buildingPrefab.m_LotDepth;
                        newBuildingCreated.m_Circular = buildingPrefab.m_Circular;

                        EditorAssetCategoryOverride EditorAssetCategoryOverride =
                            newBuildingCreated.AddOrGetComponent<EditorAssetCategoryOverride>();
                        EditorAssetCategoryOverride.m_IncludeCategories = new string[0];
                        Array.Resize(
                            ref EditorAssetCategoryOverride.m_IncludeCategories,
                            EditorAssetCategoryOverride.m_IncludeCategories.Length + 1
                        );
                        EditorAssetCategoryOverride.m_IncludeCategories[^1] =
                            $"StarQ/Buildings/Static Ploppables/ABC";

                        UIObject uiObject = newBuildingCreated.AddOrGetComponent<UIObject>();
                        //if (tabList.ContainsKey(bldgData.Tab))

                        staticPloppableData.TryGetTab(entity, out UIAssetCategoryPrefab tab);
                        uiObject.m_Group = tab;

                        if (bldgPrefab.TryGet(out SpawnableBuilding oldSpawnableBuilding))
                            uiObject.m_Icon = ImageSystem.GetIcon(oldSpawnableBuilding.m_ZoneType);
                        else if (bldgPrefab.TryGet(out UIObject oldUiObject))
                            if (oldUiObject.m_Icon == "" || oldUiObject.m_Icon == null)
                                uiObject.m_Icon = ImageSystem.GetThumbnail(bldgPrefab);
                            else
                                uiObject.m_Icon = oldUiObject.m_Icon;

                        uiObject.m_Priority = int.MaxValue;
                        if (
                            (uiObject.m_Icon == null || uiObject.m_Icon == "")
                            && uiObject.m_Group != null
                        )
                        {
                            string groupIcon = ImageSystem.GetIcon(uiObject.m_Group);
                            if (groupIcon != null)
                                uiObject.m_Icon = ImageSystem.GetIcon(uiObject.m_Group);
                        }

                        //if (guidList.ContainsKey(nonMeshName.Replace("StarQ_SP ", "")))
                        //{
                        //    ObsoleteIdentifiers obsolete =
                        //        newBuildingCreated.AddOrGetComponent<ObsoleteIdentifiers>();
                        //    obsolete.m_PrefabIdentifiers ??= new PrefabIdentifierInfo[0];
                        //    Array.Resize(
                        //        ref obsolete.m_PrefabIdentifiers,
                        //        obsolete.m_PrefabIdentifiers.Length + 1
                        //    );
                        //    obsolete.m_PrefabIdentifiers[^1] = new()
                        //    {
                        //        m_Name = nonMeshName,
                        //        m_Type = "BuildingPrefab",
                        //        m_Hash = guidList[nonMeshName.Replace("StarQ_SP ", "")],
                        //    };
                        //}

                        //if (bldgData.ObsoleteName != "" && !animated)
                        //{
                        //    ObsoleteIdentifiers obsolete =
                        //        newBuildingCreated.AddOrGetComponent<ObsoleteIdentifiers>();
                        //    obsolete.m_PrefabIdentifiers ??= new PrefabIdentifierInfo[0];
                        //    Array.Resize(
                        //        ref obsolete.m_PrefabIdentifiers,
                        //        obsolete.m_PrefabIdentifiers.Length + 1
                        //    );
                        //    obsolete.m_PrefabIdentifiers[^1] = new()
                        //    {
                        //        m_Name = $"StarQ {bldgData.ObsoleteName}",
                        //        m_Type = "BuildingPrefab",
                        //    };
                        //}

                        //if (bldgData.DLC != "")
                        //{
                        //    ContentPrerequisite contentPrerequisite =
                        //        newBuildingCreated.AddOrGetComponent<ContentPrerequisite>();
                        //    if (
                        //        prefabSystem.TryGetPrefab(
                        //            new PrefabID("ContentPrefab", GetDLCName(bldgData.DLC)),
                        //            out PrefabBase contentPrerequisitePrefab
                        //        )
                        //    )
                        //        contentPrerequisite.m_ContentPrerequisite =
                        //            (ContentPrefab)contentPrerequisitePrefab;
                        //}

                        //if (bldgData.Theme != "")
                        //{
                        //    ThemeObject themeObject =
                        //        newBuildingCreated.AddOrGetComponent<ThemeObject>();
                        //    if (
                        //        prefabSystem.TryGetPrefab(
                        //            new PrefabID("ThemePrefab", GetThemeName(bldgData.Theme)),
                        //            out PrefabBase themePrefab
                        //        )
                        //    )
                        //        themeObject.m_Theme = (ThemePrefab)themePrefab;
                        //}

                        FloatingObject floatingObject =
                            newBuildingCreated.AddOrGetComponent<FloatingObject>();
                        floatingObject.m_FloatingOffset = 0;
                        floatingObject.m_FixedToBottom = true;
                        floatingObject.m_AllowDryland = true;

                        DestructibleObject destructibleObject =
                            newBuildingCreated.AddOrGetComponent<DestructibleObject>();
                        destructibleObject.m_FireHazard = 0;
                        destructibleObject.m_StructuralIntegrity = 1e+07f;

                        UnderwaterObject underwaterObject =
                            newBuildingCreated.AddOrGetComponent<UnderwaterObject>();
                        underwaterObject.m_AllowDryland = true;

                        prefabSystem.AddOrUpdatePrefab(newBuildingCreated);

                        if (createdPrefab == null)
                            createdPrefab = newBuildingCreated;

                        AddLocale(entity, entityName, nonMeshName);

                        //AssetDataPath adp_main = AssetDataPath.Create(
                        //    $"ImportedData/SP/{folder}/{nonMeshName.Replace(".", "_")}",
                        //    nonMeshName.Replace(".", "_"),
                        //    EscapeStrategy.None
                        //);
                        //AssetDatabase
                        //    .user.AddAsset(adp_main, newBuildingCreated)
                        //    .Save(ct, false, true);
                        //prefabSystem.AddOrUpdatePrefab(newBuildingCreated);

                        //if (!done.Contains(meshName))
                        //    done.Add(meshName);

                        //createdDict[nonMeshName] = new()
                        //{
                        //    PrefabBase = newBuildingCreated,
                        //    Folder = folder,
                        //    DisplayName = bldgData.DisplayName,
                        //    IsAnimated = animated,
                        //};

                        //Mod.log_SP.Info(nonMeshName.Replace("StarQ_SP", "").Trim());
                        //if (!ailData.ContainsKey(folder))
                        //    ailData[folder] = new Dictionary<string, string>();

                        //ailData[folder][$"BuildingPrefab.{nonMeshName}"] =
                        //    $"BuildingPrefab.{bldgName.Replace("StarQ_SP ", "").Replace("_Animated", "").Trim()}";

                        //x++;
                    }
                    //}
                }
                Colossal.Hash128 guid = bldgPrefab.asset?.id.guid ?? Guid.Empty;
                staticPloppableData.AddToFile(entityName, guid);
                CheckUpgrades(entity);
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }

            return true;
        }

        public void CheckUpgrades(Entity entity)
        {
            if (
                EntityManager.TryGetBuffer(
                    entity,
                    true,
                    out DynamicBuffer<BuildingUpgradeElement> bue
                )
            )
            {
                if (bue.Length < 0)
                    return;

                for (int i = 0; i < bue.Length; i++)
                {
                    var upgradePrefab = bue[i].m_Upgrade;
                    try
                    {
                        if (upgradePrefab != null)
                            TryMakeSP(upgradePrefab, false, out _);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SendLog(ex, LogLevel.Error);
                    }
                }
            }
        }

        public void AddLocale(Entity entity, string entityName, string prefabName)
        {
            GameManager.instance.localizationManager.activeDictionary.TryGetValue(
                $"Assets.NAME[{entityName}]",
                out string displayName
            );

            if (displayName == null || displayName == string.Empty)
                displayName = prefabName;

            if (
                EntityManager.TryGetBuffer(
                    entity,
                    true,
                    out DynamicBuffer<ServiceUpgradeBuilding> sub
                )
            )
            {
                if (sub.Length == 1)
                {
                    var main = sub[0].m_Building;
                    string mainName = prefabSystem.GetPrefabName(main);
                    if (mainName != null || mainName != string.Empty)
                    {
                        displayName = $"{mainName}: {displayName}";
                    }
                }
            }

            LocaleHelper.AddLocalization($"Assets.NAME[{prefabName}]", displayName);
            LocaleHelper.AddLocalization(
                $"Assets.DESCRIPTION[{prefabName}]",
                $"Mesh: {prefabName.Replace("StarQ_SP ", "").Trim()}\r\nPloppable, static, non fuctional variant.\r\nCreated using {Mod.Name}"
            );
        }

        public void FindAndCreateMissingSP()
        {
            string prefName = string.Empty;
            int spCount = 0;
            try
            {
                PrefabSystem prefabSystem = WorldHelper.PrefabSystem;
                EntityQuery query = SystemAPI.QueryBuilder().WithAll<PrefabRef>().Build();
                NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);

                if (entities.Length <= 0)
                    return;

                List<(Entity, string)> toCheckRenderPrefabs = new();

                foreach (var entity in entities)
                {
                    try
                    {
                        EntityManager.TryGetComponent(entity, out PrefabRef prefabRef);
                        bool isEnabled = EntityManager.IsComponentEnabled<PrefabData>(
                            prefabRef.m_Prefab
                        );
                        if (isEnabled)
                            continue;

                        PrefabID obs = prefabSystem.GetObsoleteID(prefabRef.m_Prefab);

                        prefName = obs.ToString();

                        Match reg = Regex.Match(
                            prefName,
                            "^(.+?):(.+?)(?:\\s+\\(([A-Fa-f0-9]{32})\\))?$"
                        );

                        if (!reg.Success)
                            continue;

                        string pType = reg.Groups[1].Value;
                        string pName = reg.Groups[2].Value;

                        if (string.IsNullOrEmpty(pType) || string.IsNullOrEmpty(pName))
                            continue;

                        if (pType != "BuildingPrefab" || !pName.StartsWith("StarQ_SP "))
                            continue;

                        Entity foundEntity = Entity.Null;
                        string mainName = pName.Replace("StarQ_SP ", "");
                        if (
                            !prefabFinder.TryFindPrefab(
                                mainName,
                                false,
                                PrefabType.RenderPrefab,
                                out foundEntity
                            )
                        )
                            continue;

                        if (foundEntity == Entity.Null)
                            continue;

                        if (toCheckRenderPrefabs.Contains((foundEntity, mainName)))
                            continue;

                        toCheckRenderPrefabs.Add((foundEntity, mainName));
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SendLog(ex, LogLevel.Error);
                    }
                }

                if (toCheckRenderPrefabs.Count == 0)
                    return;

                EntityQuery bldgEntityQuery = SystemAPI
                    .QueryBuilder()
                    .WithAll<SubMesh, PrefabData>()
                    .WithAny<BuildingData, BuildingExtensionData>()
                    .Build();

                if (bldgEntityQuery.IsEmpty)
                    return;

                NativeArray<Entity> bldgEntities = bldgEntityQuery.ToEntityArray(Allocator.Temp);

                HashSet<Entity> prefabSet = new(toCheckRenderPrefabs.Select(x => x.Item1));

                HashSet<Entity> processed = new();

                foreach (Entity entity in bldgEntities)
                {
                    DynamicBuffer<SubMesh> subMesh = EntityManager.GetBuffer<SubMesh>(entity);

                    foreach (var sm in subMesh)
                    {
                        if (!processed.Add(sm.m_SubMesh))
                            continue;

                        if (prefabSet.Contains(sm.m_SubMesh))
                        {
                            if (TryMakeSP(entity, false, out PrefabBase _))
                                spCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog($"Stopped on {prefName}");
                LogHelper.SendLog(ex);
            }
            finally
            {
                if (spCount > 0)
                {
                    LogHelper.SendLog($"{spCount} SP created/found on startup");
                    GameManager.instance.userInterface.appBindings.ShowMessageDialog(
                        new MessageDialog(
                            Mod.Name,
                            $"{Mod.Id}.ReloadSave",
                            $"{Mod.Id}.OpenLoadGameScreen"
                        ),
                        x =>
                        {
                            WorldHelper.GetSystem<GameScreenUISystem>().activeScreen =
                                GameScreenUISystem.GameScreen.LoadGame;
                        }
                    );
                }
            }
        }
    }
}
