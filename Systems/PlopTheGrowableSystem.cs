using System;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Game;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class PlopTheGrowableSystem : GameSystemBase
    {
        public static ComponentType _ptgLockedComponent;
        public Type? ptgLockType = null;
        public bool firstTime = true;

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (!firstTime)
                return;

            Mod.m_Setting.IsPTGInGame = ModHelper.IsModActive("PlopTheGrowables");

            if (
                AssetDatabase.global.TryGetAsset(
                    SearchFilter<ExecutableAsset>.ByCondition(asset =>
                        asset.isLoaded && asset.name.Equals("PlopTheGrowables")
                    ),
                    out ExecutableAsset ptgAsset
                )
            )
            {
                ptgLockType = ptgAsset?.assembly.GetType("PlopTheGrowables.LevelLocked", false);

                if (ptgLockType == null)
                {
                    Enabled = false;
                    return;
                }

                _ptgLockedComponent = new ComponentType(
                    ptgLockType,
                    ComponentType.AccessMode.ReadOnly
                );
                LogHelper.SendLog("PTG found", LogLevel.DEV);
            }
            firstTime = false;
        }

        protected override void OnUpdate() { }

        public void LockLevelWithPTG(Entity entity)
        {
            try
            {
                if (Mod.m_Setting.IsPTGInGame && ptgLockType != null)
                    EntityManager.AddComponent(entity, _ptgLockedComponent);
            }
            catch (Exception e)
            {
                LogHelper.SendLog(e);
            }
        }
    }
}
