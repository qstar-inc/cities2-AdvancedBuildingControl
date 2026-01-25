using System;
using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Game;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class PrefabFinder : GameSystemBase
    {
        protected override void OnUpdate() { }

        public bool TryFindPrefab(string name, out Entity entity)
        {
            entity = Entity.Null;
            try
            {
                PrefabSystem prefabSystem = WorldHelper.PrefabSystem;
                var eq3 = SystemAPI.QueryBuilder().WithAll<PrefabRef>().Build();
                var ents3 = eq3.ToEntityArray(Allocator.Temp);

                if (ents3.Length <= 0)
                    return false;

                var pm = AssetDatabase.global.GetAssets<PrefabAsset>();
                Dictionary<string, PrefabBase> prefabAssets = new();
                foreach (var pmItem in pm)
                {
                    PrefabBase? prefabBase = pmItem.GetInstance<PrefabBase>();

                    if (prefabBase is not BuildingPrefab)
                        continue;

                    if (prefabBase.name == name)
                    {
                        prefabSystem.TryGetEntity(prefabBase, out entity);
                        return true;
                    }
                }

                LogHelper.SendLog($"Unable to find {name} prefab");
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
            return false;
        }
    }
}
