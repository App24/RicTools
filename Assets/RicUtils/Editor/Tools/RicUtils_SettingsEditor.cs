using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

            m_singletonManagersList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = m_singletonManagersList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float labelWidth = 200;
                float width = rect.width - labelWidth;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), "Manager");
                //rect.x += 110;
                EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("manager"), GUIContent.none);
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
    /*[CustomEditor(typeof(RicUtils_Settings))]
    public class RicUtils_SettingsEditor : UnityEditor.Editor
    {
        internal class Styles
        {
            public static readonly GUIContent scriptableEditorsLabel = new GUIContent("Scriptable Editors");
            public static readonly GUIContent scriptableEditorsListLabel = new GUIContent("Scriptable Editors List");

            public static readonly GUIContent scriptableEditorsAddButtonLabel = new GUIContent("Add Scriptable Editor");
        }

        private ReorderableList m_List;

        private const string k_UndoRedo = "UndoRedoPerformed";

        private const string SCRIPTABLE_EDITORS_PATH = "Assets/RicUtils/Editor/Resources/ScriptableEditors";

        public void OnEnable()
        {
            if (target == null)
                return;

            m_List = new ReorderableList(serializedObject, serializedObject.FindProperty("m_scriptableEditors"), false, true, true, true);

            m_List.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = m_List.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float width = 130;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("customScriptableObjectType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + width + 5, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("editorType"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + ((width + 5) * 2), rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("availableScriptableObjectType"), GUIContent.none);

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

                temp.serializedObject.ApplyModifiedProperties();
            };

            m_List.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, Styles.scriptableEditorsListLabel);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string evt_cmd = Event.current.commandName;

            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(Styles.scriptableEditorsLabel, EditorStyles.boldLabel);
            m_List.DoLayoutList();

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
            EditorGUILayout.EndVertical();

            if (serializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo)
            {
                EditorUtility.SetDirty(target);
                //TMPro_EventManager.ON_TMP_SETTINGS_CHANGED();
            }
        }
    }*/
}
