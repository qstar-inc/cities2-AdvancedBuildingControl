using System.Collections.Generic;
using System.Reflection;
using AdvancedBuildingControl.Systems;
using AdvancedBuildingControl.Systems.Changers;
using AdvancedBuildingControl.Systems.Serialization;
using Colossal.IO.AssetDatabase;
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
            foreach (var item in new LocaleHelper($"{Id}.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }

            GameManager.instance.localizationManager.onActiveDictionaryChanged +=
                LocaleHelper.OnActiveDictionaryChanged;

            m_Setting = new Setting(this);
            //Setting.RegisterInOptionsUI();

            AssetDatabase.global.LoadSettings(
                nameof(AdvancedBuildingControl),
                m_Setting,
                new Setting(this)
            );

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DataRetriever>();
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlopTheGrowableSystem>();

            //world.GetOrCreateSystemManaged<StorageChangerSystem>();
            //world.GetOrCreateSystemManaged<LevelChangerSystem>();
            //world.GetOrCreateSystemManaged<HouseholdChangerSystem>();
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RefChangerSystem>();

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();

            updateSystem.UpdateAfter<SIP_ABC>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAfter<SIP_ABC_District>(SystemUpdatePhase.UIUpdate);

            updateSystem.UpdateBefore<PreSerializationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<PreDeserializationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAfter<PreDeserializationSystem>(SystemUpdatePhase.Deserialize);

            updateSystem.UpdateAt<UpdateNextFrameSystem>(SystemUpdatePhase.Modification1);
            //updateSystem.UpdateAt<UpdateNextFrameClearSystem>(SystemUpdatePhase.ModificationEnd);
        }

        public void OnDispose()
        {
            if (m_Setting != null)
            {
                //Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }

        public static Dictionary<string, string> GetReplacements()
        {
            return new() { }; //{ "", "" } };
        }
    }
}
