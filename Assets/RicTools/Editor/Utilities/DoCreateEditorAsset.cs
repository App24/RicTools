namespace RicTools.Editor.Utilities
{
    internal class DoCreateEditorAsset : DoCreateScriptAsset
    {
        internal string scriptableObject;
        internal string availableScriptableObject;

        protected override string CustomReplaces(string content)
        {
            content = content.Replace("#SCRIPTABLEOBJECT#", scriptableObject);
            content = content.Replace("#AVAILABLESCRIPTABLEOBJECT#", availableScriptableObject);
            return content;
        }
    }
}
