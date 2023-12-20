using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class IntEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(int);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            if (extraData.TryGetValue("isSlider", out var _))
            {
                extraData.TryGetValue("minValue", out var minValue);
                if (minValue.GetType() == typeof(float))
                    minValue = (int)Mathf.Floor((float)minValue);

                extraData.TryGetValue("maxValue", out var maxValue);
                if (maxValue.GetType() == typeof(float))
                    maxValue = (int)Mathf.Floor((float)maxValue);

                if (!extraData.TryGetValue("showInputField", out var showInputField))
                    showInputField = true;

                var sliderInt = new SliderInt()
                {
                    lowValue = (int)minValue,
                    highValue = (int)maxValue,
                    showInputField = (bool)showInputField,
                    label = label,
                    value = (int)value
                };

                return sliderInt;
            }

            IntegerField integerField = new IntegerField();
            integerField.label = label;
            integerField.value = (int)value;

            return integerField;
        }
    }
}
