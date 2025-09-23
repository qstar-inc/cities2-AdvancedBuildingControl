import { ClosePanel, selectedEntity } from "bindings";
import { useValue } from "cs2/api";
import { Portal } from "cs2/ui";
import { FC, ReactElement, useEffect, useMemo, useState } from "react";
import {
    closeButtonClass, closeButtonImageClass, styleDefault, stylePanel, styleScrollable, styleSIP,
    styleSIPTheme, wrapperClass
} from "styleBindings";

import styles from "./style.module.scss";

interface PanelBase {
  header: string;
  visible: boolean;
  content: ReactElement;
  height?: number;
}

export const PanelBase: FC<PanelBase> = (props: PanelBase) => {
  const sE = useValue(selectedEntity);

  const [panelLeft, setPanelLeft] = useState(0);

  const wrapperStyle = useMemo(
    () => ({
      left: `calc(${panelLeft}px + 20rem)`,
    }),
    [panelLeft]
  );

  const calculateHeights = () => {
    const sipElement = document.querySelector(
      ".selected-info-panel_iIe"
    ) as HTMLElement | null;

    const newPanelLeft =
      (sipElement?.offsetLeft ?? 6) + (sipElement?.offsetWidth ?? 300);
    if (newPanelLeft != 306) {
      setPanelLeft(newPanelLeft);
    }
  };

  useEffect(() => {
    calculateHeights();
    const observer = new MutationObserver(() => {
      calculateHeights();
    });

    observer.observe(document.body, {
      childList: true,
      subtree: true,
    });

    return () => observer.disconnect();
  }, []);

  if (sE.index === 0 || !props.visible) return null;

  return (
    <>
      <Portal>
        <div
          id="starq-abc-panel"
          className={`${wrapperClass} ${styles.BrandChangerPanel} ${styles.BrandChangerAnimate}`}
          style={wrapperStyle}
        >
          <div id={"starq-abc-panel-header"} className={styleSIPTheme.header}>
            <div className={stylePanel.titleBar}>
              <img
                className={stylePanel.icon}
                src="Media/Tools/Net Tool/Replace.svg"
              />
              <div className={styleSIPTheme.title}>{props.header}</div>
              <button className={closeButtonClass} onClick={() => ClosePanel()}>
                <div
                  className={closeButtonImageClass}
                  style={{
                    maskImage: "url(Media/Glyphs/Close.svg)",
                  }}
                ></div>
              </button>
            </div>
          </div>
          <div id={"starq-abc-panel-content"} className={styleSIPTheme.content}>
            <div
              className={`${styleScrollable.scrollable} ${styleScrollable.y} ${styleSIP.scrollable}`}
            >
              <div className={styleScrollable.content}>
                {props.content}
                <div className="bottom-padding_JS3"></div>
              </div>
            </div>
          </div>
        </div>
      </Portal>
    </>
  );
};
