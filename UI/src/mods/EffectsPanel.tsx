import {
  RevertCityEffect,
  RevertLocalEffect,
  SetCityEffect,
  SetCityEnabled,
  SetEffectOverlayColor,
  SetLocalEffect,
  SetLocalEnabled,
  SetRadiusCommon,
  ShowEffectOverlay,
} from "bindings/backend";
import { ClosePanel, effectsPanelVisibleBinding } from "bindings/frontend";
import { useValue } from "cs2/api";
import { Color } from "cs2/bindings";
import { FOCUS_AUTO, FOCUS_DISABLED } from "cs2/input";
import { Unit } from "cs2/l10n";
import { PanelFoldout, PanelSection, PanelSectionRow } from "cs2/ui";
import { FC, useEffect, useState } from "react";
import { LocalModifierType } from "shared/enums/LocalModifierType";
import { CityModifierType } from "shared/enums/CityModifierType";
import { ModifierRadiusCombineMode } from "shared/enums/ModifierRadiusCombineMode";
import { ModifierValueMode } from "shared/enums/ModifierValueMode";
import { maxIcon, plusIcon, resetIcon } from "shared/icons";
import { waterSourcePlacementIcon } from "shared/icons_uil";
import { NumberInputSnippet, Switch } from "shared/InputSnippets";
import { FindTranslation, formatNumber } from "shared/lang";
import { ButtonGroup } from "shared/LayoutSnippets";
import { PanelBase } from "shared/PanelBase";
import { GetSectionOpen, SetSectionOpen } from "shared/section";
import { ColorField, Divider, ToolButton } from "shared/vanilla";
import commonStyle from "styles/common.module.scss";
import effectStyle from "./EffectsPanel.module.scss";
import {
  BldgEffectInfo,
  BldgEffectTypeInfoCity,
  BldgEffectTypeInfoLocal,
} from "types/BldgEffectsInfo";
import {
  effectIcon,
  modifierAbsoluteIcon,
  modifierInverseIcon,
  modifierRelativeIcon,
} from "bindings/icons";
import { EffectOptions } from "types/EffectOptions";

interface EffectsPanelProps {
  bldgEffectInfo: BldgEffectInfo;
}

let resetTooltip = "";
let localEffectText = "";
let cityEffectText = "";

let currentRadius = false;
let persistedRadiusType: number | null = null;

const localModis = Object.values(LocalModifierType).filter(
  (v): v is LocalModifierType => typeof v === "number",
);
const cityModis = Object.values(CityModifierType).filter(
  (v): v is CityModifierType => typeof v === "number",
);

export const EffectsPanel: FC<EffectsPanelProps> = (
  props: EffectsPanelProps,
) => {
  const visibleBindingValue = useValue(effectsPanelVisibleBinding);

  const headerText = FindTranslation(`Effects.Header`);
  const infoText = FindTranslation(`AppliesAll`);
  resetTooltip = FindTranslation(`Common.ACTION[Reset Property]`, true);
  localEffectText = FindTranslation(
    `Common.HAPPINESS_FACTOR_POSITIVE[LocalBonuses]`,
    true,
  );
  cityEffectText = FindTranslation(
    `SelectedInfoPanel.EFFICIENCY_FACTORS[CityModifierSoftware]`,
    true,
  );
  const radiusColorText = FindTranslation(`Effects.RadiusColor`);
  const radiusCommonText = FindTranslation(`Effects.RadiusCommon`);

  const bldgEffectInfo = props.bldgEffectInfo;
  const [overlayColor, setOverlayColor] = useState<Color>(bldgEffectInfo.Color);
  const [radiusCommon, setRadiusCommon] = useState<boolean>(currentRadius);
  const [selected, setSelected] = useState("local");

  const localCount = bldgEffectInfo.LocalModifiers.filter(
    m => m.Enabled,
  ).length;
  const cityCount = bldgEffectInfo.CityModifiers.filter(m => m.Enabled).length;

  const localCountText = localCount > 0 ? ` (${localCount})` : "";
  const cityCountText = cityCount > 0 ? ` (${cityCount})` : "";

  return (
    <>
      <PanelBase
        id="abc-effects"
        title={headerText!}
        visible={visibleBindingValue}
        icon={effectIcon}
        sipAligned={true}
        onClose={() => ClosePanel()}
        selectedTab={selected}
        onTabChange={setSelected}
        tabBarClassName={`${effectStyle.TabBar} ${commonStyle.BackgroundDark}`}
        tabs={[
          {
            id: "local",
            label: `${localEffectText}${localCountText}`,
            content: (
              <BuildLocalEntries effects={bldgEffectInfo.LocalModifiers} />
            ),
          },
          {
            id: "city",
            label: `${cityEffectText}${cityCountText}`,
            content: (
              <BuildCityEntries effects={bldgEffectInfo.CityModifiers} />
            ),
          },
        ]}
        footer={
          <>
            <Divider noMargin={1} />
            <PanelSection>
              {selected === "local" && (
                <>
                  <PanelSectionRow
                    uppercase={false}
                    disableFocus={true}
                    left={radiusColorText}
                    right={
                      <ColorField
                        focusKey={FOCUS_DISABLED}
                        value={overlayColor}
                        alpha={true}
                        hexInput={true}
                        onChange={e => {
                          setOverlayColor(e);
                          SetEffectOverlayColor(e);
                        }}
                      />
                    }
                  />
                  <PanelSectionRow
                    uppercase={false}
                    disableFocus={true}
                    left={radiusCommonText}
                    right={
                      <Switch
                        onChange={next => {
                          setRadiusCommon(next);
                          currentRadius = next;
                          SetRadiusCommon(next);
                        }}
                        checked={radiusCommon}
                      />
                    }
                  />
                </>
              )}
              <PanelSectionRow
                uppercase={false}
                disableFocus={true}
                left={infoText}
              />
            </PanelSection>
          </>
        }
      />
    </>
  );
};

const BuildLocalEntries = ({
  effects,
}: {
  effects: BldgEffectTypeInfoLocal[];
}) => {
  const [radiusType, setRadiusTypeState] = useState<number | null>(
    persistedRadiusType,
  );
  const setRadiusType = (v: number | null) => {
    persistedRadiusType = v;
    setRadiusTypeState(v);
  };
  let index = -1;
  const byType = new Map(effects.map(m => [m.Type, m]));

  useEffect(() => {
    const active = radiusType != null ? byType.get(radiusType) : undefined;
    const on = !!active?.Enabled;
    ShowEffectOverlay({
      Local: on ? 1 : -1,
      EffectType: on ? radiusType! : -1,
      Radius: on ? active!.Radius : 0,
      Enabled: on,
    });
  });

  useEffect(
    () => () => {
      ShowEffectOverlay({
        Local: -1,
        EffectType: -1,
        Radius: 0,
        Enabled: false,
      });
    },
    [],
  );

  const update = (
    modifier: BldgEffectTypeInfoLocal,
    changes: Partial<BldgEffectTypeInfoLocal>,
  ) => SetLocalEffect({ ...modifier, ...changes });

  return (
    <>
      {localModis.map(type => {
        const modifier = byType.get(type);
        const baseName = LocalModifierType[type];
        const title = FindTranslation(
          `Properties.LOCAL_MODIFIER[${baseName}]`,
          true,
        );
        const sectionKey = `local${index}${baseName}`;
        index++;

        if (modifier == null) {
          return (
            <PanelFoldout
              key={`${sectionKey}-${GetSectionOpen(sectionKey, false)}`}
              initialExpanded={GetSectionOpen(sectionKey, false)}
              onToggleExpanded={v => SetSectionOpen(sectionKey, v)}
              expandFromContent={true}
              focusKey={FOCUS_AUTO}
              header={
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={
                    <>
                      <Switch
                        style={{ marginRight: "5rem" }}
                        checked={false}
                        onClick={e => e.stopPropagation()}
                        onChange={next => {
                          SetLocalEnabled(type, next);
                          SetSectionOpen(sectionKey, next);
                        }}
                      />
                      {title}
                    </>
                  }
                  // right={<>{inactiveText}</>}
                />
              }
            ></PanelFoldout>
          );
        }

        modifier.Delta = parseFloat(modifier.Delta.toFixed(5));

        return (
          <PanelFoldout
            key={`${sectionKey}-${GetSectionOpen(sectionKey, modifier.Enabled)}`}
            initialExpanded={GetSectionOpen(sectionKey, modifier.Enabled)}
            onToggleExpanded={v => SetSectionOpen(sectionKey, v)}
            expandFromContent={true}
            focusKey={FOCUS_AUTO}
            header={
              <PanelSectionRow
                uppercase={true}
                disableFocus={true}
                left={
                  <>
                    <Switch
                      style={{ marginRight: "5rem" }}
                      checked={modifier.Enabled}
                      onClick={e => e.stopPropagation()}
                      onChange={next => {
                        SetLocalEnabled(modifier.Type, next);
                        SetSectionOpen(sectionKey, next);
                      }}
                    />
                    {title}
                  </>
                }
                right={
                  <>
                    <ToolButton
                      id={`starq-abc-effect-${sectionKey}-radius`}
                      focusKey={FOCUS_DISABLED}
                      tooltip={"Show Radius"}
                      disabled={!modifier.Enabled}
                      selected={radiusType == modifier.Type}
                      src={waterSourcePlacementIcon}
                      onSelect={() => {
                        const turnOn = radiusType !== modifier.Type;
                        setRadiusType(turnOn ? modifier.Type : null);
                        // const options: EffectOptions = {
                        //   Local: 1,
                        //   EffectType: modifier.Type,
                        //   Radius: modifier.Radius,
                        //   Enabled: turnOn,
                        // };
                        // ShowEffectOverlay(options);
                      }}
                    />
                    <ToolButton
                      id={`starq-abc-effect-${sectionKey}-reset`}
                      focusKey={FOCUS_DISABLED}
                      tooltip={resetTooltip}
                      disabled={!modifier.Customized}
                      src={resetIcon}
                      onSelect={() => {
                        RevertLocalEffect(modifier.Type);
                      }}
                    />
                  </>
                }
              />
            }
          >
            <PanelSectionRow
              key={`starq-abc-effect-${sectionKey}-line1`}
              className={commonStyle.NoMarginVertical}
              disableFocus={true}
              left={"Radius"}
              right={
                <>
                  <NumberInputSnippet
                    key={`starq-abc-effect-${sectionKey}-radius`}
                    value={formatNumber(modifier.Radius)}
                    unit={Unit.Percentage}
                    integer={true}
                    min={1}
                    step={10}
                    onCommit={newValue => {
                      update(modifier, { Radius: newValue });
                      // if (radiusType === modifier.Type) {
                      //   const options: EffectOptions = {
                      //     Local: 1,
                      //     EffectType: modifier.Type,
                      //     Radius: newValue,
                      //     Enabled: true,
                      //   };
                      //   ShowEffectOverlay(options);
                      // }
                    }}
                  />
                  <ButtonGroup
                    width={"90rem"}
                    content={
                      <>
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-maximal`}
                          focusKey={FOCUS_DISABLED}
                          selected={
                            modifier.Combine ==
                            ModifierRadiusCombineMode.Maximal
                          }
                          tooltip={"Maximal"}
                          src={maxIcon}
                          onSelect={() => {
                            update(modifier, {
                              Combine: ModifierRadiusCombineMode.Maximal,
                            });
                          }}
                        />
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-additive`}
                          focusKey={FOCUS_DISABLED}
                          selected={
                            modifier.Combine ==
                            ModifierRadiusCombineMode.Additive
                          }
                          tooltip={"Additive"}
                          src={plusIcon}
                          className={commonStyle.ToolWhite}
                          onSelect={() => {
                            update(modifier, {
                              Combine: ModifierRadiusCombineMode.Additive,
                            });
                          }}
                        />
                      </>
                    }
                  />
                </>
              }
            />
            <PanelSectionRow
              key={`starq-abc-effect-${sectionKey}-line1`}
              className={commonStyle.NoMarginVertical}
              disableFocus={true}
              left={"Delta"}
              right={
                <>
                  <NumberInputSnippet
                    key={`starq-abc-effect-${sectionKey}-delta`}
                    value={formatNumber(modifier.Delta)}
                    unit={Unit.Length}
                    onCommit={newValue => update(modifier, { Delta: newValue })}
                  />
                  <ButtonGroup
                    width={"90rem"}
                    content={
                      <>
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-relative`}
                          focusKey={FOCUS_DISABLED}
                          selected={modifier.Mode == ModifierValueMode.Relative}
                          tooltip={"Relative"}
                          src={modifierRelativeIcon}
                          onSelect={() =>
                            update(modifier, {
                              Mode: ModifierValueMode.Relative,
                            })
                          }
                        />
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-absolute`}
                          focusKey={FOCUS_DISABLED}
                          selected={modifier.Mode == ModifierValueMode.Absolute}
                          tooltip={"Absolute"}
                          src={modifierAbsoluteIcon}
                          onSelect={() => {
                            update(modifier, {
                              Mode: ModifierValueMode.Absolute,
                            });
                          }}
                        />
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-inverse`}
                          focusKey={FOCUS_DISABLED}
                          selected={
                            modifier.Mode == ModifierValueMode.InverseRelative
                          }
                          tooltip={"Inverse Relative"}
                          src={modifierInverseIcon}
                          onSelect={() => {
                            update(modifier, {
                              Mode: ModifierValueMode.InverseRelative,
                            });
                          }}
                        />
                      </>
                    }
                  />
                </>
              }
            />
          </PanelFoldout>
        );
      })}
    </>
  );
};

const BuildCityEntries = ({
  effects,
}: {
  effects: BldgEffectTypeInfoCity[];
}) => {
  let index = -1;
  const byType = new Map(effects.map(m => [m.Type, m]));

  const update = (
    modifier: BldgEffectTypeInfoCity,
    changes: Partial<BldgEffectTypeInfoCity>,
  ) => SetCityEffect({ ...modifier, ...changes });

  const ignore = [
    CityModifierType.PoliceStationUpkeep,
    CityModifierType.TaxiStartingFee,
  ];

  return (
    <>
      {cityModis.map(type => {
        const modifier = byType.get(type);
        if (ignore.includes(type)) return null;
        const baseName = CityModifierType[type];
        const title = FindTranslation(
          `Properties.CITY_MODIFIER[${baseName}]`,
          true,
        );
        const sectionKey = `city${index}${baseName}`;
        index++;

        if (modifier == null) {
          return (
            <PanelFoldout
              key={`${sectionKey}-${GetSectionOpen(sectionKey, false)}`}
              initialExpanded={GetSectionOpen(sectionKey, false)}
              onToggleExpanded={v => SetSectionOpen(sectionKey, v)}
              expandFromContent={true}
              focusKey={FOCUS_AUTO}
              header={
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={
                    <>
                      <Switch
                        style={{ marginRight: "5rem" }}
                        checked={false}
                        onClick={e => e.stopPropagation()}
                        onChange={next => {
                          SetCityEnabled(type, next);
                          SetSectionOpen(sectionKey, next);
                        }}
                      />
                      {title}
                    </>
                  }
                  // right={<>{inactiveText}</>}
                />
              }
            ></PanelFoldout>
          );
        }

        modifier.Range = parseFloat(modifier.Range.toFixed(5));

        return (
          <PanelFoldout
            key={`${sectionKey}-${GetSectionOpen(sectionKey, modifier.Enabled)}`}
            initialExpanded={GetSectionOpen(sectionKey, modifier.Enabled)}
            onToggleExpanded={v => SetSectionOpen(sectionKey, v)}
            expandFromContent={true}
            focusKey={FOCUS_AUTO}
            header={
              <PanelSectionRow
                uppercase={true}
                disableFocus={true}
                left={
                  <>
                    <Switch
                      style={{ marginRight: "5rem" }}
                      checked={modifier.Enabled}
                      onClick={e => e.stopPropagation()}
                      onChange={next => {
                        SetCityEnabled(modifier.Type, next);
                        SetSectionOpen(sectionKey, next);
                      }}
                    />
                    {title}
                  </>
                }
                right={
                  <>
                    <ToolButton
                      id={`starq-abc-effect-${sectionKey}-reset`}
                      focusKey={FOCUS_DISABLED}
                      tooltip={resetTooltip}
                      disabled={!modifier.Customized}
                      src={resetIcon}
                      onSelect={() => {
                        RevertCityEffect(modifier.Type);
                      }}
                    />
                  </>
                }
              />
            }
          >
            <PanelSectionRow
              key={`starq-abc-effect-${sectionKey}-line1`}
              className={commonStyle.NoMarginVertical}
              disableFocus={true}
              left={"Range"}
              right={
                <>
                  <NumberInputSnippet
                    key={`starq-abc-effect-${sectionKey}-line1`}
                    value={formatNumber(modifier.Range)}
                    unit={Unit.Percentage}
                    // integer={true}
                    step={
                      modifier.Mode == ModifierValueMode.Relative ? 0.1 : 10
                    }
                    onCommit={newValue => {
                      update(modifier, { Range: newValue });
                    }}
                  />
                  <ButtonGroup
                    width={"90rem"}
                    content={
                      <>
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-relative`}
                          focusKey={FOCUS_DISABLED}
                          selected={modifier.Mode == ModifierValueMode.Relative}
                          tooltip={"Relative"}
                          src={modifierRelativeIcon}
                          onSelect={() =>
                            update(modifier, {
                              Mode: ModifierValueMode.Relative,
                            })
                          }
                        />
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-absolute`}
                          focusKey={FOCUS_DISABLED}
                          selected={modifier.Mode == ModifierValueMode.Absolute}
                          tooltip={"Absolute"}
                          src={modifierAbsoluteIcon}
                          onSelect={() => {
                            update(modifier, {
                              Mode: ModifierValueMode.Absolute,
                            });
                          }}
                        />
                        <ToolButton
                          id={`starq-abc-effect-${sectionKey}-inverse`}
                          focusKey={FOCUS_DISABLED}
                          selected={
                            modifier.Mode == ModifierValueMode.InverseRelative
                          }
                          tooltip={"Inverse Relative"}
                          src={modifierInverseIcon}
                          onSelect={() => {
                            update(modifier, {
                              Mode: ModifierValueMode.InverseRelative,
                            });
                          }}
                        />
                      </>
                    }
                  />
                </>
              }
            />
          </PanelFoldout>
        );
      })}
    </>
  );
};
