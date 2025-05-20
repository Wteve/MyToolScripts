#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MyTool {

    [CustomPropertyDrawer(typeof(UIStateInfo))]
    public class UIStateInfoEditor : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            using (new EditorGUI.PropertyScope(position, label, property)) {
                //设置属性名宽度
                EditorGUIUtility.labelWidth = 50;
                position.height = EditorGUIUtility.singleLineHeight;

                var stateRect = new Rect(position) {
                    width = position.width / 3,
                };

                var colorRect = new Rect(position) {
                    x = stateRect.x + stateRect.width,
                    width = position.width / 3,
                };

                var useRect = new Rect(position) {
                    width = position.width / 3,
                    x = colorRect.x + colorRect.width,
                };

                UIStateChangeProxy changeProxy = (UIStateChangeProxy)(property.FindPropertyRelative("changeProxy").objectReferenceValue);
                var stateProperty = property.FindPropertyRelative("state");
                var colorProperty = property.FindPropertyRelative("color");
                var spriteProperty = property.FindPropertyRelative("sprite");

                GUI.enabled = false;
                stateProperty.enumValueIndex = (int)(UIChangeState)EditorGUI.EnumPopup(stateRect, (UIChangeState)stateProperty.enumValueIndex);

                GUI.enabled = true;
                switch (changeProxy.changeType) {
                    case UIChangeType.Color:
                        colorProperty.colorValue = EditorGUI.ColorField(colorRect, colorProperty.colorValue);
                        break;
                    case UIChangeType.Sprite:
                        spriteProperty.objectReferenceValue = EditorGUI.ObjectField(colorRect, spriteProperty.objectReferenceValue, typeof(Sprite));
                        break;
                }

                if (GUI.Button(useRect, "Set")) {
                    changeProxy.SetColor((UIChangeState)stateProperty.enumValueIndex);
                }
            }
        }
    }

    [CustomEditor(typeof(UIStateChangeProxy))]
    public class UIStateChangeProxyEditor : Editor {

        public static SerializedProperty uiType, changeType, useShadow, useOutline;
        private ReorderableList baseColor, shadowColor, outlineColor;

        private UIStateChangeProxy changeProxy;

        private void OnEnable() {
            changeProxy = (UIStateChangeProxy)target;

            uiType = serializedObject.FindProperty("uiType");
            changeType = serializedObject.FindProperty("changeType");
            useShadow = serializedObject.FindProperty("useShadow");
            useOutline = serializedObject.FindProperty("useOutline");

            baseColor = new ReorderableList(serializedObject, serializedObject.FindProperty("baseColor"), false, false, false, false);
            baseColor.drawHeaderCallback = (Rect rect) => {
                GUI.Label(rect, "Base Color");
            };
            //自定义绘制列表元素
            baseColor.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => {
                //根据index获取对应元素
                SerializedProperty item = baseColor.serializedProperty.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;
                EditorGUI.PropertyField(rect, item, new GUIContent("Index " + index));
            };
            shadowColor = new ReorderableList(serializedObject, serializedObject.FindProperty("shadowColor"), false, false, false, false);
            shadowColor.drawHeaderCallback = (Rect rect) => {
                GUI.Label(rect, "Shadow Color");
            };
            //自定义绘制列表元素
            shadowColor.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => {
                //根据index获取对应元素
                SerializedProperty item = shadowColor.serializedProperty.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;
                EditorGUI.PropertyField(rect, item, new GUIContent("Index " + index));
            };
            outlineColor = new ReorderableList(serializedObject, serializedObject.FindProperty("outlineColor"), false, false, false, false);
            outlineColor.drawHeaderCallback = (Rect rect) => {
                GUI.Label(rect, "Outline Color");
            };
            //自定义绘制列表元素
            outlineColor.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => {
                //根据index获取对应元素
                SerializedProperty item = outlineColor.serializedProperty.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;
                EditorGUI.PropertyField(rect, item, new GUIContent("Index " + index));
            };
        }

        public override void OnInspectorGUI() {
            //更新编辑器显示的序列化属性
            serializedObject.Update();

            bool canShowChange = false;
            switch (changeProxy.uiType) {
                case UIChangeComponentType.Image:
                    canShowChange = true;
                    break;
                case UIChangeComponentType.Text:
                    changeProxy.changeType = UIChangeType.Color;
                    break;
            }

            EditorGUILayout.PropertyField(uiType);

            if (canShowChange) {
                EditorGUILayout.PropertyField(changeType);
            }

            baseColor.DoLayoutList();

            if (changeProxy.changeType == UIChangeType.Color) {
                EditorGUILayout.PropertyField(useShadow);
                if (changeProxy.useShadow) {
                    shadowColor.DoLayoutList();
                }

                EditorGUILayout.PropertyField(useOutline);
                if (changeProxy.useOutline) {
                    outlineColor.DoLayoutList();
                }
            }

            changeProxy?.InitColor();

            //接受序列化赋值
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif