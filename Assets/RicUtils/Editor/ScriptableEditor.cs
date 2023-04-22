using TypeReferences;
using UnityEngine;
using UnityEditor;

namespace RicUtils.Editor
{
    [System.Serializable]
    public class ScriptableEditor
    {
        [Inherits(typeof(CustomScriptableObject), ShortName = true, AllowAbstract = false, IncludeBaseType = false, ShowAllTypes = true)]
        public TypeReference customScriptableObjectType;

        [Inherits(typeof(GenericEditorWindow<,>), ShortName = true, AllowAbstract = false, IncludeBaseType = false, ShowAllTypes = true)]
        public TypeReference editorType;

        [Inherits(typeof(AvailableScriptObject<>), ShortName = true, AllowAbstract = false, IncludeBaseType = false, ShowAllTypes = true)]
        public TypeReference availableScriptableObjectType;

        public System.Type Key => customScriptableObjectType;
        public (System.Type, System.Type) Value => (availableScriptableObjectType, editorType);

        public bool IsSameKeyType(System.Type type)
        {
            if(type == null || Key == null) return false;
            return type == Key || type.IsSubclassOf(Key);
        }
    }
}
