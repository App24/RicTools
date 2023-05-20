using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicUtils.Editor.Utilities
{
    public static class EditorGUIHelper
    {
        public const float LABEL_WIDTH = 125;

        public static GUIStyle labelStyle;

        public static GUILayoutOption[] labelGUILayoutOptions;

        public static void DrawLabel(string text)
        {
            if (labelStyle == null) ResetLabelStyle();
            if (labelGUILayoutOptions == null) ResetLabelGUILayoutOptions();
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

        public static ObjectField AddObjectField<T>(this VisualElement root, EditorContainer<T> data, string text = "Object Field", System.Action onSelectionChange = null, bool allowScene = false) where T : Object
        {
            var objectField = new ObjectField()
            {
                label = text,
                allowSceneObjects = allowScene,
                objectType = typeof(T),
                value = data.Value,
            };

            objectField.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue as T;
                onSelectionChange?.Invoke();
            });

            root.Add(objectField);

            return objectField;
        }

        public static ObjectField AddTextureField(this VisualElement root, EditorContainer<Texture2D> data, string text = "Object Field", System.Action onSelectionChange = null)
        {
            var field = root.AddObjectField(data, text, onSelectionChange);

            return field;
        }

        public static EnumField AddEnumField<T>(this VisualElement root, EditorContainer<T> data, string text = "Enum Popup", System.Action onSelectionChange = null) where T : System.Enum
        {
            var enumField = new EnumField()
            {
                value = data.Value,
                label = text
            };

            enumField.Init(data.Value);

            enumField.RegisterValueChangedCallback(callback =>
            {
                data.Value = (T)callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(enumField);

            return enumField;
        }

        public static TextField AddTextField(this VisualElement root, EditorContainer<string> data, string text = "String", System.Action<string> onSelectionChange = null)
        {
            var textField = new TextField()
            {
                value = data.Value,
                label = text,
            };

            textField.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke(callback.previousValue);
            });

            root.Add(textField);

            return textField;
        }

        public static TextField AddTextBox(this VisualElement root, EditorContainer<string> data, string text = "String", System.Action<string> onSelectionChange = null)
        {
            var textField = root.AddTextField(data, text, onSelectionChange);

            textField.multiline = true;

            return textField;
        }

        public static ColorField AddColorField(this VisualElement root, EditorContainer<Color> data, string text = "Color", bool showAlpha = true, bool hdr = false, System.Action onSelectionChange = null)
        {
            var colorField = new ColorField()
            {
                label = text,
                value = data.Value,
                showAlpha = showAlpha,
                hdr = hdr,
            };

            colorField.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(colorField);

            return colorField;
        }

        public static FloatField AddFloatField(this VisualElement root, EditorContainer<float> data, string text = "Float", System.Action onSelectionChange = null)
        {
            var floatField = new FloatField()
            {
                label = text,
                value = data.Value,
            };

            floatField.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(floatField);

            return floatField;
        }

        public static IntegerField AddIntField(this VisualElement root, EditorContainer<int> data, string text = "Int", System.Action onSelectionChange = null)
        {
            var intField = new IntegerField()
            {
                label = text,
                value = data.Value,
            };

            intField.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(intField);

            return intField;
        }

        public static Toggle AddToggle(this VisualElement root, EditorContainer<bool> data, string text = "Boolean", System.Action onSelectionChange = null)
        {
            var toggle = new Toggle()
            {
                label = text,
                value = data.Value,
            };

            toggle.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(toggle);

            return toggle;
        }

        public static Vector3Field AddVector3Field(this VisualElement root, EditorContainer<Vector3> data, string text = "Vector 3", System.Action onSelectionChange = null)
        {
            var field = new Vector3Field()
            {
                label = text,
                value = data.Value,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static Vector2Field AddVector2Field(this VisualElement root, EditorContainer<Vector2> data, string text = "Vector 2", System.Action onSelectionChange = null)
        {
            var field = new Vector2Field()
            {
                label = text,
                value = data.Value,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static Vector3IntField AddVector3IntField(this VisualElement root, EditorContainer<Vector3Int> data, string text = "Vector 3 Int", System.Action onSelectionChange = null)
        {
            var field = new Vector3IntField()
            {
                label = text,
                value = data.Value,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static Vector2IntField AddVector2IntField(this VisualElement root, EditorContainer<Vector2Int> data, string text = "Vector 2 Int", System.Action onSelectionChange = null)
        {
            var field = new Vector2IntField()
            {
                label = text,
                value = data.Value,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static Slider AddSlider(this VisualElement root, EditorContainer<float> data, float lowValue, float highValue, string text = "Slider", System.Action onSelectionChange = null)
        {
            var field = new Slider()
            {
                label = text,
                value = data.Value,
                lowValue = lowValue,
                highValue = highValue,
                showInputField = true,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static SliderInt AddSliderInt(this VisualElement root, EditorContainer<int> data, int lowValue, int highValue, string text = "Slider Int", System.Action onSelectionChange = null)
        {
            var field = new SliderInt()
            {
                label = text,
                value = data.Value,
                lowValue = lowValue,
                highValue = highValue,
                showInputField = true,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static CurveField AddCurveField(this VisualElement root, EditorContainer<AnimationCurve> data, string text = "Curve", System.Action onSelectionChange = null)
        {
            var field = new CurveField()
            {
                label = text,
                value = data.Value,
            };

            field.RegisterValueChangedCallback(callback =>
            {
                data.Value = callback.newValue;
                onSelectionChange?.Invoke();
            });

            root.Add(field);

            return field;
        }

        public static PropertyField AddPropertyField(this VisualElement root, SerializedProperty data, string text = "Property Field", System.Action onSelectionChange = null)
        {
            var field = new PropertyField(data)
            {
                label = text
            };

            field.RegisterValueChangeCallback(callback =>
            {
                data.serializedObject.ApplyModifiedProperties();
                onSelectionChange?.Invoke();
            });

            root.Add(field);
            root.Bind(data.serializedObject);

            return field;
        }

        /*public static void DrawPreviewModel(UnityEditor.Editor gameObjectEditor)
        {
            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.colorField);
            gameObjectEditor.ReloadPreviewInstances();
        }*/

        public static Button AddButton(this VisualElement root, string text = "Button", System.Action onClick = null)
        {
            var button = new Button(onClick)
            {
                text = text
            };

            root.Add(button);

            return button;
        }

        public static VisualElement AddSeparator(this VisualElement root)
        {
            return root.AddSeparator(Color.grey);
        }

        public static VisualElement AddSeparator(this VisualElement root, Color color)
        {
            var box = new Image();

            box.style.backgroundImage = EditorGUIUtility.whiteTexture;
            box.style.unityBackgroundImageTintColor = color;

            box.style.marginTop = 4;
            box.style.marginBottom = 4;

            box.style.height = 1;

            box.focusable = false;

            root.Add(box);

            return box;
        }

        public static Label AddLabel(this VisualElement root, string text)
        {
            var label = new Label()
            {
                text = text
            };

            root.Add(label);

            return label;
        }

        public static Label AddTitle(this VisualElement root, string text)
        {
            var label = root.AddLabel(text);

            label.style.fontSize = 15;
            label.style.alignSelf = Align.Center;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;

            return label;
        }
    }
}
