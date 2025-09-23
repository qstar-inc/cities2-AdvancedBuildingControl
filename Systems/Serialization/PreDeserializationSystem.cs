using Game;
using StarQ.Shared.Extensions;

namespace AdvancedBuildingControl.Systems.Serialization
{
    public partial class PreDeserializationSystem : GameSystemBase
    {
#nullable disable
        public CreatedEntitiesManagementSystem createdEntitiesManagementSystem;

        //private StorageChangerSystem storageChangerSystem;
        //private LevelChangerSystem levelChangerSystem;
        //private HouseholdChangerSystem householdChangerSystem;
        private RefChangerSystem refChangerSystem;

#nullable enable

        protected override void OnCreate()
        {
            createdEntitiesManagementSystem =
                Mod.world.GetOrCreateSystemManaged<CreatedEntitiesManagementSystem>();
            //storageChangerSystem = Mod.world.GetOrCreateSystemManaged<StorageChangerSystem>();
            //levelChangerSystem = Mod.world.GetOrCreateSystemManaged<LevelChangerSystem>();
            //householdChangerSystem = Mod.world.GetOrCreateSystemManaged<HouseholdChangerSystem>();
            refChangerSystem = Mod.world.GetOrCreateSystemManaged<RefChangerSystem>();
        }

        protected override void OnUpdate()
        {
            LogHelper.SendLog(
                $"Starting InitOnGameStart on PreDeserializationSystem OnUpdate",
                LogLevel.DEV
            );
            createdEntitiesManagementSystem.DoUpdate();
            //storageChangerSystem.InitOnGameStart();
            //levelChangerSystem.InitOnGameStart();
            //householdChangerSystem.InitOnGameStart();
            refChangerSystem.InitOnGameStart();
        }
    }
}
