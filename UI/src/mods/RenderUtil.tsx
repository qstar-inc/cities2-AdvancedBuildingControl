import { ToolButton } from "bindings";
import { FOCUS_DISABLED } from "cs2/input";
import { LocalizedNumber, Unit } from "cs2/l10n";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { FC } from "react";
import {
  infoRowModule,
  sipTextInputModule,
  textElipsisInputModule,
  textElipsisInputThemeModule,
  uilStandard,
} from "styleBindings";
import { FindTranslation, InfoTypeCOCEWithData } from "types";

interface FormWithResetProps {
  id: string;
  currentVal: number;
  resetTooltip?: string;
  unit?: Unit;
  onChange: (value: number) => void;
  onReset: () => void;
}

export const FormWithReset: FC<FormWithResetProps> = ({
  id,
  currentVal,
  resetTooltip = FindTranslation(`ResetTooltip`),
  unit = Unit.Integer,
  onChange,
  onReset,
}) => {
  return (
    <>
      <div>
        <div className={textElipsisInputThemeModule.wrapper}>
          <div
            className={`${textElipsisInputModule.container} ${sipTextInputModule.container}`}
          >
            <input
              className={`${textElipsisInputModule.input} ${sipTextInputModule.input}`}
              maxLength={3}
              type="text"
              placeholder={`${(<LocalizedNumber value={currentVal} unit={unit} />)}`}
              onKeyDown={e => {
                if (e.key === "Enter") {
                  const value = Number.parseInt(e.currentTarget.value);
                  if (!Number.isNaN(value)) {
                    onChange(value);
                  }
                }
              }}
              onBlur={e => {
                const value = Number.parseInt(e.currentTarget.value);
                if (!Number.isNaN(value)) {
                  onChange(value);
                }
              }}
            />
            <div
              className={`${infoRowModule.right} ${textElipsisInputModule.label} ${sipTextInputModule.label}`}
            >
              <LocalizedNumber value={currentVal} />
            </div>
          </div>
        </div>
      </div>
      <ToolButton
        id={`starq-abc-${id}-reset`}
        focusKey={FOCUS_DISABLED}
        tooltip={resetTooltip}
        src={uilStandard + "Reset.svg"}
        onSelect={() => {
          onReset();
        }}
      />
    </>
  );
};

interface MultiSectionProps {
  infoLabel?: string;
  coceArray: InfoTypeCOCEWithData[];
}

export const MultiSection: FC<MultiSectionProps> = ({
  infoLabel = "",
  coceArray,
}) => {
  const OverrideToLabel = FindTranslation(`OverrideTo`);
  const BaseLabel = FindTranslation(`Base`);
  const BaseTooltip = FindTranslation(`BaseTooltip`);

  console.log(JSON.stringify(coceArray));

  return (
    <>
      {infoLabel != "" && (
        <PanelSection>
          <PanelSectionRow
            uppercase={false}
            disableFocus={true}
            left={infoLabel}
          />
        </PanelSection>
      )}
      {coceArray.map(coceItem => (
        <PanelSection>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={coceItem.title}
          ></PanelSectionRow>
          <PanelSectionRow
            disableFocus={true}
            left={OverrideToLabel}
            right={
              <FormWithReset
                id={coceItem.id}
                currentVal={coceItem.coce.Current}
                onChange={coceItem.onChange}
                onReset={coceItem.onReset}
              />
            }
          />
          {coceItem.coce.Enabled && (
            <PanelSectionRow
              tooltip={BaseTooltip!}
              left={BaseLabel}
              right={
                <LocalizedNumber
                  value={coceItem.coce.Original}
                  unit={Unit.Integer}
                />
              }
            />
          )}
        </PanelSection>
      ))}
    </>
  );
};
