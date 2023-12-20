using RicTools.EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public abstract class EditorVariableAttributeDrawer
    {
        public Type EditorVariableAttributeType { get; private set; }
        public abstract Type FieldType { get; }

        public EditorVariableAttributeDrawer()
        {
            SetEditorVariableAttributeType<EditorVariableAttribute>();
        }

        public void SetEditorVariableAttributeType<T>() where T : EditorVariableAttribute
        {
            EditorVariableAttributeType = typeof(T);
        }

        public abstract VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData);
    }
}
