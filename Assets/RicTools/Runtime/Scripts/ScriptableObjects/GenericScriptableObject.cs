using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RicTools.ScriptableObjects
{
    public class GenericScriptableObject : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        internal bool setForDeletion;
        public string guid;
    }
}
