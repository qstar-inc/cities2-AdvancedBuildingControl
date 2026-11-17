import { Entity } from "cs2/bindings";
import { BldgEffectInfo } from "types/BldgEffectsInfo";

export interface EditorBuildingData {
  PrefabName: string;
  Entity: Entity;
  BldgEffectInfo: BldgEffectInfo;
}
