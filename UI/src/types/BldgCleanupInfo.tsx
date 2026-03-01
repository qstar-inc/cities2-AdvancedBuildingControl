export interface BldgCleanupInfo {
  Enabled: boolean;
  Array: BldgCleanupTypeInfo[];
}

export enum BldgCleanupType {
  _None,
  Garbage,
  Crime,
  OutgoingMail,
  PhysicalDamage,
  FireDamage,
  WaterDamage,
  _All,
}

export enum CleanupValueType {
  _None,
  Number,
}

export interface BldgCleanupTypeInfo {
  Enabled: boolean;
  CleanupType: BldgCleanupType;
  CurrentValueNumber: number;
  // CurrentValueString: string;
}

export const GetCleanupMetadata = (cleanupType: BldgCleanupType) => {
  switch (cleanupType) {
    case BldgCleanupType.Garbage:
      return [CleanupValueType.Number, "kg"];

    case BldgCleanupType.OutgoingMail:
      return [CleanupValueType.Number, "pcs"];

    case BldgCleanupType.Crime:
      return [CleanupValueType.Number, "Accumulate"];

    case BldgCleanupType.PhysicalDamage:
    case BldgCleanupType.FireDamage:
    case BldgCleanupType.WaterDamage:
      return [CleanupValueType.Number, "%"];

    default:
      return [CleanupValueType._None, ""];
  }
};
