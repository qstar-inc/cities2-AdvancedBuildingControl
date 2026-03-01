export const SectionParams: Record<string, boolean> = {};

export function GetSectionOpen(key: string) {
  if (!(key in SectionParams)) {
    SectionParams[key] = false;
  }
  return SectionParams[key];
}

export function SetSectionOpen(key: string, value: boolean) {
  SectionParams[key] = value;
}
