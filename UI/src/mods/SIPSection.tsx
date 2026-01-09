import {
  brandPanelVisibleBinding,
  levelPanelVisibleBinding,
  PanelIndex,
  RandomizeStyle,
  selectedEntity,
  storagePanelVisibleBinding,
  togglePanel,
  ToolButton,
  utilityPanelVisibleBinding,
  vehiclePanelVisibleBinding,
} from "bindings";
import { useValue } from "cs2/api";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_DISABLED, PanelSection, PanelSectionRow } from "cs2/ui";
import { baseGameIcons, uilStandard } from "styleBindings";
import {
  BldgBrandInfo,
  BldgGeneralInfo,
  BldgPropertiesInfo,
  BldgStorageInfo,
  BldgUtilityInfo,
  BldgVehicleInfo,
  LocaleKeys,
} from "types";

import { BrandPanel } from "./BrandPanel";
import { PropertiesPanel } from "./PropertiesPanel";
import { StoragePanel } from "./StoragePanel";
import { UtilityPanel } from "./UtilityPanel";
import { VehiclePanel } from "./VehiclePanel";

interface SIP_ABC extends SelectedInfoSectionBase {
  bldgGeneralInfo: BldgGeneralInfo;
  bldgPropertiesInfo: BldgPropertiesInfo;
  bldgBrandInfo: BldgBrandInfo;
  bldgStorageInfo: BldgStorageInfo;
  bldgUtilityInfo: BldgUtilityInfo;
  bldgVehicleInfo: BldgVehicleInfo;
}

export const SIP_ABC = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIP_ABC"] = (
    props: SIP_ABC,
  ) => {
    const { translate } = useLocalization();

    // console.log(JSON.stringify(props));

    const bldgGeneralInfo = props.bldgGeneralInfo;
    const bldgPropertiesInfo = props.bldgPropertiesInfo;
    const bldgBrandInfo = props.bldgBrandInfo;
    const bldgStorageInfo = props.bldgStorageInfo;
    const bldgUtilityInfo = props.bldgUtilityInfo;
    const bldgVehicleInfo = props.bldgVehicleInfo;

    console.log(JSON.stringify(bldgGeneralInfo));

    const isBrandPanelOpen = useValue(brandPanelVisibleBinding);
    const isLevelPanelOpen = useValue(levelPanelVisibleBinding);
    const isStoragePanelOpen = useValue(storagePanelVisibleBinding);
    const isUtilityPanelOpen = useValue(utilityPanelVisibleBinding);
    const isVehiclePanelOpen = useValue(vehiclePanelVisibleBinding);

    const selectedEntityVal = useValue(selectedEntity);

    const modNameText = translate(LocaleKeys.NAME) ?? "NAME";

    const tooltipRandomizeButton =
      translate(LocaleKeys.RANDOMIZE_TOOLTIP) ?? "RANDOMIZE_TOOLTIP";
    const tooltipBrandChanger =
      translate(LocaleKeys.BRAND_TOOLTIP) ?? "BRAND_TOOLTIP";
    const tooltipStorageChanger =
      translate(LocaleKeys.STORAGE_TOOLTIP) ?? "STORAGE_TOOLTIP";
    const tooltipUtilityChanger =
      translate(LocaleKeys.UTILITY_TOOLTIP) ?? "UTILITY_TOOLTIP";

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
                  {(bldgPropertiesInfo.HasLevel ||
                    bldgPropertiesInfo.HasHousehold ||
                    bldgPropertiesInfo.IsWorkplace) && (
                    <>
                      <ToolButton
                        id="starq-abc-level"
                        focusKey={FOCUS_AUTO}
                        selected={isLevelPanelOpen}
                        src={baseGameIcons + "Upkeep.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Level);
                        }}
                      />
                      {isLevelPanelOpen && (
                        <PropertiesPanel
                          key={selectedEntityVal.index}
                          bldgPropertiesInfo={bldgPropertiesInfo}
                        />
                      )}
                    </>
                  )}
                  {bldgStorageInfo.HasStorage && (
                    <>
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
                      {isStoragePanelOpen && (
                        <StoragePanel
                          key={selectedEntityVal.index}
                          bldgStorageInfo={bldgStorageInfo}
                        />
                      )}
                    </>
                  )}
                  {(bldgUtilityInfo.IsWaterPump ||
                    bldgUtilityInfo.IsSewageOutlet ||
                    bldgUtilityInfo.IsPowerPlant) && (
                    <>
                      <ToolButton
                        id="starq-abc-utility"
                        focusKey={FOCUS_AUTO}
                        // tooltip={tooltipServiceChanger}
                        selected={isUtilityPanelOpen}
                        src={uilStandard + "ServiceBuilding.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Utility);
                        }}
                      />
                      {isUtilityPanelOpen && (
                        <UtilityPanel
                          key={selectedEntityVal.index}
                          bldgUtilityInfo={bldgUtilityInfo}
                          bldgGeneralInfo={bldgGeneralInfo}
                        />
                      )}
                    </>
                  )}
                  {(bldgVehicleInfo.IsDepot ||
                    bldgVehicleInfo.IsGarbageFacility ||
                    bldgVehicleInfo.IsHospital ||
                    bldgVehicleInfo.IsDeathcare ||
                    bldgVehicleInfo.IsPoliceStation ||
                    bldgVehicleInfo.IsPrison ||
                    bldgVehicleInfo.IsFireStation ||
                    bldgVehicleInfo.IsEmergencyShelter ||
                    bldgVehicleInfo.IsPostFacility ||
                    bldgVehicleInfo.IsMaintenanceDepot) && (
                    <>
                      <ToolButton
                        id="starq-abc-vehicle"
                        focusKey={FOCUS_AUTO}
                        // tooltip={tool}
                        selected={isVehiclePanelOpen}
                        src={uilStandard + "DeliveryVan.svg"}
                        onSelect={() => {
                          togglePanel(PanelIndex.Vehicle);
                        }}
                      />
                      {isVehiclePanelOpen && (
                        <VehiclePanel
                          key={selectedEntityVal.index}
                          bldgVehicleInfo={bldgVehicleInfo}
                          bldgGeneralInfo={bldgGeneralInfo}
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
