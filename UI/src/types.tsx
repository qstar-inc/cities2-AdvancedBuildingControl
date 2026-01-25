import { Entity } from "cs2/bindings";

// export class LocaleKeys {
//   public static NAME: string = `${mod.id}.NAME`;

//   public static RANDOMIZE_TOOLTIP: string = `${mod.id}.SIP[RandomizeTooltip]`;
//   public static BRAND_TOOLTIP: string = `${mod.id}.SIP[BrandTooltip]`;
//   public static STORAGE_TOOLTIP: string = `${mod.id}.SIP[StorageTooltip]`;
//   public static UTILITY_TOOLTIP: string = `${mod.id}.SIP[UtilityTooltip]`;

//   public static BRAND_HEADER: string = `${mod.id}.BRAND[Header]`;
//   public static BRAND_CURRENT_BRAND: string = `${mod.id}.BRAND[CurrentBrand]`;
//   public static BRAND_CURRENT_COMPANY: string = `${mod.id}.BRAND[CurrentCompany]`;
//   public static BRAND_OTHER_LIST: string = `${mod.id}.BRAND[OtherBrands]`;
//   public static BRAND_OTHER_TOOLTIP: string = `${mod.id}.BRAND[OtherBrandsTooltip]`;
//   public static BRAND_SUPPORTED_LIST: string = `${mod.id}.BRAND[SupportedBrands]`;
//   public static BRAND_SUPPORTED_TOOLTIP: string = `${mod.id}.BRAND[SupportedBrandsTooltip]`;
//   public static BRAND_GROUP_HOVER: string = `${mod.id}.BRAND[BrandGroupHover]`;
// }

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

export interface BldgBrandInfo {
  HasBrand: boolean;
  BrandName: string;
  BrandIcon: string;
  CompanyName: string;
  BrandList: BrandDataInfo[];
}
