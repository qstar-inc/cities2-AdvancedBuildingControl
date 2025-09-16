import {
    brandPanelVisibleBinding, levelPanelVisibleBinding, PanelIndex, RandomizeStyle, selectedEntity,
    storagePanelVisibleBinding, togglePanel, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { uilStandard } from "styleBindings";
import { BrandDataInfo, LocaleKeys, ResourceDataInfo, ZoneDataInfo } from "types";

import { BrandPanel } from "./BrandPanel";
import styles from "./BrandPanel.module.scss";
import { LevelPanel } from "./LevelPanel";
import { StoragePanel } from "./StoragePanel";

interface SIPAdvancedBuildingControl extends SelectedInfoSectionBase {
  h_brand: boolean;
  w_brand: string;
  w_brandicon: string;
  w_brandlist: BrandDataInfo[];
  w_company: string;

  h_level: boolean;
  w_level: number;
  w_upkeep: number;
  w_zone: string;
  w_zonelist: ZoneDataInfo[];
  w_variant: string;

  h_storage: boolean;
  w_resources: ResourceDataInfo[];
  w_resourceslist: ResourceDataInfo[];
}

export const AdvancedBuildingControlSIP = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIPAdvancedBuildingControl"] =
    (e: SIPAdvancedBuildingControl) => {
      const { translate } = useLocalization();

      const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
      const isLevelPanelOpen = useValue(levelPanelVisibleBinding);
      const isStoragePanelOpen = useValue(storagePanelVisibleBinding);

      const selectedEntityVal = useValue(selectedEntity);

      const modNameText = translate(LocaleKeys.NAME) ?? "NAME";

      const tooltipRandomizeButton =
        translate(LocaleKeys.RANDOMIZE_TOOLTIP) ?? "RANDOMIZE_TOOLTIP";
      const tooltipBrandChanger =
        translate(LocaleKeys.BRAND_CHANGER_TOOLTIP) ?? "BRAND_CHANGER_TOOLTIP";
      const tooltipStorageChanger =
        translate(LocaleKeys.STORAGE_CHANGER_TOOLTIP) ??
        "STORAGE_CHANGER_TOOLTIP";

      // console.log(e.w_resourceslist);

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
                      // className={styles.ToolWhite}
                      src={uilStandard + "Dice.svg"}
                      onSelect={() => {
                        RandomizeStyle();
                      }}
                    />
                    {e.h_brand && (
                      <ToolButton
                        id="starq-abc-brand"
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipBrandChanger}
                        selected={isBrandPanelOpen}
                        src={e.w_brandicon}
                        onSelect={() => {
                          togglePanel(PanelIndex.Brand);
                        }}
                      />
                    )}
                    {/* {e.h_level && (
                      <ToolButton
                        id="starq-abc-level"
                        focusKey={FOCUS_AUTO}
                        selected={isLevelPanelOpen}
                        src={uilStandard + "ArrowUp.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Level);
                        }}
                      />
                    )} */}
                    {e.h_storage && (
                      <ToolButton
                        id="starq-abc-storage"
                        focusKey={FOCUS_AUTO}
                        tooltip={tooltipStorageChanger}
                        selected={isStoragePanelOpen}
                        src={uilStandard + "DeliveryVan.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Storage);
                        }}
                      />
                    )}
                  </FocusDisabled>
                </>
              }
            />
          </PanelSection>
          <BrandPanel
            key={selectedEntityVal.index}
            h_brand={e.h_brand}
            w_brand={e.w_brand}
            w_brandlist={e.w_brandlist}
            w_company={e.w_company}
            w_entity={selectedEntityVal}
          />
          <LevelPanel
            key={selectedEntityVal.index}
            h_level={e.h_level}
            w_level={e.w_level}
            w_upkeep={e.w_upkeep}
            w_zone={e.w_zone}
            w_zonelist={e.w_zonelist}
            w_variant={e.w_variant}
          />
          <StoragePanel
            key={selectedEntityVal.index}
            h_storage={e.h_storage}
            w_resources={e.w_resources}
            w_resourceslist={e.w_resourceslist}
            w_entity={selectedEntityVal}
          />
        </>
      );
    };
  return componentList as any;
};
