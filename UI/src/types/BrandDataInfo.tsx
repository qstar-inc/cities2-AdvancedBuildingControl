import { Entity } from "cs2/bindings";

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

export interface BldgBrandInfo {
  HasBrand: boolean;
  BrandName: string;
  BrandIcon: string;
  CompanyName: string;
  BrandList: BrandDataInfo[];
}
