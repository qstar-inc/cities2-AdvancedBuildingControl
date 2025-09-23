import {
  ChangeHousehold,
  ChangeLevel,
  InfoButton,
  levelPanelVisibleBinding,
  ResetHousehold,
  ResetLevel,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_AUTO, FOCUS_DISABLED, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { Button, PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";
import {
  infoRowModule,
  sipTextInputModule,
  styleLevelProgress,
  styleLevelSection,
  styleProgress,
  textElipsisInputModule,
  textElipsisInputThemeModule,
  uilStandard,
} from "styleBindings";
import { BldgZoningInfo, LocaleKeys, ZoneDataInfo } from "types";

import { PanelBase } from "./PanelBase";
import styles from "./style.module.scss";

interface LevelPanelProps {
  bldgZoningInfo: BldgZoningInfo;
  // h_level: boolean;
  // w_level: number;
  // w_upkeep: number;
  // h_household: boolean;
  // w_household: number;
  // w_maxhousehold: string;
  // w_rent: number;
  // w_areaType: string;
  // w_spacemult: number;
  // // w_zone: string;
  // // w_zonelist: ZoneDataInfo[];

  // w_zonetypebase: number;
  // w_landvaluemodifier: number;
  // w_ignorelandvalue: boolean;
  // w_lotsize: number;
  // w_landvaluebase: number;
  // w_totalrent: number;
  // w_propertiescount: number;
  // w_mixedpercent: number;
  // w_ismixed: boolean;
}

export const LevelPanel: FC<LevelPanelProps> = (props: LevelPanelProps) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(levelPanelVisibleBinding);

  let bldgZoningInfo = props.bldgZoningInfo;

  const headerText = translate(LocaleKeys.ZONING_HEADER);
  const changeLevelText = translate(LocaleKeys.ZONING_CHANGE_LEVEL);
  const infoText = translate(LocaleKeys.ZONING_INFORMATION);
  const currentUpkeepText = translate(LocaleKeys.ZONING_CURRENT_UPKEEP);
  const changeHouseholdText = translate(LocaleKeys.ZONING_CHANGE_HOUSEHOLD);
  const currentRentText = translate(LocaleKeys.ZONING_CURRENT_RENT);
  const maxHouseholdText = translate(LocaleKeys.ZONING_MAX_HOUSEHOLD);
  const maxHouseholdTooltipText = translate(
    LocaleKeys.ZONING_MAX_HOUSEHOLD_TOOLTIP
  );
  const resetLevelTooltip = translate(LocaleKeys.ZONING_RESET_LEVEL_TOOLTIP);
  const resetHouseholdTooltip = translate(
    LocaleKeys.ZONING_RESET_HOUSEHOLD_TOOLTIP
  );

  const visible = useMemo(
    () =>
      visibleBindingValue &&
      (bldgZoningInfo.HasLevel || bldgZoningInfo.HasHousehold),
    [visibleBindingValue]
  );

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
            {bldgZoningInfo.HasLevel ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeLevelText}
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
                  left={currentUpkeepText}
                  right={bldgZoningInfo.Upkeep}
                />
              </PanelSection>
            ) : null}

            <PanelSection>
              {/* {props.h_household ? ( */}
              {bldgZoningInfo.AreaType == "Residential" ? (
                <>
                  <PanelSectionRow
                    uppercase={true}
                    left={`${changeHouseholdText}`}
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
                                placeholder={`${bldgZoningInfo.Household}`}
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
                                {bldgZoningInfo.Household}
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
                    tooltip={maxHouseholdTooltipText!}
                    uppercase={true}
                    left={maxHouseholdText}
                    right={bldgZoningInfo.MaxHousehold}
                  />
                </>
              ) : null}
              {/* ) : null} */}
              <PanelSectionRow
                uppercase={true}
                left={currentRentText}
                right={bldgZoningInfo.Rent}
              />
            </PanelSection>
            <PanelSection>
              <PanelSectionRow uppercase={false} left={"RENT CALCULATION"} />
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

                // ${
                //   ((props.w_ignorelandvalue
                //     ? 0
                //     : props.w_landvaluebase * props.w_landvaluemodifier) +
                //     props.w_zonetypebase * props.w_level) *
                //   props.w_lotsize *
                //   props.w_spacemult
                //   }
              />
              <PanelSectionRow
                uppercase={false}
                left={"F. Is Mixed Building"}
                right={bldgZoningInfo.IsMixed ? "TRUE" : "FALSE"}
              />
              <PanelSectionRow
                uppercase={false}
                left={"G. Business Rent Percent if Mixed"}
                right={(bldgZoningInfo.MixedPercent * 100)?.toFixed(0) + "%"}
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
            </PanelSection>
          </>
        }
      />
    </>
  );
};

// <PanelSection>
//   <>
//     <PanelSectionRow
//       uppercase={true}
//       disableFocus={true}
//       left={
//         <FocusDisabled>
//           {/* <ToolButton
//             id="starq-abc-level-reset"
//             focusKey={FOCUS_DISABLED}
//             tooltip={resetTooltip!}
//             // selected={false}
//             // disabled={true}
//             src={uilStandard + "Reset.svg"}
//             onSelect={() => {
//               ResetLevel();
//             }}
//           /> */}
//           {/* <ToolButton
//             id="starq-abc-household-reset"
//             focusKey={FOCUS_DISABLED}
//             tooltip={resetTooltip!}
//             // selected={false}
//             // disabled={true}
//             src={uilStandard + "Reset.svg"}
//             onSelect={() => {
//               ResetHousehold();
//             }}
//           /> */}
//         </FocusDisabled>
//       }
//       right={
//         <FocusDisabled>
//           <ToolButton
//             id="starq-abc-level-copy"
//             className={styles.DisabledToolButton}
//             focusKey={FOCUS_AUTO}
//             tooltip={"Disabled"}
//             // selected={false}
//             disabled={true}
//             // className={styles.ToolWhite}
//             src={uilStandard + "RectangleCopy.svg"}
//             onSelect={() => {
//               // RandomizeStyle();
//             }}
//           />
//           <ToolButton
//             id="starq-abc-level-paste"
//             className={styles.DisabledToolButton}
//             focusKey={FOCUS_AUTO}
//             tooltip={"Disabled"}
//             // selected={false}
//             disabled={true}
//             // className={styles.ToolWhite}
//             src={uilStandard + "RectanglePaste.svg"}
//             onSelect={() => {
//               // RandomizeStyle();
//             }}
//           />
//           <ToolButton
//             id="starq-abc-level-copyclear"
//             className={styles.DisabledToolButton}
//             focusKey={FOCUS_AUTO}
//             tooltip={"Disabled"}
//             // selected={false}
//             disabled={true}
//             // className={styles.ToolWhite}
//             src={uilStandard + "XClose.svg"}
//             onSelect={() => {
//               // RandomizeStyle();
//             }}
//           />
//         </FocusDisabled>
//       }
//     />
//   </>
// </PanelSection>;
