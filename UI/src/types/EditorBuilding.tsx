import { Entity } from "cs2/bindings";

export interface EditorBuilding {
  PrefabName: string;
  Entity: Entity;
  UIIcon: string;
  Source: string;
  AssetPacks: Entity[];
  IsUpgrade: boolean;
  LocaleName: string;
}
