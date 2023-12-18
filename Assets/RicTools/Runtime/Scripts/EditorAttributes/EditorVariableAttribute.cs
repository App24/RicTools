using System;

namespace RicTools.EditorAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EditorVariableAttribute : Attribute
    {
        public string Label { get; set; }
        public object DefaultValue { get; set; }

        public EditorVariableAttribute(string label)
        {
            this.Label = label;
        }
    }
}
