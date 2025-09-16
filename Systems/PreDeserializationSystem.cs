using AdvancedBuildingControl.Components;
using Colossal.Serialization.Entities;
using Game;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class PreDeserializationSystem : GameSystemBase
    {
#nullable disable
        private StorageChangerSystem storageChangerSystem;

#nullable enable

        protected override void OnCreate()
        {
            storageChangerSystem = World.GetOrCreateSystemManaged<StorageChangerSystem>();
        }

        //protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        //{
        //    base.OnGameLoadingComplete(purpose, mode);

        //    LogHelper.SendLog($"Clearing alteds. Count {storageChangerSystem.GetAltereds().Count}");
        //    storageChangerSystem.GetAltereds().Clear();
        //}

        protected override void OnUpdate()
        {
            if (GameModeExtensions.IsGame(DataRetriever.gameMode))
            {
                LogHelper.SendLog($"Starting InitOnGameStart on PreDeserializationSystem OnUpdate");
                storageChangerSystem.InitOnGameStart();
            }
            else
            {
                LogHelper.SendLog(
                    $"Game mode is {DataRetriever.gameMode} for PreDeserializationSystem OnUpdate"
                );
            }
        }
    }
}
