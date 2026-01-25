using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIP_ABC : ExtendedInfoSectionBase
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
        public static BldgBrandInfo bldgBrandInfo = new();
        public static BldgComponentInfo bldgComponentInfo = new();
        public static List<BldgModifiedInfo> bldgModifiedInfo = new();

        private NameSystem nameSystem;
        private PrefabSystem prefabSystem;

        private SelectedPrefabModifierSystem selectedPrefabModifier;
        private SelectedEntityModifierSystem selectedEntityModifier;
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

            CreateTrigger("RandomizeStyle", RandomizeStyle);
            CreateTrigger<string>("SetBrand", SetBrand);
            CreateTrigger<string, int>("ChangeComponentValue", ChangeComponentValue);
            CreateTrigger<int>("ResetComponentValue", ResetComponentValue);

            Enabled = false;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = Visible();
        }

        public override void OnWriteProperties(IJsonWriter writer)
        {
            writer.PropertyName("bldgBrandInfo");
            BldgInfoJsonWriterExtensions.Write(writer, bldgBrandInfo);

            writer.PropertyName("bldgComponentInfo");
            BldgComponentInfoWriterExtensions.Write(writer, bldgComponentInfo);

            writer.PropertyName("bldgModifiedInfo");
            BldgModifiedInfoWriterExtensions.Write(writer, bldgModifiedInfo.ToArray());
        }

        protected override void Reset()
        {
            bldgBrandInfo = new();
            bldgComponentInfo = new();
            bldgModifiedInfo = new();

            companyEntity = Entity.Null;
        }

        private bool Visible()
        {
            bool isVisible = false;
            if (EntityManager.TryGetComponent(selectedEntity, out PrefabRef _))
            {
                if (!prefabSystem.TryGetPrefab(selectedPrefab, out PrefabBase _))
                    return false;

                if (EntityManager.HasComponent<BuildingData>(selectedPrefab))
                    isVisible = true;

                if (!isVisible)
                    return false;

                return true;
            }
            return false;
        }

        protected override void OnProcess()
        {
            Reset();
            if (
                !EntityManager.TryGetComponent(selectedEntity, out PrefabRef _)
                || !EntityManager.HasComponent<BuildingData>(selectedPrefab)
                || EntityManager.HasComponent<Abandoned>(selectedEntity)
                || EntityManager.HasComponent<Destroyed>(selectedEntity)
            )
                return;

            CheckBrand();
            CheckComponents();
            CheckCurrentModifications();
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

            bldgBrandInfo.BrandList = DataRetriever.brandDataInfos.ToArray();
            bldgBrandInfo.BrandName = nameSystem.GetRenderedLabelName(companyData.m_Brand);
            bldgBrandInfo.CompanyName = nameSystem
                .GetRenderedLabelName(companyEntity)
                .Replace("Assets.NAME[", "")
                .Replace("]", "");
            bldgBrandInfo.HasBrand = true;
            var brandInData = DataRetriever
                .brandDataInfos.Where(v => v.Entity == companyData.m_Brand)
                ?.First();
            bldgBrandInfo.BrandIcon = brandInData?.Icon ?? "";
        }

        public void CheckCurrentModifications()
        {
            //if (!EntityManager.HasComponent<LocalModified>(selectedPrefab))
            //    return;

            if (!selectedPrefabModifier.localEntities.TryGetValue(selectedPrefab, out var data))
                return;

            if (data.PrefabChanges == null)
                return;

            foreach (var prefabChanges in data.PrefabChanges)
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
                return;
            }

            if (
                !WorldHelper
                    .GetSystem<BufferControlSystem>()
                    .TryGetEntryFromBuffer(selectedPrefab, updateType, out var mods)
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
    }
}
