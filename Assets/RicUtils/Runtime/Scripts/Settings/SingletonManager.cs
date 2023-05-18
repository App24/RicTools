using RicUtils.Managers;
using RicUtils.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TypeReferences;
using UnityEngine;

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
