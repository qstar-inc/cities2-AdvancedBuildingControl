import { ChangeLevelDistrict } from "bindings";
import { SelectedInfoSectionBase } from "cs2/bindings";
import { FOCUS_AUTO, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { Button, PanelSection, PanelSectionRow } from "cs2/ui";
import {
  styleLevelProgress,
  styleLevelSection,
  styleProgress,
} from "styleBindings";
import { LocaleKeys } from "types";

interface SIP_ABC_District extends SelectedInfoSectionBase {
  CurrentLevel: number;
}

export const SIP_ABC_District = (componentList: any): any => {
  componentList["AdvancedBuildingControl.Systems.SIP_ABC_District"] = (
    e: SIP_ABC_District
  ) => {
    const { translate } = useLocalization();

    const changeLevelText = translate(LocaleKeys.ZONING_CHANGE_LEVEL);

    const modNameText = translate(LocaleKeys.NAME) ?? "NAME";

    return (
      <>
        <PanelSection>
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={modNameText}
          />
          <PanelSectionRow
            uppercase={true}
            disableFocus={true}
            left={changeLevelText}
            right={
              <FocusDisabled>
                <div className={styleLevelSection.bar}>
                  {Array.from({ length: 6 }, (_, i) => {
                    let isPassed = i <= e.CurrentLevel;
                    if (i !== 0) {
                      return (
                        <Button
                          key={i}
                          focusKey={FOCUS_AUTO}
                          onSelect={() => ChangeLevelDistrict(i)}
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
                                isPassed ? styleLevelProgress.progress : ""
                              }
                              style={{
                                margin: "auto",
                                textAlign: "center",
                                width: isPassed ? "100%" : "100%",
                                color: isPassed ? undefined : "whitesmoke",
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
                </div>
              </FocusDisabled>
            }
          />
        </PanelSection>
      </>
    );
  };
  return componentList as any;
};
