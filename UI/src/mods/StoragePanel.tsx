import {
    ClosePanel, InfoButton, ResetStorage, selectedEntity, SetBrand, SizeProvider,
    storagePanelVisibleBinding, ToggleResource, ToolButton, useUniformSizeProvider,
    VanillaVirtualList
} from "bindings";
import { useValue } from "cs2/api";
import { Entity, Resource } from "cs2/bindings";
import { AutoNavigationScope, FocusActivation, FocusDisabled } from "cs2/input";
import { useLocalization } from "cs2/l10n";
import { FOCUS_AUTO, FOCUS_DISABLED, PanelSection, PanelSectionRow, Portal } from "cs2/ui";
import { useCssLength } from "cs2/utils";
import { FC, useCallback, useEffect, useMemo, useState } from "react";
import {
    infoRowModule, infoSectionModule, resourceBox, storageBox, uilStandard
} from "styleBindings";
import { BrandDataInfo, LocaleKeys, ResourceDataInfo, ResourceGroup } from "types";

import styles from "./BrandPanel.module.scss";
import { PanelBase } from "./PanelBase";

interface StoragePanelProps {
  h_storage: boolean;
  w_resources: ResourceDataInfo[];
  w_resourceslist: ResourceDataInfo[];
  w_entity: Entity;
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
        {w_resourceslist.map((resource) => {
          const isCurrent = w_resources.some((r) => r.Id === resource.Id);
          return (
            <div
              onClick={() => {
                ToggleResource(resource.Id.toString());
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
    // <>
    // <RenderRow
    // w_resources={w_resources}
    // entity={Entity}
    // isCurrent={isCurrent}
    // currentResource={resource}
    // />
    // {/* {RenderItem}
    // <PanelSection> */}
    //   {/* <PanelSectionRow
    //     left={`${BrandsText} (${BrandsArrayX.length})`}
    //     right={BrandGroupHoverText}
    //     tooltip={BrandsTooltip}
    //   /> */}
    //   {/* <AutoNavigationScope activation={FocusActivation.AnyChildren}>
    //     <>
    //       {title}
    //       <VanillaVirtualList
    //         direction="vertical"
    //         sizeProvider={SizeProvider}
    //         renderItem={RenderItem}
    //         style={{
    //           maxHeight: `${Math.min(
    //             30 * w_resourceslist.length,
    //             MaxHeight
    //           )}rem`,
    //         }}
    //         smooth
    //       />
    //     </>
    //   </AutoNavigationScope> */}
    // {/* </PanelSection> */}
    // </>
  );
};

export const RenderRow = ({
  // w_resources,
  // entity,
  isCurrent,
  currentResource,
}: {
  // w_resources: ResourceDataInfo[];
  // entity: Entity;
  isCurrent: boolean;
  currentResource: ResourceDataInfo;
}) => {
  const brandRowClass = `${isCurrent ? styles.BrandCurrentRow : ""} ${
    styles.BrandRow
  }`;
  console.log(currentResource);

  return (
    <div
      onClick={() => {
        ToggleResource(currentResource.Id.toString());
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
  props: StoragePanelProps
) => {
  const { translate } = useLocalization();
  const visibleBindingValue = useValue(storagePanelVisibleBinding);
  const sE = useValue(selectedEntity);

  // const [heightFull, setHeightFull] = useState(0);
  // const [panelLeft, setPanelLeft] = useState(0);

  const headerText = translate(LocaleKeys.STORAGE_HEADER);
  const resetTooltip = translate(LocaleKeys.STORAGE_RESET_TOOLTIP);
  const infoText = translate(LocaleKeys.STORAGE_INFORMATION);
  // const CurrentBrandTitleText = translate(LocaleKeys.BRAND_CURRENT_BRAND);
  // const CurrentCompanyTitleText = translate(LocaleKeys.BRAND_CURRENT_COMPANY);
  // const SupportedBrandsText = translate(
  //   LocaleKeys.BRAND_SUPPORTED_LIST
  // )?.toUpperCase();
  // const SupportedBrandsTooltip = translate(LocaleKeys.BRAND_SUPPORTED_TOOLTIP);
  // const OtherBrandsText = translate(LocaleKeys.BRAND_OTHER_LIST)?.toUpperCase();
  // const OtherBrandsTooltip = translate(LocaleKeys.BRAND_OTHER_TOOLTIP);
  // const BrandGroupHoverText = translate(LocaleKeys.BRAND_GROUP_HOVER);

  // const [SupportedBrandsArray, OtherBrandsArray] = useMemo(() => {
  //   const supported: BrandDataInfo[] = [];
  //   const other: BrandDataInfo[] = [];

  //   for (const brand of props.w_brandlist ?? []) {
  //     if (
  //       Array.isArray(brand.Companies) &&
  //       brand.Companies.includes(props.w_company)
  //     ) {
  //       supported.push(brand);
  //     } else {
  //       other.push(brand);
  //     }
  //   }

  //   return [supported, other];
  // }, [props.w_brandlist, props.w_company]);

  // const wrapperStyle = useMemo(
  //   () => ({
  //     maxHeight: `${heightFull}px`,
  //     left: `calc(${panelLeft}px + 20rem)`,
  //   }),
  //   [panelLeft, heightFull]
  // );

  const visible = useMemo(
    () => visibleBindingValue && props.h_storage,
    [visibleBindingValue]
  );

  // const calculateHeights = () => {
  //   const wrapperElement = document.querySelector(
  //     ".info-layout_BVk"
  //   ) as HTMLElement | null;
  //   const sipElement = document.querySelector(
  //     ".selected-info-panel_gG8"
  //   ) as HTMLElement | null;

  //   const newHeightFull = wrapperElement?.offsetHeight ?? 1600;
  //   if (sipElement?.offsetWidth == 0) {
  //     return;
  //   } else {
  //     const newPanelLeft =
  //       (sipElement?.offsetLeft ?? 6) + (sipElement?.offsetWidth ?? 300);
  //     setPanelLeft(newPanelLeft);
  //   }
  //   setHeightFull(newHeightFull);
  // };

  // useEffect(() => {
  //   calculateHeights();
  //   const observer = new MutationObserver(() => {
  //     calculateHeights();
  //   });

  //   observer.observe(document.body, {
  //     childList: true,
  //     subtree: true,
  //   });

  //   return () => observer.disconnect();
  // }, []);

  // const sizeProviderSupported = useUniformSizeProvider(
  //   useCssLength("30rem"),
  //   props.w_resourceslist.length,
  //   5
  // );

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
                  w_resources={props.w_resources}
                  title={translate(LocaleKeys.RESOURCE_RAW)!}
                  w_resourceslist={props.w_resourceslist
                    .filter((r) => r.Group === ResourceGroup.Raw)
                    .sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                  // Entity={props.w_entity}
                  // SizeProvider={sizeProviderSupported}
                  // MaxHeight={750}
                />
                <ResourcesSection
                  w_resources={props.w_resources}
                  title={translate(LocaleKeys.RESOURCE_PROCESSED)!}
                  w_resourceslist={props.w_resourceslist
                    .filter((r) => r.Group === ResourceGroup.Processed)
                    .sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                  // Entity={props.w_entity}
                  // SizeProvider={sizeProviderSupported}
                  // MaxHeight={750}
                />
                <ResourcesSection
                  w_resources={props.w_resources}
                  title={translate(LocaleKeys.RESOURCE_MAIL)!}
                  w_resourceslist={props.w_resourceslist
                    .filter((r) => r.Group === ResourceGroup.Mail)
                    .sort((a, b) => (a.Id > b.Id ? 1 : -1))}
                  // Entity={props.w_entity}
                  // SizeProvider={sizeProviderSupported}
                  // MaxHeight={750}
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
                        // selected={false}
                        // disabled={true}
                        src={uilStandard + "Reset.svg"}
                        onSelect={() => {
                          ResetStorage();
                        }}
                      />
                    </FocusDisabled>
                  }
                  right={
                    <FocusDisabled>
                      <ToolButton
                        id="starq-abc-storage-copy"
                        className={styles.DisabledToolButton}
                        focusKey={FOCUS_AUTO}
                        tooltip={"Disabled"}
                        // selected={false}
                        disabled={true}
                        // className={styles.ToolWhite}
                        src={uilStandard + "RectangleCopy.svg"}
                        onSelect={() => {
                          // RandomizeStyle();
                        }}
                      />
                      <ToolButton
                        id="starq-abc-storage-paste"
                        className={styles.DisabledToolButton}
                        focusKey={FOCUS_AUTO}
                        tooltip={"Disabled"}
                        // selected={false}
                        disabled={true}
                        // className={styles.ToolWhite}
                        src={uilStandard + "RectanglePaste.svg"}
                        onSelect={() => {
                          // RandomizeStyle();
                        }}
                      />
                      <ToolButton
                        id="starq-abc-storage-copyclear"
                        className={styles.DisabledToolButton}
                        focusKey={FOCUS_AUTO}
                        tooltip={"Disabled"}
                        // selected={false}
                        disabled={true}
                        // className={styles.ToolWhite}
                        src={uilStandard + "XClose.svg"}
                        onSelect={() => {
                          // RandomizeStyle();
                        }}
                      />
                    </FocusDisabled>
                  }
                />
              </>
            </PanelSection>
          </>
        }
      />
    </>
  );
};
