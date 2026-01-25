import { UpdateValueType } from "types/UpdateValueType";

export function GetFlags(valueType: UpdateValueType) {
  switch (valueType) {
    case UpdateValueType.TransportDepotData_TransportType:
      return [[-1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13], true];
    case UpdateValueType.WaterPumpingStationData_Types:
      return [[1, 2], true];
    case UpdateValueType.TransportDepotData_EnergyTypes:
    case UpdateValueType.TransportStationData_CarRefuelTypes:
    case UpdateValueType.TransportStationData_TrainRefuelTypes:
    case UpdateValueType.TransportStationData_WatercraftRefuelTypes:
    case UpdateValueType.TransportStationData_AircraftRefuelTypes:
    case UpdateValueType.TransportDepotData_SizeClass:
    case UpdateValueType.WorkplaceData_Complexity:
      return [[0, 1, 2, 3], true];
    case UpdateValueType.SchoolData_EducationLevel:
      return [[1, 2, 3, 4], true];
    case UpdateValueType.PoliceStationData_PurposeMask:
      return [[1, 2, 4], true];
    case UpdateValueType.MaintenanceDepotData_MaintenanceType:
      return [[1, 2, 4, 8], true];
    case UpdateValueType.ParkingFacilityData_RoadTypes:
      return [[1, 16], true];
    default:
      return [[], false];
  }
}

export function isMultiSelect(valueType: UpdateValueType) {
  switch (valueType) {
    case UpdateValueType.MaintenanceDepotData_MaintenanceType:
    case UpdateValueType.ParkingFacilityData_RoadTypes:
    case UpdateValueType.WaterPumpingStationData_Types:
    case UpdateValueType.PoliceStationData_PurposeMask:
      return false;
    default:
      return true;
  }
}
