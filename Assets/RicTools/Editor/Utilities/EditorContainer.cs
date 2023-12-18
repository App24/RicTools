using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RicTools.Editor.Utilities
{
    [Serializable]
    public sealed class EditorContainer<TValueType>
    {
        public TValueType Value { get; set; } = default;
        private readonly TValueType defaultValue;

        public static implicit operator TValueType(EditorContainer<TValueType> value) { return value.Value; }
        public static explicit operator EditorContainer<TValueType>(TValueType value) { return new EditorContainer<TValueType>() { Value = value }; }

        public EditorContainer() : this(default)
        {
        }

        public EditorContainer(TValueType value)
        {
            Value = value;
            defaultValue = value;
        }

        public void Reset()
        {
            Value = defaultValue;
        }
    }

    [Serializable]
    public sealed class EditorContainerList<TValueType>
    {
        internal List<TValueType> List { get; set; } = new List<TValueType>();

        public void Load(IEnumerable<TValueType> list)
        {
            List = new List<TValueType>(list.Copy());
        }

        public void Clear()
        {
            List.Clear();
        }

        public TValueType[] ToArray()
        {
            return List.ToArray();
        }

        public List<TValueType> ToList()
        {
            return List.Copy();
        }
    }
}