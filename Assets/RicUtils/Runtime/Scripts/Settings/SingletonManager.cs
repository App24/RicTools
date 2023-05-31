using RicUtils.Managers;
using RicUtils.ScriptableObjects;
using TypeReferences;

namespace RicUtils.Settings
{
    [System.Serializable]
    internal class SingletonManager
    {
        [Inherits(typeof(SingletonGenericManager<>), ShortName = true, AllowAbstract = false, IncludeBaseType = false, ShowAllTypes = true)]
        public TypeReference manager;

        public DataManagerScriptableObject data;
    }
}
