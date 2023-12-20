using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class ColorEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Color);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var colorField = new ColorField();

            colorField.label = label;

            colorField.value = (Color)value;

            if (!extraData.TryGetValue("showAlpha", out var showAlpha))
            {
                showAlpha = true;
            }
            if (!extraData.TryGetValue("hdr", out var hdr))
            {
                hdr = false;
            }

            colorField.showAlpha = (bool)showAlpha;
            colorField.hdr = (bool)hdr;

            return colorField;
        }
    }
}
