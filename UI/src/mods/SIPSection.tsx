import {
    brandPanelVisibleBinding, componentPanelVisibleBinding, MakeSP, PanelIndex, RandomizeStyle,
    selectedEntity, togglePanel, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { FindTranslation } from "functions/lang";
import mod from "mod.json";
import { abcIcons, uilStandard } from "styleBindings";
import { BldgBrandInfo } from "types";
import { BldgComponentInfo } from "types/BldgComponentInfo";
import { BldgModifiedInfo } from "types/BldgModifiedInfo";

import { BrandPanel } from "./BrandPanel";
// import { StoragePanel } from "./StoragePanel";
import { ComponentPanel } from "./ComponentPanel";

interface SIP_ABC extends SelectedInfoSectionBase {
  hasSP: boolean;
  hasMesh: boolean;
  bldgBrandInfo: BldgBrandInfo;
  bldgComponentInfo: BldgComponentInfo;
  bldgModifiedInfo: BldgModifiedInfo[];
}

export const SIP_ABC = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIP_ABC"] = (
    props: SIP_ABC,
  ) => {
    const hasSP = props.hasSP;
    const hasMesh = props.hasMesh;
    const bldgBrandInfo = props.bldgBrandInfo;
    const bldgComponentInfo = props.bldgComponentInfo;
    const bldgModifiedInfo = props.bldgModifiedInfo;

    const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
    const isComponentPanelOpen = useValue(componentPanelVisibleBinding);

    const selectedEntityVal = useValue(selectedEntity);

    const modNameText = mod.name;

    const tooltipRandomizer = FindTranslation("Randomize.Tooltip");
    const tooltipBrandChanger = FindTranslation("Brand.Tooltip");
    const tooltipComponentOverrider = FindTranslation("Component.Header");
    const tooltipSPBuilder = FindTranslation("SPBuilder.Tooltip");

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
                    tooltip={tooltipRandomizer}
                    selected={false}
                    src={uilStandard + "Dice.svg"}
                    onSelect={RandomizeStyle}
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
                        tooltip={tooltipComponentOverrider}
                        selected={isComponentPanelOpen}
                        src={uilStandard + "Tools.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Component);
                        }}
                      />
                      {isComponentPanelOpen && (
                        <ComponentPanel
                          key={selectedEntityVal.index}
                          bldgComponentInfo={bldgComponentInfo}
                          bldgModifiedInfo={bldgModifiedInfo}
                        />
                      )}
                    </>
                  }
                  {
                    <ToolButton
                      id="starq-abc-sp"
                      focusKey={FOCUS_AUTO}
                      tooltip={tooltipSPBuilder}
                      // selected={isComponentPanelOpen}
                      disabled={!hasSP || !hasMesh}
                      src={abcIcons + "SP_Builder.svg"}
                      onSelect={MakeSP}
                    />
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
