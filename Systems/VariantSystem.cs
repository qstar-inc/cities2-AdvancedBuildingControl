using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Colossal.Entities;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public class VariantSystem
    {
        public class VariantsData
        {
            public Entity Entity { get; set; }
        }

        public static string prefix = "StarQ ABC ";
        public static Dictionary<int, VariantsData> variantsData = new();

        public static (string, string, int) VariantNamer(
            PrefabSystem prefabSystem,
            string currentName,
            int ver = 0
        )
        {
            string ogName = $"{Regex.Replace(currentName.Replace(prefix, ""), @"\s\d{9}$", "")}";

            string vName = $"{prefix}{ogName}";

            string newName = $"{vName} {ver:D9}";
            while (prefabSystem.TryGetPrefab(new PrefabID("BuildingPrefab", newName), out _))
            {
                ver++;
                newName = $"{vName} {ver:D9}";
            }

            //BuildingVariants variants = new()
            //{
            //    Name = newName,
            //    OGName = ogName,
            //    VariantIndex = ver,
            //    Guid = SaveComponents.sessionGuid,
            //};

            return (newName, ogName, ver);

            //PrefabBase newPrefabBase = prefabSystem.DuplicatePrefab(currentPrefabBase, newName);
            //Entity newEntity = prefabSystem.GetEntity(newPrefabBase);

            //entityManager.AddComponent<Updated>(newEntity);
            //entityManager.AddComponent<Updated>(entity);
            //entityManager.AddComponentData(entity, variants);
            //entityManager.AddComponentData(entity, new PrefabRef() { m_Prefab = newEntity });

            //Mod.log.Info($"Created {newName} from {ogName}");

            //TODO: Store the ogPrefab info in a component and reload them on game load, also save to disk.
            //CityConfigurationSystem ccs = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
        }

        public static string UnPrefixify(string og)
        {
            if (og.StartsWith(prefix))
            {
                string withoutPrefix = og.Replace(prefix, "").TrimStart();

                int lastSpaceIndex = withoutPrefix.LastIndexOf(' ');
                if (lastSpaceIndex == -1)
                    return withoutPrefix;

                string namePart = withoutPrefix[..lastSpaceIndex];
                //string suffixPart = withoutPrefix[(lastSpaceIndex + 1)..];

                //if (ulong.TryParse(suffixPart, out ulong number))
                //{
                //    return $"{namePart} [Variant: {number}]";
                //}

                return namePart;
            }
            return og;
        }

        public static Entity CreateVariant(
            EntityManager entityManager,
            PrefabSystem prefabSystem,
            Entity entity,
            Entity prefab,
            bool replace = false
        )
        {
            if (prefab == Entity.Null)
            {
                entityManager.TryGetComponent<PrefabRef>(entity, out var component);
                prefab = component.m_Prefab;
            }
            if (prefab == Entity.Null)
            {
                LogHelper.SendLog(
                    $"Failed creating variant for {prefabSystem.GetPrefabName(entity)}"
                );
                return prefab;
            }

            prefabSystem.TryGetPrefab(prefab, out PrefabBase prefabBase);
            (string oldName, string newName, int version) = VariantNamer(
                prefabSystem,
                prefabBase.name
            );

            PrefabBase newPrefabBase = prefabSystem.DuplicatePrefab(prefabBase, newName);
            newPrefabBase.TryGet(out UIObject uiONew);

            if (uiONew?.m_Group != null)
            {
                uiONew.m_Group = null;
            }

            Entity newEntity = prefabSystem.GetEntity(newPrefabBase);

            entityManager.AddComponentData(entity, new PrefabRef() { m_Prefab = newEntity });

            return newEntity;
        }
    }
}
