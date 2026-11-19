using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Enums;
using Game;
using Game.City;
using Game.UI.Localization;
using Newtonsoft.Json;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class BackupConfigSystem : GameSystemBase
    {
        ModifiedPrefabBufferSystem modifiedPrefabBufferSystem;
        SelectedPrefabModifierSystem selectedPrefabModifierSystem;

        protected override void OnCreate()
        {
            modifiedPrefabBufferSystem = WorldHelper.GetSystem<ModifiedPrefabBufferSystem>();
            selectedPrefabModifierSystem = WorldHelper.GetSystem<SelectedPrefabModifierSystem>();
        }

        protected override void OnUpdate() { }

        public class BackupFile
        {
            public string cityName;
            public string backup_time;
            public string mod_version;
            public Dictionary<string, BackupFormat[]> data;
        }

        public class BackupFormat
        {
            public int typeName;
            public long originalVal;
            public long modifiedVal;
        }

        public void BackupConfig()
        {
            if (!modifiedPrefabBufferSystem.TryGetBufferCopy(out NativeArray<ModifiedPrefab> array))
                return;

            if (array.Length == 0)
            {
                string message = $"This save contains no configuration. Nothing to backup.";
                DialogHelper.ShowMessageDialog(
                    Mod.Name,
                    LocalizedString.Value(message),
                    LocalizationHelper.GetLocaleId(LocalizationHelper.LocaleIdGettable.OK),
                    null
                );
                return;
            }

            Dictionary<string, List<BackupFormat>> dict = new();
            var time = DateTimeOffset.Now;

            for (int i = 0; i < array.Length; i++)
            {
                ModifiedPrefab entry = array[i];

                if (!entry.IsEnabled)
                    continue;

                if (!PrefabHelper.TryFindPrefabRef(entry.ModEntity, out Entity m_Prefab))
                    continue;

                string prefabName = WorldHelper.PrefabSystem.GetPrefabName(m_Prefab);

                if (!dict.TryGetValue(prefabName, out List<BackupFormat> list))
                {
                    list = new();
                    dict[prefabName] = list;
                }

                list.Add(
                    new BackupFormat
                    {
                        typeName = (int)entry.ValueType,
                        originalVal = entry.Original,
                        modifiedVal = entry.Modified,
                    }
                );
            }

            if (dict.Count == 0)
            {
                string message = $"This save contains no configuration. Nothing to backup.";
                DialogHelper.ShowMessageDialog(
                    Mod.Name,
                    LocalizedString.Value(message),
                    LocalizationHelper.GetLocaleId(LocalizationHelper.LocaleIdGettable.OK),
                    null
                );
                return;
            }

            string cityName = WorldHelper.GetSystem<CityConfigurationSystem>().cityName;

            var backup = new BackupFile
            {
                cityName = cityName,
                backup_time = time.ToString(
                    "yyyy-MM-dd HH:mm:ss zzz",
                    CultureInfo.InvariantCulture
                ),
                data = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()),
                mod_version = Mod.Version,
            };

            try
            {
                Directory.CreateDirectory(Mod.BackupDir);

                string fileTime = time.ToString(
                    "yyyy-MM-dd_HH-mm-ss",
                    CultureInfo.InvariantCulture
                );
                string fileName = StringHelper.SanitizeTextForPath(
                    $"ABC_Backup_{fileTime}_{cityName}.json"
                );

                string fullPath = Path.Combine(Mod.BackupDir, fileName);

                var options = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    Error = (sender, args) =>
                    {
                        LogHelper.SendLog(
                            $"Serialization error on property '{args.ErrorContext.Member}': {args.ErrorContext.Error.Message}"
                        );
                        args.ErrorContext.Handled = true;
                    },
                };

                string json = JsonConvert.SerializeObject(backup, options);
                File.WriteAllText(fullPath, json);
            }
            catch (Exception ex)
            {
                string message = $"Restore failed: Error backing up config.\n{ex.Message}";
                DialogHelper.ShowMessageDialog(
                    Mod.Name,
                    LocalizedString.Value(message),
                    LocalizationHelper.GetLocaleId(LocalizationHelper.LocaleIdGettable.OK),
                    null
                );
                return;
            }

            Mod.m_Setting.DropdownVersion++;
        }

        public void RestoreConfig(string filePath)
        {
            if (!File.Exists(filePath))
            {
                LogHelper.SendLog($"Restore failed: file not found: {filePath}");
                return;
            }

            BackupFile backup;

            try
            {
                backup = JsonConvert.DeserializeObject<BackupFile>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                LogHelper.SendLog($"Restore failed: JSON parse error: {ex.Message}");
                return;
            }

            if (backup?.data == null || backup.data.Count == 0)
            {
                LogHelper.SendLog("Restore skipped: backup data empty");
                return;
            }

            RestoreInternal(backup);
        }

        private void RestoreInternal(BackupFile backup)
        {
            PrefabFinder pf = WorldHelper.GetSystem<PrefabFinder>();
            int restored = 0;
            int skipped = 0;

            foreach (var prefabPair in backup.data)
            {
                if (string.IsNullOrEmpty(prefabPair.Key) || prefabPair.Value == null)
                    continue;

                string prefabName = prefabPair.Key;

                if (!pf.TryFindBuildingPrefab(prefabName, out Entity prefab))
                {
                    skipped++;
                    LogHelper.SendLog($"Restore skipped: prefab '{prefabName}' not found");
                    continue;
                }

                foreach (BackupFormat entry in prefabPair.Value)
                {
                    if (!Enum.IsDefined(typeof(UpdateValueType), entry.typeName))
                        continue;

                    UpdateValueType valueType = (UpdateValueType)entry.typeName;

                    selectedPrefabModifierSystem.Modify(
                        prefab,
                        $"(long){entry.modifiedVal}",
                        valueType
                    );
                    restored++;
                }
            }

            LogHelper.SendLog(
                $"Restore complete: {restored} entries restored, {skipped} prefabs skipped"
            );
        }
    }
}
