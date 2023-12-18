using RicTools.Editor.Settings;
using RicTools.Editor.Utilities;
using RicTools.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.Windows
{
    internal class CreateScriptableObjectEditorWindow : EditorWindow
    {
        [SerializeField]
        private EditorContainer<string> scriptableObjectName = new EditorContainer<string>();

        [SerializeField]
        private EditorContainer<string> editorWindowName = new EditorContainer<string>();

        [SerializeField]
        private EditorContainer<bool> openInEditor = new EditorContainer<bool>();

        private TextField soNameTextField;
        private TextField editorNameTextField;

        private VisualElement emptyFieldWarningContainer;

        public bool useCurrentProjectLocation;

        private void OnEnable()
        {
            titleContent = new GUIContent("RicTools Scriptable Object Creator");

            maxSize = new Vector2(400, 166);
            minSize = maxSize;

            EditorPrefs.SetBool("ReadyToUpdateSettings", false);
        }

        private void CreateGUI()
        {
            rootVisualElement.AddCommonStylesheet();

            rootVisualElement.AddLabel("Assets To Create");
            rootVisualElement.AddSeparator(new Color32(37, 37, 37, 255));

            soNameTextField = rootVisualElement.AddTextField(scriptableObjectName, "Scriptable Object", (old) =>
            {
                if (editorWindowName.Value == null || editorWindowName.Value == old)
                {
                    editorWindowName.Value = scriptableObjectName;
                }

                UpdateTextFields();
            });
            editorNameTextField = rootVisualElement.AddTextField(editorWindowName, "Editor Window");

            {

                EventCallback<FocusEvent> focusEvent = (callback) =>
                {
                    ToggleWarning(false);
                };

                soNameTextField.RegisterCallback(focusEvent);
                editorNameTextField.RegisterCallback(focusEvent);
            }

            {
                emptyFieldWarningContainer = new VisualElement();

                emptyFieldWarningContainer.AddLabel("Cannot have empty fields");

                rootVisualElement.Add(emptyFieldWarningContainer);

                ToggleWarning(false);
            }

            rootVisualElement.AddToggle(openInEditor, "Open in editor when files are created");

            rootVisualElement.AddButton("Create", CreateAssets);

            soNameTextField.Focus();
        }

        private void UpdateTextFields()
        {
            soNameTextField.value = scriptableObjectName.Value;
            editorNameTextField.value = editorWindowName.Value;
        }

        private void CreateAssets()
        {
            if (string.IsNullOrWhiteSpace(scriptableObjectName.Value) || string.IsNullOrWhiteSpace(editorWindowName.Value))
            {
                ToggleWarning(true);
                return;
            }

            Close();

            EditorPrefs.SetBool("ReadyToUpdateSettings", true);

            var path = ToolUtilities.GetSelectedPathOrFallback();
            if (useCurrentProjectLocation)
            {
                path = ToolUtilities.GetUniquePathNameAtSelectedPath("test.txt");
                path = Path.GetDirectoryName(path);
            }

            string soName = scriptableObjectName + "ScriptableObject";
            string editorWindow = editorWindowName + "EditorWindow";

            string rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(path + "/temp.cs");

            var dll = CompilationPipeline.GetAssemblyNameFromScriptPath(path + "/temp.cs").Split('.')[0];

            if (!string.IsNullOrEmpty(rootNamespace)) { rootNamespace += "."; }

            Object genericSoFile;
            Object editorWindowFile;

            {

                string defaultNewFileName = Path.Combine(path, soName + ".cs");

                string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericScriptableObject.cs.txt";

                genericSoFile = FileUtilities.CreateScriptAssetFromTemplate(defaultNewFileName, templatePath);
            }

            {
                RicUtilities.CreateAssetFolder("Assets/Editor");

                string defaultNewFileName = Path.Combine("Assets/Editor", editorWindow + ".cs");

                string templatePath = PathConstants.TEMPLATES_PATH + "/Script-NewGenericEditorWindow.cs.txt";

                editorWindowFile = FileUtilities.CreateScriptAssetFromTemplate(defaultNewFileName, templatePath, (content) =>
                {
                    content = content.Replace("#SCRIPTABLEOBJECT#", soName);
                    return content;
                });
            }

            EditorPrefs.SetString("CustomSo", $"{rootNamespace}{soName},{dll}");
            EditorPrefs.SetString("EditorWindow", $"{rootNamespace}{editorWindow},{dll}");

            if (openInEditor.Value)
            {
                AssetDatabase.OpenAsset(genericSoFile);
                AssetDatabase.OpenAsset(editorWindowFile);
            }
        }

        private void ToggleWarning(bool visible)
        {
            emptyFieldWarningContainer.ToggleClass("hidden", !visible);
        }
    }
}
