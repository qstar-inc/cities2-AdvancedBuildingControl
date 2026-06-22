import engine from "cohtml/cohtml";
import { bindLocalValue } from "cs2/api";

export const ClosePanel = () => {
  brandPanelVisibleBinding.update(false);
  componentPanelVisibleBinding.update(false);
  cleanupPanelVisibleBinding.update(false);
  // storagePanelVisibleBinding.update(false);
  engine.trigger("audio.playSound", "select-item", 1);
};

export const brandPanelVisibleBinding = bindLocalValue(false);
export const componentPanelVisibleBinding = bindLocalValue(false);
export const cleanupPanelVisibleBinding = bindLocalValue(false);
// export const storagePanelVisibleBinding = bindLocalValue(false);

export const visibleBindings = [
  brandPanelVisibleBinding,
  componentPanelVisibleBinding,
  cleanupPanelVisibleBinding,
  // storagePanelVisibleBinding,
];

export enum PanelIndex {
  Brand = 0,
  Component = 1,
  Cleanup = 2,
  // Storage = 3,
}

export const togglePanel = (indexToToggle: number) => {
  const currentlyOpen = visibleBindings[indexToToggle].value;

  visibleBindings.forEach((binding, i) => {
    binding.update(i === indexToToggle ? !currentlyOpen : false);
  });
};
