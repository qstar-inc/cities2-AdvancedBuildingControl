import { CityModifierType, Color } from "cs2/bindings";
import { LocalModifierType } from "shared/enums/LocalModifierType";
import { ModifierRadiusCombineMode } from "shared/enums/ModifierRadiusCombineMode";
import { ModifierValueMode } from "shared/enums/ModifierValueMode";

export interface BldgEffectInfo {
  LocalModifiers: BldgEffectTypeInfoLocal[];
  CityModifiers: BldgEffectTypeInfoCity[];
  Color: Color;
}

export interface BldgEffectTypeInfoLocal {
  Enabled: boolean;
  Customized: boolean;
  Type: LocalModifierType;
  Mode: ModifierValueMode;
  Combine: ModifierRadiusCombineMode;
  Delta: number;
  Radius: number;
}

export interface BldgEffectTypeInfoCity {
  Enabled: boolean;
  Customized: boolean;
  Type: CityModifierType;
  Mode: ModifierValueMode;
  Range: number;
}

// export const GetEffectsMetadata = (EffectsType: BldgEffectsType) => {
//   switch (EffectsType) {
//     case BldgEffectsType.Garbage:
//       return [EffectsValueType.Number, "kg"];

//     case BldgEffectsType.OutgoingMail:
//       return [EffectsValueType.Number, "pcs"];

//     case BldgEffectsType.Crime:
//       return [EffectsValueType.Number, "Accumulate"];

//     case BldgEffectsType.PhysicalDamage:
//     case BldgEffectsType.FireDamage:
//     case BldgEffectsType.WaterDamage:
//       return [EffectsValueType.Number, "%"];

//     case BldgEffectsType.ClearAbandonment:
//     case BldgEffectsType.ClearNotification:
//       return [EffectsValueType.Trigger, ""];

//     default:
//       return [EffectsValueType._None, ""];
//   }
// };
