using System;
using AdvancedBuildingControl.Components;
using AdvancedBuildingControl.Components.Vehicles;
using Colossal.Entities;
using Game.Economy;
using Game.Prefabs;
using Game.Vehicles;
using StarQ.Shared.Extensions;
using Unity.Entities;

namespace AdvancedBuildingControl.Systems.Changers
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
                out DeathcareFacilityData deathcareFacilityData,
                out bool isPoliceStation,
                out PoliceStationData policeStationData,
                out bool isPrison,
                out PrisonData prisonData,
                out bool isFireStation,
                out FireStationData fireStationData,
                out bool isEmergencyShelter,
                out EmergencyShelterData emergencyShelterData,
                out bool isPostFacility,
                out PostFacilityData postFacilityData,
                out bool isMaintenanceDepot,
                out MaintenanceDepotData maintenanceDepotData
            );

            if (hasProperty)
            {
                if (entityChangeData.Household > 0)
                {
                    Add2New_CheckNApply(
                        out bool skip,
                        ref components.Household,
                        ref buildingPropertyData.m_ResidentialProperties,
                        entityChangeData.Household,
                        entityChangeFlags.Household
                    );

                    if (!skip)
                    {
                        EntityManager.AddComponentData(entity, components.Household);
                        EntityManager.SetComponentData(newPrefabEntity, buildingPropertyData);
                    }
                }
            }

            if (isSpawnable)
            {
                bool skip = false;
                var component = components.Level;
                int capacityValue = 0;
                if (component.IsDefault())
                {
                    if (entityChangeFlags.Level)
                        capacityValue = entityChangeData.Level;
                    else
                        skip = true;
                }
                else
                    capacityValue = component.Modified;

                if (!skip && capacityValue != 0)
                {
                    SendIconCommand(spawnableBuildingData.m_Level, capacityValue, entity);
                    if (component.IsDefault())
                        component.Original = spawnableBuildingData.m_Level;
                    spawnableBuildingData.m_Level = (byte)capacityValue;
                    component.Modified = capacityValue;
                    component.Enabled = true;

                    EntityManager.AddComponentData(entity, component);
                    EntityManager.SetComponentData(newPrefabEntity, spawnableBuildingData);
                    plopTheGrowableSystem.LockLevelWithPTG(entity);
                    utils.UpdateUpkeep(
                        newPrefabEntity,
                        entityChangeData.Level,
                        spawnableBuildingData.m_ZonePrefab
                    );

                    //    bool skip = false;
                    //var component = components.Level;
                    //byte levelTemp = 0;
                    //if (component.IsDefault())
                    //{
                    //    component.Original = spawnableBuildingData.m_Level;
                    //    if (entityChangeFlags.Level)
                    //        levelTemp = (byte)entityChangeData.Level;
                    //    else
                    //        skip = true;
                    //}
                    //else
                    //    levelTemp = (byte)component.Level;

                    //if (!skip && levelTemp != 0)
                    //{
                    //    spawnableBuildingData.m_Level = levelTemp;

                    //    component.Level = levelTemp;
                    //    component.Enabled = true;

                    //    EntityManager.AddComponentData(entity, component);
                    //    EntityManager.SetComponentData(newPrefabEntity, spawnableBuildingData);
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
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.WaterPump,
                    ref waterPumpingStationData.m_Capacity,
                    entityChangeData.WaterPumpCap,
                    entityChangeFlags.WaterPump
                );

                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.WaterPump);
                    EntityManager.SetComponentData(newPrefabEntity, waterPumpingStationData);
                }
            }

            if (isSewageDump)
            {
                Add2New_CheckNApply(
                    out bool skip1,
                    ref components.SewageDump,
                    ref sewageOutletData.m_Capacity,
                    entityChangeData.SewageCap,
                    entityChangeFlags.SewageCap
                );
                Add2New_CheckNApply(
                    out bool skip2,
                    ref components.SewagePurification,
                    ref sewageOutletData.m_Purification,
                    entityChangeData.SewagePurification,
                    entityChangeFlags.SewagePurification
                );

                if (!skip1 || !skip2)
                {
                    if (!skip1)
                        EntityManager.AddComponentData(entity, components.SewageDump);
                    if (!skip2)
                        EntityManager.AddComponentData(entity, components.SewagePurification);
                    EntityManager.SetComponentData(newPrefabEntity, sewageOutletData);
                }
            }

            if (isPowerProd)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.PowerProd,
                    ref powerPlantData.m_ElectricityProduction,
                    entityChangeData.PowerProdCap,
                    entityChangeFlags.PowerProd
                );

                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.PowerProd);
                    EntityManager.SetComponentData(newPrefabEntity, powerPlantData);
                }
            }

            if (isDepot)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.DepotVehicle,
                    ref transportDepotData.m_VehicleCapacity,
                    entityChangeData.DepotVehicleCap,
                    entityChangeFlags.DepotVehicle
                );

                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.DepotVehicle);
                    EntityManager.SetComponentData(newPrefabEntity, transportDepotData);
                }
            }

            if (isGarbageFacility)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.GarbageTruck,
                    ref garbageFacilityData.m_VehicleCapacity,
                    entityChangeData.GarbageTruckCap,
                    entityChangeFlags.GarbageFacility
                );

                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.GarbageTruck);
                    EntityManager.SetComponentData(newPrefabEntity, garbageFacilityData);
                }
            }

            if (isHospital)
            {
                Add2New_CheckNApply(
                    out bool skip1,
                    ref components.Ambulance,
                    ref hospitalData.m_AmbulanceCapacity,
                    entityChangeData.AmbulanceCap,
                    entityChangeFlags.Ambulance
                );
                Add2New_CheckNApply(
                    out bool skip2,
                    ref components.MediHeli,
                    ref hospitalData.m_MedicalHelicopterCapacity,
                    entityChangeData.MediHeliCap,
                    entityChangeFlags.MediHeli
                );

                if (!skip1 || !skip2)
                {
                    if (!skip1)
                        EntityManager.AddComponentData(entity, components.Ambulance);
                    if (!skip2)
                        EntityManager.AddComponentData(entity, components.MediHeli);
                    EntityManager.SetComponentData(newPrefabEntity, hospitalData);
                }
            }

            if (isDeathcare)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.Hearse,
                    ref deathcareFacilityData.m_HearseCapacity,
                    entityChangeData.HearseCap,
                    entityChangeFlags.Hearse
                );

                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.Hearse);
                    EntityManager.SetComponentData(newPrefabEntity, deathcareFacilityData);
                }
            }

            if (isPoliceStation)
            {
                Add2New_CheckNApply(
                    out bool skip1,
                    ref components.PatrolCar,
                    ref policeStationData.m_PatrolCarCapacity,
                    entityChangeData.PatrolCarCap,
                    entityChangeFlags.PatrolCar
                );
                Add2New_CheckNApply(
                    out bool skip2,
                    ref components.PoliceHeli,
                    ref policeStationData.m_PoliceHelicopterCapacity,
                    entityChangeData.PoliceHeliCap,
                    entityChangeFlags.PoliceHeli
                );

                if (!skip1 || !skip2)
                {
                    if (!skip1)
                        EntityManager.AddComponentData(entity, components.PatrolCar);
                    if (!skip2)
                        EntityManager.AddComponentData(entity, components.PoliceHeli);
                    EntityManager.SetComponentData(newPrefabEntity, policeStationData);
                }
            }

            if (isPrison)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.PrisonVan,
                    ref prisonData.m_PrisonVanCapacity,
                    entityChangeData.PrisonVanCap,
                    entityChangeFlags.PrisonVan
                );
                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.PrisonVan);
                    EntityManager.SetComponentData(newPrefabEntity, prisonData);
                }
            }

            if (isFireStation)
            {
                Add2New_CheckNApply(
                    out bool skip1,
                    ref components.FireTruck,
                    ref fireStationData.m_FireEngineCapacity,
                    entityChangeData.FireTruckCap,
                    entityChangeFlags.FireTruck
                );
                Add2New_CheckNApply(
                    out bool skip2,
                    ref components.FireHeli,
                    ref fireStationData.m_FireHelicopterCapacity,
                    entityChangeData.FireHeliCap,
                    entityChangeFlags.FireHeli
                );
                if (!skip1 || !skip2)
                {
                    if (!skip1)
                        EntityManager.AddComponentData(entity, components.FireTruck);
                    if (!skip2)
                        EntityManager.AddComponentData(entity, components.FireHeli);
                    EntityManager.SetComponentData(newPrefabEntity, fireStationData);
                }
            }
            if (isEmergencyShelter)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.EvacBus,
                    ref emergencyShelterData.m_VehicleCapacity,
                    entityChangeData.EvacBusCap,
                    entityChangeFlags.EvacBus
                );
                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.EvacBus);
                    EntityManager.SetComponentData(newPrefabEntity, emergencyShelterData);
                }
            }
            if (isPostFacility)
            {
                Add2New_CheckNApply(
                    out bool skip1,
                    ref components.PostVan,
                    ref postFacilityData.m_PostVanCapacity,
                    entityChangeData.PostVanCap,
                    entityChangeFlags.PostVan
                );
                Add2New_CheckNApply(
                    out bool skip2,
                    ref components.PostTruck,
                    ref postFacilityData.m_PostTruckCapacity,
                    entityChangeData.PostTruckCap,
                    entityChangeFlags.PostTruck
                );
                if (!skip1 || !skip2)
                {
                    if (!skip1)
                        EntityManager.AddComponentData(entity, components.PostVan);
                    if (!skip2)
                        EntityManager.AddComponentData(entity, components.PostTruck);
                    EntityManager.SetComponentData(newPrefabEntity, postFacilityData);
                }
            }
            if (isMaintenanceDepot)
            {
                Add2New_CheckNApply(
                    out bool skip,
                    ref components.MaintenanceVehicle,
                    ref maintenanceDepotData.m_VehicleCapacity,
                    entityChangeData.MaintenanceVehicleCap,
                    entityChangeFlags.MaintenanceVehicle
                );
                if (!skip)
                {
                    EntityManager.AddComponentData(entity, components.MaintenanceVehicle);
                    EntityManager.SetComponentData(newPrefabEntity, maintenanceDepotData);
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
                out DeathcareFacilityData deathcareFacilityData,
                out bool isPoliceStation,
                out PoliceStationData policeStationData,
                out bool isPrison,
                out PrisonData prisonData,
                out bool isFireStation,
                out FireStationData fireStationData,
                out bool isEmergencyShelter,
                out EmergencyShelterData emergencyShelterData,
                out bool isPostFacility,
                out PostFacilityData postFacilityData,
                out bool isMaintenanceDepot,
                out MaintenanceDepotData maintenanceDepotData
            );

            if (hasProperty && components.HasHousehold)
            {
                if (entityChangeFlags.Household)
                {
                    ABC_Household ABC_Household = components.Household;
                    buildingPropertyData.m_ResidentialProperties = Math.Max(
                        1,
                        entityChangeData.Household
                    );
                    ABC_Household.Modified = buildingPropertyData.m_ResidentialProperties;
                    ABC_Household.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_Household);
                }
                EntityManager.SetComponentData(newPrefabEntity, buildingPropertyData);
            }

            if (isSpawnable && components.HasLevel)
            {
                if (entityChangeFlags.Level)
                {
                    ABC_Level ABC_Level = components.Level;
                    SendIconCommand(spawnableBuildingData.m_Level, entityChangeData.Level, entity);
                    spawnableBuildingData.m_Level = (byte)entityChangeData.Level;
                    ABC_Level.Modified = spawnableBuildingData.m_Level;
                    ABC_Level.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_Level);
                }
                EntityManager.SetComponentData(newPrefabEntity, spawnableBuildingData);

                plopTheGrowableSystem.LockLevelWithPTG(entity);
                utils.UpdateUpkeep(
                    newPrefabEntity,
                    entityChangeData.Level,
                    spawnableBuildingData.m_ZonePrefab
                );
            }

            if (hasStorage && components.HasStorage)
            {
                if (entityChangeFlags.Storage)
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
                }
                EntityManager.SetComponentData(newPrefabEntity, storageCompanyData);
            }

            if (isWaterPump && components.HasWaterPump)
            {
                if (entityChangeFlags.WaterPump)
                {
                    ABC_WaterPump ABC_WaterPump = components.WaterPump;
                    waterPumpingStationData.m_Capacity = entityChangeData.WaterPumpCap;
                    ABC_WaterPump.Modified = waterPumpingStationData.m_Capacity;
                    ABC_WaterPump.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_WaterPump);
                }
                EntityManager.SetComponentData(newPrefabEntity, waterPumpingStationData);
            }

            if (isSewageDump && (components.HasSewageDump || components.HasSewagePurification))
            {
                if (entityChangeFlags.SewageCap)
                {
                    ABC_SewageDump ABC_SewageDump = components.SewageDump;
                    sewageOutletData.m_Capacity = entityChangeData.SewageCap;
                    ABC_SewageDump.Modified = sewageOutletData.m_Capacity;
                    ABC_SewageDump.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_SewageDump);
                }
                if (entityChangeFlags.SewagePurification)
                {
                    ABC_SewagePurification ABC_SewagePurification = components.SewagePurification;
                    sewageOutletData.m_Purification = entityChangeData.SewagePurification;
                    ABC_SewagePurification.Modified = sewageOutletData.m_Purification;
                    ABC_SewagePurification.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_SewagePurification);
                }
                EntityManager.SetComponentData(newPrefabEntity, sewageOutletData);
            }

            if (isPowerProd && components.HasPowerProd)
            {
                if (entityChangeFlags.PowerProd)
                {
                    ABC_PowerPlant ABC_PowerPlant = components.PowerProd;
                    powerPlantData.m_ElectricityProduction = entityChangeData.PowerProdCap;
                    ABC_PowerPlant.Modified = powerPlantData.m_ElectricityProduction;
                    ABC_PowerPlant.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_PowerPlant);
                }
                EntityManager.SetComponentData(newPrefabEntity, powerPlantData);
            }

            if (isDepot && components.HasDepotVehicle)
            {
                if (entityChangeFlags.DepotVehicle)
                {
                    ABC_DepotVehicle ABC_TransportDepot = components.DepotVehicle;
                    transportDepotData.m_VehicleCapacity = entityChangeData.DepotVehicleCap;
                    ABC_TransportDepot.Modified = transportDepotData.m_VehicleCapacity;
                    ABC_TransportDepot.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_TransportDepot);
                }
                EntityManager.SetComponentData(newPrefabEntity, transportDepotData);
            }

            if (isGarbageFacility && components.HasGarbageTruck)
            {
                if (entityChangeFlags.GarbageFacility)
                {
                    ABC_GarbageTruck ABC_GarbageTruck = components.GarbageTruck;
                    garbageFacilityData.m_VehicleCapacity = entityChangeData.GarbageTruckCap;
                    ABC_GarbageTruck.Modified = garbageFacilityData.m_VehicleCapacity;
                    ABC_GarbageTruck.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_GarbageTruck);
                }
                EntityManager.SetComponentData(newPrefabEntity, garbageFacilityData);
            }

            if (isHospital && (components.HasAmbu || components.HasMediHeli))
            {
                if (entityChangeFlags.Ambulance)
                {
                    ABC_Ambulance ABC_Ambulance = components.Ambulance;
                    hospitalData.m_AmbulanceCapacity = entityChangeData.AmbulanceCap;
                    ABC_Ambulance.Modified = hospitalData.m_AmbulanceCapacity;
                    ABC_Ambulance.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_Ambulance);
                }
                if (entityChangeFlags.MediHeli)
                {
                    ABC_MediHeli ABC_MediHeli = components.MediHeli;
                    hospitalData.m_MedicalHelicopterCapacity = entityChangeData.MediHeliCap;
                    ABC_MediHeli.Modified = hospitalData.m_MedicalHelicopterCapacity;
                    ABC_MediHeli.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_MediHeli);
                }
                EntityManager.SetComponentData(newPrefabEntity, hospitalData);
            }

            if (isDeathcare && components.HasHearse)
            {
                if (entityChangeFlags.Hearse)
                {
                    ABC_Hearse ABC_Hearse = components.Hearse;
                    deathcareFacilityData.m_HearseCapacity = entityChangeData.HearseCap;
                    ABC_Hearse.Modified = deathcareFacilityData.m_HearseCapacity;
                    ABC_Hearse.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_Hearse);
                }
                EntityManager.SetComponentData(newPrefabEntity, deathcareFacilityData);
            }

            if (isPoliceStation && (components.HasPatrolCar || components.HasPoliceHeli))
            {
                if (entityChangeFlags.PatrolCar)
                {
                    ABC_PatrolCar ABC_PatrolCar = components.PatrolCar;
                    policeStationData.m_PatrolCarCapacity = entityChangeData.PatrolCarCap;
                    ABC_PatrolCar.Modified = policeStationData.m_PatrolCarCapacity;
                    ABC_PatrolCar.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_PatrolCar);
                }
                if (entityChangeFlags.PoliceHeli)
                {
                    ABC_PoliceHeli ABC_PoliceHeli = components.PoliceHeli;
                    policeStationData.m_PoliceHelicopterCapacity = entityChangeData.PoliceHeliCap;
                    ABC_PoliceHeli.Modified = policeStationData.m_PoliceHelicopterCapacity;
                    ABC_PoliceHeli.Enabled = true;

                    EntityManager.AddComponentData(entity, ABC_PoliceHeli);
                }
                EntityManager.SetComponentData(newPrefabEntity, policeStationData);
            }

            if (isPrison && components.HasPrisonVan)
            {
                if (entityChangeFlags.PrisonVan)
                {
                    ABC_PrisonVan ABC_PrisonVan = components.PrisonVan;
                    prisonData.m_PrisonVanCapacity = entityChangeData.PrisonVanCap;
                    ABC_PrisonVan.Modified = prisonData.m_PrisonVanCapacity;
                    ABC_PrisonVan.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_PrisonVan);
                }
                EntityManager.SetComponentData(newPrefabEntity, prisonData);
            }

            if (isFireStation && (components.HasFireTruck || components.HasFireHeli))
            {
                if (entityChangeFlags.FireTruck)
                {
                    ABC_FireTruck ABC_FireTruck = components.FireTruck;
                    fireStationData.m_FireEngineCapacity = entityChangeData.FireTruckCap;
                    ABC_FireTruck.Modified = fireStationData.m_FireEngineCapacity;
                    ABC_FireTruck.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_FireTruck);
                }
                if (entityChangeFlags.FireHeli)
                {
                    ABC_FireHeli ABC_FireHeli = components.FireHeli;
                    fireStationData.m_FireHelicopterCapacity = entityChangeData.FireHeliCap;
                    ABC_FireHeli.Modified = fireStationData.m_FireHelicopterCapacity;
                    ABC_FireHeli.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_FireHeli);
                }
                EntityManager.SetComponentData(newPrefabEntity, fireStationData);
            }

            if (isEmergencyShelter && components.HasEvacBus)
            {
                if (entityChangeFlags.EvacBus)
                {
                    ABC_EvacBus ABC_EvacBus = components.EvacBus;
                    emergencyShelterData.m_VehicleCapacity = entityChangeData.EvacBusCap;
                    ABC_EvacBus.Modified = emergencyShelterData.m_VehicleCapacity;
                    ABC_EvacBus.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_EvacBus);
                }
                EntityManager.SetComponentData(newPrefabEntity, emergencyShelterData);
            }

            if (isPostFacility && (components.HasPostVan || components.HasPostTruck))
            {
                if (entityChangeFlags.PostVan)
                {
                    ABC_PostVan ABC_PostVan = components.PostVan;
                    postFacilityData.m_PostVanCapacity = entityChangeData.PostVanCap;
                    ABC_PostVan.Modified = postFacilityData.m_PostVanCapacity;
                    ABC_PostVan.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_PostVan);
                }
                if (entityChangeFlags.PostTruck)
                {
                    ABC_PostTruck ABC_PostTruck = components.PostTruck;
                    postFacilityData.m_PostTruckCapacity = entityChangeData.PostTruckCap;
                    ABC_PostTruck.Modified = postFacilityData.m_PostTruckCapacity;
                    ABC_PostTruck.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_PostTruck);
                }
                EntityManager.SetComponentData(newPrefabEntity, postFacilityData);
            }

            if (isMaintenanceDepot && (components.HasMaintenanceVehicle || components.HasPostTruck))
            {
                if (entityChangeFlags.MaintenanceVehicle)
                {
                    ABC_MaintenanceVehicle ABC_MaintenanceVehicle = components.MaintenanceVehicle;
                    maintenanceDepotData.m_VehicleCapacity = entityChangeData.MaintenanceVehicleCap;
                    ABC_MaintenanceVehicle.Modified = maintenanceDepotData.m_VehicleCapacity;
                    ABC_MaintenanceVehicle.Enabled = true;
                    EntityManager.AddComponentData(entity, ABC_MaintenanceVehicle);
                }
                EntityManager.SetComponentData(newPrefabEntity, maintenanceDepotData);
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
                && entityChangeFlags.DepotVehicle
                && entityChangeFlags.GarbageFacility
                && entityChangeFlags.Ambulance
                && entityChangeFlags.MediHeli
                && entityChangeFlags.Hearse
                && entityChangeFlags.PatrolCar
                && entityChangeFlags.PoliceHeli
                && entityChangeFlags.PrisonVan
                && entityChangeFlags.FireTruck
                && entityChangeFlags.FireHeli
                && entityChangeFlags.EvacBus
                && entityChangeFlags.PostVan
                && entityChangeFlags.PostTruck
                && entityChangeFlags.MaintenanceVehicle
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
                    EntityManager.RemoveComponent<ABC_SewagePurification>(entity);
                    EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                    EntityManager.RemoveComponent<ABC_DepotVehicle>(entity);
                    EntityManager.RemoveComponent<ABC_GarbageTruck>(entity);
                    EntityManager.RemoveComponent<ABC_Ambulance>(entity);
                    EntityManager.RemoveComponent<ABC_MediHeli>(entity);
                    EntityManager.RemoveComponent<ABC_Hearse>(entity);
                    EntityManager.RemoveComponent<ABC_PatrolCar>(entity);
                    EntityManager.RemoveComponent<ABC_PoliceHeli>(entity);
                    EntityManager.RemoveComponent<ABC_PrisonVan>(entity);
                    EntityManager.RemoveComponent<ABC_FireTruck>(entity);
                    EntityManager.RemoveComponent<ABC_FireHeli>(entity);
                    EntityManager.RemoveComponent<ABC_EvacBus>(entity);
                    EntityManager.RemoveComponent<ABC_PostVan>(entity);
                    EntityManager.RemoveComponent<ABC_PostTruck>(entity);
                    EntityManager.RemoveComponent<ABC_MaintenanceVehicle>(entity);
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
                    entityChangeData.Level = altLevel.Modified;
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
                    entityChangeData.Household = altHousehold.Modified;
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
                    entityChangeData.WaterPumpCap = altWaterPump.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_WaterPump>(entity);
                        components.WaterPump = new();
                        components.HasWaterPump = false;
                    }
                }

                if (components.HasSewageDump && entityChangeFlags.SewageCap)
                {
                    ABC_SewageDump ABC_SewageDump = components.SewageDump;
                    entityChangeData.SewageCap = ABC_SewageDump.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_SewageDump>(entity);
                        components.SewageDump = new();
                        components.HasSewageDump = false;
                    }
                }

                if (components.HasSewagePurification && entityChangeFlags.SewagePurification)
                {
                    ABC_SewagePurification ABC_SewagePurification = components.SewagePurification;
                    entityChangeData.SewagePurification = ABC_SewagePurification.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_SewagePurification>(entity);
                        components.SewagePurification = new();
                        components.HasSewagePurification = false;
                    }
                }

                if (components.HasPowerProd && entityChangeFlags.PowerProd)
                {
                    ABC_PowerPlant altPowerProd = components.PowerProd;
                    entityChangeData.PowerProdCap = altPowerProd.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                        components.PowerProd = new();
                        components.HasPowerProd = false;
                    }
                }

                if (components.HasDepotVehicle && entityChangeFlags.DepotVehicle)
                {
                    ABC_DepotVehicle altTransportDepot = components.DepotVehicle;
                    entityChangeData.DepotVehicleCap = altTransportDepot.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_DepotVehicle>(entity);
                        components.DepotVehicle = new();
                        components.HasDepotVehicle = false;
                    }
                }

                if (components.HasGarbageTruck && entityChangeFlags.GarbageFacility)
                {
                    ABC_GarbageTruck altGarbageTruck = components.GarbageTruck;
                    entityChangeData.GarbageTruckCap = altGarbageTruck.Modified;
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
                    entityChangeData.AmbulanceCap = ABC_Ambulance.Modified;
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
                    entityChangeData.MediHeliCap = ABC_MediHeli.Modified;
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
                    entityChangeData.HearseCap = ABC_Hearse.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_Hearse>(entity);
                        components.Hearse = new();
                        components.HasHearse = false;
                    }
                }

                if (components.HasPatrolCar && entityChangeFlags.PatrolCar)
                {
                    ABC_PatrolCar ABC_PatrolCar = components.PatrolCar;
                    entityChangeData.PatrolCarCap = ABC_PatrolCar.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PatrolCar>(entity);
                        components.PatrolCar = new();
                        components.HasAmbu = false;
                    }
                }

                if (components.HasPoliceHeli && entityChangeFlags.PoliceHeli)
                {
                    ABC_PoliceHeli ABC_PoliceHeli = components.PoliceHeli;
                    entityChangeData.PoliceHeliCap = ABC_PoliceHeli.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PoliceHeli>(entity);
                        components.PoliceHeli = new();
                        components.HasPoliceHeli = false;
                    }
                }

                if (components.HasPrisonVan && entityChangeFlags.PrisonVan)
                {
                    ABC_PrisonVan ABC_PrisonVan = components.PrisonVan;
                    entityChangeData.PrisonVanCap = ABC_PrisonVan.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PrisonVan>(entity);
                        components.PrisonVan = new();
                        components.HasPrisonVan = false;
                    }
                }

                if (components.HasFireTruck && entityChangeFlags.FireTruck)
                {
                    ABC_FireTruck ABC_FireTruck = components.FireTruck;
                    entityChangeData.FireTruckCap = ABC_FireTruck.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_FireTruck>(entity);
                        components.FireTruck = new();
                        components.HasFireTruck = false;
                    }
                }

                if (components.HasFireHeli && entityChangeFlags.FireHeli)
                {
                    ABC_FireHeli ABC_FireHeli = components.FireHeli;
                    entityChangeData.FireHeliCap = ABC_FireHeli.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_FireHeli>(entity);
                        components.FireHeli = new();
                        components.HasFireHeli = false;
                    }
                }

                if (components.HasEvacBus && entityChangeFlags.EvacBus)
                {
                    ABC_EvacBus ABC_EvacBus = components.EvacBus;
                    entityChangeData.EvacBusCap = ABC_EvacBus.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_EvacBus>(entity);
                        components.EvacBus = new();
                        components.HasEvacBus = false;
                    }
                }

                if (components.HasPostVan && entityChangeFlags.PostVan)
                {
                    ABC_PostVan ABC_PostVan = components.PostVan;
                    entityChangeData.PostVanCap = ABC_PostVan.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PostVan>(entity);
                        components.PostVan = new();
                        components.HasPostVan = false;
                    }
                }

                if (components.HasPostTruck && entityChangeFlags.PostTruck)
                {
                    ABC_PostTruck ABC_PostTruck = components.PostTruck;
                    entityChangeData.PostTruckCap = ABC_PostTruck.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_PostTruck>(entity);
                        components.PostTruck = new();
                        components.HasPostTruck = false;
                    }
                }

                if (components.HasMaintenanceVehicle && entityChangeFlags.MaintenanceVehicle)
                {
                    ABC_MaintenanceVehicle ABC_MaintenanceVehicle = components.MaintenanceVehicle;
                    entityChangeData.MaintenanceVehicleCap = ABC_MaintenanceVehicle.Modified;
                    if (!keepComponent)
                    {
                        EntityManager.RemoveComponent<ABC_MaintenanceVehicle>(entity);
                        components.MaintenanceVehicle = new();
                        components.HasMaintenanceVehicle = false;
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
            UpdateValueType valueType,
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
                out DeathcareFacilityData deathcareFacilityData,
                out bool isPoliceStation,
                out PoliceStationData policeStationData,
                out bool isPrison,
                out PrisonData prisonData,
                out bool isFireStation,
                out FireStationData fireStationData,
                out bool isEmergencyShelter,
                out EmergencyShelterData emergencyShelterData,
                out bool isPostFacility,
                out PostFacilityData postFacility,
                out bool isMaintenanceDepot,
                out MaintenanceDepotData maintenanceDepotData
            );

            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out SpawnableBuildingData spawnableBuildingData_0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out BuildingPropertyData buildingPropertyData_0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out StorageCompanyData storageCompanyData_0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out WaterPumpingStationData waterPumpingStationData_0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out SewageOutletData sewageOutletData_0);
            EntityManager.TryGetComponent(ogPrefabEntity, out PowerPlantData powerPlantData_0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out TransportDepotData transportDepotData_0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out GarbageFacilityData garbageFacilityData_0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out HospitalData hospitalData_0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out DeathcareFacilityData deathcareFacilityData_0
            );
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out PoliceStationData policeStationData_0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out PrisonData prisonData_0);
            EntityManager.TryGetComponent(ogPrefabEntity, out FireStationData fireStationData_0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out EmergencyShelterData emergencyShelterData_0
            );
            EntityManager.TryGetComponent(ogPrefabEntity, out PostFacilityData postFacility_0);
            EntityManager.TryGetComponent(
                ogPrefabEntity,
                out MaintenanceDepotData maintenanceDepotData_0
            );

            ABC_Level ABC_Level = components.Level;
            ABC_Household ABC_Household = components.Household;
            ABC_Storage ABC_Storage = components.Storage;
            ABC_WaterPump ABC_WaterPump = components.WaterPump;
            ABC_SewageDump ABC_SewageDump = components.SewageDump;
            ABC_SewagePurification ABC_SewagePurification = components.SewagePurification;
            ABC_PowerPlant ABC_PowerPlant = components.PowerProd;
            ABC_DepotVehicle ABC_TransportDepot = components.DepotVehicle;
            ABC_GarbageTruck ABC_GarbageTruck = components.GarbageTruck;
            ABC_Ambulance ABC_Ambulance = components.Ambulance;
            ABC_MediHeli ABC_MediHeli = components.MediHeli;
            ABC_Hearse ABC_Hearse = components.Hearse;
            ABC_PatrolCar ABC_PatrolCar = components.PatrolCar;
            ABC_PoliceHeli ABC_PoliceHeli = components.PoliceHeli;
            ABC_PrisonVan ABC_PrisonVan = components.PrisonVan;
            ABC_FireTruck ABC_FireTruck = components.FireTruck;
            ABC_FireHeli ABC_FireHeli = components.FireHeli;
            ABC_EvacBus ABC_EvacBus = components.EvacBus;
            ABC_PostVan ABC_PostVan = components.PostVan;
            ABC_PostTruck ABC_PostTruck = components.PostTruck;
            ABC_MaintenanceVehicle ABC_MaintenanceVehicle = components.MaintenanceVehicle;

            bool skipLevel = false,
                skipHousehold = false,
                skipStorage = false,
                skipWaterPump = false,
                skipSewageDump = false,
                skipSewagePurification = false,
                skipPowerProd = false,
                skipDepot = false,
                skipGarbageTruck = false,
                skipAmbulance = false,
                skipMediHeli = false,
                skipHearse = false,
                skipPatrolCar = false,
                skipPoliceHeli = false,
                skipPrisonVan = false,
                skipFireTruck = false,
                skipFireHeli = false,
                skipEvacBus = false,
                skipPostVan = false,
                skipPostTruck = false,
                skipMaintenanceVehicle = false;

            switch (valueType)
            {
                case UpdateValueType.Level:
                    if (!ABC_Level.IsDefault() && ABC_Level.Enabled && isSpawnable)
                    {
                        spawnableBuildingData.m_Level = spawnableBuildingData_0.m_Level;
                        EntityManager.RemoveComponent<ABC_Level>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, spawnableBuildingData);
                    }
                    skipLevel = true;
                    break;
                case UpdateValueType.Household:
                    if (!ABC_Household.IsDefault() && ABC_Household.Enabled && hasProperty)
                    {
                        buildingPropertyData.m_ResidentialProperties =
                            buildingPropertyData_0.m_ResidentialProperties;
                        EntityManager.RemoveComponent<ABC_Household>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, buildingPropertyData);
                    }
                    skipHousehold = true;
                    break;
                case UpdateValueType.Storage:
                    if (!ABC_Storage.IsDefault() && ABC_Storage.Enabled && hasStorage)
                    {
                        storageCompanyData.m_StoredResources =
                            storageCompanyData_0.m_StoredResources;
                        EntityManager.RemoveComponent<ABC_Storage>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, storageCompanyData);
                    }
                    skipStorage = true;
                    break;
                case UpdateValueType.WaterPump:
                    if (!ABC_WaterPump.IsDefault() && ABC_WaterPump.Enabled && isWaterPump)
                    {
                        waterPumpingStationData.m_Capacity = waterPumpingStationData_0.m_Capacity;
                        EntityManager.RemoveComponent<ABC_WaterPump>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, waterPumpingStationData);
                    }
                    skipWaterPump = true;
                    break;
                case UpdateValueType.SewageCap:
                    if (!ABC_SewageDump.IsDefault() && ABC_SewageDump.Enabled && isSewageDump)
                    {
                        sewageOutletData.m_Capacity = sewageOutletData_0.m_Capacity;
                        EntityManager.RemoveComponent<ABC_SewageDump>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, sewageOutletData);
                    }
                    skipSewageDump = true;
                    break;
                case UpdateValueType.SewagePurification:
                    if (
                        !ABC_SewageDump.IsDefault()
                        && ABC_SewagePurification.Enabled
                        && isSewageDump
                    )
                    {
                        sewageOutletData.m_Purification = sewageOutletData_0.m_Purification;
                        EntityManager.RemoveComponent<ABC_SewagePurification>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, sewageOutletData);
                    }
                    skipSewageDump = true;
                    break;
                case UpdateValueType.PowerPlant:
                    if (!ABC_PowerPlant.IsDefault() && ABC_PowerPlant.Enabled && isPowerProd)
                    {
                        powerPlantData.m_ElectricityProduction =
                            powerPlantData_0.m_ElectricityProduction;
                        EntityManager.RemoveComponent<ABC_PowerPlant>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, powerPlantData);
                    }
                    skipPowerProd = true;
                    break;
                case UpdateValueType.DepotVehicle:
                    if (!ABC_TransportDepot.IsDefault() && ABC_TransportDepot.Enabled && isDepot)
                    {
                        transportDepotData.m_VehicleCapacity =
                            transportDepotData_0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_DepotVehicle>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, transportDepotData);
                    }
                    skipDepot = true;
                    break;
                case UpdateValueType.GarbageTruck:
                    if (
                        !ABC_GarbageTruck.IsDefault()
                        && ABC_GarbageTruck.Enabled
                        && isGarbageFacility
                    )
                    {
                        garbageFacilityData.m_VehicleCapacity =
                            garbageFacilityData_0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_GarbageTruck>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, garbageFacilityData);
                    }
                    skipGarbageTruck = true;
                    break;
                case UpdateValueType.Ambulance:
                    if (!ABC_Ambulance.IsDefault() && ABC_Ambulance.Enabled && isHospital)
                    {
                        hospitalData.m_AmbulanceCapacity = hospitalData_0.m_AmbulanceCapacity;
                        EntityManager.RemoveComponent<ABC_Ambulance>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, hospitalData);
                    }
                    skipAmbulance = true;
                    break;
                case UpdateValueType.MediHeli:
                    if (!ABC_MediHeli.IsDefault() && ABC_MediHeli.Enabled && isHospital)
                    {
                        hospitalData.m_MedicalHelicopterCapacity =
                            hospitalData_0.m_MedicalHelicopterCapacity;
                        EntityManager.RemoveComponent<ABC_MediHeli>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, hospitalData);
                    }
                    skipMediHeli = true;
                    break;
                case UpdateValueType.Hearse:
                    if (!ABC_Hearse.IsDefault() && ABC_Hearse.Enabled && isDeathcare)
                    {
                        deathcareFacilityData.m_HearseCapacity =
                            deathcareFacilityData_0.m_HearseCapacity;
                        EntityManager.RemoveComponent<ABC_Hearse>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, deathcareFacilityData);
                    }
                    skipHearse = true;
                    break;
                case UpdateValueType.PatrolCar:
                    if (!ABC_PatrolCar.IsDefault() && ABC_PatrolCar.Enabled && isPoliceStation)
                    {
                        policeStationData.m_PatrolCarCapacity =
                            policeStationData_0.m_PatrolCarCapacity;
                        EntityManager.RemoveComponent<ABC_PatrolCar>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, policeStationData);
                    }
                    skipPatrolCar = true;
                    break;
                case UpdateValueType.PoliceHeli:
                    if (!ABC_PoliceHeli.IsDefault() && ABC_PoliceHeli.Enabled && isPoliceStation)
                    {
                        policeStationData.m_PoliceHelicopterCapacity =
                            policeStationData_0.m_PoliceHelicopterCapacity;
                        EntityManager.RemoveComponent<ABC_PoliceHeli>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, policeStationData);
                    }
                    skipPoliceHeli = true;
                    break;
                //
                case UpdateValueType.PrisonVan:
                    if (!ABC_PrisonVan.IsDefault() && ABC_PrisonVan.Enabled && isPrison)
                    {
                        prisonData.m_PrisonVanCapacity = prisonData_0.m_PrisonVanCapacity;
                        EntityManager.RemoveComponent<ABC_PrisonVan>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, prisonData);
                    }
                    skipPrisonVan = true;
                    break;
                case UpdateValueType.FireTruck:
                    if (!ABC_FireTruck.IsDefault() && ABC_FireTruck.Enabled && isFireStation)
                    {
                        fireStationData.m_FireEngineCapacity =
                            fireStationData_0.m_FireEngineCapacity;
                        EntityManager.RemoveComponent<ABC_FireTruck>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, fireStationData);
                    }
                    skipFireTruck = true;
                    break;
                case UpdateValueType.FireHeli:
                    if (!ABC_FireHeli.IsDefault() && ABC_FireHeli.Enabled && isFireStation)
                    {
                        fireStationData.m_FireHelicopterCapacity =
                            fireStationData_0.m_FireHelicopterCapacity;
                        EntityManager.RemoveComponent<ABC_FireHeli>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, fireStationData);
                    }
                    skipFireHeli = true;
                    break;
                case UpdateValueType.EvacBus:
                    if (!ABC_EvacBus.IsDefault() && ABC_EvacBus.Enabled && isEmergencyShelter)
                    {
                        emergencyShelterData.m_VehicleCapacity =
                            emergencyShelterData_0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_EvacBus>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, emergencyShelterData);
                    }
                    skipEvacBus = true;
                    break;
                case UpdateValueType.PostVan:
                    if (!ABC_PostVan.IsDefault() && ABC_PostVan.Enabled && isPostFacility)
                    {
                        postFacility.m_PostVanCapacity = postFacility_0.m_PostVanCapacity;
                        EntityManager.RemoveComponent<ABC_PostVan>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, postFacility);
                    }
                    skipPostVan = true;
                    break;
                case UpdateValueType.PostTruck:
                    if (!ABC_PostTruck.IsDefault() && ABC_PostTruck.Enabled && isPostFacility)
                    {
                        postFacility.m_PostTruckCapacity = postFacility_0.m_PostTruckCapacity;
                        EntityManager.RemoveComponent<ABC_PostTruck>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, postFacility);
                    }
                    skipPostTruck = true;
                    break;
                case UpdateValueType.MaintenanceVehicle:
                    if (
                        !ABC_MaintenanceVehicle.IsDefault()
                        && ABC_MaintenanceVehicle.Enabled
                        && isMaintenanceDepot
                    )
                    {
                        maintenanceDepotData.m_VehicleCapacity =
                            maintenanceDepotData_0.m_VehicleCapacity;
                        EntityManager.RemoveComponent<ABC_MaintenanceVehicle>(entity);
                        EntityManager.AddComponentData(newPrefabEntity, maintenanceDepotData);
                    }
                    skipMaintenanceVehicle = true;
                    break;
                //
                default:
                    LogHelper.SendLog($"Failed removing one of {entity}!", LogLevel.DEV);
                    return;
            }

            if (!skipLevel && !ABC_Level.IsDefault() && ABC_Level.Enabled && isSpawnable)
                spawnableBuildingData.m_Level = (byte)ABC_Level.Modified;

            if (
                !skipHousehold
                && !ABC_Household.IsDefault()
                && ABC_Household.Enabled
                && hasProperty
            )
                buildingPropertyData.m_ResidentialProperties = ABC_Household.Modified;

            if (!skipStorage && !ABC_Storage.IsDefault() && ABC_Storage.Enabled && hasStorage)
                storageCompanyData.m_StoredResources = (Resource)ABC_Storage.Resource;

            if (
                !skipWaterPump
                && !ABC_WaterPump.IsDefault()
                && ABC_WaterPump.Enabled
                && isWaterPump
            )
                waterPumpingStationData.m_Capacity = ABC_WaterPump.Modified;

            if (
                !skipSewageDump
                && !ABC_SewageDump.IsDefault()
                && ABC_SewageDump.Enabled
                && isSewageDump
            )
                sewageOutletData.m_Capacity = ABC_SewageDump.Modified;

            if (
                !skipSewagePurification
                && !ABC_SewagePurification.IsDefault()
                && ABC_SewagePurification.Enabled
                && isSewageDump
            )
                sewageOutletData.m_Purification = ABC_SewagePurification.Modified;

            if (
                !skipPowerProd
                && !ABC_PowerPlant.IsDefault()
                && ABC_PowerPlant.Enabled
                && isPowerProd
            )
                powerPlantData.m_ElectricityProduction = ABC_PowerPlant.Modified;

            if (
                !skipDepot
                && !ABC_TransportDepot.IsDefault()
                && ABC_TransportDepot.Enabled
                && isDepot
            )
                transportDepotData.m_VehicleCapacity = ABC_TransportDepot.Modified;

            if (
                !skipGarbageTruck
                && !ABC_GarbageTruck.IsDefault()
                && ABC_GarbageTruck.Enabled
                && isGarbageFacility
            )
                garbageFacilityData.m_VehicleCapacity = ABC_GarbageTruck.Modified;

            if (!skipAmbulance && !ABC_Ambulance.IsDefault() && ABC_Ambulance.Enabled && isHospital)
                hospitalData.m_AmbulanceCapacity = ABC_Ambulance.Modified;

            if (!skipMediHeli && !ABC_MediHeli.IsDefault() && ABC_MediHeli.Enabled && isHospital)
                hospitalData.m_MedicalHelicopterCapacity = ABC_MediHeli.Modified;

            if (!skipHearse && !ABC_Hearse.IsDefault() && ABC_Hearse.Enabled && isDeathcare)
                deathcareFacilityData.m_HearseCapacity = ABC_Hearse.Modified;

            if (
                !skipPatrolCar
                && !ABC_PatrolCar.IsDefault()
                && ABC_PatrolCar.Enabled
                && isPoliceStation
            )
                policeStationData.m_PatrolCarCapacity = ABC_PatrolCar.Modified;

            if (
                !skipPoliceHeli
                && !ABC_PoliceHeli.IsDefault()
                && ABC_PoliceHeli.Enabled
                && isPoliceStation
            )
                policeStationData.m_PoliceHelicopterCapacity = ABC_PoliceHeli.Modified;

            if (!skipPrisonVan && !ABC_PrisonVan.IsDefault() && ABC_PrisonVan.Enabled && isPrison)
                prisonData.m_PrisonVanCapacity = ABC_PrisonVan.Modified;
            if (
                !skipFireTruck
                && !ABC_FireTruck.IsDefault()
                && ABC_FireTruck.Enabled
                && isFireStation
            )
                fireStationData.m_FireEngineCapacity = ABC_FireTruck.Modified;
            if (!skipFireHeli && !ABC_FireHeli.IsDefault() && ABC_FireHeli.Enabled && isFireStation)
                fireStationData.m_FireHelicopterCapacity = ABC_FireHeli.Modified;
            if (
                !skipEvacBus
                && !ABC_EvacBus.IsDefault()
                && ABC_EvacBus.Enabled
                && isEmergencyShelter
            )
                emergencyShelterData.m_VehicleCapacity = ABC_EvacBus.Modified;
            if (!skipPostVan && !ABC_PostVan.IsDefault() && ABC_PostVan.Enabled && isPostFacility)
                postFacility.m_PostVanCapacity = ABC_PostVan.Modified;
            if (
                !skipPostTruck
                && !ABC_PostTruck.IsDefault()
                && ABC_PostTruck.Enabled
                && isPostFacility
            )
                postFacility.m_PostTruckCapacity = ABC_PostTruck.Modified;
            if (
                !skipMaintenanceVehicle
                && !ABC_MaintenanceVehicle.IsDefault()
                && ABC_MaintenanceVehicle.Enabled
                && isMaintenanceDepot
            )
                maintenanceDepotData.m_VehicleCapacity = ABC_MaintenanceVehicle.Modified;

            EntityManager.SetComponentData(entity, new PrefabRef() { m_Prefab = newPrefabEntity });

            if (currentPrefabRef != ogPrefabEntity)
                createdEntitiesManagementSystem.DestroyEntity(newName, currentPrefabRef);

            EntityManager.AddComponent<UpdateNextFrame>(entity);
            //LogHelper.SendLog($"Done resetting {entity}, to {ogPrefabEntity}", LogLevel.DEV);
        }
    }
}
