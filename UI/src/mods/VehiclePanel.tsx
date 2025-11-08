import {
  ChangeVehicleCapacity,
  ResetVehicleCapacity,
  ToolButton,
  vehiclePanelVisibleBinding,
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_DISABLED } from "cs2/input";
import { LocalizedNumber, Unit } from "cs2/l10n";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";
import {
  infoRowModule,
  sipTextInputModule,
  textElipsisInputModule,
  textElipsisInputThemeModule,
  uilStandard,
} from "styleBindings";
import {
  BldgGeneralInfo,
  BldgVehicleInfo,
  FindTranslation,
  ValueType as UpdateValueType,
  VehicleInfo,
} from "types";

import { PanelBase } from "./PanelBase";

interface VehiclePanelProps {
  bldgVehicleInfo: BldgVehicleInfo;
  bldgGeneralInfo: BldgGeneralInfo;
}

function GetTitle(str: string) {
  str = str?.toLowerCase();
  switch (str) {
    case "ambulance":
      return FindTranslation("Properties.AMBULANCE_COUNT", true);
    case "bus":
      return FindTranslation("Transport.LEGEND_VEHICLES[Bus]", true);
    case "ferry":
      return FindTranslation("Transport.LEGEND_VEHICLES[Ferry]", true);
    case "garbage":
      return FindTranslation("Properties.GARBAGE_TRUCK_COUNT", true);
    case "hearse":
      return FindTranslation("Properties.HEARSE_COUNT", true);
    case "mediheli":
      return FindTranslation("Properties.MEDICAL_HELICOPTER_COUNT", true);
    // case "post":
    //   return FindTranslation("SelectedInfoPanel.POST_VEHICLE_TITLE", true);
    case "rocket":
      return FindTranslation("Transport.LEGEND_VEHICLES[Rocket]", true);
    case "subway":
      return `${FindTranslation(
        "SubServices.NAME[TransportationSubway]",
        true
      )} ${FindTranslation("Transport.LEGEND_VEHICLES[Train]", true)}`;
    case "taxi":
      return FindTranslation("Transport.LEGEND_VEHICLES[Taxi]", true);
    case "train":
      return FindTranslation("Transport.LEGEND_VEHICLES[Train]", true);
    case "tram":
      return FindTranslation("Transport.LEGEND_VEHICLES[Tram]", true);
    default:
      return str;
  }
}

const Section = ({
  IsActive,
  vType,
  valueType,
  Vehicle,
}: {
  IsActive: boolean;
  vType: string;
  valueType: UpdateValueType;
  Vehicle: VehicleInfo;
}) => {
  const OverrideToLabel = FindTranslation(`OverrideTo`);
  const BaseLabel = FindTranslation(`Base`);
  const FinalLabel = FindTranslation(`Final`);
  const BaseTooltip = FindTranslation(`BaseTooltip`);
  const FinalTooltip = FindTranslation(`FinalTooltip`);
  const ResetTooltip = FindTranslation(`ResetTooltip`);

  return (
    <>
      {IsActive ? (
        <PanelSection>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={GetTitle(vType)}
          ></PanelSectionRow>
          <PanelSectionRow
            disableFocus={true}
            left={OverrideToLabel}
            right={
              <>
                <div>
                  <div className={textElipsisInputThemeModule.wrapper}>
                    <div
                      className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
                    >
                      <input
                        className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
                        maxLength={7}
                        type="text"
                        placeholder={`${(
                          <LocalizedNumber
                            value={Vehicle.Current}
                            unit={Unit.Integer}
                          />
                        )}`}
                        onKeyDown={(e) => {
                          if (e.key === "Enter") {
                            ChangeVehicleCapacity(
                              Number.parseInt(e.currentTarget.value),
                              valueType
                            );
                          }
                        }}
                        onBlur={(e) => {
                          ChangeVehicleCapacity(
                            Number.parseInt(e.currentTarget.value),
                            valueType
                          );
                        }}
                      />
                      <div
                        className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                      >
                        {
                          <LocalizedNumber
                            value={Vehicle.Current}
                            unit={Unit.Integer}
                          />
                        }
                      </div>
                    </div>
                  </div>
                </div>
                <ToolButton
                  id={`starq-abc-${vType}-reset`}
                  focusKey={FOCUS_DISABLED}
                  tooltip={ResetTooltip!}
                  src={uilStandard + "Reset.svg"}
                  onSelect={() => {
                    ResetVehicleCapacity(valueType);
                  }}
                />
              </>
            }
          />
          {Vehicle.Original != 0 && (
            <PanelSectionRow
              tooltip={BaseTooltip!}
              left={BaseLabel}
              right={
                <LocalizedNumber value={Vehicle.Original} unit={Unit.Integer} />
              }
            />
          )}
          {Vehicle.Combined != Vehicle.Current && (
            <PanelSectionRow
              tooltip={FinalTooltip!}
              left={FinalLabel}
              right={
                <LocalizedNumber value={Vehicle.Combined} unit={Unit.Integer} />
              }
            />
          )}
        </PanelSection>
      ) : null}
    </>
  );
};

export const VehiclePanel: FC<VehiclePanelProps> = (
  props: VehiclePanelProps
) => {
  // const { translate } = useLocalization();

  const visibleBindingValue = useValue(vehiclePanelVisibleBinding);

  const headerText = FindTranslation(`VehicleHeader`);
  const infoText = FindTranslation(`VehicleInfo`);
  let bldgVehicleInfo = props.bldgVehicleInfo;
  let bldgGeneralInfo = props.bldgGeneralInfo;

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
            <Section
              vType={bldgVehicleInfo.TransportType}
              IsActive={bldgVehicleInfo.IsDepot}
              valueType={UpdateValueType.Depot}
              Vehicle={bldgVehicleInfo.DepotVehicle}
            />
            <Section
              vType="Garbage"
              IsActive={bldgVehicleInfo.IsGarbageFacility}
              valueType={UpdateValueType.GarbageTruck}
              Vehicle={bldgVehicleInfo.GarbageTruck}
            />
            <Section
              vType="Ambulance"
              IsActive={bldgVehicleInfo.IsHospital}
              valueType={UpdateValueType.Ambulance}
              Vehicle={bldgVehicleInfo.Ambulance}
            />
            {bldgGeneralInfo.HasHeli && (
              <Section
                vType="MediHeli"
                IsActive={bldgVehicleInfo.IsHospital}
                valueType={UpdateValueType.MediHeli}
                Vehicle={bldgVehicleInfo.MediHeli}
              />
            )}
            <Section
              vType="Hearse"
              IsActive={bldgVehicleInfo.IsDeathcare}
              valueType={UpdateValueType.Hearse}
              Vehicle={bldgVehicleInfo.Hearse}
            />
            {/* {bldgVehicleInfo.IsGarbageFacility ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeLabel}
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
                              placeholder={`${bldgVehicleInfo.CurrentGarbageTruckCap}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangeGarbageTruckCapacity(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangeGarbageTruckCapacity(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              {bldgVehicleInfo.CurrentGarbageTruckCap}
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-depot-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetGarbageTruckCapacity();
                        }}
                      />
                    </>
                  }
                />
                {bldgVehicleInfo.OriginalGarbageTruckCap != 0 && (
                  <PanelSectionRow
                    uppercase={true}
                    left={originalLabel}
                    right={bldgVehicleInfo.OriginalGarbageTruckCap}
                  />
                )}
                {bldgVehicleInfo.CombinedGarbageTruckCap !=
                  bldgVehicleInfo.CurrentGarbageTruckCap && (
                  <PanelSectionRow
                    uppercase={true}
                    left={totalWithUpgradesLabel}
                    right={bldgVehicleInfo.CombinedGarbageTruckCap}
                  />
                )}
              </PanelSection>
            ) : null}
            {bldgVehicleInfo.IsHospital ? (
              <PanelSection>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={changeLabel}
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
                              placeholder={`${bldgVehicleInfo.CurrentAmbulanceCap}`}
                              onKeyDown={(e) => {
                                if (e.key === "Enter") {
                                  ChangeAmbulanceCapacity(
                                    Number.parseInt(e.currentTarget.value)
                                  );
                                }
                              }}
                              onBlur={(e) => {
                                ChangeAmbulanceCapacity(
                                  Number.parseInt(e.currentTarget.value)
                                );
                              }}
                            />
                            <div
                              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                            >
                              {bldgVehicleInfo.CurrentAmbulanceCap}
                            </div>
                          </div>
                        </div>
                      </div>{" "}
                      <ToolButton
                        id="starq-abc-ambulance-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetAmbulanceCapacity();
                        }}
                      />
                    </>
                  }
                />
                {bldgVehicleInfo.OriginalAmbulanceCap != 0 && (
                  <PanelSectionRow
                    uppercase={true}
                    left={originalLabel}
                    right={bldgVehicleInfo.OriginalAmbulanceCap}
                  />
                )}
                {bldgVehicleInfo.CombinedAmbulanceCap !=
                  bldgVehicleInfo.CurrentAmbulanceCap && (
                  <PanelSectionRow
                    uppercase={true}
                    left={totalWithUpgradesLabel}
                    right={bldgVehicleInfo.CombinedAmbulanceCap}
                  />
                )}
                {bldgGeneralInfo.HasHeli ? (
                  <>
                    <PanelSectionRow
                      uppercase={true}
                      disableFocus={true}
                      left={changeLabel}
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
                                  maxLength={7}
                                  type="text"
                                  placeholder={`${bldgVehicleInfo.CurrentMediHeliCap}`}
                                  onKeyDown={(e) => {
                                    if (e.key === "Enter") {
                                      ChangeMediHeliCapacity(
                                        Number.parseInt(e.currentTarget.value)
                                      );
                                    }
                                  }}
                                  onBlur={(e) => {
                                    ChangeMediHeliCapacity(
                                      Number.parseInt(e.currentTarget.value)
                                    );
                                  }}
                                />
                                <div
                                  className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
                                >
                                  {bldgVehicleInfo.CurrentMediHeliCap}
                                </div>
                              </div>
                            </div>
                          </div>{" "}
                          <ToolButton
                            id="starq-abc-MediHel-reset"
                            focusKey={FOCUS_DISABLED}
                            tooltip={resetTooltip!}
                            src={uilStandard + "Reset.svg"}
                            onSelect={() => {
                              ResetMediHeliCapacity();
                            }}
                          />
                        </>
                      }
                    />
                    {bldgVehicleInfo.OriginalMediHeliCap != 0 && (
                      <PanelSectionRow
                        uppercase={true}
                        left={originalLabel}
                        right={bldgVehicleInfo.OriginalMediHeliCap}
                      />
                    )}
                    {bldgVehicleInfo.CombinedMediHeliCap !=
                      bldgVehicleInfo.CurrentMediHeliCap && (
                      <PanelSectionRow
                        uppercase={true}
                        left={totalWithUpgradesLabel}
                        right={bldgVehicleInfo.CombinedMediHeliCap}
                      />
                    )}
                  </>
                ) : null}
              </PanelSection>
            ) : null} */}
          </>
        }
      />
    </>
  );
};
