import { getModule } from "cs2/modding";

export const stylePanel = getModule(
  "game-ui/common/panel/panel.module.scss",
  "classes",
);

export const styleDefault = getModule(
  "game-ui/common/panel/themes/default.module.scss",
  "classes",
);

export const styleSIPTheme = getModule(
  "game-ui/game/themes/selected-info-panel.module.scss",
  "classes",
);

export const styleSIP = getModule(
  "game-ui/game/components/selected-info-panel/selected-info-panel.module.scss",
  "classes",
);

export const styleScrollable = getModule(
  "game-ui/common/scrolling/scrollable.module.scss",
  "classes",
);

export const styleIcon = getModule(
  "game-ui/common/input/button/icon-button.module.scss",
  "classes",
);

export const styleTintedIcon = getModule(
  "game-ui/common/image/tinted-icon.module.scss",
  "classes",
);

export const styleCloseButton = getModule(
  "game-ui/common/input/button/themes/round-highlight-button.module.scss",
  "classes",
);

export const styleLevelSection = getModule(
  "game-ui/game/components/selected-info-panel/selected-info-sections/building-sections/level-section/level-section.module.scss",
  "classes",
);

export const styleLevelProgress = getModule(
  "game-ui/game/components/selected-info-panel/selected-info-sections/building-sections/level-section/level-progress-bar.module.scss",
  "classes",
);

export const styleProgress = getModule(
  "game-ui/common/progress-bar/progress-bar.module.scss",
  "classes",
);

export const infoRowModule = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.module.scss",
  "classes",
);

export const infoSectionModule = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.module.scss",
  "classes",
);

export const resourceBox = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/resource-item/resource-item.module.scss",
  "classes",
);

export const storageBox = getModule(
  "game-ui/game/components/selected-info-panel/selected-info-sections/building-sections/storage-section/storage-section.module.scss",
  "classes",
);

export const textElipsisInputThemeModule = getModule(
  "game-ui/common/input/text/ellipsis-text-input/themes/default.module.scss",
  "classes",
);

export const textElipsisInputModule = getModule(
  "game-ui/common/input/text/ellipsis-text-input/ellipsis-text-input.module.scss",
  "classes",
);

export const sipTextInputModule = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/text-input/text-input.module.scss",
  "classes",
);

export const toggleFieldModule = getModule(
  "game-ui/game/widgets/toggle-field/toggle-field.module.scss",
  "classes",
);

export const editorSliderModule = getModule(
  "game-ui/editor/themes/editor-slider.module.scss",
  "classes",
);

export const dropdownModule = getModule(
  "game-ui/game/themes/game-dropdown.module.scss",
  "classes",
);

export const wrapperClass = `${stylePanel.panel} ${styleSIP.selectedInfoPanel}`;
export const closeButtonClass = `${styleCloseButton.button} ${stylePanel.closeButton}`;
export const closeButtonImageClass = `${styleTintedIcon.tintedIcon} ${styleIcon.icon}`;

// UIL
export const uilStandard = "coui://uil/Standard/";
export const uilColored = "coui://uil/Colored/";
export const baseGameIcons = "Media/Game/Icons/";
