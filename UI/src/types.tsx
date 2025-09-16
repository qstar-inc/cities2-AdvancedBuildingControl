import { Entity } from "cs2/bindings";
import mod from "mod.json";

export class LocaleKeys {
  public static NAME: string = `${mod.id}.NAME`;

  public static RANDOMIZE_TOOLTIP: string = `${mod.id}.SIP[RandomizeTooltip]`;
  public static BRAND_CHANGER_TOOLTIP: string = `${mod.id}.SIP[BrandChangerTooltip]`;
  public static STORAGE_CHANGER_TOOLTIP: string = `${mod.id}.SIP[StorageChangerTooltip]`;

  public static BRAND_HEADER: string = `${mod.id}.BRAND[Header]`;
  public static BRAND_CURRENT_BRAND: string = `${mod.id}.BRAND[CurrentBrand]`;
  public static BRAND_CURRENT_COMPANY: string = `${mod.id}.BRAND[CurrentCompany]`;
  public static BRAND_OTHER_LIST: string = `${mod.id}.BRAND[OtherBrands]`;
  public static BRAND_OTHER_TOOLTIP: string = `${mod.id}.BRAND[OtherBrandsTooltip]`;
  public static BRAND_SUPPORTED_LIST: string = `${mod.id}.BRAND[SupportedBrands]`;
  public static BRAND_SUPPORTED_TOOLTIP: string = `${mod.id}.BRAND[SupportedBrandsTooltip]`;
  public static BRAND_GROUP_HOVER: string = `${mod.id}.BRAND[BrandGroupHover]`;

  public static ZONING_HEADER: string = `${mod.id}.ZONING[Header]`;
  public static ZONING_DESCRIPTION: string = `${mod.id}.ZONING[Description]`;
  public static ZONING_CHANGE_LEVEL: string = `${mod.id}.ZONING[ChangeLevel]`;
  public static ZONING_CURRENT_UPKEEP: string = `${mod.id}.ZONING[CurrentUpkeep]`;
  public static ZONING_CREATE_VARIANTS: string = `${mod.id}.ZONING[CreateVariants]`;

  public static STORAGE_HEADER: string = `${mod.id}.STORAGE[Header]`;
  public static STORAGE_INFORMATION: string = `${mod.id}.STORAGE[Information]`;
  public static STORAGE_RESET_TOOLTIP: string = `${mod.id}.STORAGE[ResetTooltip]`;

  public static RESOURCE_RAW: string = `SelectedInfoPanel.RAW_MATERIALS`;
  public static RESOURCE_PROCESSED: string = `SelectedInfoPanel.PROCESSED_GOODS`;
  public static RESOURCE_MAIL: string = `SelectedInfoPanel.MAIL`;
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
