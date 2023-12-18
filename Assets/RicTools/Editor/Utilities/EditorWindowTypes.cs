using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.Utilities
{
    internal static class EditorWindowTypes
    {
        public static Dictionary<Type, Type> typesToVisualElements = new Dictionary<Type, Type>()
        {
            { typeof(string),  typeof(TextField)},
            {typeof(int), typeof(IntegerField) },
            {typeof(long), typeof(LongField) },
            {typeof(float), typeof(FloatField) },
            {typeof(bool), typeof(Toggle) },
            {typeof(Vector2), typeof(Vector2Field) },
            {typeof(Vector3), typeof(Vector3Field) },
            {typeof(Vector2Int), typeof(Vector2IntField) },
            {typeof(Vector3Int), typeof(Vector3IntField) },
            {typeof(Color), typeof(ColorField) },
            {typeof(AnimationCurve), typeof(CurveField) },
            {typeof(UnityEngine.Object), typeof(ObjectField) },
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

        public static Type GetVisualElementType(Type type)
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
}
