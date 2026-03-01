import {
    brandPanelVisibleBinding, componentPanelVisibleBinding, MakeSP, PanelIndex, RandomizeStyle,
    resetPanelVisibleBinding, selectedEntity, togglePanel, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { FindTranslation } from "functions/lang";
import mod from "mod.json";
import { abcIcons, uilStandard } from "styleBindings";
import { BldgCleanupInfo } from "types/BldgCleanupInfo";
import { BldgComponentInfo } from "types/BldgComponentInfo";
import { BldgModifiedInfo } from "types/BldgModifiedInfo";
import { BldgBrandInfo } from "types/BrandDataInfo";

import { BrandPanel } from "./BrandPanel";
import { CleanupPanel } from "./CleanupPanel";
// import { StoragePanel } from "./StoragePanel";
import { ComponentPanel } from "./ComponentPanel";

interface SIP_ABC extends SelectedInfoSectionBase {
  hasSP: boolean;
  hasMesh: boolean;
  bldgBrandInfo: BldgBrandInfo;
  bldgComponentInfo: BldgComponentInfo;
  bldgModifiedInfo: BldgModifiedInfo[];
  bldgCleanupInfo: BldgCleanupInfo;
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
    const bldgCleanupInfo = props.bldgCleanupInfo;

    const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
    const isComponentPanelOpen = useValue(componentPanelVisibleBinding);
    const isCleanupPanelOpen = useValue(resetPanelVisibleBinding);

    const selectedEntityVal = useValue(selectedEntity);

    const modNameText = mod.name;

    const tooltipRandomizer = FindTranslation("Randomize.Tooltip");
    const tooltipBrandChanger = FindTranslation("Brand.Header");
    const tooltipComponentOverrider = FindTranslation("Component.Header");
    const tooltipSPSuffix = hasSP
      ? hasMesh
        ? ""
        : ` (${FindTranslation("SPBuilder.Tooltip.DisabledNoMesh")})`
      : ` (${FindTranslation("SPBuilder.Tooltip.DisabledNoSP")})`;
    const tooltipSPBuilder = `${FindTranslation("SPBuilder.Tooltip")}${tooltipSPSuffix}`;
    const tooltipCleanup = FindTranslation("Cleanup.Header");

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
                  {
                    <ToolButton
                      id="starq-abc-sip-sp"
                      focusKey={FOCUS_AUTO}
                      tooltip={tooltipSPBuilder}
                      disabled={!hasSP || !hasMesh}
                      src={abcIcons + "SP_Builder.svg"}
                      onSelect={MakeSP}
                    />
                  }
                  <ToolButton
                    id="starq-abc-sip-randomize"
                    focusKey={FOCUS_AUTO}
                    tooltip={tooltipRandomizer}
                    selected={false}
                    src={uilStandard + "Dice.svg"}
                    onSelect={RandomizeStyle}
                  />
                  {bldgBrandInfo.HasBrand && (
                    <>
                      <ToolButton
                        id="starq-abc-sip-brand"
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
                  <ToolButton
                    id="starq-abc-sip-component"
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

                  {bldgCleanupInfo.Enabled && (
                    <>
                      <ToolButton
                        id="starq-abc-sip-cleanup"
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipCleanup}
                        selected={isCleanupPanelOpen}
                        src={abcIcons + "Cleanup.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Cleanup);
                        }}
                      />
                      {isCleanupPanelOpen && (
                        <CleanupPanel
                          key={selectedEntityVal.index}
                          bldgCleanupInfo={bldgCleanupInfo}
                        />
                      )}
                    </>
                  )}
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
