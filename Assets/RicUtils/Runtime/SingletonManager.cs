using RicUtils.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TypeReferences;
using UnityEngine;

namespace RicUtils
{
    [System.Serializable]
    public class SingletonManager
    {
        [Inherits(typeof(SingletonGenericManager<>), ShortName = true, AllowAbstract = false, IncludeBaseType = false, ShowAllTypes = true)]
        public TypeReference manager;

        public DataManagerScriptableObject data;
    }
}
