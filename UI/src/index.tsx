import { ModRegistrar } from "cs2/modding";
import { SIP_ABC } from "mods/SIPSection";
import { SIP_ABC_District } from "mods/SIPSectionDistrict";

const register: ModRegistrar = (moduleRegistry) => {
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    SIP_ABC
  );
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    SIP_ABC_District
  );
};

export default register;
