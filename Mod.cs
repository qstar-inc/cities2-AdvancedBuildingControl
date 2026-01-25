using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AdvancedBuildingControl.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Colossal.PSI.Environment;
using Game;
using Game.Modding;
using Game.SceneFlow;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl
{
    public class Mod : IMod
    {
        public static string Id = nameof(AdvancedBuildingControl);
        public static string Name = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyTitleAttribute>()
            .Title;
        public static string Version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version.ToString(3);

        public static ILog log = LogManager.GetLogger($"{Id}").SetShowsErrorsInUI(false);
        public static Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            LogHelper.Init(Id, log);
            LocaleHelper.Init(Id, Name, GetReplacements);

            try
            {
                Directory.CreateDirectory($"{EnvPath.kUserDataPath}/ModsData/{Id}/BackupConfig");
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();

            AssetDatabase.global.LoadSettings(
                nameof(AdvancedBuildingControl),
                m_Setting,
                new Setting(this)
            );

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DataRetriever>();
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SaveStoreSystem>();
            updateSystem.UpdateAfter<SIP_ABC>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }

        public static Dictionary<string, string> GetReplacements()
        {
            return new() { { "X", "Y" } };
        }
    }
}
