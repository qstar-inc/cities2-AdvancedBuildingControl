namespace AdvancedBuildingControl.Variables
{
    public partial class UVTHelper
    {
        public static ValueFormat GetValueFormat(UpdateValueType updateValueType)
        {
            switch (updateValueType)
            {
                case UpdateValueType._None:
                case UpdateValueType._All:
                    break;

                // start
                case UpdateValueType.AttractionData_Attractiveness:
                case UpdateValueType.BatteryData_Capacity:
                case UpdateValueType.BatteryData_PowerOutput:
                case UpdateValueType.BuildingPropertyData_ResidentialProperties:
                case UpdateValueType.ConsumptionData_Upkeep:
                case UpdateValueType.DeathcareFacilityData_HearseCapacity:
                case UpdateValueType.DeathcareFacilityData_StorageCapacity:
                case UpdateValueType.EmergencyGeneratorData_ElectricityProduction:
                case UpdateValueType.EmergencyShelterData_ShelterCapacity:
                case UpdateValueType.EmergencyShelterData_VehicleCapacity:
                case UpdateValueType.FireStationData_FireEngineCapacity:
                case UpdateValueType.FireStationData_FireHelicopterCapacity:
                case UpdateValueType.FireStationData_DisasterResponseCapacity:
                case UpdateValueType.GarbageFacilityData_GarbageCapacity:
                case UpdateValueType.GarbageFacilityData_VehicleCapacity:
                case UpdateValueType.GarbageFacilityData_TransportCapacity:
                case UpdateValueType.GarbageFacilityData_ProcessingSpeed:
                case UpdateValueType.GarbagePoweredData_Capacity:
                case UpdateValueType.GroundWaterPoweredData_Production:
                case UpdateValueType.GroundWaterPoweredData_MaximumGroundWater:
                case UpdateValueType.HospitalData_AmbulanceCapacity:
                case UpdateValueType.HospitalData_MedicalHelicopterCapacity:
                case UpdateValueType.HospitalData_PatientCapacity:
                case UpdateValueType.HospitalData_TreatmentBonus:
                case UpdateValueType.MaintenanceDepotData_MaintenanceType:
                case UpdateValueType.MaintenanceDepotData_VehicleCapacity:
                case UpdateValueType.ParkingFacilityData_RoadTypes:
                case UpdateValueType.ParkingFacilityData_GarageMarkerCapacity:
                case UpdateValueType.PoliceStationData_PatrolCarCapacity:
                case UpdateValueType.PoliceStationData_PoliceHelicopterCapacity:
                case UpdateValueType.PoliceStationData_JailCapacity:
                case UpdateValueType.PoliceStationData_PurposeMask:
                case UpdateValueType.PostFacilityData_PostVanCapacity:
                case UpdateValueType.PostFacilityData_PostTruckCapacity:
                case UpdateValueType.PostFacilityData_MailCapacity:
                case UpdateValueType.PostFacilityData_SortingRate:
                case UpdateValueType.PowerPlantData_ElectricityProduction:
                case UpdateValueType.PrisonData_PrisonVanCapacity:
                case UpdateValueType.PrisonData_PrisonerCapacity:
                case UpdateValueType.SchoolData_StudentCapacity:
                case UpdateValueType.SewageOutletData_Capacity:
                case UpdateValueType.SolarPoweredData_Production:
                case UpdateValueType.TransportDepotData_TransportType:
                case UpdateValueType.TransportDepotData_EnergyTypes:
                case UpdateValueType.TransportDepotData_SizeClass:
                case UpdateValueType.TransportDepotData_VehicleCapacity:
                case UpdateValueType.TransportStationData_CarRefuelTypes:
                case UpdateValueType.TransportStationData_TrainRefuelTypes:
                case UpdateValueType.TransportStationData_WatercraftRefuelTypes:
                case UpdateValueType.TransportStationData_AircraftRefuelTypes:
                case UpdateValueType.WaterPumpingStationData_Types:
                case UpdateValueType.WaterPumpingStationData_Capacity:
                case UpdateValueType.WindPoweredData_Production:
                case UpdateValueType.WorkplaceData_Complexity:
                case UpdateValueType.WorkplaceData_MaxWorkers:
                case UpdateValueType.WorkplaceData_MinimumWorkersLimit:
                case UpdateValueType.WorkplaceData_WorkConditions:
                    return ValueFormat.Int;
                case UpdateValueType.BuildingPropertyData_AllowedSold:
                case UpdateValueType.BuildingPropertyData_AllowedInput:
                case UpdateValueType.BuildingPropertyData_AllowedManufactured:
                case UpdateValueType.BuildingPropertyData_AllowedStored:
                case UpdateValueType.StorageCompanyData_StoredResources:
                    return ValueFormat.Ulong;
                case UpdateValueType.BuildingPropertyData_SpaceMultiplier:
                case UpdateValueType.CargoTransportStationData_WorkMultiplier:
                case UpdateValueType.ConsumptionData_ElectricityConsumption:
                case UpdateValueType.ConsumptionData_WaterConsumption:
                case UpdateValueType.ConsumptionData_GarbageAccumulation:
                case UpdateValueType.ConsumptionData_TelecomNeed:
                case UpdateValueType.CoverageData_Range:
                case UpdateValueType.CoverageData_Capacity:
                case UpdateValueType.CoverageData_Magnitude:
                case UpdateValueType.DeathcareFacilityData_ProcessingRate:
                case UpdateValueType.DestructibleObjectData_FireHazard:
                case UpdateValueType.DestructibleObjectData_StructuralIntegrity:
                case UpdateValueType.FireStationData_VehicleEfficiency:
                case UpdateValueType.GarbagePoweredData_ProductionPerUnit:
                case UpdateValueType.MaintenanceDepotData_VehicleEfficiency:
                case UpdateValueType.ParkingFacilityData_ComfortFactor:
                case UpdateValueType.SchoolData_GraduationModifier:
                case UpdateValueType.SewageOutletData_Purification:
                case UpdateValueType.TelecomFacilityData_Range:
                case UpdateValueType.TelecomFacilityData_NetworkCapacity:
                case UpdateValueType.TransportDepotData_ProductionDuration:
                case UpdateValueType.TransportDepotData_MaintenanceDuration:
                case UpdateValueType.TransportStationData_ComfortFactor:
                case UpdateValueType.TransportStationData_LoadingFactor:
                case UpdateValueType.WaterPoweredData_ProductionFactor:
                case UpdateValueType.WaterPoweredData_CapacityFactor:
                case UpdateValueType.WaterPumpingStationData_Purification:
                case UpdateValueType.WindPoweredData_MaximumWind:
                case UpdateValueType.WorkplaceData_EveningShiftProbability:
                case UpdateValueType.WorkplaceData_NightShiftProbability:
                    return ValueFormat.Float;
                case UpdateValueType.BuildingTerraformData_DontRaise:
                case UpdateValueType.BuildingTerraformData_DontLower:
                case UpdateValueType.DeathcareFacilityData_LongTermStorage:
                case UpdateValueType.GarbageFacilityData_IndustrialWasteOnly:
                case UpdateValueType.GarbageFacilityData_LongTermStorage:
                case UpdateValueType.HospitalData_TreatDiseases:
                case UpdateValueType.HospitalData_TreatInjuries:
                case UpdateValueType.ParkData_AllowHomeless:
                case UpdateValueType.TelecomFacilityData_PenetrateTerrain:
                case UpdateValueType.TransportDepotData_DispatchCenter:
                    return ValueFormat.Bool;
                case UpdateValueType.HospitalData_HealthRange:
                case UpdateValueType.StorageCompanyData_TransportInterval:
                    return ValueFormat.Int2;
                case UpdateValueType.ParkData_MaintenancePool:
                    return ValueFormat.Short;
                case UpdateValueType.PrisonData_PrisonerWellbeing:
                case UpdateValueType.PrisonData_PrisonerHealth:
                case UpdateValueType.SchoolData_StudentWellbeing:
                case UpdateValueType.SchoolData_StudentHealth:
                    return ValueFormat.SByte;
                case UpdateValueType.SchoolData_EducationLevel:
                case UpdateValueType.SpawnableBuildingData_Level:
                    return ValueFormat.Byte;
                //end
            }

            return ValueFormat.Unknown;
        }
    }
}
