import {
  ChangeValue,
  ChangeValueString,
  ResetValue,
  selectedEntity,
  storagePanelVisibleBinding,
  ToolButton,
} from "bindings";
import { useValue } from "cs2/api";
import { FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import {
  FOCUS_AUTO,
  FOCUS_DISABLED,
  PanelSection,
  PanelSectionRow,
} from "cs2/ui";
import { FC, useMemo } from "react";
import {
  infoRowModule,
  infoSectionModule,
  resourceBox,
  storageBox,
  uilStandard,
} from "styleBindings";
import {
  BldgStorageInfo,
  LocaleKeys,
  ResourceDataInfo,
  ResourceGroup,
  UpdateValueType,
} from "types";

import { PanelBase } from "./PanelBase";
import styles from "./style.module.scss";

interface StoragePanelProps {
  bldgStorageInfo: BldgStorageInfo;
}

const ResourcesSection = ({
  title,
  w_resources,
  w_resourceslist,
}: {
  title: string;
  w_resources: ResourceDataInfo[];
  w_resourceslist: ResourceDataInfo[];
}) => {
  const { translate } = useLocalization();
  return (
    <>
      <div
        className={`${infoRowModule.infoRow} ${infoRowModule.disableFocusHighlight}`}
      >
        <div className={infoRowModule.left}>{title}</div>
      </div>
      <div className={infoSectionModule.infoWrapBox}>
        {w_resourceslist.map(resource => {
          const isCurrent = w_resources.some(r => r.Id === resource.Id);
          return (
            <div
              onClick={() => {
                ChangeValueString(
                  resource.Id.toString(),
                  UpdateValueType.Storage,
                );
              }}
              className={`${storageBox.resource} ${resourceBox.field} ${
                isCurrent
                  ? `${styles.ResourceActive} ${storageBox.surplus}`
                  : ""
              }`}
              key={resource.Name}
            >
              <img
                className={`${resourceBox.icon} ${styles.BrandImage}`}
                src={`Media/Game/Resources/${resource.Name}.svg`}
              />
              <div className={resourceBox.label}>
                {translate(`Resources.TITLE[${resource.Name}]`)}
              </div>
            </div>
          );
        })}
      </div>
    </>
  );
};

export const RenderRow = ({
  isCurrent,
  currentResource,
}: {
  isCurrent: boolean;
  currentResource: ResourceDataInfo;
}) => {
  const brandRowClass = `${isCurrent ? styles.BrandCurrentRow : ""} ${
    styles.BrandRow
  }`;

  return (
    <div
      onClick={() => {
        ChangeValueString(
          currentResource.Id.toString(),
          UpdateValueType.Storage,
        );
      }}
    >
      <PanelSectionRow
        className={brandRowClass}
        left={
          <>
            <img
              className={styles.BrandImage}
              src={`${currentResource.Icon}`}
            />

            {isCurrent && (
              <span className={styles.BrandCurrent}>[Current] </span>
            )}
            <span className={styles.BrandName}>
              {currentResource.DisplayName}
            </span>
          </>
        }
      />
    </div>
  );
};

export const StoragePanel: FC<StoragePanelProps> = (
  props: StoragePanelProps,
) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(storagePanelVisibleBinding);
  const sE = useValue(selectedEntity);

  let bldgStorageInfo = props.bldgStorageInfo;

  const headerText = translate(LocaleKeys.STORAGE_HEADER);
  const resetTooltip = translate(LocaleKeys.STORAGE_RESET_TOOLTIP);
  const infoText = translate(LocaleKeys.STORAGE_INFORMATION);

  const visible = useMemo(
    () => visibleBindingValue && bldgStorageInfo.HasStorage,
    [visibleBindingValue],
  );

  if (sE.index === 0 || !visible) return null;

  return (
    <>
      <PanelBase
        header={headerText!}
        visible={visible}
        content={
          <>
            <PanelSection>
              <PanelSectionRow
                uppercase={false}
                disableFocus={true}
                left={infoText}
              />
            </PanelSection>
            <PanelSection>
              <>
                <ResourcesSection
                  w_resources={bldgStorageInfo.BuildingResources}
                  title={translate("SelectedInfoPanel.RAW_MATERIALS")!}
                  w_resourceslist={bldgStorageInfo.BuildingResourcesAll.filter(
                    r => r.Group === ResourceGroup.Raw,
                  ).sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                />
                <ResourcesSection
                  w_resources={bldgStorageInfo.BuildingResources}
                  title={translate("SelectedInfoPanel.PROCESSED_GOODS")!}
                  w_resourceslist={bldgStorageInfo.BuildingResourcesAll.filter(
                    r => r.Group === ResourceGroup.Processed,
                  ).sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                />
                <ResourcesSection
                  w_resources={bldgStorageInfo.BuildingResources}
                  title={translate("SelectedInfoPanel.MAIL")!}
                  w_resourceslist={bldgStorageInfo.BuildingResourcesAll.filter(
                    r => r.Group === ResourceGroup.Mail,
                  ).sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                />
              </>
            </PanelSection>
            <PanelSection>
              <>
                <PanelSectionRow
                  uppercase={true}
                  disableFocus={true}
                  left={
                    <FocusDisabled>
                      <ToolButton
                        id="starq-abc-storage-reset"
                        focusKey={FOCUS_DISABLED}
                        tooltip={resetTooltip!}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetValue(UpdateValueType.Storage);
                        }}
                      />
                    </FocusDisabled>
                  }
                  // right={
                  //   <FocusDisabled>
                  //     <ToolButton
                  //       id="starq-abc-storage-copy"
                  //       className={styles.DisabledToolButton}
                  //       focusKey={FOCUS_AUTO}
                  //       tooltip={"Disabled"}
                  //       // selected={false}
                  //       disabled={true}
                  //       // className={styles.ToolWhite}
                  //       src={uilStandard + "RectangleCopy.svg"}
                  //       onSelect={() => {
                  //         // RandomizeStyle();
                  //       }}
                  //     />
                  //     <ToolButton
                  //       id="starq-abc-storage-paste"
                  //       className={styles.DisabledToolButton}
                  //       focusKey={FOCUS_AUTO}
                  //       tooltip={"Disabled"}
                  //       // selected={false}
                  //       disabled={true}
                  //       // className={styles.ToolWhite}
                  //       src={uilStandard + "RectanglePaste.svg"}
                  //       onSelect={() => {
                  //         // RandomizeStyle();
                  //       }}
                  //     />
                  //     <ToolButton
                  //       id="starq-abc-storage-copyclear"
                  //       className={styles.DisabledToolButton}
                  //       focusKey={FOCUS_AUTO}
                  //       tooltip={"Disabled"}
                  //       // selected={false}
                  //       disabled={true}
                  //       // className={styles.ToolWhite}
                  //       src={uilStandard + "XClose.svg"}
                  //       onSelect={() => {
                  //         // RandomizeStyle();
                  //       }}
                  //     />
                  //   </FocusDisabled>
                  // }
                />
              </>
            </PanelSection>
          </>
        }
      />
    </>
  );
};
