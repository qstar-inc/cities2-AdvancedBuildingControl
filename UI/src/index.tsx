import { ModRegistrar } from "cs2/modding";
import { SIPSystem } from "mods/SIPSection";
import { ABCPanelButton } from "mods/ABCPanelButton";
import { BuildingEditorPanel } from "mods/BuildingEditorPanel";

const register: ModRegistrar = moduleRegistry => {
  moduleRegistry.extend(
    "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx",
    "selectedInfoSectionComponents",
    SIPSystem,
  );

  moduleRegistry.append("UniversalModMenu", ABCPanelButton);
  moduleRegistry.append("Game", BuildingEditorPanel);
};

export default register;
