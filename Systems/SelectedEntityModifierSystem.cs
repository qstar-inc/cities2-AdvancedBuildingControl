using System;
using System.Linq;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class SelectedEntityModifierSystem : GameSystemBase
    {
        Utils utils;

        protected override void OnCreate()
        {
            utils = WorldHelper.GetSystem<Utils>();
        }

        protected override void OnUpdate() { }

        public void RandomizeStyle(Entity entity)
        {
            try
            {
                if (!EntityManager.TryGetComponent(entity, out PseudoRandomSeed pseudoRandomSeed))
                    return;

                Random random = new();
                ushort randomUShort = (ushort)random.Next(0, 65536);
                pseudoRandomSeed.m_Seed = randomUShort;

                utils.AddAndUpdate(entity, pseudoRandomSeed);
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void ChangeBrand(Entity entity, string replaceBrand)
        {
            try
            {
                var match = DataRetriever.brandDataInfos.FirstOrDefault(b =>
                    b.PrefabName == replaceBrand
                );
                if (!EntityManager.Exists(entity) || replaceBrand == string.Empty || match == null)
                    return;

                EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Renter> renters);

                for (int i = 0; i < renters.Length; i++)
                {
                    Renter renter = renters[i];
                    Entity renterEntity = renter.m_Renter;

                    if (
                        !EntityManager.TryGetComponent(renterEntity, out CompanyData companyData)
                        && companyData.m_Brand.Equals(Entity.Null)
                    )
                        continue;

                    companyData.m_Brand = match.Entity;

                    utils.SetAndUpdate(renterEntity, entity, companyData);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }
    }
}
