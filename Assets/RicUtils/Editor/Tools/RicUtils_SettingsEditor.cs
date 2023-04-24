using RicUtils.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;

namespace RicUtils.Editor
{
    public class RicUtils_SettingsEditor : EditorWindow
    {
        internal class Styles
        {
            public static readonly GUIContent scriptableEditorsLabel = new GUIContent("Scriptable Editors");
            public static readonly GUIContent scriptableEditorsListLabel = new GUIContent("Scriptable Editors List");
            public static readonly GUIContent singletonManagersLabel = new GUIContent("Singleton Managers");
            public static readonly GUIContent singletonManagersListLabel = new GUIContent("Singleton Managers List");

            public static readonly GUIContent scriptableEditorsAddButtonLabel = new GUIContent("Add Scriptable Editor");
        }

        private RicUtils_EditorSettings editorSettings;
        private RicUtils_Settings settings;

        private ReorderableList m_scriptableEditorsList;
        private ReorderableList m_singletonManagersList;

        private const string k_UndoRedo = "UndoRedoPerformed";

        private SerializedObject editorSerializedObject;
        private SerializedObject serializedObject;

        [MenuItem("RicUtils/Settings", priority = 0)]
        public static RicUtils_SettingsEditor ShowWindow()
        {
            return GetWindow<RicUtils_SettingsEditor>("RicUtils Settings");
        }

        private void OnEnable()
        {
            editorSettings = RicUtils_EditorSettings.instance;
            settings = RicUtils_Settings.instance;

            editorSerializedObject = new SerializedObject(editorSettings);
            serializedObject = new SerializedObject(settings);

            m_scriptableEditorsList = new ReorderableList(editorSerializedObject, editorSerializedObject.FindProperty("m_scriptableEditors"), false, true, true, true);

            m_singletonManagersList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_singletonManagers"), false, true, true, true);

            m_scriptableEditorsList.elementHeight *= 3;

            m_scriptableEditorsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = m_scriptableEditorsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float labelWidth = 200;
                float width = rect.width - labelWidth;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Scriptable Object Type");
                //rect.x += 110;
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("customScriptableObjectType"), GUIContent.none);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                //rect.x += width + 5;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Editor Type");
                //rect.x += 70;
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("editorType"), GUIContent.none);
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                //rect.x += width + 5;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Available Scriptable Object Type");
                //rect.x += 110;
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("availableScriptableObjectType"), GUIContent.none);

                /*var temp = element.objectReferenceValue as ScriptableEditor;

                if (!temp)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                    return;
                }

                if (temp.serializedObject == null) temp.serializedObject = new SerializedObject(temp);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), temp.serializedObject.FindProperty("customScriptableObjectType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + width + 5, rect.y, width, EditorGUIUtility.singleLineHeight), temp.serializedObject.FindProperty("editorType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + ((width + 5) * 2), rect.y, width, EditorGUIUtility.singleLineHeight), temp.serializedObject.FindProperty("availableScriptableObjectType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + ((width + 8) * 3), rect.y, 6, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

                temp.serializedObject.ApplyModifiedProperties();*/
            };

            m_scriptableEditorsList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, Styles.scriptableEditorsListLabel);
            };

            m_singletonManagersList.elementHeightCallback = index =>
            {
                var manager = settings.m_singletonManagers[index].manager.Type;
                if (RicUtilities.IsSubclassOfRawGeneric(typeof(DataGenericManager<,>), manager))
                {
                    return 42;
                }
                return 21;
            };

            m_singletonManagersList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = m_singletonManagersList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float labelWidth = 200;
                float width = rect.width - labelWidth;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Manager");
                //rect.x += 110;
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("manager"), GUIContent.none);

                var manager = settings.m_singletonManagers[index].manager.Type;
                if (RicUtilities.IsSubclassOfRawGeneric(typeof(DataGenericManager<,>), manager))
                {
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Data");
                    GUI.enabled = settings.m_singletonManagers[index].data == null;
                    if (settings.m_singletonManagers[index].data == null)
                    {
                        if (GUI.Button(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), "Create"))
                        {
                            RicUtilities.CreateAssetFolder("Assets/ScriptableObject/Managers Data");

                            var data = ScriptableObject.CreateInstance(manager.BaseType.GenericTypeArguments[1]);
                            if (!AssetDatabase.Contains(data))
                                AssetDatabase.CreateAsset(data, $"Assets/ScriptableObject/Managers Data/{manager.Name}_data.asset");

                            settings.m_singletonManagers[index].data = data as DataManagerScriptableObject;

                            EditorUtility.SetDirty(settings);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("data"), GUIContent.none);
                    }
                    GUI.enabled = true;
                }
            };

            m_singletonManagersList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, Styles.singletonManagersListLabel);
            };
        }

        private void OnGUI()
        {
            editorSerializedObject.Update();
            serializedObject.Update();
            string evt_cmd = Event.current.commandName;

            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.scriptableEditorsLabel, EditorStyles.boldLabel);
            m_scriptableEditorsList.DoLayoutList();

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.singletonManagersLabel, EditorStyles.boldLabel);
            m_singletonManagersList.DoLayoutList();

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();


            /*EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (GUILayout.Button(Styles.scriptableEditorsAddButtonLabel))
            {
                var old = new List<ScriptableEditor>((target as RicUtils_Settings).m_scriptableEditors);
                old.Add(new ScriptableEditor());
                (target as RicUtils_Settings).m_scriptableEditors = old.ToArray();
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndVertical();*/

            if (editorSerializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo)
            {
                EditorUtility.SetDirty(editorSettings);
                //TMPro_EventManager.ON_TMP_SETTINGS_CHANGED();
            }

            if (serializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo)
            {
                EditorUtility.SetDirty(settings);
                //TMPro_EventManager.ON_TMP_SETTINGS_CHANGED();
            }
        }
    }
}
