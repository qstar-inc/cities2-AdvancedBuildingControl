// SUMPanelButton.tsx (mod-specific)
import {
  PanelIndex,
  togglePanel,
  buildingEditorVisibleBinding,
} from "bindings/frontend";
import { useValue } from "cs2/api";
import * as l10n from "cs2/l10n";
import { toolsIcon } from "shared/icons_uil";
import { initTranslationService } from "shared/lang";
import { UMMButton } from "shared/UMMButton";

export const ABCPanelButton = () => {
  const { translate } = l10n.useLocalization();
  initTranslationService(translate);

  const visibleBinding = useValue(buildingEditorVisibleBinding);

  return (
    <UMMButton
      id="starq-abc-button"
      selected={visibleBinding}
      onToggle={() => togglePanel(PanelIndex.BuildingEditor)}
      icon={toolsIcon}
      tooltip="Open Advanced Building Editor"
    />
  );
};
