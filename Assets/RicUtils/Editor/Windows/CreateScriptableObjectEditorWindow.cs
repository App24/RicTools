using RicUtils.Editor.ContextMenu;
using RicUtils.Editor.Settings;
using RicUtils.Editor.Utilities;
using RicUtils.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicUtils.Editor.Windows
{
    public class CreateScriptableObjectEditorWindow : EditorWindow
    {
        private EditorContainer<string> scriptableObjectName = new EditorContainer<string>();
        private EditorContainer<string> availableScriptableObjectName = new EditorContainer<string>();
        private EditorContainer<string> editorWindowName = new EditorContainer<string>();

        private TextField soNameTextField;
        private TextField availableSoTextField;
        private TextField editorNameTextField;

        private VisualElement emptyFieldWarningContainer;

        private void OnEnable()
        {
            titleContent = new GUIContent("RicUtils Scriptable Object Creator");

            maxSize = new Vector2(400, 166);
            minSize = maxSize;

            EditorPrefs.SetBool("ReadyToUpdateSettings", false);
        }

        private void CreateGUI()
        {
            {
                rootVisualElement.AddLabel("Assets To Create");
            }

            {
                rootVisualElement.AddSeparator(new Color32(37, 37, 37, 255));
            }

            soNameTextField = rootVisualElement.AddTextField(scriptableObjectName, "Scriptable Object", (old) =>
            {
                if (availableScriptableObjectName.Value == null || availableScriptableObjectName.Value == old)
                {
                    availableScriptableObjectName.Value = scriptableObjectName;
                }

                if (editorWindowName.Value == null || editorWindowName.Value == old)
                {
                    editorWindowName.Value = scriptableObjectName;
                }

                UpdateTextFields();
            });
            availableSoTextField = rootVisualElement.AddTextField(availableScriptableObjectName, "Available So");
            editorNameTextField = rootVisualElement.AddTextField(editorWindowName, "Editor Window");

            {

                EventCallback<FocusEvent> focusEvent = (callback) =>
                {
                    ToggleWarning(false);
                };

                soNameTextField.RegisterCallback(focusEvent);
                availableSoTextField.RegisterCallback(focusEvent);
                editorNameTextField.RegisterCallback(focusEvent);
            }

            {
                emptyFieldWarningContainer = new VisualElement();

                emptyFieldWarningContainer.AddLabel("Cannot have empty fields");

                rootVisualElement.Add(emptyFieldWarningContainer);

                ToggleWarning(false);
            }

            rootVisualElement.AddButton("Create", CreateAssets);

            soNameTextField.Focus();
        }

        private void UpdateTextFields()
        {
            soNameTextField.value = scriptableObjectName.Value;
            availableSoTextField.value = availableScriptableObjectName.Value;
            editorNameTextField.value = editorWindowName.Value;
        }

        private void CreateAssets()
        {
            if (string.IsNullOrWhiteSpace(scriptableObjectName.Value) || string.IsNullOrWhiteSpace(availableScriptableObjectName.Value) || string.IsNullOrWhiteSpace(editorWindowName.Value))
            {
                ToggleWarning(true);
                return;
            }

            Close();

            EditorPrefs.SetBool("ReadyToUpdateSettings", true);

            ToolUtilities.TryGetActiveFolderPath(out string path);

            string soName = scriptableObjectName + "ScriptableObject";
            string availableSo = "Available" + availableScriptableObjectName + "ScriptableObject";
            string editorWindow = editorWindowName + "EditorWindow";

            string rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(path + "/temp.cs");

            var dll = CompilationPipeline.GetAssemblyNameFromScriptPath(path + "/temp.cs").Split('.')[0];

            if (!string.IsNullOrEmpty(rootNamespace)) { rootNamespace += "."; }

            {

                string defaultNewFileName = Path.Combine(path, soName + ".cs");

                string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericScriptableObject.cs.txt";

                var endAction = ScriptableObject.CreateInstance<DoCreateScriptAsset>();

                endAction.Action(0, defaultNewFileName, templatePath);

                //ToolUtilities.CreateNewScript(defaultNewFileName, templatePath);
            }

            {
                string defaultNewFileName = Path.Combine(path, availableSo + ".cs");

                string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewAvailableScriptableObject.cs.txt";

                var endAction = ScriptableObject.CreateInstance<DoCreateAvailableAsset>();

                endAction.scriptableObject = soName;

                endAction.Action(0, defaultNewFileName, templatePath);

                //ToolUtilities.CreateNewScript(endAction, defaultNewFileName, templatePath);
            }

            {
                string defaultNewFileName = Path.Combine(path, editorWindow + ".cs");

                string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericEditorWindow.cs.txt";

                var endAction = ScriptableObject.CreateInstance<DoCreateEditorAsset>();

                endAction.scriptableObject = soName;
                endAction.availableScriptableObject = availableSo;

                endAction.Action(0, defaultNewFileName, templatePath);

                //ToolUtilities.CreateNewScript(endAction, defaultNewFileName, templatePath);
            }

            EditorPrefs.SetString("CustomSo", $"{rootNamespace}{soName},{dll}");
            EditorPrefs.SetString("AvailableSo", $"{rootNamespace}{availableSo},{dll}");
            EditorPrefs.SetString("EditorWindow", $"{rootNamespace}{editorWindow},{dll}");
        }

        private void ToggleWarning(bool visible)
        {
            emptyFieldWarningContainer.visible = visible;
            emptyFieldWarningContainer.style.height = visible ? new StyleLength(StyleKeyword.Null) : 0;
        }

        [DidReloadScripts]
        private static void UpdateSettingsWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += UpdateSettingsWhenReady;
                return;
            }

            EditorApplication.delayCall += UpdateSettingsNow;
        }

        private static void UpdateSettingsNow()
        {
            if (EditorPrefs.GetBool("ReadyToUpdateSettings"))
            {

                var so = EditorPrefs.GetString("CustomSo");
                var available = EditorPrefs.GetString("AvailableSo");
                var window = EditorPrefs.GetString("EditorWindow");

                var soType = System.Type.GetType(so);
                var availableType = System.Type.GetType(available);
                var windowType = System.Type.GetType(window);

                EditorPrefs.SetBool("ReadyToUpdateSettings", false);

                var list = new List<ScriptableEditor>(RicUtils_EditorSettings.instance.m_scriptableEditors);

                list.Add(new ScriptableEditor()
                {
                    customScriptableObjectType = new TypeReferences.TypeReference(soType),
                    availableScriptableObjectType = new TypeReferences.TypeReference(availableType),
                    editorType = new TypeReferences.TypeReference(windowType),
                });

                RicUtils_EditorSettings.instance.m_scriptableEditors = list.ToArray();

                EditorUtility.SetDirty(RicUtils_EditorSettings.instance);

                AssetDatabase.SaveAssets();
            }
        }
    }
}
