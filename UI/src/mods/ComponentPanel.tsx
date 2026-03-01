import {
    ChangeUVTValue, ChangeUVTValueString, CheckBox, componentPanelVisibleBinding, Divider,
    ResetUVTValue, ToolButton
} from "bindings";
import { useValue } from "cs2/api";
import { Number2 } from "cs2/bindings";
import { FOCUS_AUTO, FOCUS_DISABLED, FocusDisabled } from "cs2/input";
import { LocalizedNumber, Unit } from "cs2/l10n";
import {
    Button, Dropdown, DropdownItem, DropdownToggle, PanelFoldout, PanelSection, PanelSectionRow,
    Scrollable
} from "cs2/ui";
import { hasFlag, toggleFlag } from "functions/enum";
import {
    FindTranslation, GetAlternateDropdownText, GetComponentTooltip, nicifyVariableName
} from "functions/lang";
import { GetFlags, isMultiSelect as isSingleSelect } from "functions/uvt";
import { FC, useEffect, useMemo, useState } from "react";
import {
    baseGameIcons, dropdownModule, infoRowModule, sipTextInputModule, styleLevelProgress,
    styleLevelSection, styleProgress, styleSIP, textElipsisInputModule, textElipsisInputThemeModule,
    uilStandard
} from "styleBindings";
import { BldgComponentInfo } from "types/BldgComponentInfo";
import { BldgModifiedInfo } from "types/BldgModifiedInfo";
import { GetSectionOpen, SetSectionOpen } from "types/SectionControl";
import { UpdateValueType } from "types/UpdateValueType";

import { PanelBase } from "./PanelBase";
import styles from "./style.module.scss";

interface ComponentPanelProps {
  bldgComponentInfo: BldgComponentInfo;
  bldgModifiedInfo: BldgModifiedInfo[];
}

const DropDownItemSnippet = ({
  current,
  selectedVal,
  valueType,
  setVal,
}: {
  current: number;
  selectedVal: number;
  valueType: UpdateValueType;
  setVal: (e: number) => void;
}) => {
  return (
    <>
      <DropdownItem
        key={`abc-${valueType}-${current}`}
        value={current}
        focusKey={FOCUS_DISABLED}
        selected={current === selectedVal}
        closeOnSelect={false}
        theme={dropdownModule}
        onChange={e => {
          setVal(e);
          ChangeUVTValue(e, valueType);
        }}
        className={styles.DropdownItemCustomRight}
      >
        {GetAlternateDropdownText(current, valueType)}
      </DropdownItem>
    </>
  );
};

const MultiSelectDropdownItemSnippet = ({
  current,
  selectedFlags,
  valueType,
  setFlags,
}: {
  current: number;
  selectedFlags: number;
  valueType: UpdateValueType;
  setFlags: (v: number) => void;
}) => {
  const isSelected = hasFlag(selectedFlags, current);
  const valFunc = () => {
    const newValue = toggleFlag(selectedFlags, current);
    setFlags(newValue);
    ChangeUVTValue(newValue, valueType);
  };

  return (
    <DropdownItem
      key={`abc-${valueType}-${current}`}
      value={current}
      focusKey={FOCUS_DISABLED}
      selected={isSelected}
      closeOnSelect={false}
      theme={dropdownModule}
      onChange={valFunc}
      onToggleSelected={valFunc}
      className={`${styles.DropdownItemCustomRight} ${styles.DropdownLabelWithCheckbox}`}
    >
      <CheckBox
        checked={isSelected}
        focusKey={FOCUS_DISABLED}
        className={`${styles.CheckboxCustom}`}
      />
      {GetAlternateDropdownText(current, valueType)}
    </DropdownItem>
  );
};

const Switch = ({
  checked = false,
  onChange,
  disabled = false,
}: {
  checked?: boolean;
  onChange?: (value: boolean) => void;
  disabled?: boolean;
}) => {
  const toggle = () => {
    if (disabled) return;
    onChange?.(!checked);
  };

  return (
    <button
      type="button"
      className={`${styles.ToggleSwitchCustom} ${checked ? `${styles.on}` : ""} ${disabled ? `${styles.disabled}` : ""}`}
      onClick={toggle}
      aria-checked={checked}
      role="switch"
    >
      <span className={`${styles.ToggleSwitchCustomCircle}`} />
    </button>
  );
};

const NumberInputSnippet = ({
  value,
  inputPrefix,
  onCommit,
}: {
  value: number;
  inputPrefix?: string;
  onCommit: (newValue: number) => void;
}) => {
  const [editValue, setEditValue] = useState<string>(`${value}`);

  useEffect(() => {
    setEditValue(`${value}`);
  }, [value]);

  const applyIfChanged = () => {
    const parsed = Number.parseFloat(editValue);

    if (!Number.isNaN(parsed) && Math.abs(parsed - value) > 1e-6) {
      if (Number.isFinite(parsed)) onCommit(parsed);
    }
  };
  return (
    <>
      <div style={{ width: "100rem" }}>
        <div className={textElipsisInputThemeModule.wrapper}>
          <div
            className={`${textElipsisInputModule.container} ${sipTextInputModule.container} ${styles.NoMarginSide} ${styles.InputBoxCustom}`}
            style={{ height: "32rem" }}
          >
            <input
              className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
              maxLength={10}
              type="text"
              value={editValue}
              style={{ textAlign: "right" }}
              onChange={e => setEditValue(e.currentTarget.value)}
              onKeyDown={e => {
                if (e.key === "Enter") {
                  e.currentTarget.blur();
                }
              }}
              onBlur={applyIfChanged}
            />
            <div
              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label} ${styles.InputBoxCustom}`}
            >
              {inputPrefix}{" "}
              {
                <LocalizedNumber
                  value={value}
                  unit={Unit.FloatThreeFractions}
                />
              }
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

const Int2Editor: FC<{
  value: Number2;
  valueType: UpdateValueType;
  field: string;
  tooltip: string;
  isCustom: boolean;
  originalText: string;
}> = ({ value, valueType, field, tooltip, isCustom, originalText }) => {
  const [value2, setValue2] = useState(value);

  useEffect(() => setValue2(value), [value]);

  const commit = (next: Number2) => {
    setValue2(next);
    ChangeUVTValueString(`${next.x},${next.y}`, valueType);
  };

  return (
    <PanelSectionRow
      disableFocus
      subRow
      tooltip={tooltip}
      left={nicifyVariableName(field)}
      right={
        <>
          <NumberInputSnippet
            value={value2.x}
            onCommit={x => commit({ x, y: value2.y })}
            inputPrefix={FindTranslation("PhotoMode.PROPERTY_LIMIT_MIN", true)}
          />
          <NumberInputSnippet
            value={value2.y}
            onCommit={y => commit({ x: value2.x, y })}
            inputPrefix={FindTranslation("PhotoMode.PROPERTY_LIMIT_MAX", true)}
          />
          <ResetButtonSnippet
            valueType={valueType}
            isCustom={isCustom}
            originalText={originalText}
          />
        </>
      }
    />
  );
};

const ResetButtonSnippet = ({
  valueType,
  isCustom,
  originalText = "",
}: {
  valueType: UpdateValueType;
  isCustom: boolean;
  originalText?: string;
}) => {
  const [isDisabled, setDisabled] = useState<boolean>(!isCustom);

  useEffect(() => {
    setDisabled(!isCustom);
  }, [isCustom]);

  const tooltip = `${FindTranslation("Common.ACTION[Reset Property]", true)}${originalText}`;

  return (
    <>
      <div
        className={`${false ? "" : styles.MarginLeft3r} ${styles.AlignCenter}`}
      >
        <ToolButton
          id={`starq-abc-comp-${valueType}-reset`}
          focusKey={FOCUS_DISABLED}
          disabled={isDisabled}
          tooltip={tooltip}
          src={baseGameIcons + "NewUI/Reset_Button.svg"}
          onSelect={() => {
            ResetUVTValue(valueType);
          }}
        />
      </div>
    </>
  );
};

const Section = ({
  sectionKey,
  base,
  field,
  active,
  value,
  valueType,
  isCustom,
  original,
}: {
  sectionKey: string;
  base: string;
  field: string;
  active: boolean;
  value: any;
  valueType: UpdateValueType;
  isCustom: boolean;
  original: string;
}) => {
  const originalText = original == "" ? "" : ` (Original: ${original})`;
  const tooltip = `${GetComponentTooltip(valueType)}`;
  if (valueType == UpdateValueType.SpawnableBuildingData_Level)
    return (
      <>
        <PanelSectionRow
          className={styles.NoMarginVertical}
          disableFocus={true}
          subRow={true}
          tooltip={tooltip}
          left={nicifyVariableName(field)}
          right={
            <FocusDisabled>
              <div className={`${styleLevelSection.bar} ${styles.MarginRight}`}>
                {Array.from({ length: 5 }, (_, i) => i + 1).map(i => {
                  let isPassed = i <= value;

                  return (
                    <Button
                      key={i}
                      focusKey={FOCUS_AUTO}
                      onSelect={() =>
                        ChangeUVTValue(
                          i,
                          UpdateValueType.SpawnableBuildingData_Level,
                        )
                      }
                      className={`${styleLevelProgress.progressBar} ${styleLevelSection.levelSegment} ${styles.LevelSegment}`}
                    >
                      <div
                        className={styleProgress.progressBounds}
                        style={{
                          width: "100%",
                          backdropFilter: `brightness(${i / 6})`,
                        }}
                      >
                        <div
                          className={`${isPassed ? styleLevelProgress.progress : ""} ${styles.LevelBox}`}
                          style={{
                            color: isPassed ? undefined : "whitesmoke",
                          }}
                        >
                          {i}
                        </div>
                      </div>
                    </Button>
                  );
                })}
              </div>
              <ResetButtonSnippet
                valueType={valueType}
                isCustom={isCustom}
                originalText={originalText}
              />
            </FocusDisabled>
          }
        />
      </>
    );

  const [flags, haveFlags] = GetFlags(valueType) as [number[], boolean];

  if (haveFlags) {
    const singleSelect = isSingleSelect(valueType);

    const val = value;
    const setVal = (v: number) => ChangeUVTValue(v, valueType);

    if (singleSelect) {
      return (
        <>
          <PanelSectionRow
            className={styles.NoMarginVertical}
            disableFocus={true}
            subRow={true}
            tooltip={tooltip}
            left={nicifyVariableName(field)}
            right={
              <>
                <div style={{ width: "150rem" }}>
                  <Dropdown
                    focusKey={FOCUS_DISABLED}
                    theme={dropdownModule}
                    content={flags.map(flag => (
                      <DropDownItemSnippet
                        current={flag}
                        selectedVal={val}
                        setVal={setVal}
                        valueType={valueType}
                      />
                    ))}
                  >
                    <DropdownToggle
                      className={`${styles.DropdownToggleCustom}`}
                      theme={dropdownModule}
                    >
                      {GetAlternateDropdownText(val, valueType)}
                    </DropdownToggle>
                  </Dropdown>
                </div>
                <ResetButtonSnippet
                  valueType={valueType}
                  isCustom={isCustom}
                  originalText={originalText}
                />
              </>
            }
          />
        </>
      );
    }

    const flagLabels = useMemo(() => {
      return flags.map(flag => ({
        flag,
        label: GetAlternateDropdownText(flag, valueType),
      }));
    }, [flags, valueType]);
    return (
      <>
        <PanelSectionRow
          className={styles.NoMarginVertical}
          disableFocus={true}
          subRow={true}
          tooltip={tooltip}
          left={nicifyVariableName(field)}
          right={
            <>
              <div style={{ width: "200rem" }}>
                <Dropdown
                  focusKey={FOCUS_DISABLED}
                  theme={dropdownModule}
                  content={flags.map(flag => (
                    <MultiSelectDropdownItemSnippet
                      key={flag}
                      current={flag}
                      selectedFlags={val}
                      setFlags={setVal}
                      valueType={valueType}
                    />
                  ))}
                >
                  <DropdownToggle
                    className={`${styles.DropdownToggleCustom}`}
                    theme={dropdownModule}
                  >
                    {flagLabels
                      .filter(x => hasFlag(val, x.flag))
                      .map(x => x.label)
                      .join(", ") || "None"}
                  </DropdownToggle>
                </Dropdown>
              </div>
              <ResetButtonSnippet
                valueType={valueType}
                isCustom={isCustom}
                originalText={originalText}
              />
            </>
          }
        />
      </>
    );
  }

  if (typeof value === "number" && Number.isFinite(value))
    return (
      <PanelSectionRow
        className={styles.NoMarginVertical}
        disableFocus={true}
        subRow={true}
        tooltip={tooltip}
        left={nicifyVariableName(field)}
        right={
          <>
            <NumberInputSnippet
              value={value}
              onCommit={newValue => ChangeUVTValue(newValue, valueType)}
            />
            <ResetButtonSnippet
              valueType={valueType}
              isCustom={isCustom}
              originalText={originalText}
            />
          </>
        }
      />
    );

  if (typeof value === "boolean") {
    return (
      <PanelSectionRow
        className={styles.NoMarginVertical}
        disableFocus={true}
        subRow={true}
        tooltip={tooltip}
        left={nicifyVariableName(field)}
        right={
          <>
            <Switch
              onChange={next => ChangeUVTValue(next ? 1 : 0, valueType)}
              checked={value}
            />
            <ResetButtonSnippet
              valueType={valueType}
              isCustom={isCustom}
              originalText={originalText}
            />
          </>
        }
      />
    );
  }

  if (value["__Type"] == "Unity.Mathematics.int2") {
    return (
      <Int2Editor
        value={value}
        valueType={valueType}
        field={field}
        tooltip={tooltip}
        isCustom={isCustom}
        originalText={originalText}
      />
    );
  }

  return (
    <PanelSectionRow
      disableFocus={true}
      subRow={true}
      tooltip={tooltip}
      left={nicifyVariableName(field)}
      right={
        <>
          {`Not yet implemented...`}
          {/* {`(${typeof value} = ${value}), (${valueType})`} */}
          {/* <ResetButtonSnippet valueType={valueType} /> */}
        </>
      }
    />
  );
};

type Entry = {
  key: string;
  base: string;
  valueTypeInt: UpdateValueType;
  field: string;
  isCustom: boolean;
  original: string;
};

export const ComponentPanel: FC<ComponentPanelProps> = (
  props: ComponentPanelProps,
) => {
  const visibleBindingValue = useValue(componentPanelVisibleBinding);

  const headerText = FindTranslation(`Component.Header`);
  const infoText = FindTranslation(`Component.AppliesAll`);
  const bldgComponentInfo = props.bldgComponentInfo;
  const bldgModifiedInfo = props.bldgModifiedInfo;

  const isVanilla = bldgModifiedInfo.length == 0;

  const dict = Object.entries(UpdateValueType)
    .filter(
      ([_, v]) =>
        typeof v === "number" &&
        v !== UpdateValueType._None &&
        v !== UpdateValueType._All,
    )
    .reduce<Record<string, Entry[]>>((acc, [k, v]) => {
      const [base, field] = k.split("_");

      const isActive = Boolean((bldgComponentInfo as any)[base]);

      if (!isActive) return acc;

      const isCustom = (bldgModifiedInfo.find(mods => mods.ValueType == v) !=
        undefined) as boolean;

      const original =
        bldgModifiedInfo.find(mods => mods.ValueType == v)?.OriginalText || "";

      if (!acc[base]) {
        acc[base] = [];
      }

      acc[base].push({
        key: k,
        base: base,
        valueTypeInt: v as UpdateValueType,
        field,
        isCustom: isCustom,
        original: original,
      });
      return acc;
    }, {});

  const poweredBases = new Set([
    "GarbagePoweredData",
    "GroundWaterPoweredData",
    "SolarPoweredData",
    "WaterPoweredData",
    "WindPoweredData",
  ]);

  const hasPoweredVariant = Object.keys(dict).some(base =>
    poweredBases.has(base),
  );

  if (hasPoweredVariant) {
    delete dict.PowerPlantData;
  }

  return (
    <>
      <PanelBase
        header={headerText!}
        visible={visibleBindingValue}
        icon={uilStandard + "Tools.svg"}
        content={
          <>
            <Scrollable
              smooth={true}
              vertical={true}
              trackVisibility={"scrollable"}
              className={styleSIP.scrollable}
              style={{ maxHeight: "60vh" }}
            >
              {Object.entries(dict).map(([objBase, objChildren]) => {
                const isActive = Boolean((bldgComponentInfo as any)[objBase]);

                const baseX = objBase.replace("Data", "");

                if (!isActive) return null;

                return (
                  <PanelFoldout
                    header={
                      <PanelSectionRow
                        uppercase={true}
                        disableFocus={true}
                        left={nicifyVariableName(baseX)}
                      />
                    }
                    initialExpanded={GetSectionOpen(baseX) ?? false}
                    onToggleExpanded={v => SetSectionOpen(baseX, v)}
                    expandFromContent={true}
                    focusKey={FOCUS_AUTO}
                  >
                    {objChildren.map(c => (
                      <div
                        key={c.key}
                        style={{
                          display: "flex",
                          padding: "0",
                          height: "32rem",
                        }}
                      >
                        <Section
                          sectionKey={c.key}
                          base={baseX}
                          field={c.field}
                          active={
                            (bldgComponentInfo as any)[objBase] as boolean
                          }
                          isCustom={c.isCustom}
                          original={c.original}
                          value={(bldgComponentInfo as any)[c.key]}
                          valueType={c.valueTypeInt}
                        />
                      </div>
                    ))}
                  </PanelFoldout>
                );
              })}
            </Scrollable>
            <Divider noMargin={1} />
            <PanelSection>
              <PanelSectionRow
                uppercase={false}
                disableFocus={true}
                left={infoText}
                right={
                  <ResetButtonSnippet
                    valueType={UpdateValueType._All}
                    isCustom={!isVanilla}
                  />
                }
              />
            </PanelSection>
          </>
        }
      />
    </>
  );
};
