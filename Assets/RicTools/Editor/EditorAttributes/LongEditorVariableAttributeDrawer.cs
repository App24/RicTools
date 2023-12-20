using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class LongEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(long);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var longField = new LongField();

            longField.label = label;
            longField.value = (long)value;

            return longField;
        }
    }
}
