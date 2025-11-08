using System.Collections.Generic;
using System.Reflection;
using AdvancedBuildingControl.Systems;
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
        public static string Name = "Advanced Building Control";
        public static string Version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version.ToString(3);
        public static string Author = "StarQ";
        public static ILog log = LogManager
            .GetLogger($"{nameof(AdvancedBuildingControl)}")
            .SetShowsErrorsInUI(true);

#nullable disable
        public static Setting Setting;

#nullable enable
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

            Setting = new Setting(this);
            //Setting.RegisterInOptionsUI();

            AssetDatabase.global.LoadSettings(
                nameof(AdvancedBuildingControl),
                Setting,
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
            if (Setting != null)
            {
                //Setting.UnregisterInOptionsUI();
                Setting = null;
            }
        }

        public static Dictionary<string, string> GetReplacements()
        {
            return new() { }; //{ "", "" } };
        }
    }
}
