using System;

namespace AdvancedBuildingControl.Variables
{
    public partial class UVTHelper
    {

        public static void TryLimit(UpdateValueType updateValueType, ref int value)
        {
            bool clamp;
            int min = int.MinValue;
            int max = int.MaxValue;
            switch (updateValueType)
            {
case UpdateValueType.BuildingPropertyData_ResidentialProperties: clamp = true;min = 0;break;case UpdateValueType.ConsumptionData_Upkeep: clamp = true;min = 0;break;case UpdateValueType.DeathcareFacilityData_HearseCapacity: clamp = true;min = 0;break;case UpdateValueType.DeathcareFacilityData_StorageCapacity: clamp = true;min = 0;break;case UpdateValueType.EmergencyShelterData_ShelterCapacity: clamp = true;min = 0;break;case UpdateValueType.EmergencyShelterData_VehicleCapacity: clamp = true;min = 0;break;case UpdateValueType.FireStationData_FireEngineCapacity: clamp = true;min = 0;break;case UpdateValueType.FireStationData_FireHelicopterCapacity: clamp = true;min = 0;break;case UpdateValueType.FireStationData_DisasterResponseCapacity: clamp = true;min = 0;break;case UpdateValueType.GarbageFacilityData_GarbageCapacity: clamp = true;min = 0;break;case UpdateValueType.GarbageFacilityData_VehicleCapacity: clamp = true;min = 0;break;case UpdateValueType.GarbageFacilityData_TransportCapacity: clamp = true;min = 0;break;case UpdateValueType.GarbageFacilityData_ProcessingSpeed: clamp = true;min = 0;break;case UpdateValueType.GarbagePoweredData_Capacity: clamp = true;min = 0;break;case UpdateValueType.GroundWaterPoweredData_Production: clamp = true;min = 0;break;case UpdateValueType.GroundWaterPoweredData_MaximumGroundWater: clamp = true;min = 0;break;case UpdateValueType.HospitalData_AmbulanceCapacity: clamp = true;min = 0;break;case UpdateValueType.HospitalData_MedicalHelicopterCapacity: clamp = true;min = 0;break;case UpdateValueType.HospitalData_PatientCapacity: clamp = true;min = 0;break;case UpdateValueType.HospitalData_TreatmentBonus: clamp = true;min = 0;break;case UpdateValueType.MaintenanceDepotData_VehicleCapacity: clamp = true;min = 0;break;case UpdateValueType.ParkingFacilityData_GarageMarkerCapacity: clamp = true;min = 0;break;case UpdateValueType.PoliceStationData_PatrolCarCapacity: clamp = true;min = 0;break;case UpdateValueType.PoliceStationData_PoliceHelicopterCapacity: clamp = true;min = 0;break;case UpdateValueType.PoliceStationData_JailCapacity: clamp = true;min = 0;break;case UpdateValueType.PostFacilityData_PostVanCapacity: clamp = true;min = 0;break;case UpdateValueType.PostFacilityData_PostTruckCapacity: clamp = true;min = 0;break;case UpdateValueType.PostFacilityData_MailCapacity: clamp = true;min = 0;break;case UpdateValueType.PostFacilityData_SortingRate: clamp = true;min = 0;break;case UpdateValueType.PowerPlantData_ElectricityProduction: clamp = true;min = 0;break;case UpdateValueType.PrisonData_PrisonVanCapacity: clamp = true;min = 0;break;case UpdateValueType.PrisonData_PrisonerCapacity: clamp = true;min = 0;break;case UpdateValueType.SchoolData_StudentCapacity: clamp = true;min = 0;break;case UpdateValueType.SewageOutletData_Capacity: clamp = true;min = 0;break;case UpdateValueType.SolarPoweredData_Production: clamp = true;min = 0;break;case UpdateValueType.TransportDepotData_VehicleCapacity: clamp = true;min = 0;break;case UpdateValueType.WaterPumpingStationData_Capacity: clamp = true;min = 0;break;case UpdateValueType.WindPoweredData_Production: clamp = true;min = 0;break;case UpdateValueType.WorkplaceData_MaxWorkers: clamp = true;min = 0;break;case UpdateValueType.WorkplaceData_MinimumWorkersLimit: clamp = true;min = 0;break;case UpdateValueType.WorkplaceData_WorkConditions: clamp = true;min = 0;max = 100;break;
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }

        public static void TryLimit(UpdateValueType updateValueType, ref float value)
        {
            bool clamp;
            float min = float.MinValue;
            float max = float.MaxValue;
            switch (updateValueType)
            {
case UpdateValueType.BuildingPropertyData_SpaceMultiplier: clamp = true;min = 0;break;case UpdateValueType.CargoTransportStationData_WorkMultiplier: clamp = true;min = 0;break;case UpdateValueType.ConsumptionData_ElectricityConsumption: clamp = true;min = 0;break;case UpdateValueType.ConsumptionData_WaterConsumption: clamp = true;min = 0;break;case UpdateValueType.ConsumptionData_GarbageAccumulation: clamp = true;min = 0;break;case UpdateValueType.ConsumptionData_TelecomNeed: clamp = true;min = 0;break;case UpdateValueType.CoverageData_Range: clamp = true;min = 0;break;case UpdateValueType.CoverageData_Capacity: clamp = true;min = 0;break;case UpdateValueType.CoverageData_Magnitude: clamp = true;min = 0;break;case UpdateValueType.DeathcareFacilityData_ProcessingRate: clamp = true;min = 0;break;case UpdateValueType.FireStationData_VehicleEfficiency: clamp = true;min = 0;break;case UpdateValueType.GarbagePoweredData_ProductionPerUnit: clamp = true;min = 0;break;case UpdateValueType.MaintenanceDepotData_VehicleEfficiency: clamp = true;min = 0;break;case UpdateValueType.ParkingFacilityData_ComfortFactor: clamp = true;min = 0;break;case UpdateValueType.SchoolData_GraduationModifier: clamp = true;min = 0;break;case UpdateValueType.SewageOutletData_Purification: clamp = true;min = 0;break;case UpdateValueType.TelecomFacilityData_Range: clamp = true;min = 0;break;case UpdateValueType.TelecomFacilityData_NetworkCapacity: clamp = true;min = 0;break;case UpdateValueType.TransportDepotData_ProductionDuration: clamp = true;min = 0;break;case UpdateValueType.TransportDepotData_MaintenanceDuration: clamp = true;min = 0;break;case UpdateValueType.TransportStationData_ComfortFactor: clamp = true;min = 0;break;case UpdateValueType.TransportStationData_LoadingFactor: clamp = true;min = 0;break;case UpdateValueType.WaterPoweredData_ProductionFactor: clamp = true;min = 0;break;case UpdateValueType.WaterPoweredData_CapacityFactor: clamp = true;min = 0;break;case UpdateValueType.WaterPumpingStationData_Purification: clamp = true;min = 0;break;case UpdateValueType.WindPoweredData_MaximumWind: clamp = true;min = 0;break;case UpdateValueType.WorkplaceData_EveningShiftProbability: clamp = true;min = 0;max = 1;break;case UpdateValueType.WorkplaceData_NightShiftProbability: clamp = true;min = 0;max = 1;break;
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }

        public static void TryLimit(UpdateValueType updateValueType, ref short value)
        {
            bool clamp;
            short min = short.MinValue;
            short max = short.MaxValue;
            switch (updateValueType)
            {
case UpdateValueType.ParkData_MaintenancePool: clamp = true;min = 0;break;
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }

        public static void TryLimit(UpdateValueType updateValueType, ref sbyte value)
        {
            bool clamp;
            sbyte min = sbyte.MinValue;
            sbyte max = sbyte.MaxValue;
            switch (updateValueType)
            {
case UpdateValueType.PrisonData_PrisonerWellbeing: clamp = true;min = 0;break;case UpdateValueType.PrisonData_PrisonerHealth: clamp = true;min = 0;break;case UpdateValueType.SchoolData_StudentWellbeing: clamp = true;min = 0;break;case UpdateValueType.SchoolData_StudentHealth: clamp = true;min = 0;break;
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }

        public static void TryLimit(UpdateValueType updateValueType, ref byte value)
        {
            bool clamp;
            byte min = byte.MinValue;
            byte max = byte.MaxValue;
            switch (updateValueType)
            {
case UpdateValueType.SchoolData_EducationLevel: clamp = true;min = 1;max = 4;break;case UpdateValueType.SpawnableBuildingData_Level: clamp = true;min = 1;max = 5;break;
            
                default:
                    return;
            }
            if (clamp)
                Math.Clamp(value, min, max);
        }

    }
}
