import {
  ChangeMaxWorkplace,
  ChangeValue,
  levelPanelVisibleBinding,
  ResetMaxWorkplace,
  ResetValue,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_AUTO, FOCUS_DISABLED, FocusDisabled } from "cs2/input";
import { LocalizedNumber, Unit, useLocalization } from "cs2/l10n";
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
import {
  BldgPropertiesInfo as BldgPropertiesInfo,
  FindTranslation,
  InfoTypeCOCE,
  InfoTypeCOCEWithData,
  LocaleKeys,
  UpdateValueType,
} from "types";

import { PanelBase } from "./PanelBase";
import { FormWithReset, MultiSection } from "./RenderUtil";

interface PropertiesPanelProps {
  bldgPropertiesInfo: BldgPropertiesInfo;
}

const LevelSection = ({ props }: { props: PropertiesPanelProps }) => {
  let bldgPropertiesInfo = props.bldgPropertiesInfo;

  const infoLabel = FindTranslation(`LevelInfo`);
  const changeLevelLabel = FindTranslation(`ChangeLevel`);
  const currentUpkeepLabel = FindTranslation(`CurrentUpkeep`);
  const changeHouseholdLabel = FindTranslation(`ChangeHousehold`);
  const currentRentLabel = FindTranslation(`CurrentRent`);
  const maxHouseholdLabel = FindTranslation(`MaxHousehold`);
  const maxHouseholdTooltip = FindTranslation(`MaxHouseholdTooltip`);
  const resetLevelTooltip = FindTranslation(`ResetLevelTooltip`);
  const resetHouseholdTooltip = FindTranslation(`ResetHouseholdTooltip`);

  const householdLabel = FindTranslation(`SelectedInfoPanel.HOUSEHOLDS`, true);

  const OverrideToLabel = FindTranslation(`OverrideTo`);
  const BaseLabel = FindTranslation(`Base`);
  const BaseTooltip = FindTranslation(`BaseTooltip`);

  const householdCoce: InfoTypeCOCEWithData[] = [
    {
      coce: props.bldgPropertiesInfo.Household,
      id: "household",
      title: householdLabel,
      onChange: (value: number) =>
        ChangeValue(value, UpdateValueType.Household),
      onReset: () => ResetValue(UpdateValueType.Household),
    },
  ];

  return (
    <>
      {(bldgPropertiesInfo.HasLevel || bldgPropertiesInfo.HasHousehold) && (
        <>
          <PanelSection>
            <PanelSectionRow
              uppercase={false}
              disableFocus={true}
              left={infoLabel}
            />
          </PanelSection>
          {bldgPropertiesInfo.HasLevel && (
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
                          let isPassed = i <= bldgPropertiesInfo.Level.Current;
                          const isDefault =
                            i == bldgPropertiesInfo.Level.Original;
                          const backgroundColor = (() => {
                            if (isPassed && isDefault) return "#1e6bff";
                            if (isPassed && !isDefault) return undefined;
                            if (!isPassed && isDefault) return "#123a8c";
                            return undefined;
                          })();
                          if (i !== 0) {
                            return (
                              <Button
                                key={i}
                                focusKey={FOCUS_AUTO}
                                onSelect={() =>
                                  ChangeValue(i, UpdateValueType.Level)
                                }
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
                                      width: "100%",
                                      color: isPassed
                                        ? undefined
                                        : "whitesmoke",
                                      backgroundColor,
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
                          ResetValue(UpdateValueType.Level);
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
                    value={bldgPropertiesInfo.Upkeep}
                    unit={Unit.Money}
                  />
                }
              />
            </PanelSection>
          )}
          {bldgPropertiesInfo.HasHousehold &&
            bldgPropertiesInfo.AreaType == "Residential" && (
              <>
                <MultiSection coceArray={householdCoce} />
                {/* <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={householdLabel}
                ></PanelSectionRow> */}
                {/* <PanelSectionRow
                  disableFocus={true}
                  left={OverrideToLabel}
                  right={
                    <FormWithReset
                      currentVal={bldgPropertiesInfo.Household.Current}
                      id="household"
                      onChange={value =>
                        ChangeValue(value, UpdateValueType.Household)
                      }
                      onReset={() => ResetValue(UpdateValueType.Household)}
                    />
                  }
                ></PanelSectionRow> */}
                <PanelSection>
                  {bldgPropertiesInfo.AreaType == "Residential" && (
                    <>
                      {/* <PanelSectionRow
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
                                        value={
                                          bldgPropertiesInfo.Household.Current
                                        }
                                      />
                                    )}`}
                                    onKeyDown={e => {
                                      if (e.key === "Enter") {
                                        ChangeValue(
                                          Number.parseInt(
                                            e.currentTarget.value,
                                          ),
                                          UpdateValueType.Household,
                                        );
                                      }
                                    }}
                                    onBlur={e => {
                                      ChangeValue(
                                        Number.parseInt(e.currentTarget.value),
                                        UpdateValueType.Household,
                                      );
                                    }}
                                  />
                                  <div
                                    className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                                  >
                                    {
                                      <LocalizedNumber
                                        value={
                                          bldgPropertiesInfo.Household.Current
                                        }
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
                                ResetValue(UpdateValueType.Household);
                              }}
                            />
                          </>
                        }
                      /> */}
                      <PanelSectionRow
                        tooltip={maxHouseholdTooltip!}
                        uppercase={true}
                        left={maxHouseholdLabel}
                        right={
                          <LocalizedNumber
                            value={bldgPropertiesInfo.MaxHousehold}
                          />
                        }
                      />
                    </>
                  )}
                  <PanelSectionRow
                    uppercase={true}
                    left={currentRentLabel}
                    right={
                      <LocalizedNumber
                        value={bldgPropertiesInfo.Rent}
                        unit={Unit.Money}
                      />
                    }
                  />
                </PanelSection>
              </>
            )}
        </>
      )}
    </>
  );
};

export const PropertiesPanel: FC<PropertiesPanelProps> = (
  props: PropertiesPanelProps,
) => {
  const visibleBindingValue = useValue(levelPanelVisibleBinding);

  const headerLabel = FindTranslation(`PropertiesHeader`);

  const visible = useMemo(() => visibleBindingValue, [visibleBindingValue]);

  const infoLabel = FindTranslation(`WorkplaceInfo`);
  const workplaceLabel = FindTranslation(`SelectedInfoPanel.WORKPLACES`, true);

  const workplaceCoce: InfoTypeCOCEWithData[] = [
    {
      coce: props.bldgPropertiesInfo.Workplace,
      id: "workplace",
      title: workplaceLabel,
      onChange: ChangeMaxWorkplace,
      onReset: ResetMaxWorkplace,
    },
  ];

  return (
    <>
      <PanelBase
        header={headerLabel!}
        visible={visible}
        height={100}
        content={
          <>
            <LevelSection props={props} />
            {props.bldgPropertiesInfo.IsWorkplace && (
              <MultiSection infoLabel={infoLabel} coceArray={workplaceCoce} />
            )}
            {/*<UtilitySection/> */}
          </>
        }
      />
    </>
  );
};
