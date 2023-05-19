namespace RicUtils.Editor.Utilities
{
    public sealed class EditorContainer<T>
    {
        public T Value { get; set; } = default;

        public static implicit operator T(EditorContainer<T> value) { return value.Value; }
        public static explicit operator EditorContainer<T>(T value) { return new EditorContainer<T>() { Value = value }; }

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