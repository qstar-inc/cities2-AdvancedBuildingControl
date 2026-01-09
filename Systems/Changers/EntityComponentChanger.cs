using System;
using System.Linq;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems.Changers
{
    public partial class EntityComponentChanger : GameSystemBase
    {
        protected override void OnUpdate() { }

        public void RandomizeStyle(Entity entity)
        {
            try
            {
                if (EntityManager.TryGetComponent(entity, out PseudoRandomSeed pseudoRandomSeed))
                {
                    Random random = new();
                    ushort randomUShort = (ushort)random.Next(0, 65536);
                    pseudoRandomSeed.m_Seed = randomUShort;

                    EntityManager.AddComponentData(entity, pseudoRandomSeed);
                    EntityManager.AddComponent<Updated>(entity);
                }
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
                if (entity == Entity.Null || replaceBrand == string.Empty || match == null)
                    return;

                if (EntityManager.Exists(entity))
                {
                    EntityManager.TryGetBuffer(entity, false, out DynamicBuffer<Renter> renters);

                    for (int i = 0; i < renters.Length; i++)
                    {
                        Renter renter = renters[i];
                        Entity renterEntity = renter.m_Renter;

                        if (
                            EntityManager.TryGetComponent(renterEntity, out CompanyData companyData)
                        )
                        {
                            if (companyData.m_Brand.Equals(Entity.Null))
                                continue;
                            companyData.m_Brand = match.Entity;

                            EntityManager.SetComponentData(renterEntity, companyData);
                            EntityManager.AddComponent<Updated>(entity);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void ChangeMaxWorkplace(Entity entity, int newMaxWorker)
        {
            try
            {
                if (entity == Entity.Null || newMaxWorker <= 0)
                    return;

                if (EntityManager.Exists(entity))
                {
                    EntityManager.TryGetComponent(entity, out WorkProvider workProvider);

                    EntityManager.TryGetComponent(entity, out ABC_Workplace altMaxWorkplace);

                    if (altMaxWorkplace.IsDefault())
                        altMaxWorkplace.Original = workProvider.m_MaxWorkers;

                    altMaxWorkplace.Modified = newMaxWorker;
                    altMaxWorkplace.Enabled = true;

                    EntityManager.AddComponentData(entity, altMaxWorkplace);

                    workProvider.m_MaxWorkers = newMaxWorker;

                    EntityManager.SetComponentData(entity, workProvider);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }

        public void ResetMaxWorkplace(Entity entity)
        {
            try
            {
                if (entity == Entity.Null)
                    return;

                if (
                    EntityManager.Exists(entity)
                    && EntityManager.TryGetComponent(entity, out WorkProvider workProvider)
                    && EntityManager.TryGetComponent(entity, out ABC_Workplace altMaxWorkplace)
                    && !altMaxWorkplace.IsDefault()
                )
                {
                    workProvider.m_MaxWorkers = altMaxWorkplace.Original;

                    EntityManager.RemoveComponent<ABC_Workplace>(entity);

                    EntityManager.SetComponentData(entity, workProvider);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SendLog(ex, LogLevel.Error);
            }
        }
    }
}
