using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Bindings;
using AdvancedBuildingControl.Enums;
using AdvancedBuildingControl.Helpers;
using AdvancedBuildingControl.Models;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using StarQ.Shared.Extensions.UI;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIPSystem : ExtendedInfoSectionBase
    {
        public override GameMode gameMode => GameMode.Game;
        protected override string group
        {
            get { return nameof(AdvancedBuildingControl); }
        }
        protected override bool displayForUpgrades
        {
            get { return true; }
        }

#nullable disable
        public static bool hasMesh = false;
        public static BldgBrandInfo bldgBrandInfo = new();
        public static BldgComponentInfo bldgComponentInfo = new();
        public static List<BldgModifiedInfo> bldgModifiedInfo = new();
        public static BldgCleanupInfo bldgCleanupInfo = new();

        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;
        private SelectedPrefabModifierSystem selectedPrefabModifier;
        private SelectedEntityModifierSystem selectedEntityModifier;
        private ModifiedPrefabBufferSystem modifiedPrefabBufferSystem;
        private StaticPloppableBuilder staticPloppableBuilder;
        private static (string, int, Entity) lastTrigger = ("", 0, Entity.Null);

#nullable enable

        private Entity companyEntity = Entity.Null;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);

            nameSystem = WorldHelper.NameSystem;
            prefabSystem = WorldHelper.PrefabSystem;

            selectedPrefabModifier = WorldHelper.GetSystem<SelectedPrefabModifierSystem>();
            selectedEntityModifier = WorldHelper.GetSystem<SelectedEntityModifierSystem>();
            modifiedPrefabBufferSystem = WorldHelper.GetSystem<ModifiedPrefabBufferSystem>();
            staticPloppableBuilder = WorldHelper.GetSystem<StaticPloppableBuilder>();
            effectControlSystem = WorldHelper.GetSystem<EffectControlSystem>();
            effectOverlaySystem = WorldHelper.GetSystem<EffectOverlaySystem>();

            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger<string, int>("ChangeComponentValue", ChangeComponentValue);
            CreateTrigger<int>("ResetComponentValue", ResetComponentValue);
            CreateTrigger("MakeSP", MakeSP);
            CreateTrigger<string, int>("ChangeBCTValue", ChangeBCTValue);
            CreateTrigger<int>("TriggerBCT", TriggerBCT);

            CreateTrigger<EffectOptions>("ShowEffectOverlay", ShowEffectOverlay);
            CreateTrigger<UnityEngine.Color>("SetEffectOverlayColor", SetEffectOverlayColor);
            CreateTrigger<bool>("SetRadiusCommon", SetRadiusCommon);

            CreateTrigger<BldgEffectTypeInfoLocal>("SetLocalEffect", SetLocalEffect);
            CreateTrigger<LocalModifierType, bool>("SetLocalEnabled", SetLocalEnabled);
            CreateTrigger<LocalModifierType>("AddLocalEffect", AddLocalEffect);
            CreateTrigger<LocalModifierType>("RevertLocalEffect", RevertLocalEffect);
            CreateTrigger("RevertAllLocalEffects", RevertAllLocalEffects);

            CreateTrigger<BldgEffectTypeInfoCity>("SetCityEffect", SetCityEffect);
            CreateTrigger<int, bool>("SetCityEnabled", SetCityEnabled);
            CreateTrigger<int>("AddCityEffect", AddCityEffect);
            CreateTrigger<int>("RevertCityEffect", RevertCityEffect);
            CreateTrigger("RevertAllCityEffects", RevertAllCityEffects);

            CreateTrigger("RevertAllEffects", RevertAllEffects);

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = Visible();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("hasSP");
            writer.Write(StaticPloppableBuilder.hasSP);

            writer.PropertyName("hasMesh");
            writer.Write(hasMesh);

            writer.PropertyName("bldgBrandInfo");
            writer.Write(bldgBrandInfo);

            writer.PropertyName("bldgComponentInfo");
            writer.Write(bldgComponentInfo);

            writer.PropertyName("bldgModifiedInfo");
            writer.Write(bldgModifiedInfo.ToArray());

            writer.PropertyName("bldgCleanupInfo");
            writer.Write(bldgCleanupInfo);

            writer.PropertyName("bldgEffectInfo");
            writer.Write(bldgEffectInfo);
        }

        protected override void Reset()
        {
            bldgBrandInfo = new();
            bldgComponentInfo = new();
            bldgModifiedInfo = new();
            bldgCleanupInfo = new();
            bldgEffectInfo = new();

            hasMesh = false;

            companyEntity = Entity.Null;
        }

        private bool Visible()
        {
            if (!EntityManager.TryGetComponent(selectedEntity, out PrefabRef _))
                return false;
            if (!prefabSystem.TryGetPrefab(selectedPrefab, out PrefabBase _))
                return false;

            if (
                EntityManager.HasComponent<BuildingData>(selectedPrefab)
                || EntityManager.HasComponent<BuildingExtensionData>(selectedPrefab)
            )
                return true;

            return false;
        }

        protected override void OnProcess()
        {
            if (
                !EntityManager.TryGetComponent(selectedEntity, out PrefabRef _)
                || !(
                    EntityManager.HasComponent<BuildingData>(selectedPrefab)
                    || EntityManager.HasComponent<BuildingExtensionData>(selectedPrefab)
                )
                || EntityManager.HasComponent<Destroyed>(selectedEntity)
            )
                return;

            CheckMesh();
            CheckBrand();
            CheckComponents();
            CheckCurrentModifications();
            CheckEffects();
            CheckCleanupInfo();
        }

        public void CheckMesh()
        {
            if (
                EntityManager.TryGetBuffer(selectedPrefab, true, out DynamicBuffer<SubMesh> subMesh)
            )
                if (subMesh.Length > 0)
                    hasMesh = true;
        }

        public void CheckBrand()
        {
            if (
                !CompanyUIUtils.HasCompany(
                    EntityManager,
                    selectedEntity,
                    selectedPrefab,
                    out companyEntity
                )
                || !EntityManager.TryGetComponent(companyEntity, out CompanyData companyData)
                || companyData.m_Brand == null
            )
                return;

            var brandInData = DataRetriever
                .brandDataInfos.Where(v => v.Entity == companyData.m_Brand)
                ?.FirstOrDefault();

            if (brandInData == null)
                return;

            bldgBrandInfo.BrandList = DataRetriever.brandDataInfos.ToArray();
            bldgBrandInfo.BrandName = PrefabHelper.GetPrefabName(companyData.m_Brand);
            bldgBrandInfo.CompanyName = nameSystem
                .GetRenderedLabelName(companyEntity)
                .Replace("Assets.NAME[", "")
                .Replace("]", "");
            bldgBrandInfo.BrandIcon = brandInData?.Icon ?? "";
            bldgBrandInfo.HasBrand = true;
        }

        public void CheckCurrentModifications()
        {
            if (!selectedPrefabModifier.localEntities.TryGetValue(selectedPrefab, out var data))
                return;

            if (data.PrefabChanges == null)
                return;

            foreach (PrefabChange prefabChanges in data.PrefabChanges)
            {
                if (
                    prefabChanges.Modifications.IsEnabled
                    && prefabChanges.Modifications.Modified != prefabChanges.Modifications.Original
                )
                {
                    UVTHelper.TryConvertValueToString(
                        prefabChanges.Modifications.Original,
                        prefabChanges.ValueType,
                        out string originalText
                    );
                    bldgModifiedInfo.Add(
                        new BldgModifiedInfo()
                        {
                            OriginalText = originalText,
                            ValueType = prefabChanges.ValueType,
                        }
                    );
                }
            }
        }

        public void CheckEffects()
        {
            effectControlSystem.CheckEffects(selectedPrefab, out bldgEffectInfo);
        }

        public void CheckCleanupInfo()
        {
            List<BldgCleanupTypeInfo> bldgCleanupInfoArray = new();
            if (EntityManager.TryGetComponent(selectedEntity, out GarbageProducer garbageProducer))
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.Garbage,
                        Enabled = true,
                        CurrentValueNumber = garbageProducer.m_Garbage,
                    }
                );

            if (EntityManager.TryGetComponent(selectedEntity, out CrimeProducer crimeProducer))
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.Crime,
                        Enabled = true,
                        CurrentValueNumber = crimeProducer.m_Crime,
                    }
                );

            if (EntityManager.TryGetComponent(selectedEntity, out MailProducer mailProducer))
            {
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.OutgoingMail,
                        Enabled = true,
                        CurrentValueNumber = mailProducer.m_SendingMail,
                    }
                );
            }

            if (EntityManager.TryGetComponent(selectedEntity, out Damaged damaged))
            {
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.PhysicalDamage,
                        Enabled = true,
                        CurrentValueNumber = damaged.m_Damage.x * 100f,
                    }
                );
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.FireDamage,
                        Enabled = true,
                        CurrentValueNumber = damaged.m_Damage.y * 100f,
                    }
                );
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.WaterDamage,
                        Enabled = true,
                        CurrentValueNumber = damaged.m_Damage.z * 100f,
                    }
                );
            }

            if (EntityManager.HasComponent<Abandoned>(selectedEntity))
            {
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.ClearAbandonment,
                        Enabled = true,
                        CurrentValueNumber = 0,
                    }
                );
            }

            if (EntityManager.HasBuffer<IconElement>(selectedEntity))
            {
                bldgCleanupInfoArray.Add(
                    new()
                    {
                        CleanupType = BldgCleanupType.ClearNotification,
                        Enabled = true,
                        CurrentValueNumber = 0,
                    }
                );
            }

            bldgCleanupInfo.Array = bldgCleanupInfoArray.ToArray();

            if (bldgCleanupInfo.Array != null && bldgCleanupInfo.Array.Length > 0)
                bldgCleanupInfo.Enabled = true;
        }

        public void RandomizeStyle()
        {
            selectedEntityModifier.RandomizeStyle(selectedEntity);
            RequestUpdate();
        }

        public void SetBrand(string replaceBrand)
        {
            selectedEntityModifier.ChangeBrand(selectedEntity, replaceBrand);
            RequestUpdate();
        }

        public void ChangeComponentValue(string value, int valueType)
        {
            if (!Enum.IsDefined(typeof(UpdateValueType), (ushort)valueType))
                return;
            if (
                lastTrigger.Item1 == value
                && lastTrigger.Item2 == valueType
                && lastTrigger.Item3 == selectedPrefab
            )
            {
                LogHelper.SendLog("Skipping duplicate trigger", LogLevel.DEVD);
                return;
            }

            lastTrigger = (value, valueType, selectedPrefab);
            UpdateValueType updateType = (UpdateValueType)valueType;
            LogHelper.SendLog("Triggering Modify", LogLevel.DEVD);
            selectedPrefabModifier.Modify(selectedPrefab, value, updateType);
            EntityManager.AddComponent<Updated>(selectedEntity);
            RequestUpdate();
        }

        public void ResetComponentValue(int valueType)
        {
            // do all
            if (!Enum.IsDefined(typeof(UpdateValueType), (ushort)valueType))
                return;
            lastTrigger = ("", 0, Entity.Null);
            UpdateValueType updateType = (UpdateValueType)valueType;
            LogHelper.SendLog("Triggering Reset", LogLevel.DEVD);

            if (updateType == UpdateValueType._All)
            {
                selectedPrefabModifier.ResetAll(selectedPrefab);
                EntityManager.AddComponent<Updated>(selectedEntity);
                RequestUpdate();
                return;
            }

            if (
                !modifiedPrefabBufferSystem.TryGetEntryFromBuffer(
                    selectedPrefab,
                    updateType,
                    out var mods
                )
            )
                return;
            selectedPrefabModifier.Modify(
                selectedPrefab,
                $"(long){mods.Original}",
                updateType,
                true
            );
            EntityManager.AddComponent<Updated>(selectedEntity);
            RequestUpdate();
        }

        public void ChangeBCTValue(string value, int valueType)
        {
            if (!Enum.IsDefined(typeof(BldgCleanupType), valueType))
                return;

            BldgCleanupType resetType = (BldgCleanupType)valueType;
            LogHelper.SendLog("Triggering ChangeBCTValue", LogLevel.DEVD);
            selectedEntityModifier.ChangeCleanupValue(selectedEntity, value, resetType);
            RequestUpdate();
        }

        public void TriggerBCT(int valueType)
        {
            if (!Enum.IsDefined(typeof(BldgCleanupType), valueType))
                return;

            BldgCleanupType resetType = (BldgCleanupType)valueType;
            LogHelper.SendLog("Triggering TriggerBCT", LogLevel.DEVD);
            selectedEntityModifier.TriggerCleanup(selectedEntity, resetType);
            RequestUpdate();
        }

        public void MakeSP()
        {
            staticPloppableBuilder.MakeSP(selectedPrefab);
        }
    }
}
