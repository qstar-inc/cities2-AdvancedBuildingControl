import {
  ChangeValue,
  componentPanelVisibleBinding,
  ResetValue,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { FOCUS_DISABLED } from "cs2/input";
import { LocalizedNumber, Unit } from "cs2/l10n";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useMemo } from "react";

// import {
//     infoRowModule, sipTextInputModule, textElipsisInputModule, textElipsisInputThemeModule,
//     uilStandard
// } from "styleBindings";
// import {
//     BldgComponentInfo, BldgGeneralInfo, BldgVehicleInfo, FindTranslation, InfoTypeCOCE,
//     UpdateValueType
// } from "types";

// import { PanelBase } from "./PanelBase";

// interface VehiclePanelProps {
//   bldgComponentInfo: BldgComponentInfo;
//   bldgCOCEInfo: InfoTypeCOCE[];
// }

// function GetTitle(str: string) {
//   str = str?.toLowerCase();
//   switch (str) {
//     case "bus":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Bus]", true);
//     case "ferry":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Ferry]", true);
//     case "rocket":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Rocket]", true);
//     case "subway":
//       return `${FindTranslation(
//         "SubServices.NAME[TransportationSubway]",
//         true,
//       )} ${FindTranslation("Transport.LEGEND_VEHICLES[Train]", true)}`;
//     case "taxi":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Taxi]", true);
//     case "train":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Train]", true);
//     case "tram":
//       return FindTranslation("Transport.LEGEND_VEHICLES[Tram]", true);
//     case "garbage":
//       return FindTranslation("Properties.GARBAGE_TRUCK_COUNT", true);
//     case "ambulance":
//       return FindTranslation("Properties.AMBULANCE_COUNT", true);
//     case "mediheli":
//       return FindTranslation("Properties.MEDICAL_HELICOPTER_COUNT", true);
//     case "hearse":
//       return FindTranslation("Properties.HEARSE_COUNT", true);
//     case "patrolcar":
//       return FindTranslation("Properties.PATROL_CAR_COUNT", true);
//     case "policeheli":
//       return FindTranslation("Properties.POLICE_HELICOPTER_COUNT", true);
//     case "prisonvan":
//       return FindTranslation("Properties.PRISON_VAN_COUNT", true);
//     case "firetruck":
//       return FindTranslation("Properties.FIRE_ENGINE_COUNT", true);
//     case "fireheli":
//       return FindTranslation("Properties.FIRE_HELICOPTER_COUNT", true);
//     case "evacbus":
//       return FindTranslation("Properties.EVACUATION_BUS_COUNT", true);
//     case "postvan":
//       return FindTranslation("Properties.POST_VAN_COUNT", true);
//     case "posttruck":
//       return FindTranslation("Properties.POST_TRUCK_COUNT", true);
//     case "maintenancevehicle":
//       return FindTranslation("Properties.MAINTENANCE_VEHICLES", true);
//     default:
//       return str;
//   }
// }

// const Section = ({
//   IsActive,
//   vType,
//   valueType,
//   Vehicle,
// }: {
//   IsActive: boolean;
//   vType: string;
//   valueType: UpdateValueType;
//   Vehicle: InfoTypeCOCE;
// }) => {
//   const OverrideToLabel = FindTranslation(`OverrideTo`);
//   const BaseLabel = FindTranslation(`Base`);
//   const FinalLabel = FindTranslation(`Final`);
//   const BaseTooltip = FindTranslation(`BaseTooltip`);
//   const FinalTooltip = FindTranslation(`FinalTooltip`);
//   const ResetTooltip = FindTranslation(`ResetTooltip`);

//   console.log(IsActive);
//   console.log(vType);
//   console.log(JSON.stringify(valueType));
//   // console.log(JSON.stringify(Vehicle));

//   return (
//     <>
//       {IsActive ? (
//         <PanelSection>
//           <PanelSectionRow
//             uppercase={true}
//             disableFocus={true}
//             left={GetTitle(vType)}
//           ></PanelSectionRow>
//           <PanelSectionRow
//             disableFocus={true}
//             left={OverrideToLabel}
//             right={
//               <>
//                 <div>
//                   <div className={textElipsisInputThemeModule.wrapper}>
//                     <div
//                       className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
//                     >
//                       <input
//                         className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
//                         maxLength={7}
//                         type="text"
//                         placeholder={`${(
//                           <LocalizedNumber
//                             value={Vehicle.Current}
//                             unit={Unit.Integer}
//                           />
//                         )}`}
//                         onKeyDown={e => {
//                           if (e.key === "Enter") {
//                             console.log(`ChangeValue(
//                               ${Number.parseInt(e.currentTarget.value)},
//                               ${valueType},
//                             );`);
//                             ChangeValue(
//                               Number.parseInt(e.currentTarget.value),
//                               valueType,
//                             );
//                           }
//                         }}
//                         onBlur={e => {
//                           ChangeValue(
//                             Number.parseInt(e.currentTarget.value),
//                             valueType,
//                           );
//                         }}
//                       />
//                       <div
//                         className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
//                       >
//                         {
//                           <LocalizedNumber
//                             value={Vehicle.Current}
//                             unit={Unit.Integer}
//                           />
//                         }
//                       </div>
//                     </div>
//                   </div>
//                 </div>
//                 <ToolButton
//                   id={`starq-abc-${vType}-reset`}
//                   focusKey={FOCUS_DISABLED}
//                   tooltip={ResetTooltip!}
//                   src={uilStandard + "Reset.svg"}
//                   onSelect={() => {
//                     ResetValue(valueType);
//                   }}
//                 />
//               </>
//             }
//           />
//           {Vehicle.Enabled && (
//             <PanelSectionRow
//               tooltip={BaseTooltip!}
//               left={BaseLabel}
//               right={
//                 <LocalizedNumber value={Vehicle.Original} unit={Unit.Integer} />
//               }
//             />
//           )}
//           {Vehicle.Combined != Vehicle.Current && (
//             <PanelSectionRow
//               tooltip={FinalTooltip!}
//               left={FinalLabel}
//               right={
//                 <LocalizedNumber value={Vehicle.Combined} unit={Unit.Integer} />
//               }
//             />
//           )}
//         </PanelSection>
//       ) : null}
//     </>
//   );
// };

// export const VehiclePanel: FC<VehiclePanelProps> = (
//   props: VehiclePanelProps,
// ) => {
//   const visibleBindingValue = useValue(vehiclePanelVisibleBinding);

//   const headerText = FindTranslation(`VehicleHeader`);
//   const infoText = FindTranslation(`VehicleInfo`);
//   let bldgComponentInfo = props.bldgComponentInfo;

//   const visible = useMemo(() => visibleBindingValue, [visibleBindingValue]);

//   const defaultInfo: InfoTypeCOCE = {
//     ValueType: UpdateValueType._None,
//     Enabled: false,
//     Current: 0,
//     Original: 0,
//     Combined: 0,
//   };

//   return (
//     <>
//       <PanelBase
//         header={headerText!}
//         visible={visible}
//         height={100}
//         content={
//           <>
//             <PanelSection>
//               <PanelSectionRow
//                 uppercase={false}
//                 disableFocus={true}
//                 left={infoText}
//               />
//             </PanelSection>
//             {/* <Section
//               vType={bldgVehicleInfo.TransportType}
//               IsActive={bldgVehicleInfo.IsDepot}
//               valueType={UpdateValueType.TransportDepotData_VehicleCapacity}
//               COCE={bldgVehicleInfo.DepotVehicle}
//             />
//             <Section
//               vType="Garbage"
//               IsActive={bldgVehicleInfo.IsGarbageFacility}
//               valueType={UpdateValueType.GarbageFacilityData_VehicleCapacity}
//               COCE={bldgVehicleInfo.GarbageTruck}
//             />
//             <Section
//               vType="Ambulance"
//               IsActive={bldgVehicleInfo.IsHospital}
//               valueType={UpdateValueType.HospitalData_AmbulanceCapacity}
//               COCE={bldgVehicleInfo.Ambulance}
//             />
//             {bldgGeneralInfo.HasHeli && (
//               <Section
//                 vType="MediHeli"
//                 IsActive={bldgVehicleInfo.IsHospital}
//                 valueType={
//                   UpdateValueType.HospitalData_MedicalHelicopterCapacity
//                 }
//                 COCE={bldgVehicleInfo.MediHeli}
//               />
//             )}
//             <Section
//               vType="Hearse"
//               IsActive={bldgVehicleInfo.IsDeathcare}
//               valueType={UpdateValueType.DeathcareFacilityData_HearseCapacity}
//               COCE={bldgVehicleInfo.Hearse}
//             />
//             <Section
//               vType="PatrolCar"
//               IsActive={bldgVehicleInfo.IsPoliceStation}
//               valueType={UpdateValueType.PoliceStationData_PatrolCarCapacity}
//               COCE={bldgVehicleInfo.PatrolCar}
//             />
//             {bldgGeneralInfo.HasHeli && (
//               <Section
//                 vType="PoliceHeli"
//                 IsActive={bldgVehicleInfo.IsPoliceStation}
//                 valueType={
//                   UpdateValueType.PoliceStationData_PoliceHelicopterCapacity
//                 }
//                 COCE={bldgVehicleInfo.PoliceHeli}
//               />
//             )}
//             <Section
//               vType="PrisonVan"
//               IsActive={bldgVehicleInfo.IsPrison}
//               valueType={UpdateValueType.PrisonData_PrisonVanCapacity}
//               COCE={bldgVehicleInfo.PrisonVan}
//             />
//             <Section
//               vType="FireTruck"
//               IsActive={bldgVehicleInfo.IsFireStation}
//               valueType={UpdateValueType.FireStationData_FireEngineCapacity}
//               COCE={bldgVehicleInfo.FireTruck}
//             />
//             {bldgGeneralInfo.HasHeli && (
//               <Section
//                 vType="FireHeli"
//                 IsActive={bldgVehicleInfo.IsFireStation}
//                 valueType={
//                   UpdateValueType.FireStationData_FireHelicopterCapacity
//                 }
//                 COCE={bldgVehicleInfo.FireHeli}
//               />
//             )}
//             <Section
//               vType="EvacBus"
//               IsActive={bldgVehicleInfo.IsEmergencyShelter}
//               valueType={UpdateValueType.EmergencyShelterData_VehicleCapacity}
//               COCE={bldgVehicleInfo.EvacBus}
//             />
//             <Section
//               vType="PostVan"
//               IsActive={bldgVehicleInfo.IsPostFacility}
//               valueType={UpdateValueType.PostFacilityData_PostVanCapacity}
//               COCE={bldgVehicleInfo.PostVan}
//             />
//             <Section
//               vType="PostTruck"
//               IsActive={bldgVehicleInfo.IsPostFacility}
//               valueType={
//                 UpdateValueType.PoliceStationData_PoliceHelicopterCapacity
//               }
//               COCE={bldgVehicleInfo.PostTruck}
//             /> */}
//             <Section
//               vType="MaintenanceVehicle"
//               IsActive={bldgComponentInfo.MaintenanceDepotData}
//               valueType={UpdateValueType.MaintenanceDepotData_VehicleCapacity}
//               Vehicle={
//                 props.bldgCOCEInfo.find(
//                   x =>
//                     x.ValueType ==
//                     UpdateValueType.MaintenanceDepotData_VehicleCapacity,
//                 ) ?? defaultInfo
//               }
//             />
//           </>
//         }
//       />
//     </>
//   );
// };
