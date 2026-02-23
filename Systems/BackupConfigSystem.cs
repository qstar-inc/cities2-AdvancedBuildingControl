using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Colossal.PSI.Environment;
using Game;
using Game.City;
using Game.Prefabs;
using Newtonsoft.Json;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class BackupConfigSystem : GameSystemBase
    {
        BufferControlSystem bufferControlSystem;
        SelectedPrefabModifierSystem selectedPrefabModifierSystem;

        protected override void OnCreate()
        {
            bufferControlSystem = WorldHelper.GetSystem<BufferControlSystem>();
            selectedPrefabModifierSystem = WorldHelper.GetSystem<SelectedPrefabModifierSystem>();
        }

        //protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        //{
        //    base.OnGameLoadingComplete(purpose, mode);
        //    if (mode.IsGame()) { Mod.m_Setting.InGame = true; return; }
        //    Mod.m_Setting.InGame = false;
        //}

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
            if (!bufferControlSystem.TryGetBufferCopy(out NativeArray<ModifiedPrefab_T7> array))
                return;

            if (array.Length == 0)
            {
                LogHelper.SendLog("Nothing to Backup...");
                return;
            }

            Dictionary<string, List<BackupFormat>> dict = new();
            var time = DateTimeOffset.Now;

            for (int i = 0; i < array.Length; i++)
            {
                ModifiedPrefab_T7 entry = array[i];

                if (!EntityManager.TryGetComponent(entry.ModEntity, out PrefabRef prefabRef))
                    continue;

                if (!entry.IsEnabled)
                    continue;

                string prefabName = WorldHelper.PrefabSystem.GetPrefabName(prefabRef.m_Prefab);

                if (!dict.TryGetValue(prefabName, out List<BackupFormat> list))
                {
                    list = new List<BackupFormat>();
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

            var directory = $"{EnvPath.kUserDataPath}/ModsData/{Mod.Id}/BackupConfig";
            Directory.CreateDirectory(directory);

            string fileTime = time.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
            string fileName = $"ABC_Backup_{fileTime}_{cityName}.json";

            string fullPath = Path.Combine(directory, fileName);

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
            int restored = 0;
            int skipped = 0;

            foreach (var prefabPair in backup.data)
            {
                string prefabName = prefabPair.Key;

                if (
                    !WorldHelper
                        .GetSystem<PrefabFinder>()
                        .TryFindBuildingPrefab(prefabName, out Entity prefab)
                )
                {
                    skipped++;
                    LogHelper.SendLog($"Restore skipped: prefab '{prefabName}' not found");
                    continue;
                }

                foreach (var entry in prefabPair.Value)
                {
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
