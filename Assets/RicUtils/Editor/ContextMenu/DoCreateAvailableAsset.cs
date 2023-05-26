using RicUtils.Editor.Utilities;

namespace RicUtils.Editor.ContextMenu
{
    internal class DoCreateAvailableAsset : DoCreateScriptAsset
    {
        internal string scriptableObject;

        protected override string CustomReplaces(string content)
        {
            content = content.Replace("#SCRIPTABLEOBJECT#", scriptableObject);
            return content;
        }
    }
}
