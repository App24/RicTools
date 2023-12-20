using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class BoolEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(bool);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var toggle = new Toggle();
            toggle.label = label;
            toggle.value = (bool)value;

            return toggle;
        }
    }
}
