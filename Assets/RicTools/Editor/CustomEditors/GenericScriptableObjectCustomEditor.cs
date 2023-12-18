using RicTools.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicTools.Editor.CustomEditors
{
    [CustomEditor(typeof(GenericScriptableObject), true)]
    internal class GenericScriptableObjectCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}
