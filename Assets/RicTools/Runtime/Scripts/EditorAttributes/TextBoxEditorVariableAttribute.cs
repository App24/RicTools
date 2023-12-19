using RicTools.Utilities;

namespace RicTools.EditorAttributes
{
    public class TextBoxEditorVariableAttribute : EditorVariableAttribute
    {
        public TextBoxEditorVariableAttribute(string label) : base(label)
        {
            ExtraData.Set("multiline", true);
        }
    }
}
