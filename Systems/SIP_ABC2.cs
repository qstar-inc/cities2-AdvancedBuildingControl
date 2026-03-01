using AdvancedBuildingControl.Extensions;
using AdvancedBuildingControl.Variables;
using Colossal.Entities;
using Game.Prefabs;

namespace AdvancedBuildingControl.Systems
{
    public partial class SIP_ABC : ExtendedInfoSectionBase
    {
        public void CheckComponents()
        {
            BldgComponentInfo c = new();
            //start
            if (EntityManager.TryGetComponent(selectedPrefab, out AttractionData attractionData))
            {
                c.AttractionData = true;
                c.AttractionData_Attractiveness = attractionData.m_Attractiveness;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out BatteryData batteryData))
            {
                c.BatteryData = true;
                c.BatteryData_Capacity = batteryData.m_Capacity;
                c.BatteryData_PowerOutput = batteryData.m_PowerOutput;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out BuildingPropertyData buildingPropertyData
                )
            )
            {
                c.BuildingPropertyData = true;
                c.BuildingPropertyData_ResidentialProperties =
                    buildingPropertyData.m_ResidentialProperties;
                c.BuildingPropertyData_AllowedSold = buildingPropertyData.m_AllowedSold;
                c.BuildingPropertyData_AllowedInput = buildingPropertyData.m_AllowedInput;
                c.BuildingPropertyData_AllowedManufactured =
                    buildingPropertyData.m_AllowedManufactured;
                c.BuildingPropertyData_AllowedStored = buildingPropertyData.m_AllowedStored;
                c.BuildingPropertyData_SpaceMultiplier = buildingPropertyData.m_SpaceMultiplier;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out CargoTransportStationData cargoTransportStationData
                )
            )
            {
                c.CargoTransportStationData = true;
                c.CargoTransportStationData_WorkMultiplier =
                    cargoTransportStationData.m_WorkMultiplier;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out ConsumptionData consumptionData))
            {
                c.ConsumptionData = true;
                c.ConsumptionData_Upkeep = consumptionData.m_Upkeep;
                c.ConsumptionData_ElectricityConsumption = consumptionData.m_ElectricityConsumption;
                c.ConsumptionData_WaterConsumption = consumptionData.m_WaterConsumption;
                c.ConsumptionData_GarbageAccumulation = consumptionData.m_GarbageAccumulation;
                c.ConsumptionData_TelecomNeed = consumptionData.m_TelecomNeed;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out CoverageData coverageData))
            {
                c.CoverageData = true;
                c.CoverageData_Range = coverageData.m_Range;
                c.CoverageData_Capacity = coverageData.m_Capacity;
                c.CoverageData_Magnitude = coverageData.m_Magnitude;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out DeathcareFacilityData deathcareFacilityData
                )
            )
            {
                c.DeathcareFacilityData = true;
                c.DeathcareFacilityData_HearseCapacity = deathcareFacilityData.m_HearseCapacity;
                c.DeathcareFacilityData_StorageCapacity = deathcareFacilityData.m_StorageCapacity;
                c.DeathcareFacilityData_ProcessingRate = deathcareFacilityData.m_ProcessingRate;
                c.DeathcareFacilityData_LongTermStorage = deathcareFacilityData.m_LongTermStorage;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out DestructibleObjectData destructibleObjectData
                )
            )
            {
                c.DestructibleObjectData = true;
                c.DestructibleObjectData_FireHazard = destructibleObjectData.m_FireHazard;
                c.DestructibleObjectData_StructuralIntegrity =
                    destructibleObjectData.m_StructuralIntegrity;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out EmergencyGeneratorData emergencyGeneratorData
                )
            )
            {
                c.EmergencyGeneratorData = true;
                c.EmergencyGeneratorData_ElectricityProduction =
                    emergencyGeneratorData.m_ElectricityProduction;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out EmergencyShelterData emergencyShelterData
                )
            )
            {
                c.EmergencyShelterData = true;
                c.EmergencyShelterData_ShelterCapacity = emergencyShelterData.m_ShelterCapacity;
                c.EmergencyShelterData_VehicleCapacity = emergencyShelterData.m_VehicleCapacity;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out FireStationData fireStationData))
            {
                c.FireStationData = true;
                c.FireStationData_FireEngineCapacity = fireStationData.m_FireEngineCapacity;
                c.FireStationData_FireHelicopterCapacity = fireStationData.m_FireHelicopterCapacity;
                c.FireStationData_DisasterResponseCapacity =
                    fireStationData.m_DisasterResponseCapacity;
                c.FireStationData_VehicleEfficiency = fireStationData.m_VehicleEfficiency;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out GarbageFacilityData garbageFacilityData
                )
            )
            {
                c.GarbageFacilityData = true;
                c.GarbageFacilityData_GarbageCapacity = garbageFacilityData.m_GarbageCapacity;
                c.GarbageFacilityData_VehicleCapacity = garbageFacilityData.m_VehicleCapacity;
                c.GarbageFacilityData_TransportCapacity = garbageFacilityData.m_TransportCapacity;
                c.GarbageFacilityData_ProcessingSpeed = garbageFacilityData.m_ProcessingSpeed;
                c.GarbageFacilityData_IndustrialWasteOnly =
                    garbageFacilityData.m_IndustrialWasteOnly;
                c.GarbageFacilityData_LongTermStorage = garbageFacilityData.m_LongTermStorage;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out GarbagePoweredData garbagePoweredData
                )
            )
            {
                c.GarbagePoweredData = true;
                c.GarbagePoweredData_Capacity = garbagePoweredData.m_Capacity;
                c.GarbagePoweredData_ProductionPerUnit = garbagePoweredData.m_ProductionPerUnit;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out GroundWaterPoweredData groundWaterPoweredData
                )
            )
            {
                c.GroundWaterPoweredData = true;
                c.GroundWaterPoweredData_Production = groundWaterPoweredData.m_Production;
                c.GroundWaterPoweredData_MaximumGroundWater =
                    groundWaterPoweredData.m_MaximumGroundWater;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out HospitalData hospitalData))
            {
                c.HospitalData = true;
                c.HospitalData_AmbulanceCapacity = hospitalData.m_AmbulanceCapacity;
                c.HospitalData_MedicalHelicopterCapacity = hospitalData.m_MedicalHelicopterCapacity;
                c.HospitalData_PatientCapacity = hospitalData.m_PatientCapacity;
                c.HospitalData_TreatmentBonus = hospitalData.m_TreatmentBonus;
                c.HospitalData_HealthRange = hospitalData.m_HealthRange;
                c.HospitalData_TreatDiseases = hospitalData.m_TreatDiseases;
                c.HospitalData_TreatInjuries = hospitalData.m_TreatInjuries;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out MaintenanceDepotData maintenanceDepotData
                )
            )
            {
                c.MaintenanceDepotData = true;
                c.MaintenanceDepotData_MaintenanceType = maintenanceDepotData.m_MaintenanceType;
                c.MaintenanceDepotData_VehicleCapacity = maintenanceDepotData.m_VehicleCapacity;
                c.MaintenanceDepotData_VehicleEfficiency = maintenanceDepotData.m_VehicleEfficiency;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out ParkData parkData))
            {
                c.ParkData = true;
                c.ParkData_MaintenancePool = parkData.m_MaintenancePool;
                c.ParkData_AllowHomeless = parkData.m_AllowHomeless;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out ParkingFacilityData parkingFacilityData
                )
            )
            {
                c.ParkingFacilityData = true;
                c.ParkingFacilityData_RoadTypes = parkingFacilityData.m_RoadTypes;
                c.ParkingFacilityData_ComfortFactor = parkingFacilityData.m_ComfortFactor;
                c.ParkingFacilityData_GarageMarkerCapacity =
                    parkingFacilityData.m_GarageMarkerCapacity;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out PoliceStationData policeStationData
                )
            )
            {
                c.PoliceStationData = true;
                c.PoliceStationData_PatrolCarCapacity = policeStationData.m_PatrolCarCapacity;
                c.PoliceStationData_PoliceHelicopterCapacity =
                    policeStationData.m_PoliceHelicopterCapacity;
                c.PoliceStationData_JailCapacity = policeStationData.m_JailCapacity;
                c.PoliceStationData_PurposeMask = policeStationData.m_PurposeMask;
            }
            if (
                EntityManager.TryGetComponent(selectedPrefab, out PostFacilityData postFacilityData)
            )
            {
                c.PostFacilityData = true;
                c.PostFacilityData_PostVanCapacity = postFacilityData.m_PostVanCapacity;
                c.PostFacilityData_PostTruckCapacity = postFacilityData.m_PostTruckCapacity;
                c.PostFacilityData_MailCapacity = postFacilityData.m_MailCapacity;
                c.PostFacilityData_SortingRate = postFacilityData.m_SortingRate;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out PowerPlantData powerPlantData))
            {
                c.PowerPlantData = true;
                c.PowerPlantData_ElectricityProduction = powerPlantData.m_ElectricityProduction;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out PrisonData prisonData))
            {
                c.PrisonData = true;
                c.PrisonData_PrisonVanCapacity = prisonData.m_PrisonVanCapacity;
                c.PrisonData_PrisonerCapacity = prisonData.m_PrisonerCapacity;
                c.PrisonData_PrisonerWellbeing = prisonData.m_PrisonerWellbeing;
                c.PrisonData_PrisonerHealth = prisonData.m_PrisonerHealth;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out SchoolData schoolData))
            {
                c.SchoolData = true;
                c.SchoolData_StudentCapacity = schoolData.m_StudentCapacity;
                c.SchoolData_GraduationModifier = schoolData.m_GraduationModifier;
                c.SchoolData_EducationLevel = schoolData.m_EducationLevel;
                c.SchoolData_StudentWellbeing = schoolData.m_StudentWellbeing;
                c.SchoolData_StudentHealth = schoolData.m_StudentHealth;
            }
            if (
                EntityManager.TryGetComponent(selectedPrefab, out SewageOutletData sewageOutletData)
            )
            {
                c.SewageOutletData = true;
                c.SewageOutletData_Capacity = sewageOutletData.m_Capacity;
                c.SewageOutletData_Purification = sewageOutletData.m_Purification;
            }
            if (
                EntityManager.TryGetComponent(selectedPrefab, out SolarPoweredData solarPoweredData)
            )
            {
                c.SolarPoweredData = true;
                c.SolarPoweredData_Production = solarPoweredData.m_Production;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out SpawnableBuildingData spawnableBuildingData
                )
            )
            {
                c.SpawnableBuildingData = true;
                c.SpawnableBuildingData_Level = spawnableBuildingData.m_Level;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out StorageCompanyData storageCompanyData
                )
            )
            {
                c.StorageCompanyData = true;
                c.StorageCompanyData_StoredResources = storageCompanyData.m_StoredResources;
                c.StorageCompanyData_TransportInterval = storageCompanyData.m_TransportInterval;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out TelecomFacilityData telecomFacilityData
                )
            )
            {
                c.TelecomFacilityData = true;
                c.TelecomFacilityData_Range = telecomFacilityData.m_Range;
                c.TelecomFacilityData_NetworkCapacity = telecomFacilityData.m_NetworkCapacity;
                c.TelecomFacilityData_PenetrateTerrain = telecomFacilityData.m_PenetrateTerrain;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out TransportDepotData transportDepotData
                )
            )
            {
                c.TransportDepotData = true;
                c.TransportDepotData_TransportType = transportDepotData.m_TransportType;
                c.TransportDepotData_EnergyTypes = transportDepotData.m_EnergyTypes;
                c.TransportDepotData_SizeClass = transportDepotData.m_SizeClass;
                c.TransportDepotData_DispatchCenter = transportDepotData.m_DispatchCenter;
                c.TransportDepotData_VehicleCapacity = transportDepotData.m_VehicleCapacity;
                c.TransportDepotData_ProductionDuration = transportDepotData.m_ProductionDuration;
                c.TransportDepotData_MaintenanceDuration = transportDepotData.m_MaintenanceDuration;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out TransportStationData transportStationData
                )
            )
            {
                c.TransportStationData = true;
                c.TransportStationData_ComfortFactor = transportStationData.m_ComfortFactor;
                c.TransportStationData_LoadingFactor = transportStationData.m_LoadingFactor;
                c.TransportStationData_CarRefuelTypes = transportStationData.m_CarRefuelTypes;
                c.TransportStationData_TrainRefuelTypes = transportStationData.m_TrainRefuelTypes;
                c.TransportStationData_WatercraftRefuelTypes =
                    transportStationData.m_WatercraftRefuelTypes;
                c.TransportStationData_AircraftRefuelTypes =
                    transportStationData.m_AircraftRefuelTypes;
            }
            if (
                EntityManager.TryGetComponent(selectedPrefab, out WaterPoweredData waterPoweredData)
            )
            {
                c.WaterPoweredData = true;
                c.WaterPoweredData_ProductionFactor = waterPoweredData.m_ProductionFactor;
                c.WaterPoweredData_CapacityFactor = waterPoweredData.m_CapacityFactor;
            }
            if (
                EntityManager.TryGetComponent(
                    selectedPrefab,
                    out WaterPumpingStationData waterPumpingStationData
                )
            )
            {
                c.WaterPumpingStationData = true;
                c.WaterPumpingStationData_Types = waterPumpingStationData.m_Types;
                c.WaterPumpingStationData_Capacity = waterPumpingStationData.m_Capacity;
                c.WaterPumpingStationData_Purification = waterPumpingStationData.m_Purification;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out WindPoweredData windPoweredData))
            {
                c.WindPoweredData = true;
                c.WindPoweredData_MaximumWind = windPoweredData.m_MaximumWind;
                c.WindPoweredData_Production = windPoweredData.m_Production;
            }
            if (EntityManager.TryGetComponent(selectedPrefab, out WorkplaceData workplaceData))
            {
                c.WorkplaceData = true;
                c.WorkplaceData_Complexity = workplaceData.m_Complexity;
                c.WorkplaceData_MaxWorkers = workplaceData.m_MaxWorkers;
                c.WorkplaceData_EveningShiftProbability = workplaceData.m_EveningShiftProbability;
                c.WorkplaceData_NightShiftProbability = workplaceData.m_NightShiftProbability;
                c.WorkplaceData_MinimumWorkersLimit = workplaceData.m_MinimumWorkersLimit;
                c.WorkplaceData_WorkConditions = workplaceData.m_WorkConditions;
            }
            //end
            bldgComponentInfo = c;
        }
    }
}
