using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    public static class EditorGUIHelper
    {
        public const float LABEL_WIDTH = 125;

        public static GUIStyle labelStyle = GUI.skin.label;

        public static GUILayoutOption[] labelGUILayoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(LABEL_WIDTH) };

        public static void DrawLabel(string text)
        {
            EditorGUILayout.LabelField(text, labelStyle, labelGUILayoutOptions);
        }

        public static void ResetLabelStyle()
        {
            labelStyle = GUI.skin.label;
        }

        public static void ResetLabelGUILayoutOptions()
        {
            labelGUILayoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(LABEL_WIDTH) };
        }

        public static void DrawTitle(string text)
        {
            GUILayout.BeginHorizontal();
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            labelGUILayoutOptions = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true)
            };
            DrawLabel(text);
            GUILayout.EndHorizontal();
            ResetLabelStyle();
            ResetLabelGUILayoutOptions();
        }

        public static void AddIndentAmount(float amountPerIndent = LABEL_WIDTH)
        {
            GUILayout.Space(EditorGUI.indentLevel * amountPerIndent);
        }

        public static void DrawStringInput(ref string id, string text = "String", System.Action onSelectionChange = null)
        {
            var previous = id;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            id = EditorGUILayout.TextField(id);
            GUILayout.EndHorizontal();
            if (previous != id) onSelectionChange?.Invoke();
        }

        public static void DrawStringTextBox(ref string id, ref Vector2 scroll, string text = "String", System.Action onSelectionChange = null)
        {
            var previous = id;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(105));
            id = EditorGUILayout.TextArea(id, GUILayout.Height(100));
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (previous != id) onSelectionChange?.Invoke();
        }

        public static void DrawObjectField<T>(ref T data, string text = "Object Field", System.Action onSelectionChange = null, bool allowScene = false) where T : Object
        {
            var previous = data;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            data = (T)EditorGUILayout.ObjectField(data, typeof(T), allowScene);
            GUILayout.EndHorizontal();
            if (previous != data) onSelectionChange?.Invoke();
        }

        public static void DrawSeparator()
        {
            DrawSeparator(Color.grey);
        }

        public static void DrawSeparator(Color color)
        {
            GUIStyle horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;

            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

        public static void DrawEnumPopup<T>(ref T data, string text = "Enum Popup", System.Action onSelectionChange = null, bool allowScene = false) where T : System.Enum
        {
            var previous = data;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            data = (T)EditorGUILayout.EnumPopup(data);
            GUILayout.EndHorizontal();
            if (!previous.Equals(data)) onSelectionChange?.Invoke();
        }

        public static void DrawPreviewModel(UnityEditor.Editor gameObjectEditor)
        {
            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.colorField);
            gameObjectEditor.ReloadPreviewInstances();
        }

        public static void DrawColorField(ref Color color, bool allowAlpha = true, string text = "Color", System.Action onSelectionChange = null)
        {
            var previous = color;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            color = EditorGUILayout.ColorField(color);
            if (!allowAlpha)
                color.a = 1;
            GUILayout.EndHorizontal();
            if (previous != color) onSelectionChange?.Invoke();
        }

        public static void DrawFloat(ref float value, string text = "Float", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.FloatField(value);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawVector3(ref Vector3 value, string text = "Vector 3", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.Vector3Field(new GUIContent(), value);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawInt(ref int value, string text = "Int", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.IntField(value);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawToggle(ref bool value, string text = "Boolean", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.Toggle(value);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawSlider(ref float value, float min, float max, string text = "Slider", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.Slider(value, min, max);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawSliderInt(ref int value, int min, int max, string text = "Int", System.Action onSelectionChange = null)
        {
            var previous = value;
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            value = EditorGUILayout.IntSlider(value, min, max);
            GUILayout.EndHorizontal();
            if (previous != value) onSelectionChange?.Invoke();
        }

        public static void DrawAnimationCurve(ref AnimationCurve animationCurve, string text = "Animation Curve", System.Action onSelectionChange = null)
        {
            var previous = animationCurve;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(text, GUILayout.MaxWidth(125));
            animationCurve = EditorGUILayout.CurveField(animationCurve);
            GUILayout.EndHorizontal();
            if (previous != animationCurve) onSelectionChange?.Invoke();
        }

        public static void DrawPropertyField(SerializedProperty serializedProperty, string text)
        {
            GUILayout.BeginHorizontal();
            DrawLabel(text);
            EditorGUILayout.PropertyField(serializedProperty);
            GUILayout.EndHorizontal();
        }
    }
}
