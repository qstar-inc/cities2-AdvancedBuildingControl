using System.Collections.Generic;
using System.Reflection;
using AdvancedBuildingControl.Systems;
using Colossal.Logging;
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
        public static string Name = "Advanced Building Control";
        public static string Version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version.ToString(3);
        public static string Author = "StarQ";
        public static ILog log = LogManager
            .GetLogger($"{nameof(AdvancedBuildingControl)}")
            .SetShowsErrorsInUI(true);

        //public static string SessionGuid = string.Empty;

        //private Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            LogHelper.Init(Id, log);
            LocaleHelper.Init(Id, GetReplacements);
            foreach (var item in new LocaleHelper($"{Id}.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }

            GameManager.instance.localizationManager.onActiveDictionaryChanged +=
                LocaleHelper.OnActiveDictionaryChanged;

            //log.Info(nameof(OnLoad));

            //if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            //    log.Info($"Current mod asset at {asset.path}");

            //m_Setting = new Setting(this);
            //m_Setting.RegisterInOptionsUI();
            //GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            //AssetDatabase.global.LoadSettings(nameof(AdvancedBuildingControl), m_Setting, new Setting(this));
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DataRetriever>();
            //World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SessionDataSystem>();

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StorageChangerSystem>();
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LevelChangerSystem>();

            updateSystem.UpdateBefore<CreatedEntitiesManagementSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateBefore<PreSerializationSystem>(SystemUpdatePhase.Serialize);
            //updateSystem.UpdateBefore<PreSerializationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<PreDeserializationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<PreDeserializationSystem>(SystemUpdatePhase.Deserialize);
            updateSystem.UpdateAfter<SIPAdvancedBuildingControl>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<UpdateNextFrameSystem>(SystemUpdatePhase.Modification1);
            updateSystem.UpdateAt<UpdateNextFrameClearSystem>(SystemUpdatePhase.ModificationEnd);
        }

        public void OnDispose()
        {
            //log.Info(nameof(OnDispose));
            //if (m_Setting != null)
            //{
            //    m_Setting.UnregisterInOptionsUI();
            //    m_Setting = null;
            //}
        }

        public static Dictionary<string, string> GetReplacements()
        {
            return new() { }; //{ "", "" } };
        }
    }
}
