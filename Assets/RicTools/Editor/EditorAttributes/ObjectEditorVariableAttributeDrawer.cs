using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class ObjectEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(UnityEngine.Object);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var objectField = new ObjectField();

            if (!extraData.TryGetValue("allowSceneObjects", out var allowSceneObjects))
            {
                allowSceneObjects = false;
            }

            objectField.label = label;
            objectField.allowSceneObjects = (bool)allowSceneObjects;
            objectField.value = (UnityEngine.Object)value;
            objectField.objectType = fieldType;

            return objectField;
        }
    }
}
