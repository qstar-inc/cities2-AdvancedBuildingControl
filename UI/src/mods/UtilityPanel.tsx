import {
  ChangePowerProdCapacity,
  ChangeSewageDumpCapacity,
  ChangeWaterPumpCapacity,
  ResetPowerProdCapacity,
  ResetSewageDumpCapacity,
  ResetWaterPumpCapacity,
  ToolButton,
  utilityPanelVisibleBinding,
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_DISABLED } from "cs2/input";
import { LocalizedNumber, Unit, useLocalization } from "cs2/l10n";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";
import {
  infoRowModule,
  sipTextInputModule,
  textElipsisInputModule,
  textElipsisInputThemeModule,
  uilStandard,
} from "styleBindings";
import { BldgGeneralInfo, BldgUtilityInfo, LocaleKeys } from "types";

import { PanelBase } from "./PanelBase";

interface UtilityPanelProps {
  bldgGeneralInfo: BldgGeneralInfo;
  bldgUtilityInfo: BldgUtilityInfo;
}

export const UtilityPanel: FC<UtilityPanelProps> = (
  props: UtilityPanelProps
) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(utilityPanelVisibleBinding);

  let bldgGeneralInfo = props.bldgGeneralInfo;
  let bldgUtilityInfo = props.bldgUtilityInfo;

  const efficiencyLabel = translate("SelectedInfoPanel.EFFICIENCY");
  const headerText = translate(LocaleKeys.UTILITIES_HEADER);
  const infoText = translate(LocaleKeys.UTILITIES_INFORMATION);

  const changePumpCapLabel = translate(LocaleKeys.UTILITIES_CHANGE_PUMP_CAP);
  const originalPumpCapLabel = translate(LocaleKeys.UTILITIES_OG_PUMP_CAP);
  const resetPumpCapTooltip = translate(LocaleKeys.UTILITIES_RESET_PUMP_CAP);
  const actualPumpCapLabel = translate(LocaleKeys.UTILITIES_ACTUAL_PUMP_CAP);

  const changeDumpCapLabel = translate(LocaleKeys.UTILITIES_CHANGE_DUMP_CAP);
  const originalDumpCapLabel = translate(LocaleKeys.UTILITIES_OG_DUMP_CAP);
  const resetDumpCapTooltip = translate(LocaleKeys.UTILITIES_RESET_DUMP_CAP);
  const actualDumpCapLabel = translate(LocaleKeys.UTILITIES_ACTUAL_DUMP_CAP);

  const changePowerCapLabel = translate(LocaleKeys.UTILITIES_CHANGE_POWER_CAP);
  const originalPowerCapLabel = translate(LocaleKeys.UTILITIES_OG_POWER_CAP);
  const resetPowerCapTooltip = translate(LocaleKeys.UTILITIES_RESET_POWER_CAP);
  const actualPowerCapLabel = translate(LocaleKeys.UTILITIES_ACTUAL_POWER_CAP);

  const powerHeader = translate(LocaleKeys.UTILITIES_PowerProduction);
  const changeLabel = translate(LocaleKeys.UTILITIES_Change);
  const originalLabel = translate(LocaleKeys.UTILITIES_Original);
  const boostedLabel = translate(LocaleKeys.UTILITIES_Boosted);

  const visible = useMemo(() => visibleBindingValue, [visibleBindingValue]);

  return (
    <>
      <PanelBase
        header={headerText!}
        visible={visible}
        height={100}
        content={
          <>
            <PanelSection>
              <PanelSectionRow
                uppercase={false}
                disableFocus={true}
                left={infoText}
              />
            </PanelSection>
            {bldgUtilityInfo.IsWaterPump ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changePumpCapLabel}
                  right={
                    <>
                      <div>
                        <div className={textElipsisInputThemeModule.wrapper}>
                          {" "}
                          <div
                            className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
                          >
                            <input
                              className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
                              maxLength={7}
                              type="text"
                              placeholder={`${bldgUtilityInfo.CurrentWaterPumpCap}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangeWaterPumpCapacity(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangeWaterPumpCapacity(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              {bldgUtilityInfo.CurrentWaterPumpCap}
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-waterpump-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetPumpCapTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetWaterPumpCapacity();
                        }}
                      />
                    </>
                  }
                />
                {bldgGeneralInfo.Efficiency != 0 && (
                  <>
                    <PanelSectionRow
                      uppercase={true}
                      left={efficiencyLabel}
                      right={
                        <LocalizedNumber
                          value={bldgGeneralInfo.Efficiency}
                          unit={Unit.Percentage}
                        />
                      }
                    />
                    <PanelSectionRow
                      uppercase={true}
                      left={actualPumpCapLabel}
                      right={
                        <LocalizedNumber
                          value={
                            bldgUtilityInfo.CurrentWaterPumpCap *
                            (bldgGeneralInfo.Efficiency / 100)
                          }
                          unit={Unit.VolumePerMonth}
                        />
                      }
                    />
                  </>
                )}
                {bldgUtilityInfo.OriginalWaterPumpCap != 0 && (
                  <PanelSectionRow
                    uppercase={true}
                    left={originalPumpCapLabel}
                    right={bldgUtilityInfo.OriginalWaterPumpCap}
                  />
                )}
              </PanelSection>
            ) : null}
            {bldgUtilityInfo.IsSewageOutlet ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeDumpCapLabel}
                  right={
                    <>
                      <div>
                        <div className={textElipsisInputThemeModule.wrapper}>
                          {" "}
                          <div
                            className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
                          >
                            <input
                              className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
                              maxLength={7}
                              type="text"
                              placeholder={`${(
                                <LocalizedNumber
                                  value={bldgUtilityInfo.CurrentSewageDumpCap}
                                  unit={Unit.VolumePerMonth}
                                />
                              )}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangeSewageDumpCapacity(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangeSewageDumpCapacity(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              {
                                <LocalizedNumber
                                  value={bldgUtilityInfo.CurrentSewageDumpCap}
                                  unit={Unit.VolumePerMonth}
                                />
                              }
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-sewagedump-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetDumpCapTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetSewageDumpCapacity();
                        }}
                      />
                    </>
                  }
                />
                {bldgGeneralInfo.Efficiency != 0 && (
                  <>
                    <PanelSectionRow
                      uppercase={true}
                      left={efficiencyLabel}
                      right={
                        <LocalizedNumber
                          value={bldgGeneralInfo.Efficiency}
                          unit={Unit.Percentage}
                        />
                      }
                    />
                    <PanelSectionRow
                      uppercase={true}
                      left={actualDumpCapLabel}
                      right={
                        <LocalizedNumber
                          value={Math.round(
                            bldgUtilityInfo.CurrentSewageDumpCap *
                              (bldgGeneralInfo.Efficiency / 100)
                          )}
                          unit={Unit.VolumePerMonth}
                        />
                      }
                    />
                  </>
                )}
                {bldgUtilityInfo.OriginalSewageDumpCap != 0 && (
                  <PanelSectionRow
                    uppercase={true}
                    left={originalDumpCapLabel}
                    right={
                      <LocalizedNumber
                        value={bldgUtilityInfo.OriginalSewageDumpCap}
                        unit={Unit.VolumePerMonth}
                      />
                    }
                  />
                )}
              </PanelSection>
            ) : null}
            {bldgUtilityInfo.IsPowerPlant ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={powerHeader!}
                />
                <PanelSectionRow
                  disableFocus={true}
                  left={changeLabel!}
                  right={
                    <>
                      <div>
                        <div className={textElipsisInputThemeModule.wrapper}>
                          {" "}
                          <div
                            className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
                          >
                            <input
                              className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
                              maxLength={7}
                              type="text"
                              placeholder={`${(
                                <LocalizedNumber
                                  value={bldgUtilityInfo.CurrentPowerProdCap}
                                  unit={Unit.Power}
                                />
                              )}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangePowerProdCapacity(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangePowerProdCapacity(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              {
                                <LocalizedNumber
                                  value={bldgUtilityInfo.CurrentPowerProdCap}
                                  unit={Unit.Power}
                                />
                              }
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-powerprod-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetPowerCapTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetPowerProdCapacity();
                        }}
                      />
                    </>
                  }
                />
                {bldgGeneralInfo.Efficiency != 0 &&
                  bldgGeneralInfo.Efficiency != 100 && (
                    <>
                      <PanelSectionRow
                        left={
                          <>
                            {`${boostedLabel}: `}
                            <LocalizedNumber
                              value={bldgGeneralInfo.Efficiency}
                              unit={Unit.Integer}
                            />
                            {"% "}
                            {efficiencyLabel}
                          </>
                        }
                        right={
                          <LocalizedNumber
                            value={Math.round(
                              bldgUtilityInfo.CurrentPowerProdCap *
                                (bldgGeneralInfo.Efficiency / 100)
                            )}
                            unit={Unit.Power}
                          />
                        }
                      />
                    </>
                  )}
                {bldgUtilityInfo.OriginalPowerProdCap != 0 && (
                  <PanelSectionRow
                    left={originalLabel}
                    right={
                      <LocalizedNumber
                        value={bldgUtilityInfo.OriginalPowerProdCap}
                        unit={Unit.Power}
                      />
                    }
                  />
                )}
              </PanelSection>
            ) : null}
          </>
        }
      />
    </>
  );
};
