import {
  brandPanelVisibleBinding,
  componentPanelVisibleBinding,
  PanelIndex,
  RandomizeStyle,
  selectedEntity,
  togglePanel,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { FindTranslation } from "functions/lang";
import mod from "mod.json";
import { uilStandard } from "styleBindings";
import { BldgBrandInfo } from "types";
import { BldgComponentInfo } from "types/BldgComponentInfo";
import { BldgModifiedInfo } from "types/BldgModifiedInfo";

import { BrandPanel } from "./BrandPanel";
// import { StoragePanel } from "./StoragePanel";
import { ComponentPanel } from "./ComponentPanel";

interface SIP_ABC extends SelectedInfoSectionBase {
  bldgBrandInfo: BldgBrandInfo;
  bldgComponentInfo: BldgComponentInfo;
  bldgModifiedInfo: BldgModifiedInfo[];
}

export const SIP_ABC = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIP_ABC"] = (
    props: SIP_ABC,
  ) => {
    const bldgBrandInfo = props.bldgBrandInfo;
    const bldgComponentInfo = props.bldgComponentInfo;
    const bldgModifiedInfo = props.bldgModifiedInfo;

    const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
    const isVehiclePanelOpen = useValue(componentPanelVisibleBinding);

    const selectedEntityVal = useValue(selectedEntity);

    const modNameText = mod.name;

    const tooltipRandomizeButton = FindTranslation("Randomize.Tooltip");
    const tooltipBrandChanger = FindTranslation("Brand.Tooltip");

    return (
      <>
        <PanelSection>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={modNameText}
            right={
              <>
                <FocusDisabled>
                  <ToolButton
                    id="starq-abc-dice"
                    focusKey={FOCUS_DISABLED}
                    tooltip={tooltipRandomizeButton}
                    selected={false}
                    src={uilStandard + "Dice.svg"}
                    onSelect={() => {
                      RandomizeStyle();
                    }}
                  />
                  {bldgBrandInfo.HasBrand && (
                    <>
                      <ToolButton
                        id="starq-abc-brand"
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipBrandChanger}
                        selected={isBrandPanelOpen}
                        src={props.bldgBrandInfo.BrandIcon}
                        onSelect={() => {
                          togglePanel(PanelIndex.Brand);
                        }}
                      />
                      {isBrandPanelOpen && (
                        <BrandPanel
                          key={selectedEntityVal.index}
                          bldgBrandInfo={bldgBrandInfo}
                        />
                      )}
                    </>
                  )}
                  {
                    <>
                      <ToolButton
                        id="starq-abc-component"
                        focusKey={FOCUS_AUTO}
                        // tooltip={tool}
                        selected={isVehiclePanelOpen}
                        src={uilStandard + "Tools.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Component);
                        }}
                      />
                      {isVehiclePanelOpen && (
                        <ComponentPanel
                          key={selectedEntityVal.index}
                          bldgComponentInfo={bldgComponentInfo}
                          bldgModifiedInfo={bldgModifiedInfo}
                        />
                      )}
                    </>
                  }
                </FocusDisabled>
              </>
            }
          />
        </PanelSection>
      </>
    );
  };
  return componentList as any;
};
