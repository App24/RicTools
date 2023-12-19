using RicTools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicTools
{
    [CustomEditor(typeof(SetupScriptableObject))]
    public class SetupScriptableObjectCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var setup = (SetupScriptableObject)target;

            if (GUILayout.Button("Setup"))
            {
                foreach(var folder in setup.folders)
                {
                    RicUtilities.CreateAssetFolder($"Assets/{folder}");
                }

                FileUtil.DeleteFileOrDirectory("Assets/RicTools/Scripts/Setup");
            }
        }
    }
}
