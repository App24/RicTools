using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class StringEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(string);

        public override VisualElement CreateVisualElement(string label, object data, Type fieldType, Dictionary<string, object> extraData)
        {
            TextField textField = new TextField();
            textField.label = label;
            textField.value = (string)data;

            if (!extraData.TryGetValue("multiline", out var multiline))
                multiline = false;

            textField.multiline = (bool)multiline;

            return textField;
        }
    }
}
