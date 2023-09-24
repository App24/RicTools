namespace RicTools.Editor.Utilities
{
    public sealed class EditorContainer<TValueType>
    {
        public TValueType Value { get; set; } = default;

        public static implicit operator TValueType(EditorContainer<TValueType> value) { return value.Value; }
        public static explicit operator EditorContainer<TValueType>(TValueType value) { return new EditorContainer<TValueType>() { Value = value }; }

        public EditorContainer() : this(default)
        {
        }

        public EditorContainer(TValueType value)
        {
            Value = value;
        }

        public bool IsNull()
        {
            return Value == null;
        }

        public override string ToString()
        {
            if (IsNull()) return null;
            return Value.ToString();
        }
    }
}