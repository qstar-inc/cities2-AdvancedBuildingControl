X.add("game-ui/common/input/text/int-input.tsx", {
  get IntInput() {
    return XB;
  },
  set IntInput(e) {
    XB = e;
  },
});
var XB = ({ min: e = _d, max: t = bd, ...n }) => {
  const s = (0, Y.useCallback)(
    n =>
      (function (e, t, n) {
        if ("-" === e) return;
        if ("" === e) return t <= 0 && n >= 0 ? 0 : void 0;
        const s = parseInt(e, 10);
        return isNaN(s) ? void 0 : Od(s, t, n);
      })(n, e, t),
    [e, t],
  );
  return (0, z.jsx)(LB, {
    ...n,
    showHint: !0,
    valueFormatter: QB,
    inputParser: s,
    inputValidator: e < 0 ? ZB : JB,
  });
};
function Od(e, t, n) {
  return e <= t ? t : e >= n ? n : e;
}
function LB({
  value: e,
  valueFormatter: t,
  inputValidator: n,
  inputTransformer: s = AB,
  inputParser: i,
  onChange: a,
  onFocus: o,
  onBlur: r,
  ...l
}) {
  const [c, u] = (0, Y.useState)(null),
    [d, m] = (0, Y.useState)(!1),
    h = (0, Y.useMemo)(() => t(e), [e, t]),
    g = Dm(),
    p = (0, Y.useCallback)(
      e => {
        const t = s(e.target.value || "");
        if (n(t)) {
          const e = c;
          if ((u(t), a)) {
            const n = null !== e ? i(e) : void 0,
              s = null !== t ? i(t) : void 0;
            if (void 0 !== s && s !== n) return a(s);
          }
        }
      },
      [i, s, n, c, a],
    ),
    f = (0, Y.useCallback)(
      n => {
        (m(!0), u(t(e)), o && o(n));
      },
      [o, e, t],
    ),
    x = (0, Y.useCallback)(
      e => {
        (m(!1), u(null), r && r(e));
      },
      [r],
    );
  return (
    (0, Y.useEffect)(() => {
      (!g && d) || u(t(e));
    }, [g, d, e, t]),
    (0, z.jsx)(oI, {
      ...l,
      value: c ?? h,
      onChange: p,
      onFocus: f,
      onBlur: x,
    })
  );
}
function QB(e) {
  return Math.trunc(e).toString(10);
}
function ZB(e) {
  return /^-?[0-9]*$/.test(e);
}
function JB(e) {
  return /^[0-9]*$/.test(e);
}
function AB(e) {
  return e;
}
function Dm() {
  return sl(Em) === vm.gamepad;
}
var oI = Y.forwardRef(function (
  {
    focusKey: e,
    debugName: t = "TextInput",
    type: n = "text",
    value: s = "",
    selectAllOnFocus: i = !0,
    placeholder: a = "",
    vkTitle: o = "",
    vkDescription: r = "",
    disabled: l,
    forceHint: c,
    showHint: u,
    className: d,
    multiline: m,
    onFocus: h,
    onBlur: g,
    onKeyDown: p,
    onChange: f,
    onMouseUp: x,
    onSelect: v,
    onBack: _,
    onDoubleClick: b,
    ...T
  },
  E,
) {
  const I = (0, Y.useRef)(null),
    S = gd(E, I),
    y = gp(jg(e ?? (l ? Sg : yg), t), I),
    j = wh(y),
    C = Dm(),
    N = sl(Mm),
    [w, O] = (0, Y.useState)(!1),
    P = (0, Y.useCallback)(() => {
      null != I.current &&
        document.activeElement === I.current &&
        (I.current.blur(), _ && _());
    }, [_]),
    L = (0, Y.useMemo)(
      () =>
        w
          ? { Close: P, Back: P }
          : {
              Select: () => {
                if (null != I.current) {
                  const e = I.current.getBoundingClientRect();
                  (Am(e.x, e.y, e.width, e.height),
                    R_(I, "--selectAnimation", "--selectDuration"),
                    I.current.focus(),
                    v && v());
                }
              },
            },
      [w, v, P],
    ),
    A = (0, Y.useCallback)(
      e => {
        ((M.current = !1),
          ((!m && 13 === e.keyCode) || 27 === e.keyCode) &&
            e.currentTarget.blur(),
          e.stopPropagation(),
          p && p(e));
      },
      [m, p],
    ),
    M = (0, Y.useRef)(!1),
    R = (0, Y.useCallback)(
      e => {
        ((M.current = !1), f && f(e));
      },
      [f],
    ),
    k = (0, Y.useCallback)(
      e => {
        (O(!0), (M.current = !0), h && h(e), cp(lp.selectItem));
      },
      [h],
    ),
    D = (0, Y.useCallback)(() => cp(lp.hoverItem), []),
    F = (0, Y.useCallback)(
      e => {
        if (M.current) {
          if (
            null != I.current &&
            I.current.selectionStart === I.current.selectionEnd
          ) {
            const e = I.current.value.length;
            I.current.setSelectionRange(0, e);
          }
          M.current = !1;
        }
        x && x(e);
      },
      [x],
    ),
    U = (0, Y.useCallback)(
      e => {
        (O(!1), g && g(e));
      },
      [g],
    );
  ((0, Y.useEffect)(() => {
    C && !j && I.current && I.current.blur();
  }, [j, C]),
    (0, Y.useEffect)(() => {
      if (I.current) {
        const e = I.current,
          t = () => {
            if (e && M.current) {
              const t = e.value.length;
              e.setSelectionRange(i ? 0 : t, t);
            }
            M.current = !1;
          };
        return (
          e.addEventListener("focus", t),
          () => e.removeEventListener("focus", t)
        );
      }
    }, [i]));
  const B = (0, Y.useCallback)(
      e => {
        (b?.(e),
          I.current && I.current.setSelectionRange(0, I.current.value.length));
      },
      [b],
    ),
    G = ic(a),
    V = !w && !s && G,
    H = ic(o),
    K = ic(r),
    W = void 0 !== m ? "textarea" : "input",
    {
      displayName: $,
      hidden: q,
      tooltip: X,
      tutorialTag: Q,
      uiTag: Z,
      __Type: J,
      warning: ee,
      ...te
    } = T;
  return (0, z.jsx)(Np, {
    disabled: !w || !C || !N,
    excludes: ["Save Game", "Change Value"],
    children: (0, z.jsx)(Mg, {
      actions: L,
      children: (0, z.jsxs)(Lh, {
        controller: y,
        children: [
          u &&
            (0, z.jsx)(UT, {
              action: w ? "Back" : "Select",
              active: !(!j && !c),
              theme: aI,
            }),
          (0, z.jsx)(W, {
            ...te,
            ref: S,
            type: "password" === n && V ? "text" : n,
            disabled: l,
            className: Zu()(d, j && C && "focused", V && "placeholder"),
            "vk-title": H,
            "vk-description": K,
            "vk-type": n,
            rows: "number" == typeof m ? m : 5,
            onFocus: k,
            onBlur: U,
            onKeyDown: A,
            onMouseUp: F,
            onMouseEnter: D,
            onDoubleClick: B,
            value: V ? G : s,
            onChange: R,
          }),
        ],
      }),
    }),
  });
});
X.add("game-ui/common/input/text/text-input.tsx", {
  get TextInput() {
    return oI;
  },
  set TextInput(e) {
    oI = e;
  },
});
function gd(e, t) {
  return (0, Y.useCallback)(
    n => {
      (pd(e, n), pd(t, n));
    },
    [e, t],
  );
}
function sl(e) {
  const t = (0, Y.useMemo)(() => e.subscribe(), [e]),
    [n, s] = (0, Y.useState)(t.value),
    i = (0, Y.useRef)(n);
  return (
    (0, Y.useEffect)(() => {
      const e = e => {
        if (
          Array.isArray(e) &&
          Array.isArray(i.current) &&
          !i.current.length &&
          !e.length
        )
          return;
        let t = e;
        "number" == typeof e && e && (t = Math.floor(1e6 * e) / 1e6);
        let n = !1;
        ((n =
          "object" == typeof t && "object" == typeof i.current
            ? !Kr(i.current, t, 10)
            : t !== i.current),
          n && ((i.current = t), s(t)));
      };
      return (e(t.value), t.setChangeListener(e), () => t.dispose());
    }, [t]),
    n
  );
}
const Hr = Object.prototype.hasOwnProperty;
function Kr(e, t, n = 1) {
  if (Object.is(e, t)) return !0;
  if (
    n <= 0 ||
    ("object" != typeof e && !Array.isArray(e)) ||
    null === e ||
    ("object" != typeof t && !Array.isArray(t)) ||
    null === t
  )
    return !1;
  if (Array.isArray(e)) {
    if (!Array.isArray(t)) return !1;
    if (e.length !== t.length) return !1;
    for (let s = 0; s < e.length; s++) if (!Kr(e[s], t[s], n - 1)) return !1;
    return !0;
  }
  const s = Object.keys(e),
    i = Object.keys(t);
  if (s.length !== i.length) return !1;
  for (let i = 0; i < s.length; i++)
    if (!Hr.call(t, s[i]) || !Kr(e[s[i]], t[s[i]], n - 1)) return !1;
  return !0;
}
function gp(e, t, n = qu.Always, s = !1) {
  const i = (0, Y.useCallback)(
      () => t.current?.getBoundingClientRect() || null,
      [t],
    ),
    a = (0, Y.useMemo)(() => new pp(e, i, n, s), [e, i, n, s]),
    o = (0, Y.useContext)(hp),
    r = al(Em);
  return (
    Ph(
      a,
      (0, Y.useCallback)(
        e => {
          e &&
            r.current === vm.gamepad &&
            null != t.current &&
            requestAnimationFrame(() => {
              t.current && o.scrollIntoView(t.current);
            });
        },
        [r, t, o],
      ),
    ),
    a
  );
}
function al(e) {
  const t = (0, Y.useRef)(e.value);
  return (
    (0, Y.useEffect)(() => {
      const n = e.subscribe(e => (t.current = e));
      return n.dispose;
    }),
    t
  );
}
function Ph(e, t) {
  (0, Y.useEffect)(() => {
    if (null != t) return (e.attachCallback(t), () => e.detachCallback(t));
  }, [t, e]);
}
var Sg = new Ig("FOCUS_DISABLED"),
  yg = new Ig("FOCUS_AUTO");
function jg(e, t) {
  return (0, Y.useMemo)(
    () => (e === Sg ? null : e === yg ? new Ig(t) : e),
    [e, t],
  );
}
function wh(e) {
  const [t, n] = (0, Y.useState)(!1);
  return (
    (0, Y.useEffect)(
      () => (e.attachCallback(n), () => e.detachCallback(n)),
      [e],
    ),
    t
  );
}
function Dm() {
  return sl(Em) === vm.gamepad;
}
