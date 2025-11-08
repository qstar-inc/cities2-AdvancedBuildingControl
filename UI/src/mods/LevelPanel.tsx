import {
    ChangeHousehold, ChangeLevel, ChangeMaxWorkplace, levelPanelVisibleBinding, ResetHousehold,
    ResetLevel, ResetMaxWorkplace, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_AUTO, FOCUS_DISABLED, FocusDisabled } from "cs2/input";
import { LocalizedNumber, Unit, useLocalization } from "cs2/l10n";
import { Button, PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";
import {
    infoRowModule, sipTextInputModule, styleLevelProgress, styleLevelSection, styleProgress,
    textElipsisInputModule, textElipsisInputThemeModule, uilStandard
} from "styleBindings";
import { BldgZoningInfo, LocaleKeys } from "types";

import { PanelBase } from "./PanelBase";

interface LevelPanelProps {
  bldgZoningInfo: BldgZoningInfo;
}

export const LevelPanel: FC<LevelPanelProps> = (props: LevelPanelProps) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(levelPanelVisibleBinding);

  let bldgZoningInfo = props.bldgZoningInfo;

  const headerLabel = translate(LocaleKeys.ZONING_HEADER);
  const infoLabel = translate(LocaleKeys.ZONING_INFORMATION);
  const changeLevelLabel = translate(LocaleKeys.ZONING_CHANGE_LEVEL);
  const currentUpkeepLabel = translate(LocaleKeys.ZONING_CURRENT_UPKEEP);
  const changeHouseholdLabel = translate(LocaleKeys.ZONING_CHANGE_HOUSEHOLD);
  const currentRentLabel = translate(LocaleKeys.ZONING_CURRENT_RENT);
  const maxHouseholdLabel = translate(LocaleKeys.ZONING_MAX_HOUSEHOLD);
  const maxHouseholdTooltip = translate(
    LocaleKeys.ZONING_MAX_HOUSEHOLD_TOOLTIP
  );
  const resetLevelTooltip = translate(LocaleKeys.ZONING_RESET_LEVEL_TOOLTIP);
  const resetHouseholdTooltip = translate(
    LocaleKeys.ZONING_RESET_HOUSEHOLD_TOOLTIP
  );
  const changeWorkplaceLabel = translate(LocaleKeys.ZONING_CHANGE_WORKPLACE);
  const ogWorkplaceLabel = translate(LocaleKeys.ZONING_ORIGINAL_WORKPLACE);
  const resetWorkplaceTooltip =
    translate(LocaleKeys.ZONING_RESET_WORKPLACE_TOOLTIP) +
    " (Original: " +
    bldgZoningInfo.OriginalMaxWorkplaceCount +
    ")";

  const visible = useMemo(() => visibleBindingValue, [visibleBindingValue]);

  return (
    <>
      <PanelBase
        header={headerLabel!}
        visible={visible}
        height={100}
        content={
          <>
            <PanelSection>
              <PanelSectionRow
                uppercase={false}
                disableFocus={true}
                left={infoLabel}
              />
            </PanelSection>
            {bldgZoningInfo.HasLevel ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeLevelLabel}
                  right={
                    <FocusDisabled>
                      <>
                        {" "}
                        <div className={styleLevelSection.bar}>
                          {Array.from({ length: 6 }, (_, i) => {
                            let isPassed = i <= bldgZoningInfo.Level;
                            if (i !== 0) {
                              return (
                                <Button
                                  key={i}
                                  focusKey={FOCUS_AUTO}
                                  onSelect={() => ChangeLevel(i)}
                                  className={`${styleLevelProgress.progressBar} ${styleLevelSection.levelSegment}`}
                                  style={{
                                    border: "none",
                                    display: "flex",
                                    flexDirection: "column",
                                  }}
                                >
                                  <div
                                    className={styleProgress.progressBounds}
                                    style={{
                                      width: "100%",
                                      backdropFilter: `brightness(${i / 6})`,
                                    }}
                                  >
                                    <div
                                      className={
                                        isPassed
                                          ? styleLevelProgress.progress
                                          : ""
                                      }
                                      style={{
                                        margin: "auto",
                                        textAlign: "center",
                                        width: isPassed ? "100%" : "100%",
                                        color: isPassed
                                          ? undefined
                                          : "whitesmoke",
                                      }}
                                    >
                                      {i}
                                    </div>
                                  </div>
                                </Button>
                              );
                            } else {
                              return;
                            }
                          })}
                        </div>{" "}
                        <ToolButton
                          id="starq-abc-level-reset"
                          focusKey={FOCUS_DISABLED}
                          tooltip={resetLevelTooltip!}
                          src={uilStandard + "Reset.svg"}
                          onSelect={() => {
                            ResetLevel();
                          }}
                        />
                      </>
                    </FocusDisabled>
                  }
                />
                <PanelSectionRow
                  uppercase={true}
                  left={currentUpkeepLabel}
                  right={
                    <LocalizedNumber
                      value={bldgZoningInfo.Upkeep}
                      unit={Unit.Money}
                    />
                  }
                />
              </PanelSection>
            ) : null}
            {bldgZoningInfo.HasWorkplace ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeWorkplaceLabel}
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
                              maxLength={3}
                              type="text"
                              placeholder={`${(
                                <LocalizedNumber
                                  value={
                                    bldgZoningInfo.CurrentMaxWorkplaceCount
                                  }
                                />
                              )}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangeMaxWorkplace(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangeMaxWorkplace(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              <LocalizedNumber
                                value={bldgZoningInfo.CurrentMaxWorkplaceCount}
                              />
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-workplace-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetWorkplaceTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetMaxWorkplace();
                        }}
                      />
                    </>
                  }
                />
                {bldgZoningInfo.OriginalMaxWorkplaceCount > 0 ? (
                  <PanelSectionRow
                    uppercase={true}
                    left={ogWorkplaceLabel}
                    right={bldgZoningInfo.OriginalMaxWorkplaceCount}
                  />
                ) : null}
              </PanelSection>
            ) : null}
            {bldgZoningInfo.HasHousehold ? (
              <>
                <PanelSection>
                  {/* {props.h_household ? ( */}
                  {bldgZoningInfo.AreaType == "Residential" ? (
                    <>
                      <PanelSectionRow
                        uppercase={true}
                        left={`${changeHouseholdLabel}`}
                        right={
                          <>
                            <div>
                              <div
                                className={textElipsisInputThemeModule.wrapper}
                              >
                                {" "}
                                <div
                                  className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
                                >
                                  <input
                                    className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
                                    maxLength={5}
                                    type="text"
                                    placeholder={`${(
                                      <LocalizedNumber
                                        value={bldgZoningInfo.Household}
                                      />
                                    )}`}
                                    onKeyDown={(e) => {
                                      if (e.key === "Enter") {
                                        ChangeHousehold(
                                          Number.parseInt(e.currentTarget.value)
                                        );
                                      }
                                    }}
                                    onBlur={(e) => {
                                      ChangeHousehold(
                                        Number.parseInt(e.currentTarget.value)
                                      );
                                    }}
                                  />
                                  <div
                                    className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                                  >
                                    {
                                      <LocalizedNumber
                                        value={bldgZoningInfo.Household}
                                      />
                                    }
                                  </div>
                                </div>
                              </div>
                            </div>{" "}
                            <ToolButton
                              id="starq-abc-household-reset"
                              focusKey={FOCUS_DISABLED}
                              tooltip={resetHouseholdTooltip!}
                              src={uilStandard + "Reset.svg"}
                              onSelect={() => {
                                ResetHousehold();
                              }}
                            />
                          </>
                        }
                      />
                      <PanelSectionRow
                        tooltip={maxHouseholdTooltip!}
                        uppercase={true}
                        left={maxHouseholdLabel}
                        right={
                          <LocalizedNumber
                            value={bldgZoningInfo.MaxHousehold}
                          />
                        }
                      />
                    </>
                  ) : null}
                  {/* ) : null} */}
                  <PanelSectionRow
                    uppercase={true}
                    left={currentRentLabel}
                    right={
                      <LocalizedNumber
                        value={bldgZoningInfo.Rent}
                        unit={Unit.Money}
                      />
                    }
                  />
                </PanelSection>
                {/* <PanelSection>
                  <PanelSectionRow
                    uppercase={false}
                    left={"RENT CALCULATION"}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={
                      "(For development only, will be removed on the final release)"
                    }
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"A. Level"}
                    right={bldgZoningInfo.Level}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"B. Lot Size"}
                    right={bldgZoningInfo.LotSize}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"C. Space Multiplier"}
                    right={bldgZoningInfo.SpaceMultiplier}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"D. Base Rent (" + bldgZoningInfo.AreaType + ")"}
                    right={bldgZoningInfo.ZoneTypeBase?.toFixed(3)}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={
                      "E. Land Value Modifier (" + bldgZoningInfo.AreaType + ")"
                    }
                    right={bldgZoningInfo.LandValueModifier?.toFixed(3)}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"F. Land Value Base"}
                    right={bldgZoningInfo.LandValueBase?.toFixed(3)}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"G. Ignore Land Value"}
                    right={bldgZoningInfo.IgnoreLandValue ? "TRUE" : "FALSE"}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"H. Land Value Rate [E x F]"}
                    right={`${
                      bldgZoningInfo.IgnoreLandValue
                        ? "(Ignored) 0"
                        : `= ${(
                            bldgZoningInfo.LandValueBase *
                            bldgZoningInfo.LandValueModifier
                          ).toFixed(3)}`
                    }`}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"G. Total Rent [(H + (D x A)) x B x C]"}
                    right={`= ${bldgZoningInfo.TotalRent?.toFixed(3)}`}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"F. Is Mixed Building"}
                    right={bldgZoningInfo.IsMixed ? "TRUE" : "FALSE"}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"G. Business Rent Percent if Mixed"}
                    right={
                      (bldgZoningInfo.MixedPercent * 100)?.toFixed(0) + "%"
                    }
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"H. Household Count"}
                    right={`${(
                      bldgZoningInfo.PropertiesCount *
                      (1 - bldgZoningInfo.MixedPercent)
                    ).toFixed(0)}`}
                  />
                  <PanelSectionRow
                    uppercase={false}
                    left={"I. Rent Per Renter"}
                    right={bldgZoningInfo.Rent}
                  />
                </PanelSection> */}
              </>
            ) : null}
          </>
        }
      />
    </>
  );
};
