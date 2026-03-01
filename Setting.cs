using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using AdvancedBuildingControl.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Colossal.PSI.Environment;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;
using StarQ.Shared.Extensions;

namespace AdvancedBuildingControl
{
    [FileLocation("ModsSettings\\StarQ\\" + nameof(AdvancedBuildingControl))]
    [SettingsUITabOrder(GeneralTab, AboutTab, LogTab)]
    [SettingsUIShowGroupName(CityComponentOverride)]
    public class Setting : ModSetting
    {
        public Setting(IMod mod)
            : base(mod) => SetDefaults();

        public const string GeneralTab = "GeneralTab";
        public const string GeneralGroup = "GeneralGroup";
        public const string CityComponentOverride = "CityComponentOverride";
        public const string StaticPloppableBuilder = "StaticPloppableBuilder";

        public const string AboutTab = "AboutTab";
        public const string InfoGroup = "InfoGroup";

        public const string LogTab = "LogTab";

        [Exclude]
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

            string[] files = Directory.GetFiles(
                $"{EnvPath.kUserDataPath}/ModsData/{Mod.Id}/BackupConfig",
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
        [SettingsUIDisableByCondition(typeof(Setting), nameof(NoSPCache))]
        [SettingsUISection(GeneralTab, StaticPloppableBuilder)]
        public bool DeleteLocalSPCache
        {
            set => WorldHelper.GetSystem<StaticPloppableData>().DeleteFile();
        }

        public override void SetDefaults() { }

        [SettingsUISection(AboutTab, InfoGroup)]
        public string NameText => Mod.Name;

        [SettingsUISection(AboutTab, InfoGroup)]
        public string VersionText => VariableHelper.AddDevSuffix(Mod.Version);

        [SettingsUISection(AboutTab, InfoGroup)]
        public string AuthorText => VariableHelper.StarQ;

        [SettingsUIButton]
        [SettingsUIButtonGroup("Social")]
        [SettingsUISection(AboutTab, InfoGroup)]
        public bool BMaCLink
        {
            set => VariableHelper.OpenBMAC();
        }

        [SettingsUIButton]
        [SettingsUIButtonGroup("Social")]
        [SettingsUISection(AboutTab, InfoGroup)]
        public bool Discord
        {
            set => VariableHelper.OpenDiscord("1464858378747645983");
        }

        [SettingsUIMultilineText]
        [SettingsUIDisplayName(typeof(LogHelper), nameof(LogHelper.LogText))]
        [SettingsUISection(LogTab, "")]
        public string LogText => string.Empty;

        [Exclude]
        [SettingsUIHidden]
        public bool IsLogMissing
        {
            get => VariableHelper.CheckLog(Mod.Id);
        }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsLogMissing))]
        [SettingsUISection(LogTab, "")]
        public bool OpenLog
        {
            set => VariableHelper.OpenLog(Mod.Id);
        }
    }
}
