import {
  brandPanelVisibleBinding,
  levelPanelVisibleBinding,
  PanelIndex,
  RandomizeStyle,
  selectedEntity,
  storagePanelVisibleBinding,
  togglePanel,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { uilStandard } from "styleBindings";
import {
  BldgBrandInfo,
  BldgStorageInfo,
  BldgZoningInfo,
  BrandDataInfo,
  LocaleKeys,
  ResourceDataInfo,
  ZoneDataInfo,
} from "types";

import { BrandPanel } from "./BrandPanel";
import { LevelPanel } from "./LevelPanel";
import { StoragePanel } from "./StoragePanel";

interface SIPAdvancedBuildingControl extends SelectedInfoSectionBase {
  // h_brand: boolean;
  // w_brand: string;
  // w_brandicon: string;
  // w_brandlist: BrandDataInfo[];
  // w_company: string;

  bldgZoningInfo: BldgZoningInfo;
  bldgBrandInfo: BldgBrandInfo;
  bldgStorageInfo: BldgStorageInfo;

  // h_level: boolean;
  // w_level: number;
  // w_upkeep: number;
  // h_household: boolean;
  // w_household: number;
  // w_maxhousehold: string;
  // w_rent: number;
  // w_areatype: string;
  // w_spacemult: number;

  // w_zonetypebase: number;
  // w_landvaluemodifier: number;
  // w_ignorelandvalue: boolean;
  // w_lotsize: number;
  // w_landvaluebase: number;
  // w_totalrent: number;
  // w_propertiescount: number;
  // w_mixedpercent: number;
  // w_ismixed: boolean;

  // w_zone: string;
  // w_zonelist: ZoneDataInfo[];

  // h_storage: boolean;
  // w_resources: ResourceDataInfo[];
  // w_resourceslist: ResourceDataInfo[];
}

export const AdvancedBuildingControlSIP = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIP_ABC"] = (
    e: SIPAdvancedBuildingControl
  ) => {
    const { translate } = useLocalization();

    const bldgZoningInfo = e.bldgZoningInfo;
    const bldgBrandInfo = e.bldgBrandInfo;
    const bldgStorageInfo = e.bldgStorageInfo;

    console.log(bldgStorageInfo);

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
                  {bldgBrandInfo.HasBrand && (
                    <ToolButton
                      id="starq-abc-brand"
                      focusKey={FOCUS_AUTO}
                      tooltip={tooltipBrandChanger}
                      selected={isBrandPanelOpen}
                      src={e.bldgBrandInfo.BrandIcon}
                      onSelect={() => {
                        togglePanel(PanelIndex.Brand);
                      }}
                    />
                  )}
                  {bldgZoningInfo.HasLevel && (
                    <ToolButton
                      id="starq-abc-level"
                      focusKey={FOCUS_AUTO}
                      selected={isLevelPanelOpen}
                      src={uilStandard + "ArrowUp.svg"}
                      onSelect={() => {
                        togglePanel(PanelIndex.Level);
                      }}
                    />
                  )}
                  {bldgStorageInfo.HasStorage && (
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
          bldgBrandInfo={bldgBrandInfo}
          // h_brand={e.h_brand}
          // w_brand={e.w_brand}
          // w_brandlist={e.w_brandlist}
          // w_company={e.w_company}
          // w_entity={selectedEntityVal}
        />
        <LevelPanel
          key={selectedEntityVal.index}
          bldgZoningInfo={bldgZoningInfo}
          // h_level={e.h_level}
          // w_level={e.w_level}
          // w_upkeep={e.w_upkeep}
          // h_household={e.h_household}
          // w_household={e.w_household}
          // w_maxhousehold={e.w_maxhousehold}
          // w_rent={e.w_rent}
          // w_areaType={e.w_areatype}
          // w_spacemult={e.w_spacemult}
          // w_zonetypebase={e.w_zonetypebase}
          // w_landvaluemodifier={e.w_landvaluemodifier}
          // w_ignorelandvalue={e.w_ignorelandvalue}
          // w_lotsize={e.w_lotsize}
          // w_landvaluebase={e.w_landvaluebase}
          // w_totalrent={e.w_totalrent}
          // w_propertiescount={e.w_propertiescount}
          // w_mixedpercent={e.w_mixedpercent}
          // w_ismixed={e.w_ismixed}
          // w_zone={e.w_zone}
          // w_zonelist={e.w_zonelist}
        />
        <StoragePanel
          key={selectedEntityVal.index}
          bldgStorageInfo={bldgStorageInfo}
          // h_storage={e.h_storage}
          // w_resources={e.w_resources}
          // w_resourceslist={e.w_resourceslist}
          // w_entity={selectedEntityVal}
        />
      </>
    );
  };
  return componentList as any;
};
