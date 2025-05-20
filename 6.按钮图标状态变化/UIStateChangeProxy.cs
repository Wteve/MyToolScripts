using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyTool {

    public class UIStateChangeProxy : MonoBehaviour {
        [SerializeField]
        public UIChangeComponentType uiType;

        [SerializeField]
        public UIChangeType changeType;

        [SerializeField]
        public List<UIStateInfo> baseColor = new List<UIStateInfo>();

        [SerializeField]
        public bool useShadow = false;
        [SerializeField]
        public List<UIStateInfo> shadowColor = new List<UIStateInfo>();

        [SerializeField]
        public bool useOutline = false;
        [SerializeField]
        public List<UIStateInfo> outlineColor = new List<UIStateInfo>();

        public void InitColor() {
            if (GetComponent<Image>() != null) {
                uiType = UIChangeComponentType.Image;
            }
            if (GetComponent<Text>() != null) {
                uiType = UIChangeComponentType.Text;
            }

            foreach (UIChangeState state in System.Enum.GetValues(typeof(UIChangeState))) {
                if (!ContainsState(baseColor, state)) {
                    Color color = Color.white;
                    Sprite sprite = null;
                    switch (uiType) {
                        case UIChangeComponentType.Image:
                            color = state == UIChangeState.Active ? GetColor<Image>() : ToolMgr.Instance.HexToColor("#7D7D7D");
                            break;
                        case UIChangeComponentType.Text:
                            color = state == UIChangeState.Active ? GetColor<Text>() : ToolMgr.Instance.HexToColor("#7D7D7D");
                            break;
                    }
                    baseColor.Add(AddStateInfo(state, color, sprite));
                }

                if (useShadow) {
                    if (!ContainsState(shadowColor, state)) {
                        Color color = state == UIChangeState.Active ? GetEffectColor<Shadow>() : ToolMgr.Instance.HexToColor("#525252");
                        shadowColor.Add(AddStateInfo(state, color));
                    }
                }
                if (useOutline) {
                    if (!ContainsState(outlineColor, state)) {
                        Color color = state == UIChangeState.Active ? GetEffectColor<Outline>() : ToolMgr.Instance.HexToColor("#525252");
                        outlineColor.Add(AddStateInfo(state, color));
                    }
                }
            }
        }

        private bool ContainsState(List<UIStateInfo> _list, UIChangeState _state) {
            for (int i = 0; i < _list.Count; i++) {
                if (_list[i].state == _state) {
                    return true;
                }
            }
            return false;
        }

        private UIStateInfo AddStateInfo(UIChangeState _state, Color _color, Sprite _sprite = null) {
            UIStateInfo info = new UIStateInfo();
            info.SetProxy(this);
            info.SetData(_state, _color, _sprite);
            return info;
        }

        private int curStateID = -1;
        public void SetColor(UIChangeState state) {
            if ((UIChangeState)curStateID == state)
                return;

            if (Application.isPlaying) {
                curStateID = (int)state;
            }

            for (int i = 0; i < baseColor.Count; i++) {
                if (baseColor[i].state == state) {
                    switch (uiType) {
                        case UIChangeComponentType.Image:
                            switch (changeType) {
                                case UIChangeType.Color:
                                    SetColor<Image>(baseColor[i].color);
                                    break;
                                case UIChangeType.Sprite:
                                    SetSprite(baseColor[i].sprite);
                                    break;
                            }
                            break;
                        case UIChangeComponentType.Text:
                            SetColor<Text>(baseColor[i].color);
                            break;
                    }
                    break;
                }
            }

            switch (changeType) {
                case UIChangeType.Color:
                    if (useShadow) {
                        for (int i = 0; i < shadowColor.Count; i++) {
                            if (shadowColor[i].state == state) {
                                SetEffectColor<Shadow>(shadowColor[i].color);
                                break;
                            }
                        }
                    }

                    if (useOutline) {
                        for (int i = 0; i < outlineColor.Count; i++) {
                            if (outlineColor[i].state == state) {
                                SetEffectColor<Outline>(outlineColor[i].color);
                                break;
                            }
                        }
                    }
                    break;
                case UIChangeType.Sprite:
                    break;
            }
        }

        private Color GetColor<T>() where T : MaskableGraphic {
            T t = GetComponent<T>();
            return t == null ? Color.white : t.color;
        }

        private Color GetEffectColor<T>() where T : Shadow {
            T t = GetComponent<T>();
            return t == null ? Color.white : t.effectColor;
        }

        private void SetColor<T>(Color _color) where T : MaskableGraphic {
            T t = GetComponent<T>();
            if (t != null) {
                t.color = _color;
            }
        }

        private void SetEffectColor<T>(Color _color) where T : Shadow {
            T t = GetComponent<T>();
            if (t != null) {
                t.effectColor = _color;
            }
        }

        private void SetSprite(Sprite _sprite) {
            if (_sprite == null)
                return;
            Image image = GetComponent<Image>();
            if (image != null && image.sprite != _sprite) {
                image.sprite = _sprite;
            }
        }
    }

    public enum UIChangeComponentType {
        Image,
        Text,
    }

    public enum UIChangeType {
        Color,
        Sprite,
    }

    public enum UIChangeState {
        Active,
        InActive,
    }

    [System.Serializable]
    public class UIStateInfo {
        [SerializeField]
        public UIStateChangeProxy changeProxy;
        [SerializeField]
        public UIChangeState state;
        [SerializeField]
        public Color color = Color.white;
        [SerializeField]
        public Sprite sprite = null;

        public void SetProxy(UIStateChangeProxy _changeProxy) {
            changeProxy = _changeProxy;
        }

        public void SetData(UIChangeState _state, Color _color, Sprite _sprite) {
            state = _state;
            color = _color;
            sprite = _sprite;
        }
    }

}