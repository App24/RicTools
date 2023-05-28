using RicUtils.Editor.Utilities;
using RicUtils.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor.CustomEditors
{
    [UnityEditor.CustomEditor(typeof(GenericScriptableObject), true)]
    [CanEditMultipleObjects]
    public class CustomScriptableObjectCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (ToolUtilities.HasCustomEditor(target.GetType()))
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((GenericScriptableObject)target), typeof(GenericScriptableObject), false);
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 15
                };
                EditorGUILayout.LabelField("Double Click to Open Editor Window", style, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open Editor"))
                {
                    AssetDatabase.OpenAsset(target);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                var style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 15
                };
                EditorGUILayout.LabelField("There is no custom editor for this scriptable object.\nCheck if there should be one.", style, GUILayout.ExpandWidth(true), GUILayout.Height(50));
                EditorGUILayout.EndHorizontal();
                base.OnInspectorGUI();
            }
        }
    }
}
