import { LocalizedNumber, Unit } from "cs2/l10n";
import { useEffect, useState } from "react";
import {
  infoRowModule,
  sipTextInputModule,
  textElipsisInputModule,
  textElipsisInputThemeModule,
} from "styleBindings";

import styles from "../mods/style.module.scss";

export const NumberInputSnippet = ({
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
