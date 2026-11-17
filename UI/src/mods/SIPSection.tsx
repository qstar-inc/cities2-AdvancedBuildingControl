import { MakeSP, RandomizeStyle, selectedEntity } from "bindings/backend";
import {
  brandPanelVisibleBinding,
  cleanupPanelVisibleBinding,
  componentPanelVisibleBinding,
  effectsPanelVisibleBinding,
  PanelIndex,
  togglePanel,
} from "bindings/frontend";
import { cleanupIcon, effectIcon, spIcon } from "bindings/icons";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import * as l10n from "cs2/l10n";
import { PanelSection, PanelSectionRow, Tooltip } from "cs2/ui";
import mod from "mod.json";
import { EffectsPanel } from "mods/EffectsPanel";
import { diceIcon } from "shared/icons";
import { toolsIcon } from "shared/icons_uil";
import { FindTranslation, initTranslationService } from "shared/lang";
import { ToolButton } from "shared/vanilla";
import { BldgCleanupInfo } from "types/BldgCleanupInfo";
import { BldgComponentInfo } from "types/BldgComponentInfo";
import { BldgEffectInfo } from "types/BldgEffectsInfo";
import { BldgModifiedInfo } from "types/BldgModifiedInfo";
import { BldgBrandInfo } from "types/BrandDataInfo";

import { BrandPanel } from "./BrandPanel";
import { CleanupPanel } from "./CleanupPanel";
// import { StoragePanel } from "./StoragePanel";
import { ComponentPanel } from "./ComponentPanel";

interface SIPSystemProps extends SelectedInfoSectionBase {
  hasSP: boolean;
  hasMesh: boolean;
  bldgBrandInfo: BldgBrandInfo;
  bldgComponentInfo: BldgComponentInfo;
  bldgModifiedInfo: BldgModifiedInfo[];
  bldgCleanupInfo: BldgCleanupInfo;
  bldgEffectInfo: BldgEffectInfo;
}

export const SIPSystem = (componentList: any): any => {
  componentList[`${mod.id}.Systems.SIPSystem`] = (props: SIPSystemProps) => {
    const { translate } = l10n.useLocalization();
    initTranslationService(translate);

    const hasSP = props.hasSP;
    const hasMesh = props.hasMesh;

    const bldgBrandInfo = props.bldgBrandInfo;
    const bldgComponentInfo = props.bldgComponentInfo;
    const bldgModifiedInfo = props.bldgModifiedInfo;
    const bldgCleanupInfo = props.bldgCleanupInfo;
    const bldgEffectInfo = props.bldgEffectInfo;

    const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
    const isComponentPanelOpen = useValue(componentPanelVisibleBinding);
    const isCleanupPanelOpen = useValue(cleanupPanelVisibleBinding);
    const isEffectsPanelOpen = useValue(effectsPanelVisibleBinding);

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
    const tooltipEffects = FindTranslation("Effects.Header");

    const baseCount =
      3 + (bldgBrandInfo.HasBrand ? 1 : 0) + (bldgCleanupInfo.Enabled ? 1 : 0);

    const canMakeSP = hasSP && hasMesh;
    const showSPButton = canMakeSP || baseCount != 4;

    const itemCounts = baseCount + (showSPButton ? 1 : 0);
    const shortenName = itemCounts > 4;

    return (
      <>
        <PanelSection>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={
              shortenName ? (
                <Tooltip tooltip={modNameText}>
                  <span>{"ABC"}</span>
                </Tooltip>
              ) : (
                modNameText
              )
            }
            right={
              <>
                <FocusDisabled>
                  {showSPButton && (
                    <ToolButton
                      id="starq-abc-sip-sp"
                      focusKey={FOCUS_AUTO}
                      tooltip={tooltipSPBuilder}
                      disabled={!hasSP || !hasMesh}
                      src={spIcon}
                      onSelect={MakeSP}
                    />
                  )}
                  <ToolButton
                    id="starq-abc-sip-randomize"
                    focusKey={FOCUS_AUTO}
                    tooltip={tooltipRandomizer}
                    selected={false}
                    src={diceIcon}
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
                    src={toolsIcon}
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
                  <ToolButton
                    id="starq-abc-sip-effects"
                    focusKey={FOCUS_AUTO}
                    tooltip={tooltipEffects}
                    selected={isEffectsPanelOpen}
                    src={effectIcon}
                    onSelect={() => {
                      togglePanel(PanelIndex.Effects);
                    }}
                  />
                  {isEffectsPanelOpen && (
                    <EffectsPanel
                      key={selectedEntityVal.index}
                      bldgEffectInfo={bldgEffectInfo}
                    />
                  )}
                  {bldgCleanupInfo.Enabled && (
                    <>
                      <ToolButton
                        id="starq-abc-sip-cleanup"
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipCleanup}
                        selected={isCleanupPanelOpen}
                        src={cleanupIcon}
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
