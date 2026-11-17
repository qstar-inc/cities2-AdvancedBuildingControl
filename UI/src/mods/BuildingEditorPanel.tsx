import {
  togglePanel,
  PanelIndex,
  buildingEditorVisibleBinding,
} from "bindings/frontend";
import { useValue } from "cs2/api";
import { PanelBase } from "shared/PanelBase";
import mod from "mod.json";
import { toolsIcon } from "shared/icons_uil";
import { PanelSectionRow, Scrollable } from "cs2/ui";
import { SearchField } from "shared/vanilla-modules/game-ui/editor/widgets/search-field/search-field";
import {
  ToolButton,
  useUniformSizeProvider,
  VanillaVirtualList,
} from "shared/vanilla";
import {
  EditorBuildingAssetPacks$,
  EditorBuildings$,
  EditorBuildingSources$,
} from "bindings/backend";
import { Section } from "shared/vanilla-modules/game-ui/common/section/section";
import { FC, memo, useCallback, useMemo, useState } from "react";
import {
  playSound,
  UISound,
} from "shared/vanilla-modules/game-ui/common/data-binding/audio-bindings";
import { AutoNavigationScope, FocusActivation } from "cs2/input";
import { entityKey, useCssLength } from "cs2/utils";
import { EditorBuilding } from "types/EditorBuilding";
import { FindTranslation } from "shared/lang";

const ROW_HEIGHT = 30;
const CONTENT_HEIGHT = "635rem";
const LIST_HEIGHT = 540;

const keyOf = (b: EditorBuilding) => `${b.Entity.index}:${b.Entity.version}`;

export const BuildingEditorPanel = () => {
  const visibleBindingValue = useValue(buildingEditorVisibleBinding);

  const allBuildings = useValue(EditorBuildings$);
  const allPacks = useValue(EditorBuildingAssetPacks$);
  const allSources = useValue(EditorBuildingSources$);

  const buildings = useMemo(() => {
    return allBuildings
      .map(b => {
        const key = b.IsUpgrade
          ? `Assets.UPGRADE_NAME[${b.PrefabName}]`
          : `Assets.NAME[${b.PrefabName}]`;
        const translated = FindTranslation(key, true);
        const LocaleName =
          !translated || translated === key ? b.PrefabName : translated;
        return { ...b, LocaleName, PackKeys: b.AssetPacks.map(entityKey) };
      })
      .sort((a, b) => a.LocaleName.localeCompare(b.LocaleName));
  }, [allBuildings]);

  const packs = useMemo(
    () =>
      allPacks
        .map(p => {
          const key = `Assets.NAME[${p.PrefabName}]`;
          const t = FindTranslation(key, true);
          return {
            ...p,
            key: entityKey(p.Entity),
            LocaleName: !t || t === key ? p.PrefabName : t,
          };
        })
        .sort((a, b) => a.LocaleName.localeCompare(b.LocaleName)),
    [allPacks],
  );

  const packCounts = useMemo(() => {
    const m = new Map<string, number>();
    for (const b of buildings)
      for (const k of b.PackKeys) m.set(k, (m.get(k) ?? 0) + 1);
    return m;
  }, [buildings]);

  const [search, setSearch] = useState("");
  const [selectedKey, setSelectedKey] = useState<string | null>(null);
  const [selectedPacks, setSelectedPacks] = useState<ReadonlySet<string>>(
    new Set(),
  );
  const [packMode, setPackMode] = useState<"any" | "all">("any");

  const togglePack = useCallback((key: string) => {
    setSelectedPacks(prev => {
      const next = new Set(prev);
      next.has(key) ? next.delete(key) : next.add(key);
      return next;
    });
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    const wanted = [...selectedPacks];

    return buildings.filter(b => {
      if (
        q &&
        !b.LocaleName.toLowerCase().includes(q) &&
        !b.PrefabName.toLowerCase().includes(q)
      )
        return false;
      if (wanted.length) {
        const test = (k: string) => b.PackKeys.includes(k);
        if (packMode === "any" ? !wanted.some(test) : !wanted.every(test))
          return false;
      }
      return true;
    });
  }, [buildings, search, selectedPacks, packMode]);

  const selectedBuilding = useMemo(
    () => buildings.find(b => keyOf(b) === selectedKey),
    [buildings, selectedKey],
  );

  const sizeProvider = useUniformSizeProvider(
    useCssLength(`${ROW_HEIGHT}rem`),
    filtered.length,
    5,
  );

  const handleSelect = useCallback((key: string) => setSelectedKey(key), []);

  const handleSearchChange = useCallback((value: string) => {
    setSearch(value.replace(/[^a-zA-Z0-9\s,.'\u0100-\uffff]/g, ""));
  }, []);

  const renderItem = useCallback(
    (itemIndex: number) => {
      if (itemIndex < 0 || itemIndex >= filtered.length) return null;
      const building = filtered[itemIndex];
      const key = keyOf(building);

      return (
        <BuildingRow
          key={key}
          id={key}
          building={building}
          selected={key === selectedKey}
          onSelect={handleSelect}
        />
      );
    },
    [filtered, selectedKey, handleSelect],
  );

  return (
    <PanelBase
      id="abc-editor"
      title={mod.name}
      visible={visibleBindingValue}
      icon={toolsIcon}
      sipAligned={false}
      width={1000}
      height={680}
      onClose={() => togglePanel(PanelIndex.BuildingEditor)}
      content={
        <>
          <div
            style={{
              padding: "10rem",
              textAlign: "center",
            }}
          >
            <span style={{ fontSize: "130%", fontWeight: "bolder" }}>
              UNDER CONSTRUCTION
            </span>
            <br />
            Check back in a future update
          </div>
          {/* <Section header={<PanelSectionRow uppercase left="Asset Packs" />}>
            {/* Any/All toggle: two small buttons that set packMode, shown when selectedPacks.size > 1 *
            <div>
              {/*  style={{ display: "flex", flexDirection: "row" }}> *
              <Scrollable
                horizontal
                vertical={false}
                trackVisibility="scrollable"
              >
                <div style={{ display: "flex", flexDirection: "row" }}>
                  {packs.map(p => (
                    <PackRow
                      key={p.key}
                      k={p.key}
                      name={p.LocaleName}
                      icon={p.UIIcon}
                      count={packCounts.get(p.key) ?? 0}
                      active={selectedPacks.has(p.key)}
                      onToggle={togglePack}
                    />
                  ))}
                </div>
              </Scrollable>
            </div>  
          </Section>
          <div
            style={{
              display: "flex",
              flexDirection: "row",
              padding: "10rem",
              height: CONTENT_HEIGHT,
              boxSizing: "border-box",
            }}
          >
            <div
              style={{
                width: "280rem",
                flexShrink: 0,
                marginRight: "10rem",
              }}
            >
              <Section header={<PanelSectionRow uppercase left="Buildings" />}>
                <div style={{ height: "585rem" }}>
                  <div style={{ height: "46rem" }}>
                    <SearchField
                      value={search}
                      onChange={handleSearchChange}
                      placeholder="Search buildings..."
                    />
                  </div>
                  <AutoNavigationScope activation={FocusActivation.AnyChildren}>
                    <VanillaVirtualList
                      key={`${search}|${[...selectedPacks].join()}|${packMode}`}
                      direction="vertical"
                      sizeProvider={sizeProvider}
                      renderItem={renderItem}
                      style={{
                        maxHeight: `${Math.min(ROW_HEIGHT * filtered.length, LIST_HEIGHT)}rem`,
                      }}
                      smooth
                    />
                  </AutoNavigationScope>
                </div>
              </Section>
            </div>

            <div style={{ flex: "1 1 0", minWidth: 0 }}>
              <Section
                header={
                  <PanelSectionRow
                    left={selectedBuilding?.PrefabName ?? "Nothing selected"}
                  />
                }
              >
                <div style={{ height: "585rem" }}>
                  <Scrollable vertical trackVisibility="scrollable">
                    {selectedBuilding ? (
                      <PanelSectionRow center="This view is under construction! Check back later." />
                    ) : (
                      <PanelSectionRow center="Select a building from the list" />
                    )}
                  </Scrollable>
                </div>
              </Section>
            </div>
          </div> */}
        </>
      }
    />
  );
};

const BuildingRow: FC<{
  id: string;
  building: EditorBuilding;
  selected: boolean;
  onSelect: (id: string) => void;
}> = memo(({ id, building, selected, onSelect }) => {
  const [hovered, setHovered] = useState(false);

  const handleEnter = useCallback(() => {
    setHovered(true);
    playSound(UISound.hoverItem);
  }, []);

  const handleLeave = useCallback(() => setHovered(false), []);

  const handleClick = useCallback(() => {
    playSound(UISound.selectItem);
    onSelect(id);
  }, [onSelect, id]);

  const background = selected
    ? "rgba(255, 255, 255, 0.2)"
    : hovered
      ? "rgba(255, 255, 255, 0.1)"
      : "transparent";

  return (
    <div
      onClick={handleClick}
      onMouseEnter={handleEnter}
      onMouseLeave={handleLeave}
      style={{
        height: `${ROW_HEIGHT}rem`,
        overflow: "hidden",
        background,
        borderRadius: "4rem",
      }}
    >
      <PanelSectionRow left={building.LocaleName} icon={building.UIIcon} />
    </div>
  );
});

const PackRow: FC<{
  k: string;
  name: string;
  icon: string;
  count: number;
  active: boolean;
  onToggle: (k: string) => void;
}> = memo(({ k, name, icon, count, active, onToggle }) => (
  <div
    onClick={() => {
      playSound(UISound.selectItem);
      onToggle(k);
    }}
    onMouseEnter={() => playSound(UISound.hoverItem)}
    style={{
      background: active ? "rgba(255,255,255,0.2)" : "transparent",
      borderRadius: "4rem",
    }}
  >
    <ToolButton src={icon} tooltip={name} />
    {/* <PanelSectionRow icon=" left={name} right={String(count)} /> */}
  </div>
));
