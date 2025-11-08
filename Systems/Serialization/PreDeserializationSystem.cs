using Game;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems.Serialization
{
    public partial class PreDeserializationSystem : GameSystemBase
    {
#nullable disable
        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;

        private RefChangerSystem refChangerSystem;

#nullable enable

        protected override void OnCreate()
        {
            createdEntitiesManagementSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
            refChangerSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RefChangerSystem>();
        }

        protected override void OnUpdate()
        {
            LogHelper.SendLog("Starting loading", LogLevel.DEV);
            //LogHelper.SendLog(
            //    $"Starting InitOnGameStart on PreDeserializationSystem OnUpdate",
            //    LogLevel.DEV
            //);
            createdEntitiesManagementSystem.DoUpdate();
            refChangerSystem.InitOnGameStart();
            LogHelper.SendLog("Ending loading", LogLevel.DEV);
        }
    }
}
