import engine from "cohtml/cohtml";
import { bindLocalValue, bindValue, trigger } from "cs2/api";
import { Entity } from "cs2/bindings";
import { getModule } from "cs2/modding";
import { ButtonProps, ScrollController } from "cs2/ui";
import mod from "mod.json";
import { ReactElement } from "react";
import { UpdateValueType } from "types";

import styles from "./mods/style.module.scss";

export const selectedEntity = bindValue<Entity>(
  "selectedInfo",
  "selectedEntity",
);

export const SetBrand = (replaceBrand: string) => {
  trigger(mod.id, "SetBrand", replaceBrand);
  engine.trigger("audio.playSound", "select-toggle", 1);
};

export const RandomizeStyle = () => {
  trigger(mod.id, "RandomizeStyle");
};

export const ChangeLevelDistrict = (level: number) => {
  trigger(mod.id, "ChangeLevelDistrict", level);
};

export const ChangeMaxWorkplace = (workplace: number) => {
  trigger(mod.id, "ChangeMaxWorkplace", workplace);
};

export const ResetMaxWorkplace = () => {
  trigger(mod.id, "ResetMaxWorkplace");
};

export const ChangeValueString = (cap: string, valueType: UpdateValueType) => {
  trigger(mod.id, "ChangeValue", cap, valueType);
};

export const ChangeValue = (cap: number, valueType: UpdateValueType) => {
  trigger(mod.id, "ChangeValue", `${cap}`, valueType);
};

export const ResetValue = (valueType: UpdateValueType) => {
  trigger(mod.id, "ResetValue", valueType);
};

export const ClosePanel = () => {
  brandPanelVisibleBinding.update(false);
  levelPanelVisibleBinding.update(false);
  storagePanelVisibleBinding.update(false);
  utilityPanelVisibleBinding.update(false);
  vehiclePanelVisibleBinding.update(false);
  engine.trigger("audio.playSound", "select-item", 1);
};

export const SplitTextToDiv = ({ text }: { text: string }) => {
  const lines = text.split("\r\n");

  if (lines.length === 1) {
    return <>{text}</>;
  }

  return (
    <>
      {lines.map((line, index) => (
        <div
          className={
            index !== lines.length - 1 ? styles.TooltipMarginBottom : undefined
          }
        >
          {line}
        </div>
      ))}
    </>
  );
};

export const brandPanelVisibleBinding = bindLocalValue(false);
export const levelPanelVisibleBinding = bindLocalValue(false);
export const storagePanelVisibleBinding = bindLocalValue(false);
export const utilityPanelVisibleBinding = bindLocalValue(false);
export const vehiclePanelVisibleBinding = bindLocalValue(false);

export const visibleBindings = [
  brandPanelVisibleBinding,
  levelPanelVisibleBinding,
  storagePanelVisibleBinding,
  utilityPanelVisibleBinding,
  vehiclePanelVisibleBinding,
];

export enum PanelIndex {
  Brand = 0,
  Level = 1,
  Storage = 2,
  Utility = 3,
  Vehicle = 4,
}

export const togglePanel = (indexToToggle: number) => {
  const currentlyOpen = visibleBindings[indexToToggle].value;

  visibleBindings.forEach((binding, i) => {
    binding.update(i === indexToToggle ? !currentlyOpen : false);
  });
};

interface InfoButtonProps extends ButtonProps {
  label: string;
  // tooltip?: string;
}

export const InfoButton = getModule(
  "game-ui/game/components/selected-info-panel/shared-components/info-button/info-button.tsx",
  "InfoButton",
) as React.FC<InfoButtonProps>;

// export const A = getModule(
//   "game-ui/common/localization/loc.generated.ts",
//   "Loc"
// );

// console.log(A.SelectedInfoPanel.RAW_MATERIALS);

interface ToolButtonProps extends ButtonProps {
  src: string;
  tooltip?: string;
}

export const ToolButton = getModule(
  "game-ui/game/components/tool-options/tool-button/tool-button.tsx",
  "ToolButton",
) as React.FC<ToolButtonProps>;

export type SizeProvider = {
  getRenderedRange: () => {
    offset: number;
    size: number;
    startIndex: number;
    endIndex: number;
  };
  getTotalSize: () => number;
};
export type RenderItemFn = (
  itemIndex: number,
  indexInRange: number,
) => ReactElement | null;
type RenderedRangeChangedCallback = (
  startIndex: number,
  endIndex: number,
) => void;

interface VirtualListProps {
  className?: string;
  controller?: ScrollController;
  direction?: "vertical" | "horizontal";
  onRenderedRangeChange?: RenderedRangeChangedCallback;
  renderItem: RenderItemFn;
  sizeProvider: SizeProvider;
  smooth?: boolean;
  style?: Partial<CSSStyleDeclaration>;
}

export const VanillaVirtualList = getModule(
  "game-ui/common/scrolling/virtual-list/virtual-list.tsx",
  "VirtualList",
) as React.FC<VirtualListProps>;

export const useUniformSizeProvider: (
  height: number,
  visible: number,
  extents: number,
) => SizeProvider = getModule(
  "game-ui/common/scrolling/virtual-list/virtual-list-size-provider.ts",
  "useUniformSizeProvider",
);
