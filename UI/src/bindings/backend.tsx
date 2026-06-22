import engine from "cohtml/cohtml";
import { bindValue, trigger } from "cs2/api";
import { Entity } from "cs2/bindings";
import mod from "mod.json";
import { BldgCleanupType as BldgCleanupType } from "types/BldgCleanupInfo";
import { UpdateValueType } from "types/UpdateValueType";

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

export const ChangeUVTValueString = (
  value: string,
  valueType: UpdateValueType,
) => {
  trigger(mod.id, "ChangeComponentValue", value, valueType);
};

export const ChangeUVTValue = (value: number, valueType: UpdateValueType) => {
  console.log(
    `trigger(${mod.id}, "ChangeComponentValue", ${value}, ${valueType});`,
  );
  trigger(mod.id, "ChangeComponentValue", `${value}`, valueType);
};

export const ResetUVTValue = (valueType: UpdateValueType) => {
  trigger(mod.id, "ResetComponentValue", valueType);
};

export const ChangeBCTValue = (value: number, valueType: BldgCleanupType) => {
  trigger(mod.id, "ChangeBCTValue", `${value}`, valueType);
};

export const TriggerBCT = (valueType: BldgCleanupType) => {
  trigger(mod.id, "TriggerBCT", valueType);
};
