using System;
using System.Linq;
using Colossal.Entities;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems.Changers
{
    public class BrandChangerSystem
    {
        public static void ChangeBrand(
            EntityManager entityManager,
            Entity entity,
            string replaceBrand
        )
        {
            try
            {
                var match = DataRetriever.brandDataInfos.FirstOrDefault(b =>
                    b.PrefabName == replaceBrand
                );
                if (entity == Entity.Null || replaceBrand == string.Empty || match == null)
                    return;

                if (entityManager.Exists(entity))
                {
                    entityManager.TryGetBuffer(entity, false, out DynamicBuffer<Renter> renters);

                    for (int i = 0; i < renters.Length; i++)
                    {
                        Renter renter = renters[i];
                        Entity renterEntity = renter.m_Renter;

                        if (
                            entityManager.TryGetComponent(renterEntity, out CompanyData companyData)
                        )
                        {
                            if (companyData.m_Brand.Equals(Entity.Null))
                                continue;
                            companyData.m_Brand = match.Entity;

                            entityManager.SetComponentData(renterEntity, companyData);
                            entityManager.AddComponent<Updated>(entity);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }
    }
}
