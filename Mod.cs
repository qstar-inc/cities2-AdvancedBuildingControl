using System.Collections.Generic;
using System.IO;
using AdvancedBuildingControl.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.UI;
using Game;
using Game.Modding;
using Game.SceneFlow;
using StarQ.Shared.Extensions;
using StarQ.Shared.Generators;

namespace AdvancedBuildingControl
{
    [GenerateModInfo]
    public partial class Mod : IMod
    {
        public static string uiHostName = "starq-advanced-building-control";
        public static string BackupDir = "";

        public void OnLoad(UpdateSystem updateSystem)
        {
            LogHelper.Init(Id, log);
            LocaleHelper.Init(Id, Name, GetReplacements);
            //UIHostHelper.Init(Id, uiHostName);

            Directory.CreateDirectory(DataDir);
            BackupDir = Path.Combine(DataDir, "BackupConfig");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(Id, m_Setting, new Setting(this));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                UIManager.defaultUISystem.AddHostLocation(
                    uiHostName,
                    Path.Combine(Path.GetDirectoryName(asset.path), "Icons"),
                    false
                );

            WorldHelper.GetSystem<DataRetriever>();
            WorldHelper.GetSystem<SaveStoreSystem>();
            WorldHelper.GetSystem<StaticPloppableData>();
            WorldHelper.GetSystem<StaticPloppableBuilder>();
            WorldHelper.GetSystem<EffectControlSystem>();
            updateSystem.UpdateAt<EffectOverlaySystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAt<BuildingEditorUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAfter<SIPSystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            LocaleHelper.Dispose();
            m_Setting?.UnregisterInOptionsUI();
            m_Setting = null;
        }

        public static Dictionary<string, string> GetReplacements()
        {
            return new() { };
        }
    }
}
