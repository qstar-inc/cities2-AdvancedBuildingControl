import { selectedEntity, SetBrand } from "bindings/backend";
import { brandPanelVisibleBinding } from "bindings/frontend";
import { useValue } from "cs2/api";
import { AutoNavigationScope, FocusActivation } from "cs2/input";
import { PanelSection, PanelSectionRow } from "cs2/ui";
import { useCssLength } from "cs2/utils";
import { FC, useCallback, useMemo } from "react";
import { FindTranslation, nicifyVariableName } from "shared/lang";
import { PanelBase } from "shared/PanelBase";
import {
  Divider,
  SizeProvider,
  useUniformSizeProvider,
  VanillaVirtualList,
} from "shared/vanilla";
import { BldgBrandInfo, BrandDataInfo } from "types/BrandDataInfo";

import brandStyles from "./BrandPanel.module.scss";

interface BrandPanelProps {
  bldgBrandInfo: BldgBrandInfo;
}

const BrandSection = ({
  BrandsText,
  BrandsTooltip,
  BrandsArrayX,
  SelectedBrand,
  MaxHeight,
  SizeProvider,
}: {
  BrandsText: string;
  BrandsTooltip: string;
  BrandsArrayX: BrandDataInfo[];
  SelectedBrand: string;
  MaxHeight: number;
  SizeProvider: SizeProvider;
}) => {
  const RenderItem = useCallback(
    (itemIndex: number, indexInRange: number) => {
      if (itemIndex < 0 || itemIndex >= BrandsArrayX.length) return null;
      const brand = BrandsArrayX[itemIndex];
      const isCurrent = brand.Name === SelectedBrand;
      const brandRowClass = `${isCurrent ? brandStyles.BrandCurrentRow : ""} ${
        brandStyles.BrandRow
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
          right={"[?]"}
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
            <img className={brandStyles.BrandImage} src={`${brand.Icon}`} />

            {isCurrent && (
              <span className={brandStyles.BrandCurrent}>[Current] </span>
            )}
            <span className={brandStyles.BrandName}>{brand.Name}</span>
          </>
        }
        right={
          <>
            {[brand.Color1, brand.Color2, brand.Color3].map((color, i) => (
              <div
                key={i}
                className={brandStyles.BrandColorBox}
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

  const headerText = FindTranslation("Brand.Header");
  const CurrentBrandTitleText = FindTranslation("Brand.CurrentBrand");
  const CurrentCompanyTitleText = FindTranslation("Brand.CurrentCompany");
  const SupportedBrandsText = FindTranslation(
    "Brand.SupportedBrands",
  )?.toUpperCase();
  const SupportedBrandsTooltip = FindTranslation(
    "Brand.SupportedBrands.Tooltip",
  );
  const OtherBrandsText = FindTranslation("Brand.OtherBrands")?.toUpperCase();
  const OtherBrandsTooltip = FindTranslation("Brand.OtherBrands.Tooltip");

  const infoText = FindTranslation("Brand.Info");

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
        id="abc-brand"
        title={headerText!}
        visible={visible}
        sipAligned={true}
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
                right={nicifyVariableName(
                  bldgBrandInfo.CompanyName.replace("_", ": "),
                )}
              />
            </PanelSection>
            <PanelSection>
              <BrandSection
                BrandsText={SupportedBrandsText!}
                BrandsTooltip={SupportedBrandsTooltip!}
                BrandsArrayX={SupportedBrandsArray}
                SelectedBrand={bldgBrandInfo.BrandName}
                MaxHeight={210}
                SizeProvider={sizeProviderSupported}
              />
              <BrandSection
                BrandsText={OtherBrandsText!}
                BrandsTooltip={OtherBrandsTooltip!}
                BrandsArrayX={OtherBrandsArray}
                SelectedBrand={bldgBrandInfo.BrandName}
                MaxHeight={440 - Math.min(SupportedBrandsArray.length, 7) * 30}
                SizeProvider={sizeProviderOther}
              />
            </PanelSection>
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
