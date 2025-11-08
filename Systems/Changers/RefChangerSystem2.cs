using System;
using AdvancedBuildingControl.Components;
using Colossal.Entities;
using Game.Economy;
using Game.Prefabs;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems
{
    public partial class RefChangerSystem
    {
        public void AddToNew(
            Entity entity,
            Entity currentPrefabRef,
            EntityChangeData entityChangeData,
            EntityChangeFlag entityChangeFlags,
            EntityAltComponents components
        )
        {
            var ogComponent = components.Original;
            string newName = !string.IsNullOrEmpty(ogComponent.OGEntity.ToString())
                ? ogComponent.OGEntity.ToString()
                : prefabSystem.GetPrefabName(currentPrefabRef);

            if (!utils.TryGetPrefabEntity(newName, out var _))
                return;

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            utils.CheckPrefabData(
                newPrefabEntity,
                entity,
                out bool hasStorage,
                out StorageCompanyData storageCompanyData,
                out bool isSpawnable,
                out SpawnableBuildingData spawnableBuildingData,
                out bool hasProperty,
                out BuildingPropertyData buildingPropertyData,
                out bool isWaterPump,
                out WaterPumpingStationData waterPumpingStationData,
                out bool isSewageDump,
                out SewageOutletData sewageOutletData,
                out bool isPowerProd,
                out PowerPlantData powerPlantData,
                out bool isDepot,
                out TransportDepotData transportDepotData,
                out bool isGarbageFacility,
                out GarbageFacilityData garbageFacilityData,
                out bool isHospital,
                out HospitalData hospitalData,
                out bool isDeathcare,
                out DeathcareFacilityData deathcareFacilityData
            );

            if (hasProperty)
            {
                bool skip = false;
                var component = components.Household;
                int resiPropTemp = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Household)
                        resiPropTemp = entityChangeData.Household;
                    else
                        skip = true;
                }
                else
                    resiPropTemp = component.Household;

                if (!skip && resiPropTemp != 0)
                {
                    buildingPropertyData.m_ResidentialProperties = resiPropTemp;

                    component.Household = resiPropTemp;
                    component.Enabled = true;
                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, buildingPropertyData);
                }
            }

            if (isSpawnable)
            {
                bool skip = false;
                var component = components.Level;
                byte levelTemp = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Level)
                        levelTemp = (byte)entityChangeData.Level;
                    else
                        skip = true;
                }
                else
                    levelTemp = (byte)component.Level;

                if (!skip && levelTemp != 0)
                {
                    SendIconCommand(spawnableBuildingData.m_Level, levelTemp, entity);
                    spawnableBuildingData.m_Level = levelTemp;

                    component.Level = levelTemp;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, spawnableBuildingData);
                    plopTheGrowableSystem.LockLevelWithPTG(entity);
                    utils.UpdateUpkeep(
                        newPrefabEntity,
                        entityChangeData.Level,
                        spawnableBuildingData.m_ZonePrefab
                    );
                }
            }

            if (hasStorage)
            {
                bool skip = false;
                var component = components.Storage;
                Resource resTemp = storageCompanyData.m_StoredResources;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Storage)
                    {
                        if ((resTemp & entityChangeData.ResId) != 0)
                            resTemp &= ~entityChangeData.ResId;
                        else
                            resTemp |= entityChangeData.ResId;
                    }
                    else
                        skip = true;
                }
                else
                    resTemp = (Resource)component.Resource;

                if (!skip)
                {
                    storageCompanyData.m_StoredResources = resTemp;

                    utils.RemoveCurrentResource(entity, storageCompanyData.m_StoredResources);

                    component.Resource = (ulong)storageCompanyData.m_StoredResources;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, storageCompanyData);
                }
            }

            if (isWaterPump)
            {
                bool skip = false;
                var component = components.WaterPump;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.WaterPump)
                        capacityValue = entityChangeData.WaterPumpCap;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Capacity;

                if (!skip && capacityValue != 0)
                {
                    if (component.IsDefault())
                        component.Original = waterPumpingStationData.m_Capacity;
                    waterPumpingStationData.m_Capacity = capacityValue;
                    component.Capacity = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, waterPumpingStationData);
                }
            }

            if (isSewageDump)
            {
                bool skip = false;
                var component = components.SewageDump;
                int capacityValue1 = 0;
                float capacityValue2 = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.SewageCap)
                    {
                        capacityValue1 = entityChangeData.SewageCap;
                        capacityValue2 = entityChangeData.SewagePurification;
                    }
                    else
                        skip = true;
                }
                else
                {
                    capacityValue1 = component.Capacity;
                    capacityValue2 = component.Purification;
                }

                if (!skip && capacityValue1 != 0)
                {
                    if (component.IsDefault())
                    {
                        component.OriginalCap = sewageOutletData.m_Capacity;
                        component.OriginalPurification = sewageOutletData.m_Purification;
                    }
                    sewageOutletData.m_Capacity = capacityValue1;
                    sewageOutletData.m_Purification = capacityValue2;
                    component.Capacity = capacityValue1;
                    component.Purification = capacityValue2;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, sewageOutletData);
                }
            }

            if (isPowerProd)
            {
                bool skip = false;
                var component = components.PowerProd;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.PowerProd)
                        capacityValue = entityChangeData.PowerProdCap;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Capacity;

                if (!skip && capacityValue != 0)
                {
                    if (component.IsDefault())
                        component.Original = powerPlantData.m_ElectricityProduction;
                    powerPlantData.m_ElectricityProduction = capacityValue;
                    component.Capacity = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, powerPlantData);
                }
            }

            if (isDepot)
            {
                bool skip = false;
                var component = components.TransportDepot;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Depot)
                        capacityValue = entityChangeData.DepotCap;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Capacity;

                if (!skip && capacityValue != 0)
                {
                    if (component.IsDefault())
                        component.Original = transportDepotData.m_VehicleCapacity;
                    transportDepotData.m_VehicleCapacity = capacityValue;
                    component.Capacity = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, transportDepotData);
                }
            }

            if (isGarbageFacility)
            {
                bool skip = false;
                var component = components.GarbageTruck;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.GarbageFacility)
                        capacityValue = entityChangeData.GarbageTruckCap;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Capacity;

                if (!skip && capacityValue != 0)
                {
                    if (component.IsDefault())
                        component.Original = garbageFacilityData.m_VehicleCapacity;
                    garbageFacilityData.m_VehicleCapacity = capacityValue;
                    component.Capacity = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, garbageFacilityData);
                }
            }

            if (isHospital)
            {
                bool skip1 = false;
                var component1 = components.Ambulance;
                int capacityValue1 = 0;
                ;
                if (component1.IsDefault())
                {
                    if (entityChangeFlags.Ambulance)
                        capacityValue1 = entityChangeData.AmbulanceCap;
                    else
                        skip1 = true;
                }
                else
                    capacityValue1 = component1.Capacity;

                if (!skip1 && capacityValue1 != 0)
                {
                    if (component1.IsDefault())
                        component1.Original = hospitalData.m_AmbulanceCapacity;
                    hospitalData.m_AmbulanceCapacity = capacityValue1;
                    component1.Capacity = capacityValue1;
                    component1.Enabled = true;
                }

                bool skip2 = false;
                var component2 = components.MediHeli;
                int capacityValue2 = 0;

                if (component2.IsDefault())
                {
                    if (entityChangeFlags.MediHeli)
                        capacityValue2 = entityChangeData.MediHeliCap;
                    else
                        skip2 = true;
                }
                else
                    capacityValue2 = component2.Capacity;

                if (!skip2 && capacityValue2 != 0)
                {
                    if (component2.IsDefault())
                        component2.Original = hospitalData.m_MedicalHelicopterCapacity;
                    hospitalData.m_MedicalHelicopterCapacity = capacityValue2;
                    component2.Capacity = capacityValue2;
                    component2.Enabled = true;
                }

                if (!skip1 || !skip2)
                {
                    EntityManager.AddComponentData(entity, component2);
                    EntityManager.SetComponentData(newPrefabEntity, hospitalData);
                }
            }

            if (isDeathcare)
            {
                bool skip = false;
                var component = components.Hearse;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Hearse)
                        capacityValue = entityChangeData.HearseCap;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Capacity;

                if (!skip && capacityValue != 0)
                {
                    if (component.IsDefault())
                        component.Original = deathcareFacilityData.m_HearseCapacity;
                    deathcareFacilityData.m_HearseCapacity = capacityValue;
                    component.Capacity = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, deathcareFacilityData);
                }
            }

            OriginalEntity originalEntity = new() { OGEntity = newName };
            EntityManager.AddComponentData(entity, originalEntity);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            //LogHelper.SendLog($"Done processing {entity}, adding {newPrefabEntity}", LogLevel.DEV);
        }

        public void UpdateExisting(
            Entity entity,
            Entity currentPrefabRef,
            EntityChangeData entityChangeData,
            EntityChangeFlag entityChangeFlags,
            EntityAltComponents components
        )
        {
            var ogComponent = components.Original;
            string newName = !string.IsNullOrEmpty(ogComponent.OGEntity.ToString())
                ? ogComponent.OGEntity.ToString()
                : prefabSystem.GetPrefabName(currentPrefabRef);

            if (!utils.TryGetPrefabEntity(newName, out var ogPrefabEntity))
                return;

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            utils.CheckPrefabData(
                newPrefabEntity,
                entity,
                out bool hasStorage,
                out StorageCompanyData storageCompanyData,
                out bool isSpawnable,
                out SpawnableBuildingData spawnableBuildingData,
                out bool hasProperty,
                out BuildingPropertyData buildingPropertyData,
                out bool isWaterPump,
                out WaterPumpingStationData waterPumpingStationData,
                out bool isSewageDump,
                out SewageOutletData sewageOutletData,
                out bool isPowerProd,
                out PowerPlantData powerPlantData,
                out bool isDepot,
                out TransportDepotData transportDepotData,
                out bool isGarbageFacility,
                out GarbageFacilityData garbageFacilityData,
                out bool isHospital,
                out HospitalData hospitalData,
                out bool isDeathcare,
                out DeathcareFacilityData deathcareFacilityData
            );

            if (hasProperty && entityChangeFlags.Household && components.HasHousehold)
            {
                ABC_Household ABC_Household = components.Household;
                buildingPropertyData.m_ResidentialProperties = Math.Max(
                    1,
                    entityChangeData.Household
                );
                ABC_Household.Household = buildingPropertyData.m_ResidentialProperties;
                ABC_Household.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_Household);
                EntityManager.SetComponentData(newPrefabEntity, buildingPropertyData);
            }

            if (isSpawnable && entityChangeFlags.Level && components.HasLevel)
            {
                ABC_Level ABC_Level = components.Level;
                SendIconCommand(spawnableBuildingData.m_Level, entityChangeData.Level, entity);
                spawnableBuildingData.m_Level = (byte)entityChangeData.Level;
                ABC_Level.Level = spawnableBuildingData.m_Level;
                ABC_Level.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_Level);
                EntityManager.SetComponentData(newPrefabEntity, spawnableBuildingData);

                plopTheGrowableSystem.LockLevelWithPTG(entity);
                utils.UpdateUpkeep(
                    newPrefabEntity,
                    entityChangeData.Level,
                    spawnableBuildingData.m_ZonePrefab
                );
            }

            if (hasStorage && entityChangeFlags.Storage && components.HasStorage)
            {
                ABC_Storage ABC_Storage = components.Storage;
                Resource resTemp = (Resource)ABC_Storage.Resource;

                if ((resTemp & entityChangeData.ResId) != 0)
                    resTemp &= ~entityChangeData.ResId;
                else
                    resTemp |= entityChangeData.ResId;

                storageCompanyData.m_StoredResources = resTemp;
                ABC_Storage.Resource = (ulong)resTemp;
                ABC_Storage.Enabled = true;

                Resource currentResources = storageCompanyData.m_StoredResources;

                utils.RemoveCurrentResource(entity, currentResources);

                EntityManager.AddComponentData(entity, ABC_Storage);
                EntityManager.SetComponentData(newPrefabEntity, storageCompanyData);
            }

            if (isWaterPump && entityChangeFlags.WaterPump && components.HasWaterPump)
            {
                ABC_WaterPump ABC_WaterPump = components.WaterPump;
                waterPumpingStationData.m_Capacity = entityChangeData.WaterPumpCap;
                ABC_WaterPump.Capacity = waterPumpingStationData.m_Capacity;
                ABC_WaterPump.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_WaterPump);
                EntityManager.SetComponentData(newPrefabEntity, waterPumpingStationData);
            }

            if (isSewageDump && components.HasSewageDump)
            {
                ABC_SewageDump ABC_SewageDump = components.SewageDump;
                if (entityChangeFlags.SewageCap)
                {
                    sewageOutletData.m_Capacity = entityChangeData.SewageCap;
                    ABC_SewageDump.Capacity = sewageOutletData.m_Capacity;
                }
                if (entityChangeFlags.SewagePurification)
                {
                    sewageOutletData.m_Purification = entityChangeData.SewagePurification;
                    ABC_SewageDump.Purification = sewageOutletData.m_Purification;
                }

                ABC_SewageDump.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_SewageDump);
                EntityManager.SetComponentData(newPrefabEntity, sewageOutletData);
            }

            if (isPowerProd && entityChangeFlags.PowerProd && components.HasPowerProd)
            {
                ABC_PowerPlant ABC_PowerPlant = components.PowerProd;
                powerPlantData.m_ElectricityProduction = entityChangeData.PowerProdCap;
                ABC_PowerPlant.Capacity = powerPlantData.m_ElectricityProduction;
                ABC_PowerPlant.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_PowerPlant);
                EntityManager.SetComponentData(newPrefabEntity, powerPlantData);
            }

            if (isDepot && entityChangeFlags.Depot && components.HasDepot)
            {
                ABC_TransportDepot ABC_TransportDepot = components.TransportDepot;
                transportDepotData.m_VehicleCapacity = entityChangeData.DepotCap;
                ABC_TransportDepot.Capacity = transportDepotData.m_VehicleCapacity;
                ABC_TransportDepot.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_TransportDepot);
                EntityManager.SetComponentData(newPrefabEntity, transportDepotData);
            }

            if (
                isGarbageFacility
                && entityChangeFlags.GarbageFacility
                && components.HasGarbageTruck
            )
            {
                ABC_GarbageTruck ABC_GarbageTruck = components.GarbageTruck;
                garbageFacilityData.m_VehicleCapacity = entityChangeData.GarbageTruckCap;
                ABC_GarbageTruck.Capacity = garbageFacilityData.m_VehicleCapacity;
                ABC_GarbageTruck.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_GarbageTruck);
                EntityManager.SetComponentData(newPrefabEntity, garbageFacilityData);
            }

            if (isHospital)
            {
                if (entityChangeFlags.Ambulance)
                {
                    ABC_Ambulance ABC_Ambulance = components.Ambulance;
                    hospitalData.m_AmbulanceCapacity = entityChangeData.AmbulanceCap;
                    ABC_Ambulance.Capacity = hospitalData.m_AmbulanceCapacity;
                    ABC_Ambulance.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_Ambulance);
                }
                if (entityChangeFlags.MediHeli)
                {
                    ABC_MediHeli ABC_MediHeli = components.MediHeli;
                    hospitalData.m_MedicalHelicopterCapacity = entityChangeData.MediHeliCap;
                    ABC_MediHeli.Capacity = hospitalData.m_MedicalHelicopterCapacity;
                    ABC_MediHeli.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_MediHeli);
                }
                EntityManager.SetComponentData(newPrefabEntity, hospitalData);
            }

            if (isDeathcare && entityChangeFlags.Hearse && components.HasHearse)
            {
                ABC_Hearse ABC_Hearse = components.Hearse;
                deathcareFacilityData.m_HearseCapacity = entityChangeData.HearseCap;
                ABC_Hearse.Capacity = deathcareFacilityData.m_HearseCapacity;
                ABC_Hearse.Enabled = true;

                EntityManager.AddComponentData(entity, ABC_Hearse);
                EntityManager.SetComponentData(newPrefabEntity, deathcareFacilityData);
            }

            OriginalEntity originalEntity = new() { OGEntity = newName };
            EntityManager.AddComponentData(entity, originalEntity);

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            //LogHelper.SendLog(
            //    $"Done processing {entity}, replacing {newPrefabEntity}",
            //    LogLevel.DEV
            //);
        }

        public void ResetToOG(
            Entity entity,
            Entity currentPrefabRef,
            EntityChangeFlag entityChangeFlags,
            EntityAltComponents components,
            bool keepComponent
        )
        {
            var ogComponent = components.Original;
            string newName = !string.IsNullOrEmpty(ogComponent.OGEntity.ToString())
                ? ogComponent.OGEntity.ToString()
                : prefabSystem.GetPrefabName(currentPrefabRef);

            if (!utils.TryGetPrefabEntity(newName, out var ogPrefabEntity))
                return;

            if (
                entityChangeFlags.Household
                && entityChangeFlags.Storage
                && entityChangeFlags.Level
                && entityChangeFlags.WaterPump
                && entityChangeFlags.SewageCap
                && entityChangeFlags.SewagePurification
                && entityChangeFlags.PowerProd
                && entityChangeFlags.Depot
                && entityChangeFlags.GarbageFacility
                && entityChangeFlags.Ambulance
                && entityChangeFlags.MediHeli
                && entityChangeFlags.Hearse
            )
            {
                if (!keepComponent)
                {
                    EntityManager.RemoveComponent<OriginalEntity>(entity);
                    EntityManager.RemoveComponent<ABC_Storage>(entity);
                    EntityManager.RemoveComponent<ABC_Level>(entity);
                    EntityManager.RemoveComponent<ABC_Household>(entity);
                    EntityManager.RemoveComponent<ABC_WaterPump>(entity);
                    EntityManager.RemoveComponent<ABC_SewageDump>(entity);
                    EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                    EntityManager.RemoveComponent<ABC_TransportDepot>(entity);
                    EntityManager.RemoveComponent<ABC_GarbageTruck>(entity);
                    EntityManager.RemoveComponent<ABC_Ambulance>(entity);
                    EntityManager.RemoveComponent<ABC_MediHeli>(entity);
                    EntityManager.RemoveComponent<ABC_Hearse>(entity);
                }
            }
            else
            {
                EntityChangeData entityChangeData = new();
                if (components.HasStorage && entityChangeFlags.Storage)
                {
                    ABC_Storage altStorage = components.Storage;
                    entityChangeData.ResId = (Resource)altStorage.Resource;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_Storage>(entity);
                        components.Storage = new();
                        components.HasStorage = false;
                    }
                }

                if (components.HasLevel && entityChangeFlags.Level)
                {
                    ABC_Level altLevel = components.Level;
                    entityChangeData.Level = altLevel.Level;
                    if (!keepComponent)
                    {
                        if (
                            EntityManager.TryGetComponent(
                                ogPrefabEntity,
                                out SpawnableBuildingData sbd
                            )
                        )
                            SendIconCommand(sbd.m_Level, entityChangeData.Level, entity);

                        EntityManager.RemoveComponent<ABC_Level>(entity);
                        components.Level = new();
                        components.HasLevel = false;
                    }
                }

                if (components.HasHousehold && entityChangeFlags.Household)
                {
                    ABC_Household altHousehold = components.Household;
                    entityChangeData.Household = altHousehold.Household;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_Household>(entity);
                        components.Household = new();
                        components.HasHousehold = false;
                    }
                }

                if (components.HasWaterPump && entityChangeFlags.WaterPump)
                {
                    ABC_WaterPump altWaterPump = components.WaterPump;
                    entityChangeData.WaterPumpCap = altWaterPump.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_WaterPump>(entity);
                        components.WaterPump = new();
                        components.HasWaterPump = false;
                    }
                }

                if (components.HasSewageDump)
                {
                    ABC_SewageDump altSewageDump = components.SewageDump;
                    if (entityChangeFlags.SewageCap)
                        entityChangeData.SewageCap = altSewageDump.Capacity;
                    if (entityChangeFlags.SewagePurification)
                        entityChangeData.SewagePurification = altSewageDump.Purification;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_SewageDump>(entity);
                        components.SewageDump = new();
                        components.HasSewageDump = false;
                    }
                }

                if (components.HasPowerProd && entityChangeFlags.PowerProd)
                {
                    ABC_PowerPlant altPowerProd = components.PowerProd;
                    entityChangeData.PowerProdCap = altPowerProd.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                        components.PowerProd = new();
                        components.HasPowerProd = false;
                    }
                }

                if (components.HasDepot && entityChangeFlags.Depot)
                {
                    ABC_TransportDepot altTransportDepot = components.TransportDepot;
                    entityChangeData.DepotCap = altTransportDepot.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_TransportDepot>(entity);
                        components.TransportDepot = new();
                        components.HasDepot = false;
                    }
                }

                if (components.HasGarbageTruck && entityChangeFlags.GarbageFacility)
                {
                    ABC_GarbageTruck altGarbageTruck = components.GarbageTruck;
                    entityChangeData.GarbageTruckCap = altGarbageTruck.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_GarbageTruck>(entity);
                        components.GarbageTruck = new();
                        components.HasGarbageTruck = false;
                    }
                }

                if (components.HasAmbu && entityChangeFlags.Ambulance)
                {
                    ABC_Ambulance ABC_Ambulance = components.Ambulance;
                    entityChangeData.AmbulanceCap = ABC_Ambulance.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_Ambulance>(entity);
                        components.Ambulance = new();
                        components.HasAmbu = false;
                    }
                }

                if (components.HasMediHeli && entityChangeFlags.MediHeli)
                {
                    ABC_MediHeli ABC_MediHeli = components.MediHeli;
                    entityChangeData.MediHeliCap = ABC_MediHeli.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_MediHeli>(entity);
                        components.MediHeli = new();
                        components.HasMediHeli = false;
                    }
                }

                if (components.HasHearse && entityChangeFlags.Hearse)
                {
                    ABC_Hearse ABC_Hearse = components.Hearse;
                    entityChangeData.HearseCap = ABC_Hearse.Capacity;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_Hearse>(entity);
                        components.Hearse = new();
                        components.HasHearse = false;
                    }
                }
            }

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = ogPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            //LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);
        }

        public void RemoveSingle(
            Entity entity,
            Entity currentPrefabRef,
            ValueType valueType,
            EntityAltComponents components
        )
        {
            var ogComponent = components.Original;
            string newName = !string.IsNullOrEmpty(ogComponent.OGEntity.ToString())
                ? ogComponent.OGEntity.ToString()
                : prefabSystem.GetPrefabName(currentPrefabRef);

            if (!utils.TryGetPrefabEntity(newName, out var ogPrefabEntity))
                return;

            Entity newPrefabEntity = createdEntitiesManagementSystem.CreateEntity(
                currentPrefabRef,
                newName
            );

            utils.CheckPrefabData(
                newPrefabEntity,
                entity,
                out bool hasStorage,
                out StorageCompanyData storageCompanyData,
                out bool isSpawnable,
                out SpawnableBuildingData spawnableBuildingData,
                out bool hasProperty,
                out BuildingPropertyData buildingPropertyData,
                out bool isWaterPump,
                out WaterPumpingStationData waterPumpingStationData,
                out bool isSewageDump,
                out SewageOutletData sewageOutletData,
                out bool isPowerProd,
                out PowerPlantData powerPlantData,
                out bool isDepot,
                out TransportDepotData transportDepotData,
                out bool isGarbageFacility,
                out GarbageFacilityData garbageFacilityData,
                out bool isHospital,
                out HospitalData hospitalData,
                out bool isDeathcare,
                out DeathcareFacilityData deathcareFacilityData
            );

            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out SpawnableBuildingData spawnableBuildingData0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out BuildingPropertyData buildingPropertyData0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out StorageCompanyData storageCompanyData0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out WaterPumpingStationData waterPumpingStationData0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out SewageOutletData sewageOutletData0);
            EntityManager.TryGetComponent(ogPrefabEntity, out PowerPlantData powerPlantData0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out TransportDepotData transportDepotData0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out GarbageFacilityData garbageFacilityData0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out HospitalData hospitalData0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out DeathcareFacilityData deathcareFacilityData0
            );

            ABC_Level ABC_Level = components.Level;
            ABC_Household ABC_Household = components.Household;
            ABC_Storage ABC_Storage = components.Storage;
            ABC_WaterPump ABC_WaterPump = components.WaterPump;
            ABC_SewageDump ABC_SewageDump = components.SewageDump;
            ABC_PowerPlant ABC_PowerPlant = components.PowerProd;
            ABC_TransportDepot ABC_TransportDepot = components.TransportDepot;
            ABC_GarbageTruck ABC_GarbageTruck = components.GarbageTruck;
            ABC_Ambulance ABC_Ambulance = components.Ambulance;
            ABC_MediHeli ABC_MediHeli = components.MediHeli;
            ABC_Hearse ABC_Hearse = components.Hearse;

            bool skipLevel = false,
                skipHousehold = false,
                skipStorage = false,
                skipWaterPump = false,
                skipSewageDump = false,
                skipPowerProd = false,
                skipDepot = false,
                skipGarbageTruck = false,
                skipAmbulance = false,
                skipMediHeli = false,
                skipHearse = false;

            switch (valueType)
            {
                case ValueType.Level:
                    if (!ABC_Level.IsDefault() && ABC_Level.Enabled && isSpawnable)
                    {
                        spawnableBuildingData.m_Level = spawnableBuildingData0.m_Level;
                        EntityManager.RemoveComponent<ABC_Level>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, spawnableBuildingData);
                    }
                    skipLevel = true;
                    break;
                case ValueType.Household:
                    if (!ABC_Household.IsDefault() && ABC_Household.Enabled && hasProperty)
                    {
                        buildingPropertyData.m_ResidentialProperties =
                            buildingPropertyData0.m_ResidentialProperties;
                        EntityManager.RemoveComponent<ABC_Household>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, buildingPropertyData);
                    }
                    skipHousehold = true;
                    break;
                case ValueType.Storage:
                    if (!ABC_Storage.IsDefault() && ABC_Storage.Enabled && hasStorage)
                    {
                        storageCompanyData.m_StoredResources =
                            storageCompanyData0.m_StoredResources;
                        EntityManager.RemoveComponent<ABC_Storage>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, storageCompanyData);
                    }
                    skipStorage = true;
                    break;
                case ValueType.WaterPump:
                    if (!ABC_WaterPump.IsDefault() && ABC_WaterPump.Enabled && isWaterPump)
                    {
                        waterPumpingStationData.m_Capacity = waterPumpingStationData0.m_Capacity;
                        EntityManager.RemoveComponent<ABC_WaterPump>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, waterPumpingStationData);
                    }
                    skipWaterPump = true;
                    break;
                case ValueType.SewageCap:
                    if (!ABC_SewageDump.IsDefault() && ABC_SewageDump.Enabled && isSewageDump)
                    {
                        sewageOutletData.m_Capacity = sewageOutletData0.m_Capacity;
                        sewageOutletData.m_Purification = sewageOutletData0.m_Purification;
                        EntityManager.RemoveComponent<ABC_SewageDump>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, sewageOutletData);
                    }
                    skipSewageDump = true;
                    break;
                case ValueType.PowerPlant:
                    if (!ABC_PowerPlant.IsDefault() && ABC_PowerPlant.Enabled && isPowerProd)
                    {
                        powerPlantData.m_ElectricityProduction =
                            powerPlantData0.m_ElectricityProduction;
                        EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, powerPlantData);
                    }
                    skipPowerProd = true;
                    break;
                case ValueType.Depot:
                    if (!ABC_TransportDepot.IsDefault() && ABC_TransportDepot.Enabled && isDepot)
                    {
                        transportDepotData.m_VehicleCapacity =
                            transportDepotData0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_TransportDepot>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, transportDepotData);
                    }
                    skipDepot = true;
                    break;
                case ValueType.GarbageTruck:
                    if (
                        !ABC_GarbageTruck.IsDefault()
                        && ABC_GarbageTruck.Enabled
                        && isGarbageFacility
                    )
                    {
                        garbageFacilityData.m_VehicleCapacity =
                            garbageFacilityData0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_GarbageTruck>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, garbageFacilityData);
                    }
                    skipGarbageTruck = true;
                    break;
                case ValueType.Ambulance:
                    if (!ABC_Ambulance.IsDefault() && ABC_Ambulance.Enabled && isHospital)
                    {
                        hospitalData.m_AmbulanceCapacity = hospitalData0.m_AmbulanceCapacity;
                        EntityManager.RemoveComponent<ABC_Ambulance>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, garbageFacilityData);
                    }
                    skipAmbulance = true;
                    break;
                case ValueType.MediHeli:
                    if (!ABC_MediHeli.IsDefault() && ABC_MediHeli.Enabled && isHospital)
                    {
                        hospitalData.m_MedicalHelicopterCapacity =
                            hospitalData0.m_MedicalHelicopterCapacity;
                        EntityManager.RemoveComponent<ABC_MediHeli>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, garbageFacilityData);
                    }
                    skipMediHeli = true;
                    break;
                case ValueType.Hearse:
                    if (!ABC_Hearse.IsDefault() && ABC_Hearse.Enabled && isDeathcare)
                    {
                        deathcareFacilityData.m_HearseCapacity =
                            deathcareFacilityData0.m_HearseCapacity;
                        EntityManager.RemoveComponent<ABC_Hearse>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, deathcareFacilityData);
                    }
                    skipHearse = true;
                    break;
                default:
                    LogHelper.SendLog($"Failed removing one of {entity}!", LogLevel.DEV);
                    return;
            }

            if (!skipLevel && !ABC_Level.IsDefault() && ABC_Level.Enabled && isSpawnable)
                spawnableBuildingData.m_Level = (byte)ABC_Level.Level;

            if (
                !skipHousehold
                && !ABC_Household.IsDefault()
                && ABC_Household.Enabled
                && hasProperty
            )
                buildingPropertyData.m_ResidentialProperties = ABC_Household.Household;

            if (!skipStorage && !ABC_Storage.IsDefault() && ABC_Storage.Enabled && hasStorage)
                storageCompanyData.m_StoredResources = (Resource)ABC_Storage.Resource;

            if (
                !skipWaterPump
                && !ABC_WaterPump.IsDefault()
                && ABC_WaterPump.Enabled
                && isWaterPump
            )
                waterPumpingStationData.m_Capacity = ABC_WaterPump.Capacity;

            if (
                !skipSewageDump
                && !ABC_SewageDump.IsDefault()
                && ABC_SewageDump.Enabled
                && isSewageDump
            )
            {
                sewageOutletData.m_Capacity = ABC_SewageDump.Capacity;
                sewageOutletData.m_Purification = ABC_SewageDump.Purification;
            }

            if (
                !skipPowerProd
                && !ABC_PowerPlant.IsDefault()
                && ABC_PowerPlant.Enabled
                && isPowerProd
            )
                powerPlantData.m_ElectricityProduction = ABC_PowerPlant.Capacity;

            if (
                !skipDepot
                && !ABC_TransportDepot.IsDefault()
                && ABC_TransportDepot.Enabled
                && isDepot
            )
                transportDepotData.m_VehicleCapacity = ABC_TransportDepot.Capacity;

            if (
                !skipGarbageTruck
                && !ABC_GarbageTruck.IsDefault()
                && ABC_GarbageTruck.Enabled
                && isGarbageFacility
            )
                garbageFacilityData.m_VehicleCapacity = ABC_GarbageTruck.Capacity;

            if (!skipAmbulance && !ABC_Ambulance.IsDefault() && ABC_Ambulance.Enabled && isHospital)
                hospitalData.m_AmbulanceCapacity = ABC_Ambulance.Capacity;

            if (!skipMediHeli && !ABC_MediHeli.IsDefault() && ABC_MediHeli.Enabled && isHospital)
                hospitalData.m_MedicalHelicopterCapacity = ABC_MediHeli.Capacity;

            if (!skipHearse && !ABC_Hearse.IsDefault() && ABC_Hearse.Enabled && isDeathcare)
                deathcareFacilityData.m_HearseCapacity = ABC_Hearse.Capacity;

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            //LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);
        }
    }
}
