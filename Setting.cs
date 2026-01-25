using System;
using System.Collections.Generic;
using System.IO;
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
    public class Setting : ModSetting
    {
        public Setting(IMod mod)
            : base(mod) => SetDefaults();

        public const string GeneralTab = "GeneralTab";
        public const string GeneralGroup = "GeneralGroup";

        public const string AboutTab = "AboutTab";
        public const string InfoGroup = "InfoGroup";

        public const string LogTab = "LogTab";

        public bool hasBackup = false;
        public bool InGame => WorldHelper.IsGame;

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, GeneralGroup)]
        public bool BackupConfig
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

            foreach (var file in files)
            {
                list.Add(new DropdownItem<string> { value = file, displayName = file });
            }

            if (list.Count > 0)
                hasBackup = true;

            list.Sort((a, b) => a.displayName.id.CompareTo(b.displayName.id));

            return list.ToArray();
        }

        [Exclude]
        [SettingsUIDropdown(typeof(Setting), nameof(GetBackupFileList))]
        [SettingsUISection(GeneralTab, GeneralGroup)]
        public string BackupFileName { get; set; } = string.Empty;

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, GeneralGroup)]
        public bool RestoreConfig
        {
            set => WorldHelper.GetSystem<BackupConfigSystem>().RestoreConfig(BackupFileName);
        }

        [SettingsUIButton]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(InGame))]
        [SettingsUISection(GeneralTab, GeneralGroup)]
        public bool ResetAll
        {
            set => WorldHelper.GetSystem<SelectedPrefabModifierSystem>().ResetAll();
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

        //[SettingsUIButton]
        //[SettingsUIButtonGroup("Social")]
        //[SettingsUISection(AboutTab, InfoGroup)]
        //public bool Discord
        //{
        //    set => VariableHelper.OpenDiscord(XXXX);
        //}

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
