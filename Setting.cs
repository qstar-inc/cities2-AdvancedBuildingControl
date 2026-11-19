using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AdvancedBuildingControl.Systems;
using Colossal.Json;
using Colossal.PSI.Environment;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;
using StarQ.Shared.Extensions;
using StarQ.Shared.Generators;
using UnityEngine;

namespace AdvancedBuildingControl
{
    [SettingsUIShowGroupName(CityComponentOverride)]
    [GenerateSettingCommonAttribute]
    public partial class Setting : ModSetting
    {
        public override void SetDefaults()
        {
            EffectOverlayR = Color.blue.r;
            EffectOverlayB = Color.blue.b;
            EffectOverlayG = Color.blue.g;
            EffectOverlayA = 0.08f;
        }

        public const string CityComponentOverride = "CityComponentOverride";
        public const string StaticPloppableBuilder = "StaticPloppableBuilder";
        public const string Effects = "Effects";

        [Exclude]
        [SettingsUIHidden]
        public bool hasBackup = false;

        [Exclude]
        [SettingsUIHidden]
        public bool InGame
        {
            get => !WorldHelper.IsGame;
        }

        [Exclude]
        [SettingsUIHidden]
        public bool NoSPCache { get; set; } = false;

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, CityComponentOverride)]
        public bool CityComponentOverride_BackupConfig
        {
            set => WorldHelper.GetSystem<BackupConfigSystem>().BackupConfig();
        }

        public DropdownItem<string>[] GetBackupFileList()
        {
            List<DropdownItem<string>> list = new();

            if (!Directory.Exists(Mod.BackupDir))
                return list.ToArray();

            string[] files = Directory.GetFiles(
                Mod.BackupDir,
                "*.json",
                SearchOption.TopDirectoryOnly
            );

            for (int i = files.Length - 1; i >= 0; i--)
            {
                string file = files[i];
                string fileName = Path.GetFileName(file);

                var regex = Regex.Match(fileName, @"ABC_Backup_([\d\-]+)_([\d\-]+)_(.+)\.json");

                string dName = fileName;
                if (regex.Success)
                    dName =
                        $"{regex.Groups[3]?.Value} : {regex.Groups[1]?.Value} {regex.Groups[2]?.Value}";

                list.Add(new DropdownItem<string> { value = file, displayName = dName });
            }

            if (list.Count > 0)
                hasBackup = true;

            list.Sort((a, b) => a.displayName.id.CompareTo(b.displayName.id));

            return list.ToArray();
        }

        [Exclude]
        [SettingsUIHidden]
        internal int DropdownVersion { get; set; } = 0;

        [Exclude]
        [SettingsUIValueVersion(typeof(Setting), nameof(DropdownVersion))]
        [SettingsUIDropdown(typeof(Setting), nameof(GetBackupFileList))]
        [SettingsUISection(GeneralTab, CityComponentOverride)]
        public string CityComponentOverride_BackupFileName { get; set; } = string.Empty;

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, CityComponentOverride)]
        public bool CityComponentOverride_RestoreConfig
        {
            set =>
                WorldHelper
                    .GetSystem<BackupConfigSystem>()
                    .RestoreConfig(CityComponentOverride_BackupFileName);
        }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, CityComponentOverride)]
        public bool CityComponentOverride_ResetAll
        {
            set => WorldHelper.GetSystem<SelectedPrefabModifierSystem>().ResetAll();
        }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(NoSPCache), true)]
        [SettingsUISection(GeneralTab, StaticPloppableBuilder)]
        public bool DeleteLocalSPCache
        {
            set => WorldHelper.GetSystem<StaticPloppableData>().DeleteFile();
        }

        [SettingsUIButton]
        [SettingsUISection(GeneralTab, Effects)]
        public bool ResetEffectColor
        {
            set =>
                WorldHelper
                    .GetSystem<EffectOverlaySystem>()
                    .SetColor(new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.08f));
        }

        [SettingsUIHidden]
        public float EffectOverlayR { get; set; } = Color.blue.r;

        [SettingsUIHidden]
        public float EffectOverlayB { get; set; } = Color.blue.b;

        [SettingsUIHidden]
        public float EffectOverlayG { get; set; } = Color.blue.g;

        [SettingsUIHidden]
        public float EffectOverlayA { get; set; } = 0.08f;

        //#if DEBUG
        //        [SettingsUIButton]
        //        [SettingsUIDisplayName(overrideValue: "Test")]
        //        [SettingsUISection(GeneralTab, StaticPloppableBuilder)]
        //        public bool Test
        //        {
        //            set => WorldHelper.GetSystem<PrefabFinder>().Test();
        //        }
        //#endif
    }
}
