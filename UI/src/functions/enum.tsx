export function hasFlag(value: number, flag: number): boolean {
  return (value & flag) === flag;
}

export function addFlag(value: number, flag: number): number {
  return value | flag;
}

export function removeFlag(value: number, flag: number): number {
  return value & ~flag;
}

export function toggleFlag(value: number, flag: number): number {
  return hasFlag(value, flag) ? removeFlag(value, flag) : addFlag(value, flag);
}
