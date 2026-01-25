import { useLocalization } from "cs2/l10n";
import mod from "mod.json";
import { UpdateValueType } from "types/UpdateValueType";

export function FindTranslation(str: string, vanilla: boolean = false) {
  const { translate } = useLocalization();
  if (vanilla) return translate(str) ?? `${str}`;
  return translate(`${mod.id}.${str}`) ?? `${str}`;
}

export function GetCompName(str: string) {
  switch (str) {
    case "HospitalData":
      return FindTranslation("Infoviews.INFOMODE[Hospital Buildings]", true);
    case "MaintenanceDepotData":
      return FindTranslation(
        "Infoviews.INFOMODE[MaintenanceDepot Buildings]",
        true,
      );
    default:
      break;
  }
  return "Component Name Unknown";
}

export function nicifyVariableName(name?: string | null): string {
  if (!name) return "";

  if (name.startsWith("m_")) {
    name = name.substring(2);
  } else if (name.startsWith("Get")) {
    name = name.substring(3);
  }

  name = name.replace(/\B([A-Z][a-z])/g, " $1");
  name = name.replace(/([^A-Z\s])([A-Z])/g, "$1 $2");
  name = name.replace(/(?<![\d\s]|\dx|\d-)(\d)/g, " $1");
  name = name.replace(/^([a-z])/, m => m.toUpperCase());

  return name;
}

export function GetAlternateDropdownText(
  value: number,
  valueType: UpdateValueType,
) {
  switch (valueType) {
    case UpdateValueType.SchoolData_EducationLevel:
      switch (value) {
        case 1:
          return FindTranslation(
            "EducationInfoPanel.EDUCATION_LEVEL[Elementary]",
            true,
          );
        case 2:
          return FindTranslation(
            "EducationInfoPanel.EDUCATION_LEVEL[High School]",
            true,
          );
        case 3:
          return FindTranslation(
            "EducationInfoPanel.EDUCATION_LEVEL[College]",
            true,
          );
        case 4:
          return FindTranslation(
            "EducationInfoPanel.EDUCATION_LEVEL[University]",
            true,
          );

        default:
          break;
      }
    case UpdateValueType.MaintenanceDepotData_MaintenanceType:
      switch (value) {
        case 1:
          return FindTranslation("SelectedInfoPanel.PARK_TITLE", true);
        case 2:
          return FindTranslation("SubServices.NAME[TransportationRoad]", true);
        case 4:
          return FindTranslation("Assets.NAME[Snowplow01]", true);
        case 8:
          return FindTranslation("SelectedInfoPanel.TOOLTIP[Vehicle]", true);
        default:
          break;
      }
    case UpdateValueType.ParkingFacilityData_RoadTypes:
      switch (value) {
        case 1:
          return FindTranslation("SelectedInfoPanel.TOOLTIP[Vehicle]", true);
        case 16:
          return FindTranslation("Infoviews.INFOVIEW[Bicycles]", true);
        default:
          break;
      }
    case UpdateValueType.TransportDepotData_EnergyTypes:
    case UpdateValueType.TransportStationData_CarRefuelTypes:
    case UpdateValueType.TransportStationData_TrainRefuelTypes:
    case UpdateValueType.TransportStationData_WatercraftRefuelTypes:
    case UpdateValueType.TransportStationData_AircraftRefuelTypes:
      switch (value) {
        case 0:
          return FindTranslation("Common.NONE", true);
        case 1:
          return FindTranslation("Fuel");
        case 2:
          return FindTranslation(
            "Editor.ASSET_CATEGORY_TITLE[Buildings/Services/Electricity]",
            true,
          );
        case 3:
          return `${FindTranslation("Fuel")} ${FindTranslation("and")} ${FindTranslation(
            "Editor.ASSET_CATEGORY_TITLE[Buildings/Services/Electricity]",
            true,
          )}`;
        default:
          break;
      }
    case UpdateValueType.TransportDepotData_SizeClass:
      switch (value) {
        case 0:
          return FindTranslation("Small");
        case 1:
          return FindTranslation("Medium");
        case 2:
          return FindTranslation("Large");
        case 3:
          return FindTranslation("Undefined");
        default:
          break;
      }
    case UpdateValueType.TransportDepotData_TransportType:
      switch (value) {
        case -1:
          return FindTranslation("Common.NONE", true);
        case 0:
          return FindTranslation("Transport.LEGEND_VEHICLES[Bus]", true);
        case 1:
          return FindTranslation("Transport.LEGEND_VEHICLES[Train]", true);
        case 2:
          return FindTranslation("Transport.LEGEND_VEHICLES[Taxi]", true);
        case 3:
          return FindTranslation("Transport.LEGEND_VEHICLES[Tram]", true);
        case 4:
          return FindTranslation("Transport.LEGEND_VEHICLES[Ship]", true);
        case 5:
          return FindTranslation("Infoviews.INFOMODE[Post Van Vehicles]", true);
        case 6:
          return FindTranslation("Transport.LEGEND_VEHICLES[Helicopter]", true);
        case 7:
          return FindTranslation("Transport.LEGEND_VEHICLES[Airplane]", true);
        case 8:
          return FindTranslation(
            "SubServices.NAME[TransportationSubway]",
            true,
          );
        case 9:
          return FindTranslation("Transport.LEGEND_VEHICLES[Rocket]", true);
        case 10:
          return FindTranslation("Editor.WORKSPACE", true);
        case 11:
          return FindTranslation("Transport.LEGEND_VEHICLES[Ferry]", true);
        case 12:
          return FindTranslation("Infoviews.INFOVIEW[Bicycles]", true);
        case 13:
          return FindTranslation(
            "Editor.ASSET_CATEGORY_TITLE[Vehicles/Residential/Cars]",
            true,
          );
        default:
          break;
      }
    case UpdateValueType.WaterPumpingStationData_Types:
      switch (value) {
        case 0:
          return FindTranslation("Common.NONE", true);
        case 1:
          return FindTranslation("Properties.MAP_RESOURCE[GroundWater]", true);
        case 2:
          return FindTranslation("Properties.MAP_RESOURCE[SurfaceWater]", true);
        default:
          break;
      }
    case UpdateValueType.PoliceStationData_PurposeMask:
      switch (value) {
        case 1:
          return FindTranslation(
            "SelectedInfoPanel.POLICE_VEHICLES[Patrol]",
            true,
          ).split(" ")[0];
        case 2:
          return FindTranslation(
            "SelectedInfoPanel.POLICE_VEHICLE_TITLE[PoliceEmergencyCar]",
            true,
          ).split(" ")[0];
        case 4:
          return FindTranslation(
            "SelectedInfoPanel.POLICE_VEHICLES[Intelligence]",
            true,
          ).split(" ")[0];
        default:
          break;
      }
    case UpdateValueType.WorkplaceData_Complexity:
      switch (value) {
        case 0:
          return FindTranslation("WorkplaceComplexity[Manual]");
        case 1:
          return FindTranslation("WorkplaceComplexity[Simple]");
        case 2:
          return FindTranslation("WorkplaceComplexity[Complex]");
        case 3:
          return FindTranslation("WorkplaceComplexity[Hitech]");
        default:
          break;
      }
    default:
      break;
  }
  return `${value}`;
}

export function GetComponentTooltip(valueType: UpdateValueType) {
  const tt = FindTranslation(`CompTooltip[${valueType.toString()}]`);

  if (tt != `CompTooltip[${valueType.toString()}]`) return tt;

  return FindTranslation("NoTooltipYet");
}
