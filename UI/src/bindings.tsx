import engine from "cohtml/cohtml";
import { bindLocalValue, bindValue, trigger } from "cs2/api";
import { Entity, LocElement, Number2, ToggleField, UISound } from "cs2/bindings";
import { StyleProps } from "cs2/input";
import { getModule } from "cs2/modding";
import { ButtonProps, FocusKey, ScrollController } from "cs2/ui";
import mod from "mod.json";
import { CSSProperties, ReactElement } from "react";
import { UpdateValueType } from "types/UpdateValueType";

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

// export const ChangeLevelDistrict = (level: number) => {
//   trigger(mod.id, "ChangeLevelDistrict", level);
// };

// export const ChangeMaxWorkplace = (workplace: number) => {
//   trigger(mod.id, "ChangeMaxWorkplace", workplace);
// };

// export const ResetMaxWorkplace = () => {
//   trigger(mod.id, "ResetMaxWorkplace");
// };

export const ChangeValueString = (
  value: string,
  valueType: UpdateValueType,
) => {
  console.log(`TSX: Triggering string ChangeValue(${value},${valueType})`);
  trigger(mod.id, "ChangeComponentValue", value, valueType);
};

export const ChangeValue = (value: number, valueType: UpdateValueType) => {
  console.log(`TSX: Triggering number ChangeValue(${value},${valueType})`);
  trigger(mod.id, "ChangeComponentValue", `${value}`, valueType);
};

export const ResetValue = (valueType: UpdateValueType) => {
  trigger(mod.id, "ResetComponentValue", valueType);
};

export const ClosePanel = () => {
  brandPanelVisibleBinding.update(false);
  componentPanelVisibleBinding.update(false);
  storagePanelVisibleBinding.update(false);
  engine.trigger("audio.playSound", "select-item", 1);
};

export const brandPanelVisibleBinding = bindLocalValue(false);
export const componentPanelVisibleBinding = bindLocalValue(false);
export const storagePanelVisibleBinding = bindLocalValue(false);

export const visibleBindings = [
  brandPanelVisibleBinding,
  componentPanelVisibleBinding,
  storagePanelVisibleBinding,
];

export enum PanelIndex {
  Brand = 0,
  Component = 1,
  Storage = 2,
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

export const Divider: any = getModule(
  "game-ui/editor/widgets/divider/divider.tsx",
  "Divider",
);

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

export const BooleanInput = getModule(
  "game-ui/game/widgets/toggle-field/toggle-field.tsx",
  "BoundToggleField",
) as React.FC<ToggleField>;

interface ToggleProps extends StyleProps {
  focusKey?: FocusKey;
  debugName?: string;
  checked?: boolean;
  disabled?: boolean;
  toggleSound?: UISound | string | null;
  children?: any;
  showHint?: boolean;
  multistate?: boolean;
  onChange?: () => void;
  onMultistateChange?: () => void;
  onMouseOver?: () => void;
  onMouseLeave?: () => void;
}

interface CheckBoxProps extends ToggleProps {
  theme?: any;
}

export const CheckBox = getModule(
  "game-ui/common/input/toggle/checkbox/checkbox.tsx",
  "Checkbox",
) as React.FC<CheckBoxProps>;

export const IntSliderField = getModule(
  "game-ui/editor/widgets/fields/number-slider-field.tsx",
  "IntSliderField",
);

export const IntSlider = getModule(
  "game-ui/common/input/slider/slider.tsx",
  "Slider",
);

export const IntTransformer = getModule(
  "game-ui/common/input/slider/slider.tsx",
  "intTransformer",
);

export const DropdownMultiSection = getModule(
  "game-ui/common/input/dropdown/dropdown.tsx",
  "Dropdown",
);
