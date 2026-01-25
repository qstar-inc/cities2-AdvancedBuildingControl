import {
  brandPanelVisibleBinding,
  selectedEntity,
  SetBrand,
  SizeProvider,
  useUniformSizeProvider,
  VanillaVirtualList,
} from "bindings";
import { useValue } from "cs2/api";
import { AutoNavigationScope, FocusActivation } from "cs2/input";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { useCssLength } from "cs2/utils";
import { FindTranslation } from "functions/lang";
import { FC, useCallback, useMemo } from "react";
import { BldgBrandInfo, BrandDataInfo } from "types";

import { PanelBase } from "./PanelBase";
import styles from "./style.module.scss";

interface BrandPanelProps {
  bldgBrandInfo: BldgBrandInfo;
}

const BrandSection = ({
  BrandsText,
  BrandsTooltip,
  BrandsArrayX,
  BrandGroupHoverText,
  SelectedBrand,
  MaxHeight,
  SizeProvider,
}: {
  BrandsText: string;
  BrandsTooltip: string;
  BrandsArrayX: BrandDataInfo[];
  BrandGroupHoverText: string;
  SelectedBrand: string;
  MaxHeight: number;
  SizeProvider: SizeProvider;
}) => {
  const RenderItem = useCallback(
    (itemIndex: number, indexInRange: number) => {
      if (itemIndex < 0 || itemIndex >= BrandsArrayX.length) return null;
      const brand = BrandsArrayX[itemIndex];
      const isCurrent = brand.Name === SelectedBrand;
      const brandRowClass = `${isCurrent ? styles.BrandCurrentRow : ""} ${
        styles.BrandRow
      }`;
      return (
        <RenderRow
          brand={brand}
          isCurrent={isCurrent}
          brandRowClass={brandRowClass}
        />
      );
    },
    [SelectedBrand, BrandsArrayX],
  );

  return (
    <>
      <PanelSection>
        <PanelSectionRow
          left={`${BrandsText} (${BrandsArrayX.length})`}
          right={BrandGroupHoverText}
          tooltip={BrandsTooltip}
        />
        <AutoNavigationScope activation={FocusActivation.AnyChildren}>
          <VanillaVirtualList
            direction="vertical"
            sizeProvider={SizeProvider}
            renderItem={RenderItem}
            style={{
              maxHeight: `${Math.min(30 * BrandsArrayX.length, MaxHeight)}rem`,
            }}
            smooth
          />
        </AutoNavigationScope>
      </PanelSection>
    </>
  );
};

export const RenderRow = ({
  isCurrent,
  brand,
  brandRowClass,
}: {
  brand: BrandDataInfo;
  isCurrent: boolean;
  brandRowClass: string;
}) => {
  return (
    <div
      onClick={() => {
        SetBrand(brand.PrefabName);
      }}
    >
      <PanelSectionRow
        className={brandRowClass}
        left={
          <>
            <img className={styles.BrandImage} src={`${brand.Icon}`} />

            {isCurrent && (
              <span className={styles.BrandCurrent}>[Current] </span>
            )}
            <span className={styles.BrandName}>{brand.Name}</span>
          </>
        }
        right={
          <>
            {[brand.Color1, brand.Color2, brand.Color3].map((color, i) => (
              <div
                key={i}
                className={styles.BrandColorBox}
                style={{
                  background: color.slice(0, -2) + "FF",
                }}
              />
            ))}
          </>
        }
      />
    </div>
  );
};

export const BrandPanel: FC<BrandPanelProps> = (props: BrandPanelProps) => {
  const visibleBindingValue = useValue(brandPanelVisibleBinding);
  const sE = useValue(selectedEntity);

  let bldgBrandInfo = props.bldgBrandInfo;

  const headerText = FindTranslation("headerText");
  const CurrentBrandTitleText = FindTranslation("CurrentBrandTitleText");
  const CurrentCompanyTitleText = FindTranslation("CurrentCompanyTitleText");
  const SupportedBrandsText = FindTranslation(
    "SupportedBrandsText",
  )?.toUpperCase();
  const SupportedBrandsTooltip = FindTranslation("SupportedBrandsTooltip");
  const OtherBrandsText = FindTranslation("OtherBrandsText")?.toUpperCase();
  const OtherBrandsTooltip = FindTranslation("OtherBrandsTooltip");
  const BrandGroupHoverText = FindTranslation("BrandGroupHoverText");

  const [SupportedBrandsArray, OtherBrandsArray] = useMemo(() => {
    const supported: BrandDataInfo[] = [];
    const other: BrandDataInfo[] = [];

    for (const brand of bldgBrandInfo.BrandList ?? []) {
      if (
        Array.isArray(brand.Companies) &&
        brand.Companies.includes(bldgBrandInfo.CompanyName)
      ) {
        supported.push(brand);
      } else {
        other.push(brand);
      }
    }

    return [supported, other];
  }, [bldgBrandInfo.BrandList, bldgBrandInfo.CompanyName]);

  const visible = useMemo(
    () => visibleBindingValue && bldgBrandInfo.HasBrand,
    [visibleBindingValue],
  );

  const sizeProviderSupported = useUniformSizeProvider(
    useCssLength("30rem"),
    SupportedBrandsArray.length,
    5,
  );
  const sizeProviderOther = useUniformSizeProvider(
    useCssLength("30rem"),
    OtherBrandsArray.length,
    5,
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
                uppercase={true}
                left={CurrentBrandTitleText}
                right={bldgBrandInfo.BrandName}
              />
              <PanelSectionRow
                uppercase={true}
                left={CurrentCompanyTitleText}
                right={bldgBrandInfo.CompanyName}
              />
            </PanelSection>
            <PanelSection>
              <BrandSection
                BrandsText={SupportedBrandsText!}
                BrandsTooltip={SupportedBrandsTooltip!}
                BrandsArrayX={SupportedBrandsArray}
                BrandGroupHoverText={BrandGroupHoverText!}
                SelectedBrand={bldgBrandInfo.BrandName}
                MaxHeight={210}
                SizeProvider={sizeProviderSupported}
              />
              <BrandSection
                BrandsText={OtherBrandsText!}
                BrandsTooltip={OtherBrandsTooltip!}
                BrandsArrayX={OtherBrandsArray}
                BrandGroupHoverText={BrandGroupHoverText!}
                SelectedBrand={bldgBrandInfo.BrandName}
                MaxHeight={650 - Math.min(SupportedBrandsArray.length, 7) * 30}
                SizeProvider={sizeProviderOther}
              />
            </PanelSection>
          </>
        }
      />
    </>
  );
};
