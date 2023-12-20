using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.EditorAttributes
{
    public class Vector2IntEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Vector2Int);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var field = new Vector2IntField();
            field.label = label;
            field.value = (Vector2Int)value;

            return field;
        }
    }

    public class Vector3IntEditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Vector3Int);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var field = new Vector3IntField();
            field.label = label;
            field.value = (Vector3Int)value;

            return field;
        }
    }

    public class Vector2EditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Vector2);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var field = new Vector2Field();
            field.label = label;
            field.value = (Vector2)value;

            return field;
        }
    }

    public class Vector3EditorVariableAttributeDrawer : EditorVariableAttributeDrawer
    {
        public override Type FieldType => typeof(Vector3);

        public override VisualElement CreateVisualElement(string label, object value, Type fieldType, Dictionary<string, object> extraData)
        {
            var field = new Vector3Field();
            field.label = label;
            field.value = (Vector3)value;

            return field;
        }
    }
}
