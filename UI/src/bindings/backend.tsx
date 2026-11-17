import engine from "cohtml/cohtml";
import { bindValue, trigger, ValueBinding } from "cs2/api";
import { Color, Entity } from "cs2/bindings";
import mod from "mod.json";
import { CityModifierType } from "shared/enums/CityModifierType";
import { LocalModifierType } from "shared/enums/LocalModifierType";
import { BldgCleanupType as BldgCleanupType } from "types/BldgCleanupInfo";
import {
  BldgEffectTypeInfoCity,
  BldgEffectTypeInfoLocal,
} from "types/BldgEffectsInfo";
import { EditorBuildingData } from "types/EditorBuildingData";
import { EditorBuilding } from "types/EditorBuilding";
import { UpdateValueType } from "types/UpdateValueType";
import { EditorBuildingAssetPack } from "types/EditorBuildingAssetPack";
import { EffectOptions } from "types/EffectOptions";

export const selectedEntity = bindValue<Entity>(
  "selectedInfo",
  "selectedEntity",
);

export const SetBrand = (replaceBrand: string) => {
  trigger(mod.id, "SetBrand", replaceBrand);
  engine.trigger("audio.playSound", "select-toggle", 1);
};

export const RandomizeStyle = () => {
  trigger(mod.id, "RandomizeStyle");
};

export const MakeSP = () => {
  trigger(mod.id, "MakeSP");
};

// Component
export const ChangeUVTValueString = (
  value: string,
  valueType: UpdateValueType,
) => {
  trigger(mod.id, "ChangeComponentValue", value, valueType);
};

export const ChangeUVTValue = (value: number, valueType: UpdateValueType) => {
  trigger(mod.id, "ChangeComponentValue", `${value}`, valueType);
};

export const ResetUVTValue = (valueType: UpdateValueType) => {
  trigger(mod.id, "ResetComponentValue", valueType);
};

// Cleanup
export const ChangeBCTValue = (value: number, valueType: BldgCleanupType) => {
  trigger(mod.id, "ChangeBCTValue", `${value}`, valueType);
};

export const TriggerBCT = (valueType: BldgCleanupType) => {
  trigger(mod.id, "TriggerBCT", valueType);
};

// Effects
export const ShowEffectOverlay = (options: EffectOptions) => {
  trigger(mod.id, "ShowEffectOverlay", options);
};

export const SetEffectOverlayColor = (color: Color) => {
  trigger(mod.id, "SetEffectOverlayColor", color);
};

export const SetRadiusCommon = (radiusCommon: boolean) => {
  trigger(mod.id, "SetRadiusCommon", radiusCommon);
};

export const RevertAllEffects = () => {
  trigger(mod.id, "RevertAllEffects");
};

export const SetLocalEffect = (type: BldgEffectTypeInfoLocal) => {
  trigger(mod.id, "SetLocalEffect", type);
};

export const SetLocalEnabled = (t: LocalModifierType, on: boolean) => {
  trigger(mod.id, "SetLocalEnabled", t, on);
};

export const AddLocalEffect = (t: LocalModifierType) => {
  trigger(mod.id, "AddLocalEffect", t);
};

export const RevertLocalEffect = (t: LocalModifierType) => {
  trigger(mod.id, "RevertLocalEffect", t);
};

export const RevertAllLocalEffects = () => {
  trigger(mod.id, "RevertAllLocalEffects");
};

export const SetCityEffect = (type: BldgEffectTypeInfoCity) => {
  trigger(mod.id, "SetCityEffect", type);
};

export const SetCityEnabled = (t: CityModifierType, on: boolean) => {
  trigger(mod.id, "SetCityEnabled", t, on);
};

export const AddCityEffect = (t: CityModifierType) => {
  trigger(mod.id, "AddCityEffect", t);
};

export const RevertCityEffect = (t: CityModifierType) => {
  trigger(mod.id, "RevertCityEffect", t);
};

export const RevertAllCityEffects = () => {
  trigger(mod.id, "RevertAllCityEffects");
};

// Building Editor
// export const EditorDataLoaded$: ValueBinding<boolean> = bindValue(
//   mod.id,
//   "EditorDataLoadedBinding",
//   false,
// );

export const EditorBuildings$: ValueBinding<EditorBuilding[]> = bindValue(
  mod.id,
  "EditorBuildings",
  [],
);
export const EditorBuildingSources$: ValueBinding<string[]> = bindValue(
  mod.id,
  "EditorBuildingSources",
  [],
);
export const EditorBuildingAssetPacks$: ValueBinding<
  EditorBuildingAssetPack[]
> = bindValue(mod.id, "EditorBuildingAssetPacks", []);

export const EditorBuildingData$: ValueBinding<EditorBuildingData> = bindValue(
  mod.id,
  "EditorBuildingData",
);

export const GetSelectedBuildingsData = (entity: Entity) => {
  trigger(mod.id, "GetSelectedBuildingsData", entity);
};
