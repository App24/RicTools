namespace RicTools.EditorAttributes
{
    public class ObjectEditorVariableAttribute : EditorVariableAttribute
    {
        public bool AllowSceneObjects { get; set; }

        public ObjectEditorVariableAttribute(string label) : base(label)
        {
        }
    }
}
