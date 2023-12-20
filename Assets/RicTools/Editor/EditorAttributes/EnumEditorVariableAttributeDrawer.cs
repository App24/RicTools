using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class EnumEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Enum);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            if (!extraData.TryGetValue("isFlagField", out var isFlagField))
                isFlagField = false;
            if ((bool)isFlagField)
            {
                var enumFlagField = new EnumFlagsField()
                {
                    label = label,
                    value = (Enum)value
                };
                enumFlagField.Init((Enum)value);

                return enumFlagField;
            }

            var enumField = new EnumField()
            {
                label = label,
                value = (Enum)value
            };
            enumField.Init((Enum)value);

            return enumField;
        }
    }
}
