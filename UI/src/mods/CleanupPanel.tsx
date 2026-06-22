import { ChangeBCTValue, TriggerBCT } from "bindings/backend";
import { cleanupPanelVisibleBinding } from "bindings/frontend";
import { cleanupIcon } from "bindings/icons";
import { useValue } from "cs2/api";
import { FOCUS_DISABLED } from "cs2/input";
import { PanelSection, PanelSectionRow, Scrollable } from "cs2/ui";
import { NumberInputSnippet } from "partials/InputSnippets";
import { FC } from "react";
import { FindTranslation, nicifyVariableName } from "shared/lang";
import { PanelBase } from "shared/PanelBase";
import { styleSIP } from "shared/style";
import { Divider, InfoButton, ToolButton } from "shared/vanilla";
import commonStyles from "styles/common.module.scss";
import {
  BldgCleanupInfo,
  BldgCleanupType,
  BldgCleanupTypeInfo,
  CleanupValueType,
  GetCleanupMetadata,
} from "types/BldgCleanupInfo";

interface CleanupPanelProps {
  bldgCleanupInfo: BldgCleanupInfo;
}

const ZeroButtonSnippet = ({
  valueType,
  isDisabled = false,
}: {
  valueType: BldgCleanupType;
  isDisabled?: boolean;
}) => {
  const tooltip = FindTranslation("Cleanup.SetToZero");

  return (
    <>
      <div
        className={`${false ? "" : commonStyles.MarginLeft3r} ${commonStyles.AlignCenter}`}
      >
        <ToolButton
          id={`starq-abc-resetPanel-${valueType}-reset`}
          focusKey={FOCUS_DISABLED}
          disabled={isDisabled}
          tooltip={tooltip}
          src={cleanupIcon}
          onSelect={() => {
            ChangeBCTValue(0, valueType);
          }}
        />
      </div>
    </>
  );
};

const Section = ({
  base,
  resetData,
}: {
  base: string;
  resetData: BldgCleanupTypeInfo;
}) => {
  const [valueType, unit] = GetCleanupMetadata(resetData.CleanupType);

  const title =
    unit == ""
      ? nicifyVariableName(base)
      : `${nicifyVariableName(base)} (${unit})`;

  if (valueType === CleanupValueType.Number)
    return (
      <PanelSectionRow
        className={commonStyles.NoMarginVertical}
        disableFocus={true}
        left={title}
        right={
          <>
            <NumberInputSnippet
              value={resetData.CurrentValueNumber}
              onCommit={newValue =>
                ChangeBCTValue(newValue, resetData.CleanupType)
              }
            />
            <ZeroButtonSnippet
              valueType={resetData.CleanupType}
              isDisabled={resetData.CurrentValueNumber === 0}
            />
          </>
        }
      />
    );

  if (valueType === CleanupValueType.Trigger) {
    const buttonText = FindTranslation(`Cleanup.${base}`);

    return (
      <PanelSectionRow
        className={commonStyles.NoMargin}
        disableFocus={true}
        center={
          <InfoButton
            label={buttonText}
            // icon={resetIcon}
            onSelect={() => {
              TriggerBCT(resetData.CleanupType);
            }}
          ></InfoButton>
        }
      />
    );
  }

  return (
    <PanelSectionRow
      disableFocus={true}
      subRow={true}
      left={nicifyVariableName(base)}
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

export const CleanupPanel: FC<CleanupPanelProps> = (
  props: CleanupPanelProps,
) => {
  const visibleBindingValue = useValue(cleanupPanelVisibleBinding);

  const headerText = FindTranslation(`Cleanup.Header`);
  const infoText = FindTranslation(`Cleanup.AppliesOne`);
  const bldgCleanupInfo = props.bldgCleanupInfo;

  const dict = bldgCleanupInfo.Array;
  return (
    <>
      <PanelBase
        id="abc-cleanup"
        title={headerText!}
        visible={visibleBindingValue}
        icon={cleanupIcon}
        sipAligned={true}
        content={
          <>
            <Scrollable
              smooth={true}
              vertical={true}
              trackVisibility={"scrollable"}
              className={styleSIP.scrollable}
              style={{ maxHeight: "60vh" }}
            >
              <PanelSection>
                {Object.entries(dict).map(([objBase, objChildren]) => {
                  const isActive = objChildren.Enabled;

                  if (!isActive) return null;
                  const baseName = BldgCleanupType[objChildren.CleanupType];

                  return <Section base={baseName} resetData={objChildren} />;
                })}
              </PanelSection>
            </Scrollable>
            <Divider noMargin={1} />
            <PanelSection>
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
