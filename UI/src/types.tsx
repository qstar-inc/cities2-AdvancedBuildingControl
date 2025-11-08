import { Entity } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import mod from "mod.json";

export class LocaleKeys {
  public static NAME: string = `${mod.id}.NAME`;

  public static RANDOMIZE_TOOLTIP: string = `${mod.id}.SIP[RandomizeTooltip]`;
  public static BRAND_TOOLTIP: string = `${mod.id}.SIP[BrandTooltip]`;
  public static STORAGE_TOOLTIP: string = `${mod.id}.SIP[StorageTooltip]`;
  public static UTILITY_TOOLTIP: string = `${mod.id}.SIP[UtilityTooltip]`;

  public static BRAND_HEADER: string = `${mod.id}.BRAND[Header]`;
  public static BRAND_CURRENT_BRAND: string = `${mod.id}.BRAND[CurrentBrand]`;
  public static BRAND_CURRENT_COMPANY: string = `${mod.id}.BRAND[CurrentCompany]`;
  public static BRAND_OTHER_LIST: string = `${mod.id}.BRAND[OtherBrands]`;
  public static BRAND_OTHER_TOOLTIP: string = `${mod.id}.BRAND[OtherBrandsTooltip]`;
  public static BRAND_SUPPORTED_LIST: string = `${mod.id}.BRAND[SupportedBrands]`;
  public static BRAND_SUPPORTED_TOOLTIP: string = `${mod.id}.BRAND[SupportedBrandsTooltip]`;
  public static BRAND_GROUP_HOVER: string = `${mod.id}.BRAND[BrandGroupHover]`;

  public static ZONING_HEADER: string = `${mod.id}.ZONING[Header]`;
  public static ZONING_INFORMATION: string = `${mod.id}.ZONING[Information]`;
  public static ZONING_CHANGE_LEVEL: string = `${mod.id}.ZONING[ChangeLevel]`;
  public static ZONING_CURRENT_UPKEEP: string = `${mod.id}.ZONING[CurrentUpkeep]`;
  public static ZONING_RESET_LEVEL_TOOLTIP: string = `${mod.id}.ZONING[ResetLevelTooltip]`;
  public static ZONING_CHANGE_HOUSEHOLD: string = `${mod.id}.ZONING[ChangeHousehold]`;
  public static ZONING_MAX_HOUSEHOLD: string = `${mod.id}.ZONING[MaxHousehold]`;
  public static ZONING_MAX_HOUSEHOLD_TOOLTIP: string = `${mod.id}.ZONING[MaxHouseholdTooltip]`;
  public static ZONING_CURRENT_RENT: string = `${mod.id}.ZONING[CurrentRent]`;
  public static ZONING_RESET_HOUSEHOLD_TOOLTIP: string = `${mod.id}.ZONING[ResetHouseholdTooltip]`;
  public static ZONING_CHANGE_WORKPLACE: string = `${mod.id}.ZONING[ChangeWorkplace]`;
  public static ZONING_ORIGINAL_WORKPLACE: string = `${mod.id}.ZONING[OriginalWorkplace]`;
  public static ZONING_RESET_WORKPLACE_TOOLTIP: string = `${mod.id}.ZONING[ResetWorkplaceTooltip]`;

  public static STORAGE_HEADER: string = `${mod.id}.STORAGE[Header]`;
  public static STORAGE_INFORMATION: string = `${mod.id}.STORAGE[Information]`;
  public static STORAGE_RESET_TOOLTIP: string = `${mod.id}.STORAGE[ResetTooltip]`;

  public static UTILITIES_HEADER: string = `${mod.id}.UTILITIES[Header]`;
  public static UTILITIES_INFORMATION: string = `${mod.id}.UTILITIES[Information]`;

  public static UTILITIES_CHANGE_PUMP_CAP: string = `${mod.id}.UTILITIES[ChangePumpCap]`;
  public static UTILITIES_OG_PUMP_CAP: string = `${mod.id}.UTILITIES[OriginalPumpCap]`;
  public static UTILITIES_ACTUAL_PUMP_CAP: string = `${mod.id}.UTILITIES[ActualPumpCap]`;
  public static UTILITIES_RESET_PUMP_CAP: string = `${mod.id}.UTILITIES[ResetPumpCapTooltip]`;

  public static UTILITIES_CHANGE_DUMP_CAP: string = `${mod.id}.UTILITIES[ChangeDumpCap]`;
  public static UTILITIES_OG_DUMP_CAP: string = `${mod.id}.UTILITIES[OriginalDumpCap]`;
  public static UTILITIES_ACTUAL_DUMP_CAP: string = `${mod.id}.UTILITIES[ActualDumpCap]`;
  public static UTILITIES_RESET_DUMP_CAP: string = `${mod.id}.UTILITIES[ResetDumpCapTooltip]`;

  public static UTILITIES_CHANGE_POWER_CAP: string = `${mod.id}.UTILITIES[ChangePowerCap]`;
  public static UTILITIES_OG_POWER_CAP: string = `${mod.id}.UTILITIES[OriginalPowerCap]`;
  public static UTILITIES_ACTUAL_POWER_CAP: string = `${mod.id}.UTILITIES[ActualPowerCap]`;
  public static UTILITIES_RESET_POWER_CAP: string = `${mod.id}.UTILITIES[ResetPowerCapTooltip]`;

  public static UTILITIES_PowerProduction: string = `${mod.id}.UTILITIES[PowerProduction]`;
  public static UTILITIES_Change: string = `${mod.id}.UTILITIES[Change]`;
  public static UTILITIES_Original: string = `${mod.id}.UTILITIES[Original]`;
  public static UTILITIES_Boosted: string = `${mod.id}.UTILITIES[Boosted]`;
}

export function FindTranslation(str: string, vanilla: boolean = false) {
  const { translate } = useLocalization();
  if (vanilla) return translate(str) ?? `${str}`;
  return translate(`${mod.id}.${str}`) ?? `${str}`;
}

export interface BrandDataInfo {
  Name: string;
  PrefabName: string;
  Color1: string;
  Color2: string;
  Color3: string;
  Entity: Entity;
  Icon: string;
  Companies: string[];
}

export interface ZoneDataInfo {
  Name: string;
  PrefabName: string;
  Color1: string;
  Color2: string;
  Entity: Entity;
  Icon: string;
  AreaTypeString: string;
}

export enum ResourceGroup {
  None = 0,
  Raw = 1,
  Processed = 2,
  Mail = 3,
  Others = 4,
  Money = 5,
}

export interface ResourceDataInfo {
  Group: ResourceGroup;
  Id: bigint;
  Name: string;
  Icon: string;
  DisplayName: string;
}

export enum ValueType {
  None,
  Storage,
  Level,
  Household,
  WaterPump,
  SewageCap,
  SewagePurification,
  PowerPlant,
  Depot,
  GarbageTruck,
  Ambulance,
  MediHeli,
  Hearse,
  All,
}

export interface BldgGeneralInfo {
  Efficiency: number;
  HasHeli: boolean;
}

export interface BldgZoningInfo {
  HasLevel: boolean;
  Level: number;
  Upkeep: number;
  HasHousehold: boolean;
  Household: number;
  MaxHousehold: number;
  Rent: number;
  AreaType: string;
  // SpaceMultiplier: number;
  // ZoneTypeBase: number;
  // TotalRent: number;
  // PropertiesCount: number;
  // MixedPercent: number;
  // LandValueBase: number;
  // LandValueModifier: number;
  // IgnoreLandValue: boolean;
  // LotSize: number;
  // IsMixed: boolean;
  HasWorkplace: boolean;
  CurrentMaxWorkplaceCount: number;
  OriginalMaxWorkplaceCount: number;
}

export interface BldgBrandInfo {
  HasBrand: boolean;
  BrandName: string;
  BrandIcon: string;
  CompanyName: string;
  BrandList: BrandDataInfo[];
}

export interface BldgStorageInfo {
  HasStorage: boolean;
  BuildingResources: ResourceDataInfo[];
  BuildingResourcesAll: ResourceDataInfo[];
}

export interface BldgUtilityInfo {
  IsWaterPump: boolean;
  CurrentWaterPumpCap: number;
  OriginalWaterPumpCap: number;
  IsSewageOutlet: boolean;
  CurrentSewageDumpCap: number;
  OriginalSewageDumpCap: number;
  CurrentSewagePurification: number;
  OriginalSewagePurification: number;
  IsPowerPlant: boolean;
  CurrentPowerProdCap: number;
  OriginalPowerProdCap: number;
}

export interface BldgGeneralInfo {
  Efficiency: number;
  HasHeli: boolean;
}

export interface VehicleInfo {
  Current: number;
  Original: number;
  Combined: number;
}

export interface BldgVehicleInfo {
  IsDepot: boolean;
  TransportType: string;
  DepotVehicle: VehicleInfo;
  // CurrentDepotCap: number;
  // OriginalDepotCap: number;
  // CombinedDepotCap: number;
  IsGarbageFacility: boolean;
  GarbageTruck: VehicleInfo;
  // CurrentGarbageTruckCap: number;
  // OriginalGarbageTruckCap: number;
  // CombinedGarbageTruckCap: number;
  IsHospital: boolean;
  Ambulance: VehicleInfo;
  MediHeli: VehicleInfo;
  // CurrentAmbulanceCap: number;
  // OriginalAmbulanceCap: number;
  // CombinedAmbulanceCap: number;
  // CurrentMediHeliCap: number;
  // OriginalMediHeliCap: number;
  // CombinedMediHeliCap: number;
  IsDeathcare: boolean;
  Hearse: VehicleInfo;
  // CurrentHearseCap: number;
  // OriginalHearseCap: number;
  // CombinedHearseCap: number;
}
