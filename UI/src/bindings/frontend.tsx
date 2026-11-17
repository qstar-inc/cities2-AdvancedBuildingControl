import { bindLocalValue } from "cs2/api";
import {
  playSound,
  UISound,
} from "shared/vanilla-modules/game-ui/common/data-binding/audio-bindings";

export const ClosePanel = () => {
  brandPanelVisibleBinding.update(false);
  componentPanelVisibleBinding.update(false);
  cleanupPanelVisibleBinding.update(false);
  effectsPanelVisibleBinding.update(false);
  // storagePanelVisibleBinding.update(false);
  playSound(UISound.closePanel, 1);
};

export const buildingEditorVisibleBinding = bindLocalValue(false);
export const brandPanelVisibleBinding = bindLocalValue(false);
export const componentPanelVisibleBinding = bindLocalValue(false);
export const cleanupPanelVisibleBinding = bindLocalValue(false);
export const effectsPanelVisibleBinding = bindLocalValue(false);
// export const storagePanelVisibleBinding = bindLocalValue(false);

export const visibleBindings = [
  buildingEditorVisibleBinding,
  brandPanelVisibleBinding,
  componentPanelVisibleBinding,
  cleanupPanelVisibleBinding,
  effectsPanelVisibleBinding,
  // storagePanelVisibleBinding,
];

export enum PanelIndex {
  BuildingEditor = 0,
  Brand = 1,
  Component = 2,
  Cleanup = 3,
  Effects = 4,
  // Storage = 5,
}

export const togglePanel = (indexToToggle: number) => {
  const currentlyOpen = visibleBindings[indexToToggle].value;

  visibleBindings.forEach((binding, i) => {
    binding.update(i === indexToToggle ? !currentlyOpen : false);
  });
};
