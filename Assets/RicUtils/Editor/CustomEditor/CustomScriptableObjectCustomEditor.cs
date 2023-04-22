using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RicUtils.Editor
{
    [CustomEditor(typeof(CustomScriptableObject), true)]
    [CanEditMultipleObjects]
    public class CustomScriptableObjectCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (ToolUtilities.HasCustomEditor(target.GetType()))
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((CustomScriptableObject)target), typeof(CustomScriptableObject), false);
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
                EditorGUILayout.LabelField("Check with Ricardo to ensure that there\n should be a custom editor for this", style, GUILayout.ExpandWidth(true), GUILayout.Height(50));
                EditorGUILayout.EndHorizontal();
                base.OnInspectorGUI();
            }
        }
    }
}
