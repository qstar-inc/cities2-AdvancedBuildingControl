using System;
using System.Linq;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Companies;
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
                Mod.log.Info(ex.ToString());
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
                Mod.log.Info(ex.ToString());
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
                    {
                        altMaxWorkplace = new() { Workplace = newMaxWorker, Enabled = true };
                        EntityManager.AddComponentData(entity, altMaxWorkplace);
                    }

                    workProvider.m_MaxWorkers = newMaxWorker;

                    EntityManager.SetComponentData(entity, workProvider);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
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
                    workProvider.m_MaxWorkers = altMaxWorkplace.Workplace;

                    EntityManager.RemoveComponent<ABC_Workplace>(entity);

                    EntityManager.SetComponentData(entity, workProvider);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }

        public void ChangeWaterPumpCapacity(Entity entity, int newCapacity)
        {
            try
            {
                if (entity == Entity.Null || newCapacity <= 0)
                    return;

                if (EntityManager.Exists(entity))
                {
                    EntityManager.TryGetComponent(
                        entity,
                        out WaterPumpingStation waterPumpingStation
                    );

                    EntityManager.TryGetComponent(entity, out ABC_WaterPump altWaterPumpingStation);

                    if (altWaterPumpingStation.IsDefault())
                    {
                        altWaterPumpingStation = new() { Capacity = newCapacity, Enabled = true };
                        EntityManager.AddComponentData(entity, altWaterPumpingStation);
                    }

                    waterPumpingStation.m_Capacity = newCapacity;

                    EntityManager.SetComponentData(entity, waterPumpingStation);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }

        public void ResetWaterPumpCapacity(Entity entity)
        {
            try
            {
                if (entity == Entity.Null)
                    return;

                if (
                    EntityManager.Exists(entity)
                    && EntityManager.TryGetComponent(
                        entity,
                        out WaterPumpingStation waterPumpingStation
                    )
                    && EntityManager.TryGetComponent(
                        entity,
                        out ABC_WaterPump altWaterPumpingStation
                    )
                    && !altWaterPumpingStation.IsDefault()
                )
                {
                    waterPumpingStation.m_Capacity = altWaterPumpingStation.Capacity;

                    EntityManager.RemoveComponent<ABC_WaterPump>(entity);

                    EntityManager.SetComponentData(entity, waterPumpingStation);
                    EntityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }
    }
}
