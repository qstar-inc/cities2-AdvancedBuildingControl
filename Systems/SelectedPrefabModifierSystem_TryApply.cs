using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game;
using Game.Economy;
using Game.Net;
using Game.Prefabs;
using Game.Simulation;
using Game.Vehicles;
using StarQ.Shared.Extensions;
using Unity.Entities;
using Unity.Mathematics;

namespace AdvancedBuildingControl.Systems
{
    public partial class SelectedPrefabModifierSystem : GameSystemBase
    {
        public bool TryApply(
            Entity selectedPrefab,
            UpdateValueType valueType,
            long modifiedValue,
            out long originalValue
        )
        {
            originalValue = -1L;
            LogHelper.SendLog($"TryApply: {valueType}, {modifiedValue}", LogLevel.DEVD);
            ComponentName componentName = UVTHelper.GetComponentName(valueType);
            AttractionData attractionData = default;
            BatteryData batteryData = default;
            BuildingPropertyData buildingPropertyData = default;
            CargoTransportStationData cargoTransportStationData = default;
            ConsumptionData consumptionData = default;
            CoverageData coverageData = default;
            DeathcareFacilityData deathcareFacilityData = default;
            DestructibleObjectData destructibleObjectData = default;
            EmergencyGeneratorData emergencyGeneratorData = default;
            EmergencyShelterData emergencyShelterData = default;
            FireStationData fireStationData = default;
            GarbageFacilityData garbageFacilityData = default;
            GarbagePoweredData garbagePoweredData = default;
            GroundWaterPoweredData groundWaterPoweredData = default;
            HospitalData hospitalData = default;
            MaintenanceDepotData maintenanceDepotData = default;
            ParkData parkData = default;
            ParkingFacilityData parkingFacilityData = default;
            PoliceStationData policeStationData = default;
            PostFacilityData postFacilityData = default;
            PowerPlantData powerPlantData = default;
            PrisonData prisonData = default;
            SchoolData schoolData = default;
            SewageOutletData sewageOutletData = default;
            SolarPoweredData solarPoweredData = default;
            SpawnableBuildingData spawnableBuildingData = default;
            StorageCompanyData storageCompanyData = default;
            TelecomFacilityData telecomFacilityData = default;
            TransportDepotData transportDepotData = default;
            TransportStationData transportStationData = default;
            WaterPoweredData waterPoweredData = default;
            WaterPumpingStationData waterPumpingStationData = default;
            WindPoweredData windPoweredData = default;
            WorkplaceData workplaceData = default;
            //end

            switch (componentName)
            {
                case ComponentName._None:
                    return false;

                //start
                case ComponentName.AttractionData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out attractionData))
                        return false;
                    break;
                case ComponentName.BatteryData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out batteryData))
                        return false;
                    break;
                case ComponentName.BuildingPropertyData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out buildingPropertyData))
                        return false;
                    break;
                case ComponentName.CargoTransportStationData:
                    if (
                        !EntityManager.TryGetComponent(
                            selectedPrefab,
                            out cargoTransportStationData
                        )
                    )
                        return false;
                    break;
                case ComponentName.ConsumptionData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out consumptionData))
                        return false;
                    break;
                case ComponentName.CoverageData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out coverageData))
                        return false;
                    break;
                case ComponentName.DeathcareFacilityData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out deathcareFacilityData))
                        return false;
                    break;
                case ComponentName.DestructibleObjectData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out destructibleObjectData))
                        return false;
                    break;
                case ComponentName.EmergencyGeneratorData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out emergencyGeneratorData))
                        return false;
                    break;
                case ComponentName.EmergencyShelterData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out emergencyShelterData))
                        return false;
                    break;
                case ComponentName.FireStationData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out fireStationData))
                        return false;
                    break;
                case ComponentName.GarbageFacilityData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out garbageFacilityData))
                        return false;
                    break;
                case ComponentName.GarbagePoweredData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out garbagePoweredData))
                        return false;
                    break;
                case ComponentName.GroundWaterPoweredData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out groundWaterPoweredData))
                        return false;
                    break;
                case ComponentName.HospitalData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out hospitalData))
                        return false;
                    break;
                case ComponentName.MaintenanceDepotData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out maintenanceDepotData))
                        return false;
                    break;
                case ComponentName.ParkData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out parkData))
                        return false;
                    break;
                case ComponentName.ParkingFacilityData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out parkingFacilityData))
                        return false;
                    break;
                case ComponentName.PoliceStationData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out policeStationData))
                        return false;
                    break;
                case ComponentName.PostFacilityData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out postFacilityData))
                        return false;
                    break;
                case ComponentName.PowerPlantData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out powerPlantData))
                        return false;
                    break;
                case ComponentName.PrisonData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out prisonData))
                        return false;
                    break;
                case ComponentName.SchoolData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out schoolData))
                        return false;
                    break;
                case ComponentName.SewageOutletData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out sewageOutletData))
                        return false;
                    break;
                case ComponentName.SolarPoweredData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out solarPoweredData))
                        return false;
                    break;
                case ComponentName.SpawnableBuildingData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out spawnableBuildingData))
                        return false;
                    break;
                case ComponentName.StorageCompanyData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out storageCompanyData))
                        return false;
                    break;
                case ComponentName.TelecomFacilityData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out telecomFacilityData))
                        return false;
                    break;
                case ComponentName.TransportDepotData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out transportDepotData))
                        return false;
                    break;
                case ComponentName.TransportStationData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out transportStationData))
                        return false;
                    break;
                case ComponentName.WaterPoweredData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out waterPoweredData))
                        return false;
                    break;
                case ComponentName.WaterPumpingStationData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out waterPumpingStationData))
                        return false;
                    break;
                case ComponentName.WindPoweredData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out windPoweredData))
                        return false;
                    break;
                case ComponentName.WorkplaceData:
                    if (!EntityManager.TryGetComponent(selectedPrefab, out workplaceData))
                        return false;
                    break;
                //end
                default:
                    break;
            }

            switch (valueType)
            {
                case UpdateValueType._None:
                    return true;
                case UpdateValueType._All:
                    return true;
                //start

                case UpdateValueType.AttractionData_Attractiveness:
                    int orig_AttractionData_Attractiveness = attractionData.m_Attractiveness;
                    originalValue = UVTHelper.ConvertToLong(orig_AttractionData_Attractiveness);
                    attractionData.m_Attractiveness = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, attractionData);
                    return true;

                case UpdateValueType.BatteryData_Capacity:
                    int orig_BatteryData_Capacity = batteryData.m_Capacity;
                    originalValue = UVTHelper.ConvertToLong(orig_BatteryData_Capacity);
                    batteryData.m_Capacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, batteryData);
                    return true;

                case UpdateValueType.BatteryData_PowerOutput:
                    int orig_BatteryData_PowerOutput = batteryData.m_PowerOutput;
                    originalValue = UVTHelper.ConvertToLong(orig_BatteryData_PowerOutput);
                    batteryData.m_PowerOutput = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, batteryData);
                    return true;

                case UpdateValueType.BuildingPropertyData_ResidentialProperties:
                    int orig_BuildingPropertyData_ResidentialProperties =
                        buildingPropertyData.m_ResidentialProperties;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_BuildingPropertyData_ResidentialProperties
                    );
                    buildingPropertyData.m_ResidentialProperties = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.BuildingPropertyData_AllowedSold:
                    Resource orig_BuildingPropertyData_AllowedSold =
                        buildingPropertyData.m_AllowedSold;
                    originalValue = UVTHelper.ConvertToLong(
                        (ulong)orig_BuildingPropertyData_AllowedSold
                    );
                    buildingPropertyData.m_AllowedSold = (Resource)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.BuildingPropertyData_AllowedInput:
                    Resource orig_BuildingPropertyData_AllowedInput =
                        buildingPropertyData.m_AllowedInput;
                    originalValue = UVTHelper.ConvertToLong(
                        (ulong)orig_BuildingPropertyData_AllowedInput
                    );
                    buildingPropertyData.m_AllowedInput = (Resource)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.BuildingPropertyData_AllowedManufactured:
                    Resource orig_BuildingPropertyData_AllowedManufactured =
                        buildingPropertyData.m_AllowedManufactured;
                    originalValue = UVTHelper.ConvertToLong(
                        (ulong)orig_BuildingPropertyData_AllowedManufactured
                    );
                    buildingPropertyData.m_AllowedManufactured = (Resource)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.BuildingPropertyData_AllowedStored:
                    Resource orig_BuildingPropertyData_AllowedStored =
                        buildingPropertyData.m_AllowedStored;
                    originalValue = UVTHelper.ConvertToLong(
                        (ulong)orig_BuildingPropertyData_AllowedStored
                    );
                    buildingPropertyData.m_AllowedStored = (Resource)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.BuildingPropertyData_SpaceMultiplier:
                    float orig_BuildingPropertyData_SpaceMultiplier =
                        buildingPropertyData.m_SpaceMultiplier;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_BuildingPropertyData_SpaceMultiplier
                    );
                    buildingPropertyData.m_SpaceMultiplier = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, buildingPropertyData);
                    return true;

                case UpdateValueType.CargoTransportStationData_WorkMultiplier:
                    float orig_CargoTransportStationData_WorkMultiplier =
                        cargoTransportStationData.m_WorkMultiplier;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_CargoTransportStationData_WorkMultiplier
                    );
                    cargoTransportStationData.m_WorkMultiplier = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, cargoTransportStationData);
                    return true;

                case UpdateValueType.ConsumptionData_Upkeep:
                    int orig_ConsumptionData_Upkeep = consumptionData.m_Upkeep;
                    originalValue = UVTHelper.ConvertToLong(orig_ConsumptionData_Upkeep);
                    consumptionData.m_Upkeep = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, consumptionData);
                    AdditionalPostApply(selectedPrefab, modifiedValue, valueType);
                    return true;

                case UpdateValueType.ConsumptionData_ElectricityConsumption:
                    float orig_ConsumptionData_ElectricityConsumption =
                        consumptionData.m_ElectricityConsumption;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_ConsumptionData_ElectricityConsumption
                    );
                    consumptionData.m_ElectricityConsumption = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, consumptionData);
                    return true;

                case UpdateValueType.ConsumptionData_WaterConsumption:
                    float orig_ConsumptionData_WaterConsumption =
                        consumptionData.m_WaterConsumption;
                    originalValue = UVTHelper.ConvertToLong(orig_ConsumptionData_WaterConsumption);
                    consumptionData.m_WaterConsumption = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, consumptionData);
                    return true;

                case UpdateValueType.ConsumptionData_GarbageAccumulation:
                    float orig_ConsumptionData_GarbageAccumulation =
                        consumptionData.m_GarbageAccumulation;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_ConsumptionData_GarbageAccumulation
                    );
                    consumptionData.m_GarbageAccumulation = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, consumptionData);
                    return true;

                case UpdateValueType.ConsumptionData_TelecomNeed:
                    float orig_ConsumptionData_TelecomNeed = consumptionData.m_TelecomNeed;
                    originalValue = UVTHelper.ConvertToLong(orig_ConsumptionData_TelecomNeed);
                    consumptionData.m_TelecomNeed = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, consumptionData);
                    return true;

                case UpdateValueType.CoverageData_Range:
                    float orig_CoverageData_Range = coverageData.m_Range;
                    originalValue = UVTHelper.ConvertToLong(orig_CoverageData_Range);
                    coverageData.m_Range = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, coverageData);
                    return true;

                case UpdateValueType.CoverageData_Capacity:
                    float orig_CoverageData_Capacity = coverageData.m_Capacity;
                    originalValue = UVTHelper.ConvertToLong(orig_CoverageData_Capacity);
                    coverageData.m_Capacity = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, coverageData);
                    return true;

                case UpdateValueType.CoverageData_Magnitude:
                    float orig_CoverageData_Magnitude = coverageData.m_Magnitude;
                    originalValue = UVTHelper.ConvertToLong(orig_CoverageData_Magnitude);
                    coverageData.m_Magnitude = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, coverageData);
                    return true;

                case UpdateValueType.DeathcareFacilityData_HearseCapacity:
                    int orig_DeathcareFacilityData_HearseCapacity =
                        deathcareFacilityData.m_HearseCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_DeathcareFacilityData_HearseCapacity
                    );
                    deathcareFacilityData.m_HearseCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, deathcareFacilityData);
                    return true;

                case UpdateValueType.DeathcareFacilityData_StorageCapacity:
                    int orig_DeathcareFacilityData_StorageCapacity =
                        deathcareFacilityData.m_StorageCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_DeathcareFacilityData_StorageCapacity
                    );
                    deathcareFacilityData.m_StorageCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, deathcareFacilityData);
                    return true;

                case UpdateValueType.DeathcareFacilityData_ProcessingRate:
                    float orig_DeathcareFacilityData_ProcessingRate =
                        deathcareFacilityData.m_ProcessingRate;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_DeathcareFacilityData_ProcessingRate
                    );
                    deathcareFacilityData.m_ProcessingRate = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, deathcareFacilityData);
                    return true;

                case UpdateValueType.DeathcareFacilityData_LongTermStorage:
                    bool orig_DeathcareFacilityData_LongTermStorage =
                        deathcareFacilityData.m_LongTermStorage;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_DeathcareFacilityData_LongTermStorage
                    );
                    deathcareFacilityData.m_LongTermStorage = UVTHelper.ConvertToBool(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, deathcareFacilityData);
                    return true;

                case UpdateValueType.DestructibleObjectData_FireHazard:
                    float orig_DestructibleObjectData_FireHazard =
                        destructibleObjectData.m_FireHazard;
                    originalValue = UVTHelper.ConvertToLong(orig_DestructibleObjectData_FireHazard);
                    destructibleObjectData.m_FireHazard = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, destructibleObjectData);
                    return true;

                case UpdateValueType.DestructibleObjectData_StructuralIntegrity:
                    float orig_DestructibleObjectData_StructuralIntegrity =
                        destructibleObjectData.m_StructuralIntegrity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_DestructibleObjectData_StructuralIntegrity
                    );
                    destructibleObjectData.m_StructuralIntegrity = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, destructibleObjectData);
                    return true;

                case UpdateValueType.EmergencyGeneratorData_ElectricityProduction:
                    int orig_EmergencyGeneratorData_ElectricityProduction =
                        emergencyGeneratorData.m_ElectricityProduction;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_EmergencyGeneratorData_ElectricityProduction
                    );
                    emergencyGeneratorData.m_ElectricityProduction = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, emergencyGeneratorData);
                    return true;

                case UpdateValueType.EmergencyShelterData_ShelterCapacity:
                    int orig_EmergencyShelterData_ShelterCapacity =
                        emergencyShelterData.m_ShelterCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_EmergencyShelterData_ShelterCapacity
                    );
                    emergencyShelterData.m_ShelterCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, emergencyShelterData);
                    return true;

                case UpdateValueType.EmergencyShelterData_VehicleCapacity:
                    int orig_EmergencyShelterData_VehicleCapacity =
                        emergencyShelterData.m_VehicleCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_EmergencyShelterData_VehicleCapacity
                    );
                    emergencyShelterData.m_VehicleCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, emergencyShelterData);
                    return true;

                case UpdateValueType.FireStationData_FireEngineCapacity:
                    int orig_FireStationData_FireEngineCapacity =
                        fireStationData.m_FireEngineCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_FireStationData_FireEngineCapacity
                    );
                    fireStationData.m_FireEngineCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, fireStationData);
                    return true;

                case UpdateValueType.FireStationData_FireHelicopterCapacity:
                    int orig_FireStationData_FireHelicopterCapacity =
                        fireStationData.m_FireHelicopterCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_FireStationData_FireHelicopterCapacity
                    );
                    fireStationData.m_FireHelicopterCapacity = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, fireStationData);
                    return true;

                case UpdateValueType.FireStationData_DisasterResponseCapacity:
                    int orig_FireStationData_DisasterResponseCapacity =
                        fireStationData.m_DisasterResponseCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_FireStationData_DisasterResponseCapacity
                    );
                    fireStationData.m_DisasterResponseCapacity = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, fireStationData);
                    return true;

                case UpdateValueType.FireStationData_VehicleEfficiency:
                    float orig_FireStationData_VehicleEfficiency =
                        fireStationData.m_VehicleEfficiency;
                    originalValue = UVTHelper.ConvertToLong(orig_FireStationData_VehicleEfficiency);
                    fireStationData.m_VehicleEfficiency = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, fireStationData);
                    return true;

                case UpdateValueType.GarbageFacilityData_GarbageCapacity:
                    int orig_GarbageFacilityData_GarbageCapacity =
                        garbageFacilityData.m_GarbageCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_GarbageCapacity
                    );
                    garbageFacilityData.m_GarbageCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbageFacilityData_VehicleCapacity:
                    int orig_GarbageFacilityData_VehicleCapacity =
                        garbageFacilityData.m_VehicleCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_VehicleCapacity
                    );
                    garbageFacilityData.m_VehicleCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbageFacilityData_TransportCapacity:
                    int orig_GarbageFacilityData_TransportCapacity =
                        garbageFacilityData.m_TransportCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_TransportCapacity
                    );
                    garbageFacilityData.m_TransportCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbageFacilityData_ProcessingSpeed:
                    int orig_GarbageFacilityData_ProcessingSpeed =
                        garbageFacilityData.m_ProcessingSpeed;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_ProcessingSpeed
                    );
                    garbageFacilityData.m_ProcessingSpeed = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbageFacilityData_IndustrialWasteOnly:
                    bool orig_GarbageFacilityData_IndustrialWasteOnly =
                        garbageFacilityData.m_IndustrialWasteOnly;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_IndustrialWasteOnly
                    );
                    garbageFacilityData.m_IndustrialWasteOnly = UVTHelper.ConvertToBool(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbageFacilityData_LongTermStorage:
                    bool orig_GarbageFacilityData_LongTermStorage =
                        garbageFacilityData.m_LongTermStorage;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbageFacilityData_LongTermStorage
                    );
                    garbageFacilityData.m_LongTermStorage = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbageFacilityData);
                    return true;

                case UpdateValueType.GarbagePoweredData_Capacity:
                    int orig_GarbagePoweredData_Capacity = garbagePoweredData.m_Capacity;
                    originalValue = UVTHelper.ConvertToLong(orig_GarbagePoweredData_Capacity);
                    garbagePoweredData.m_Capacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, garbagePoweredData);
                    return true;

                case UpdateValueType.GarbagePoweredData_ProductionPerUnit:
                    float orig_GarbagePoweredData_ProductionPerUnit =
                        garbagePoweredData.m_ProductionPerUnit;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GarbagePoweredData_ProductionPerUnit
                    );
                    garbagePoweredData.m_ProductionPerUnit = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, garbagePoweredData);
                    return true;

                case UpdateValueType.GroundWaterPoweredData_Production:
                    int orig_GroundWaterPoweredData_Production =
                        groundWaterPoweredData.m_Production;
                    originalValue = UVTHelper.ConvertToLong(orig_GroundWaterPoweredData_Production);
                    groundWaterPoweredData.m_Production = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, groundWaterPoweredData);
                    return true;

                case UpdateValueType.GroundWaterPoweredData_MaximumGroundWater:
                    int orig_GroundWaterPoweredData_MaximumGroundWater =
                        groundWaterPoweredData.m_MaximumGroundWater;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_GroundWaterPoweredData_MaximumGroundWater
                    );
                    groundWaterPoweredData.m_MaximumGroundWater = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, groundWaterPoweredData);
                    return true;

                case UpdateValueType.HospitalData_AmbulanceCapacity:
                    int orig_HospitalData_AmbulanceCapacity = hospitalData.m_AmbulanceCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_AmbulanceCapacity);
                    hospitalData.m_AmbulanceCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_MedicalHelicopterCapacity:
                    int orig_HospitalData_MedicalHelicopterCapacity =
                        hospitalData.m_MedicalHelicopterCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_HospitalData_MedicalHelicopterCapacity
                    );
                    hospitalData.m_MedicalHelicopterCapacity = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_PatientCapacity:
                    int orig_HospitalData_PatientCapacity = hospitalData.m_PatientCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_PatientCapacity);
                    hospitalData.m_PatientCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_TreatmentBonus:
                    int orig_HospitalData_TreatmentBonus = hospitalData.m_TreatmentBonus;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_TreatmentBonus);
                    hospitalData.m_TreatmentBonus = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_HealthRange:
                    int2 orig_HospitalData_HealthRange = hospitalData.m_HealthRange;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_HealthRange);
                    hospitalData.m_HealthRange = UVTHelper.ConvertToInt2(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_TreatDiseases:
                    bool orig_HospitalData_TreatDiseases = hospitalData.m_TreatDiseases;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_TreatDiseases);
                    hospitalData.m_TreatDiseases = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.HospitalData_TreatInjuries:
                    bool orig_HospitalData_TreatInjuries = hospitalData.m_TreatInjuries;
                    originalValue = UVTHelper.ConvertToLong(orig_HospitalData_TreatInjuries);
                    hospitalData.m_TreatInjuries = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, hospitalData);
                    return true;

                case UpdateValueType.MaintenanceDepotData_MaintenanceType:
                    MaintenanceType orig_MaintenanceDepotData_MaintenanceType =
                        maintenanceDepotData.m_MaintenanceType;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_MaintenanceDepotData_MaintenanceType
                    );
                    maintenanceDepotData.m_MaintenanceType = (MaintenanceType)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, maintenanceDepotData);
                    return true;

                case UpdateValueType.MaintenanceDepotData_VehicleCapacity:
                    int orig_MaintenanceDepotData_VehicleCapacity =
                        maintenanceDepotData.m_VehicleCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_MaintenanceDepotData_VehicleCapacity
                    );
                    maintenanceDepotData.m_VehicleCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, maintenanceDepotData);
                    return true;

                case UpdateValueType.MaintenanceDepotData_VehicleEfficiency:
                    float orig_MaintenanceDepotData_VehicleEfficiency =
                        maintenanceDepotData.m_VehicleEfficiency;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_MaintenanceDepotData_VehicleEfficiency
                    );
                    maintenanceDepotData.m_VehicleEfficiency = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, maintenanceDepotData);
                    return true;

                case UpdateValueType.ParkData_MaintenancePool:
                    short orig_ParkData_MaintenancePool = parkData.m_MaintenancePool;
                    originalValue = UVTHelper.ConvertToLong(orig_ParkData_MaintenancePool);
                    parkData.m_MaintenancePool = UVTHelper.ConvertToShort(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, parkData);
                    return true;

                case UpdateValueType.ParkData_AllowHomeless:
                    bool orig_ParkData_AllowHomeless = parkData.m_AllowHomeless;
                    originalValue = UVTHelper.ConvertToLong(orig_ParkData_AllowHomeless);
                    parkData.m_AllowHomeless = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, parkData);
                    return true;

                case UpdateValueType.ParkingFacilityData_RoadTypes:
                    RoadTypes orig_ParkingFacilityData_RoadTypes = parkingFacilityData.m_RoadTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_ParkingFacilityData_RoadTypes
                    );
                    parkingFacilityData.m_RoadTypes = (RoadTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, parkingFacilityData);
                    return true;

                case UpdateValueType.ParkingFacilityData_ComfortFactor:
                    float orig_ParkingFacilityData_ComfortFactor =
                        parkingFacilityData.m_ComfortFactor;
                    originalValue = UVTHelper.ConvertToLong(orig_ParkingFacilityData_ComfortFactor);
                    parkingFacilityData.m_ComfortFactor = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, parkingFacilityData);
                    return true;

                case UpdateValueType.ParkingFacilityData_GarageMarkerCapacity:
                    int orig_ParkingFacilityData_GarageMarkerCapacity =
                        parkingFacilityData.m_GarageMarkerCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_ParkingFacilityData_GarageMarkerCapacity
                    );
                    parkingFacilityData.m_GarageMarkerCapacity = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, parkingFacilityData);
                    return true;

                case UpdateValueType.PoliceStationData_PatrolCarCapacity:
                    int orig_PoliceStationData_PatrolCarCapacity =
                        policeStationData.m_PatrolCarCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_PoliceStationData_PatrolCarCapacity
                    );
                    policeStationData.m_PatrolCarCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, policeStationData);
                    return true;

                case UpdateValueType.PoliceStationData_PoliceHelicopterCapacity:
                    int orig_PoliceStationData_PoliceHelicopterCapacity =
                        policeStationData.m_PoliceHelicopterCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_PoliceStationData_PoliceHelicopterCapacity
                    );
                    policeStationData.m_PoliceHelicopterCapacity = UVTHelper.ConvertToInt(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, policeStationData);
                    return true;

                case UpdateValueType.PoliceStationData_JailCapacity:
                    int orig_PoliceStationData_JailCapacity = policeStationData.m_JailCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_PoliceStationData_JailCapacity);
                    policeStationData.m_JailCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, policeStationData);
                    return true;

                case UpdateValueType.PoliceStationData_PurposeMask:
                    PolicePurpose orig_PoliceStationData_PurposeMask =
                        policeStationData.m_PurposeMask;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_PoliceStationData_PurposeMask
                    );
                    policeStationData.m_PurposeMask = (PolicePurpose)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, policeStationData);
                    return true;

                case UpdateValueType.PostFacilityData_PostVanCapacity:
                    int orig_PostFacilityData_PostVanCapacity = postFacilityData.m_PostVanCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_PostFacilityData_PostVanCapacity);
                    postFacilityData.m_PostVanCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, postFacilityData);
                    return true;

                case UpdateValueType.PostFacilityData_PostTruckCapacity:
                    int orig_PostFacilityData_PostTruckCapacity =
                        postFacilityData.m_PostTruckCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_PostFacilityData_PostTruckCapacity
                    );
                    postFacilityData.m_PostTruckCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, postFacilityData);
                    return true;

                case UpdateValueType.PostFacilityData_MailCapacity:
                    int orig_PostFacilityData_MailCapacity = postFacilityData.m_MailCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_PostFacilityData_MailCapacity);
                    postFacilityData.m_MailCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, postFacilityData);
                    return true;

                case UpdateValueType.PostFacilityData_SortingRate:
                    int orig_PostFacilityData_SortingRate = postFacilityData.m_SortingRate;
                    originalValue = UVTHelper.ConvertToLong(orig_PostFacilityData_SortingRate);
                    postFacilityData.m_SortingRate = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, postFacilityData);
                    return true;

                case UpdateValueType.PowerPlantData_ElectricityProduction:
                    int orig_PowerPlantData_ElectricityProduction =
                        powerPlantData.m_ElectricityProduction;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_PowerPlantData_ElectricityProduction
                    );
                    powerPlantData.m_ElectricityProduction = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, powerPlantData);
                    return true;

                case UpdateValueType.PrisonData_PrisonVanCapacity:
                    int orig_PrisonData_PrisonVanCapacity = prisonData.m_PrisonVanCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_PrisonData_PrisonVanCapacity);
                    prisonData.m_PrisonVanCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, prisonData);
                    return true;

                case UpdateValueType.PrisonData_PrisonerCapacity:
                    int orig_PrisonData_PrisonerCapacity = prisonData.m_PrisonerCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_PrisonData_PrisonerCapacity);
                    prisonData.m_PrisonerCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, prisonData);
                    return true;

                case UpdateValueType.PrisonData_PrisonerWellbeing:
                    sbyte orig_PrisonData_PrisonerWellbeing = prisonData.m_PrisonerWellbeing;
                    originalValue = UVTHelper.ConvertToLong(orig_PrisonData_PrisonerWellbeing);
                    prisonData.m_PrisonerWellbeing = UVTHelper.ConvertToSByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, prisonData);
                    return true;

                case UpdateValueType.PrisonData_PrisonerHealth:
                    sbyte orig_PrisonData_PrisonerHealth = prisonData.m_PrisonerHealth;
                    originalValue = UVTHelper.ConvertToLong(orig_PrisonData_PrisonerHealth);
                    prisonData.m_PrisonerHealth = UVTHelper.ConvertToSByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, prisonData);
                    return true;

                case UpdateValueType.SchoolData_StudentCapacity:
                    int orig_SchoolData_StudentCapacity = schoolData.m_StudentCapacity;
                    originalValue = UVTHelper.ConvertToLong(orig_SchoolData_StudentCapacity);
                    schoolData.m_StudentCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, schoolData);
                    return true;

                case UpdateValueType.SchoolData_GraduationModifier:
                    float orig_SchoolData_GraduationModifier = schoolData.m_GraduationModifier;
                    originalValue = UVTHelper.ConvertToLong(orig_SchoolData_GraduationModifier);
                    schoolData.m_GraduationModifier = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, schoolData);
                    return true;

                case UpdateValueType.SchoolData_EducationLevel:
                    byte orig_SchoolData_EducationLevel = schoolData.m_EducationLevel;
                    originalValue = UVTHelper.ConvertToLong(orig_SchoolData_EducationLevel);
                    schoolData.m_EducationLevel = UVTHelper.ConvertToByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, schoolData);
                    return true;

                case UpdateValueType.SchoolData_StudentWellbeing:
                    sbyte orig_SchoolData_StudentWellbeing = schoolData.m_StudentWellbeing;
                    originalValue = UVTHelper.ConvertToLong(orig_SchoolData_StudentWellbeing);
                    schoolData.m_StudentWellbeing = UVTHelper.ConvertToSByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, schoolData);
                    return true;

                case UpdateValueType.SchoolData_StudentHealth:
                    sbyte orig_SchoolData_StudentHealth = schoolData.m_StudentHealth;
                    originalValue = UVTHelper.ConvertToLong(orig_SchoolData_StudentHealth);
                    schoolData.m_StudentHealth = UVTHelper.ConvertToSByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, schoolData);
                    return true;

                case UpdateValueType.SewageOutletData_Capacity:
                    int orig_SewageOutletData_Capacity = sewageOutletData.m_Capacity;
                    originalValue = UVTHelper.ConvertToLong(orig_SewageOutletData_Capacity);
                    sewageOutletData.m_Capacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, sewageOutletData);
                    return true;

                case UpdateValueType.SewageOutletData_Purification:
                    float orig_SewageOutletData_Purification = sewageOutletData.m_Purification;
                    originalValue = UVTHelper.ConvertToLong(orig_SewageOutletData_Purification);
                    sewageOutletData.m_Purification = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, sewageOutletData);
                    return true;

                case UpdateValueType.SolarPoweredData_Production:
                    int orig_SolarPoweredData_Production = solarPoweredData.m_Production;
                    originalValue = UVTHelper.ConvertToLong(orig_SolarPoweredData_Production);
                    solarPoweredData.m_Production = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, solarPoweredData);
                    return true;

                case UpdateValueType.SpawnableBuildingData_Level:
                    byte orig_SpawnableBuildingData_Level = spawnableBuildingData.m_Level;
                    originalValue = UVTHelper.ConvertToLong(orig_SpawnableBuildingData_Level);
                    spawnableBuildingData.m_Level = UVTHelper.ConvertToByte(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, spawnableBuildingData);
                    return true;

                case UpdateValueType.StorageCompanyData_StoredResources:
                    Resource orig_StorageCompanyData_StoredResources =
                        storageCompanyData.m_StoredResources;
                    originalValue = UVTHelper.ConvertToLong(
                        (ulong)orig_StorageCompanyData_StoredResources
                    );
                    storageCompanyData.m_StoredResources = (Resource)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, storageCompanyData);
                    return true;

                case UpdateValueType.StorageCompanyData_TransportInterval:
                    int2 orig_StorageCompanyData_TransportInterval =
                        storageCompanyData.m_TransportInterval;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_StorageCompanyData_TransportInterval
                    );
                    storageCompanyData.m_TransportInterval = UVTHelper.ConvertToInt2(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, storageCompanyData);
                    return true;

                case UpdateValueType.TelecomFacilityData_Range:
                    float orig_TelecomFacilityData_Range = telecomFacilityData.m_Range;
                    originalValue = UVTHelper.ConvertToLong(orig_TelecomFacilityData_Range);
                    telecomFacilityData.m_Range = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, telecomFacilityData);
                    return true;

                case UpdateValueType.TelecomFacilityData_NetworkCapacity:
                    float orig_TelecomFacilityData_NetworkCapacity =
                        telecomFacilityData.m_NetworkCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TelecomFacilityData_NetworkCapacity
                    );
                    telecomFacilityData.m_NetworkCapacity = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, telecomFacilityData);
                    return true;

                case UpdateValueType.TelecomFacilityData_PenetrateTerrain:
                    bool orig_TelecomFacilityData_PenetrateTerrain =
                        telecomFacilityData.m_PenetrateTerrain;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TelecomFacilityData_PenetrateTerrain
                    );
                    telecomFacilityData.m_PenetrateTerrain = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, telecomFacilityData);
                    return true;

                case UpdateValueType.TransportDepotData_TransportType:
                    TransportType orig_TransportDepotData_TransportType =
                        transportDepotData.m_TransportType;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportDepotData_TransportType
                    );
                    transportDepotData.m_TransportType = (TransportType)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_EnergyTypes:
                    EnergyTypes orig_TransportDepotData_EnergyTypes =
                        transportDepotData.m_EnergyTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportDepotData_EnergyTypes
                    );
                    transportDepotData.m_EnergyTypes = (EnergyTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_SizeClass:
                    SizeClass orig_TransportDepotData_SizeClass = transportDepotData.m_SizeClass;
                    originalValue = UVTHelper.ConvertToLong((int)orig_TransportDepotData_SizeClass);
                    transportDepotData.m_SizeClass = (SizeClass)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_DispatchCenter:
                    bool orig_TransportDepotData_DispatchCenter =
                        transportDepotData.m_DispatchCenter;
                    originalValue = UVTHelper.ConvertToLong(orig_TransportDepotData_DispatchCenter);
                    transportDepotData.m_DispatchCenter = UVTHelper.ConvertToBool(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_VehicleCapacity:
                    int orig_TransportDepotData_VehicleCapacity =
                        transportDepotData.m_VehicleCapacity;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TransportDepotData_VehicleCapacity
                    );
                    transportDepotData.m_VehicleCapacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_ProductionDuration:
                    float orig_TransportDepotData_ProductionDuration =
                        transportDepotData.m_ProductionDuration;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TransportDepotData_ProductionDuration
                    );
                    transportDepotData.m_ProductionDuration = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportDepotData_MaintenanceDuration:
                    float orig_TransportDepotData_MaintenanceDuration =
                        transportDepotData.m_MaintenanceDuration;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TransportDepotData_MaintenanceDuration
                    );
                    transportDepotData.m_MaintenanceDuration = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, transportDepotData);
                    return true;

                case UpdateValueType.TransportStationData_ComfortFactor:
                    float orig_TransportStationData_ComfortFactor =
                        transportStationData.m_ComfortFactor;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TransportStationData_ComfortFactor
                    );
                    transportStationData.m_ComfortFactor = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.TransportStationData_LoadingFactor:
                    float orig_TransportStationData_LoadingFactor =
                        transportStationData.m_LoadingFactor;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_TransportStationData_LoadingFactor
                    );
                    transportStationData.m_LoadingFactor = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.TransportStationData_CarRefuelTypes:
                    EnergyTypes orig_TransportStationData_CarRefuelTypes =
                        transportStationData.m_CarRefuelTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportStationData_CarRefuelTypes
                    );
                    transportStationData.m_CarRefuelTypes = (EnergyTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.TransportStationData_TrainRefuelTypes:
                    EnergyTypes orig_TransportStationData_TrainRefuelTypes =
                        transportStationData.m_TrainRefuelTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportStationData_TrainRefuelTypes
                    );
                    transportStationData.m_TrainRefuelTypes = (EnergyTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.TransportStationData_WatercraftRefuelTypes:
                    EnergyTypes orig_TransportStationData_WatercraftRefuelTypes =
                        transportStationData.m_WatercraftRefuelTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportStationData_WatercraftRefuelTypes
                    );
                    transportStationData.m_WatercraftRefuelTypes = (EnergyTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.TransportStationData_AircraftRefuelTypes:
                    EnergyTypes orig_TransportStationData_AircraftRefuelTypes =
                        transportStationData.m_AircraftRefuelTypes;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_TransportStationData_AircraftRefuelTypes
                    );
                    transportStationData.m_AircraftRefuelTypes = (EnergyTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, transportStationData);
                    return true;

                case UpdateValueType.WaterPoweredData_ProductionFactor:
                    float orig_WaterPoweredData_ProductionFactor =
                        waterPoweredData.m_ProductionFactor;
                    originalValue = UVTHelper.ConvertToLong(orig_WaterPoweredData_ProductionFactor);
                    waterPoweredData.m_ProductionFactor = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, waterPoweredData);
                    return true;

                case UpdateValueType.WaterPoweredData_CapacityFactor:
                    float orig_WaterPoweredData_CapacityFactor = waterPoweredData.m_CapacityFactor;
                    originalValue = UVTHelper.ConvertToLong(orig_WaterPoweredData_CapacityFactor);
                    waterPoweredData.m_CapacityFactor = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, waterPoweredData);
                    return true;

                case UpdateValueType.WaterPumpingStationData_Types:
                    AllowedWaterTypes orig_WaterPumpingStationData_Types =
                        waterPumpingStationData.m_Types;
                    originalValue = UVTHelper.ConvertToLong(
                        (int)orig_WaterPumpingStationData_Types
                    );
                    waterPumpingStationData.m_Types = (AllowedWaterTypes)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, waterPumpingStationData);
                    return true;

                case UpdateValueType.WaterPumpingStationData_Capacity:
                    int orig_WaterPumpingStationData_Capacity = waterPumpingStationData.m_Capacity;
                    originalValue = UVTHelper.ConvertToLong(orig_WaterPumpingStationData_Capacity);
                    waterPumpingStationData.m_Capacity = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, waterPumpingStationData);
                    return true;

                case UpdateValueType.WaterPumpingStationData_Purification:
                    float orig_WaterPumpingStationData_Purification =
                        waterPumpingStationData.m_Purification;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_WaterPumpingStationData_Purification
                    );
                    waterPumpingStationData.m_Purification = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, waterPumpingStationData);
                    return true;

                case UpdateValueType.WindPoweredData_MaximumWind:
                    float orig_WindPoweredData_MaximumWind = windPoweredData.m_MaximumWind;
                    originalValue = UVTHelper.ConvertToLong(orig_WindPoweredData_MaximumWind);
                    windPoweredData.m_MaximumWind = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, windPoweredData);
                    return true;

                case UpdateValueType.WindPoweredData_Production:
                    int orig_WindPoweredData_Production = windPoweredData.m_Production;
                    originalValue = UVTHelper.ConvertToLong(orig_WindPoweredData_Production);
                    windPoweredData.m_Production = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, windPoweredData);
                    return true;

                case UpdateValueType.WorkplaceData_Complexity:
                    WorkplaceComplexity orig_WorkplaceData_Complexity = workplaceData.m_Complexity;
                    originalValue = UVTHelper.ConvertToLong((int)orig_WorkplaceData_Complexity);
                    workplaceData.m_Complexity = (WorkplaceComplexity)modifiedValue;
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    return true;

                case UpdateValueType.WorkplaceData_MaxWorkers:
                    int orig_WorkplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                    originalValue = UVTHelper.ConvertToLong(orig_WorkplaceData_MaxWorkers);
                    workplaceData.m_MaxWorkers = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    AdditionalPostApply(selectedPrefab, modifiedValue, valueType);
                    return true;

                case UpdateValueType.WorkplaceData_EveningShiftProbability:
                    float orig_WorkplaceData_EveningShiftProbability =
                        workplaceData.m_EveningShiftProbability;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_WorkplaceData_EveningShiftProbability
                    );
                    workplaceData.m_EveningShiftProbability = UVTHelper.ConvertToFloat(
                        modifiedValue
                    );
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    return true;

                case UpdateValueType.WorkplaceData_NightShiftProbability:
                    float orig_WorkplaceData_NightShiftProbability =
                        workplaceData.m_NightShiftProbability;
                    originalValue = UVTHelper.ConvertToLong(
                        orig_WorkplaceData_NightShiftProbability
                    );
                    workplaceData.m_NightShiftProbability = UVTHelper.ConvertToFloat(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    return true;

                case UpdateValueType.WorkplaceData_MinimumWorkersLimit:
                    int orig_WorkplaceData_MinimumWorkersLimit =
                        workplaceData.m_MinimumWorkersLimit;
                    originalValue = UVTHelper.ConvertToLong(orig_WorkplaceData_MinimumWorkersLimit);
                    workplaceData.m_MinimumWorkersLimit = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    return true;

                case UpdateValueType.WorkplaceData_WorkConditions:
                    int orig_WorkplaceData_WorkConditions = workplaceData.m_WorkConditions;
                    originalValue = UVTHelper.ConvertToLong(orig_WorkplaceData_WorkConditions);
                    workplaceData.m_WorkConditions = UVTHelper.ConvertToInt(modifiedValue);
                    utils.SetOrAdd(selectedPrefab, workplaceData);
                    return true;

                //end
                default:
                    return false;
            }
        }
    }
}
