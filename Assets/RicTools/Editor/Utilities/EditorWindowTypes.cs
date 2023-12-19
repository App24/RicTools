using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.Utilities
{
    internal static class EditorWindowTypes
    {
        private static Dictionary<Type, TypeVisualElementData> typesToVisualElements = new Dictionary<Type, TypeVisualElementData>()
        {
            { typeof(string),  new TypeVisualElementData(typeof(TextField), (visualElement, _, _, data)=>{
                if(!data.TryGetValue("multiline", out var multiline))
                    return visualElement;

                visualElement.GetType().GetProperty("multiline").SetValue(visualElement, multiline);
                return visualElement;
            })},
            {typeof(int), new TypeVisualElementData(typeof(IntegerField), (visualElement, _, _, data) =>
            {
                if(!data.TryGetValue("isSlider", out var _))
                {
                    return visualElement;
                }
                data.TryGetValue("minValue", out var minValue);
                if(minValue.GetType() == typeof(float))
                    minValue = (int)Mathf.Floor((float)minValue);

                data.TryGetValue("maxValue", out var maxValue);
                if(maxValue.GetType() == typeof(float))
                    maxValue = (int)Mathf.Floor((float)maxValue);

                if(!data.TryGetValue("showInputField", out var showInputField))
                    showInputField = true;

                visualElement = new SliderInt()
                {
                    lowValue = (int)minValue,
                    highValue = (int)maxValue,
                    showInputField = (bool)showInputField
                };
                return visualElement;
            }) },
            {typeof(long), new TypeVisualElementData(typeof(LongField)) },
            {typeof(float), new TypeVisualElementData(typeof(FloatField), (visualElement, _, _, data) =>
            {
                if(!data.TryGetValue("isSlider", out var _))
                {
                    return visualElement;
                }
                data.TryGetValue("minValue", out var minValue);
                data.TryGetValue("maxValue", out var maxValue);

                if(minValue.GetType() == typeof(int))
                    minValue = Convert.ToSingle(minValue);

                if(maxValue.GetType() == typeof(int))
                    maxValue = Convert.ToSingle(maxValue);

                if(!data.TryGetValue("showInputField", out var showInputField))
                    showInputField = true;

                visualElement = new Slider()
                {
                    lowValue = (float)minValue,
                    highValue = (float)maxValue,
                    showInputField = (bool)showInputField
                };
                return visualElement;
            }) },
            {typeof(bool), new TypeVisualElementData(typeof(Toggle)) },
            {typeof(Vector2), new TypeVisualElementData(typeof(Vector2Field)) },
            {typeof(Vector3), new TypeVisualElementData(typeof(Vector3Field)) },
            {typeof(Vector2Int), new TypeVisualElementData(typeof(Vector2IntField)) },
            {typeof(Vector3Int), new TypeVisualElementData(typeof(Vector3IntField)) },
            {typeof(Color), new TypeVisualElementData(typeof(ColorField), (visualElement, _, _, extraData) =>
            {
                if(!extraData.TryGetValue("showAlpha", out var showAlpha))
                {
                    showAlpha = true;
                }
                if(!extraData.TryGetValue("hdr", out var hdr))
                {
                    hdr = false;
                }
                visualElement.GetType().GetProperty("showAlpha").SetValue(visualElement, showAlpha);
                visualElement.GetType().GetProperty("hdr").SetValue(visualElement, hdr);
                return visualElement;
            }) },
            {typeof(AnimationCurve), new TypeVisualElementData(typeof(CurveField)) },
            {typeof(UnityEngine.Object), new TypeVisualElementData(typeof(ObjectField), (visualElement, _, fieldType, data) =>
            {
                if(!data.TryGetValue("allowSceneObjects", out var allowSceneObjects))
                {
                    allowSceneObjects = false;
                }
                var type = visualElement.GetType();
                type.GetProperty("allowSceneObjects").SetValue(visualElement, allowSceneObjects);
                type.GetProperty("objectType").SetValue(visualElement, fieldType);
                return visualElement;
            }) },
            { typeof(Enum), new TypeVisualElementData(typeof(EnumField), (visualElement, value, _, _) =>
            {
                visualElement.GetType().GetMethod("Init", new Type[]{ typeof(Enum) }).Invoke(visualElement, new object[]{ value });
                return visualElement;
            }) }
        };

        public static bool HasTypeToVisualElement(Type type)
        {
            if (typesToVisualElements.ContainsKey(type))
                return true;

            foreach (var keyValuePair in typesToVisualElements)
            {
                if (type.IsSubclassOf(keyValuePair.Key)) return true;
            }

            return false;
        }

        public static TypeVisualElementData GetVisualElementType(Type type)
        {
            if (!typesToVisualElements.TryGetValue(type, out var result))
            {
                foreach (var keyValuePair in typesToVisualElements)
                {
                    if (type.IsSubclassOf(keyValuePair.Key))
                    {
                        result = keyValuePair.Value;
                    }
                }
            }

            return result;
        }
    }

    internal class TypeVisualElementData
    {
        public Type visualElementType;
        public TypeVisualElementDataProcessingDelegate extraVisualElementProcessing;

        public TypeVisualElementData(Type visualElementType)
        {
            this.visualElementType = visualElementType;
        }

        public TypeVisualElementData(Type visualElementType, TypeVisualElementDataProcessingDelegate extraVisualElementProcessing) : this(visualElementType)
        {
            this.extraVisualElementProcessing = extraVisualElementProcessing;
        }

        public VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var visualElement = System.Activator.CreateInstance(this.visualElementType);
            visualElement = extraVisualElementProcessing?.Invoke((VisualElement)visualElement, value, fieldType, extraData);
            var visualElementType = visualElement.GetType();
            var labelPropertyInfo = visualElementType.GetProperty("label");
            if (labelPropertyInfo != null)
            {
                labelPropertyInfo.SetValue(visualElement, label);
            }
            visualElementType.GetProperty("value").SetValue(visualElement, value);
            return (VisualElement)visualElement;
        }
    }

    internal delegate VisualElement TypeVisualElementDataProcessingDelegate(VisualElement visualElement, object value, Type fieldType, Dictionary<string, object> extraData);
}
