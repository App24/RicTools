using RicUtils.Editor.Utilities;
using RicUtils.ScriptableObjects;
using RicUtils.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicUtils.Editor.Windows
{
    public abstract class GenericEditorWindow<GenericSoType, AvailableSoType> : EditorWindow where GenericSoType : GenericScriptableObject where AvailableSoType : AvailableScriptableObject<GenericSoType>
    {
        protected EditorContainer<GenericSoType> scriptableObject = new EditorContainer<GenericSoType>();

        protected string AvailableSOPath => RicUtilities.GetAvailableScriptableObjectPath(typeof(AvailableSoType));

        protected string SavePath => RicUtilities.GetScriptableObjectPath(typeof(GenericSoType));

        protected EditorContainer<string> spawnableId = new EditorContainer<string>();

        protected SerializedObject serializedObject;

        private ObjectField soObjectField;

        private TextField idTextField;

        private Button saveButton, deleteButton;

        private System.Action onLoad;

        protected virtual void OnEnable()
        {
            serializedObject = new SerializedObject(this);
        }

        private void CreateGUI()
        {
            rootVisualElement.AddCommonStylesheet();

            onLoad = null;
            soObjectField = rootVisualElement.AddObjectField(scriptableObject, "Scriptable Object", () =>
            {
                LoadScriptableObjectInternal(scriptableObject);
            });

            DrawIDInput();

            DrawGUI();

            DrawSaveDeleteButtons();

            LoadScriptableObjectInternal(scriptableObject);
        }

        private void DrawIDInput()
        {
            idTextField = rootVisualElement.AddTextField(spawnableId, "ID");

            RegisterCheckCompletion(idTextField);
        }

        protected abstract void DrawGUI();

        private void DrawSaveDeleteButtons()
        {
            rootVisualElement.AddSeparator();

            saveButton = rootVisualElement.AddButton("Save Asset", () =>
            {
                AvailableSoType available = GetAvailableAsset();

                List<GenericSoType> items = new List<GenericSoType>(available.items);

                int index = -1;

                var item = AssetDatabase.LoadAssetAtPath<GenericSoType>($"{SavePath}/{spawnableId}.asset");

                if (item != null)
                {
                    if (!EditorUtility.DisplayDialog("Error", "There is an asset by that ID already, you sure you want to replace it?", "Continue", "Cancel"))
                        return;
                    index = items.IndexOf(item);
                }
                else
                {
                    item = ScriptableObject.CreateInstance<GenericSoType>();
                }


                CreateAsset(ref item);

                item.id = spawnableId;

                SaveAsset(item, spawnableId);

                if (index < 0)
                    items.Add(item);
                else
                    items[index] = item;

                available.items = items.ToArray();

                EditorUtility.SetDirty(item);
                EditorUtility.SetDirty(available);

                AssetDatabase.SaveAssets();

                scriptableObject.Value = item;
                LoadScriptableObjectInternal(scriptableObject);
            });
            deleteButton = rootVisualElement.AddButton("Delete Asset", () =>
            {
                if (!EditorUtility.DisplayDialog("Warning", "You sure you want to delete this asset?", "Continue", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(scriptableObject));

                scriptableObject.Value = null;
                LoadScriptableObjectInternal(scriptableObject);
            });
        }

        private AvailableSoType GetAvailableAsset()
        {
            RicUtilities.CreateAssetFolder(AvailableSOPath);

            AvailableSoType available = AssetDatabase.LoadAssetAtPath<AvailableSoType>(AvailableSOPath);
            if (available == null)
            {

                available = ScriptableObject.CreateInstance<AvailableSoType>();

                available.items = new GenericSoType[] { };

                AssetDatabase.CreateAsset(available, AvailableSOPath);

                AssetDatabase.SaveAssets();

                available = AssetDatabase.LoadAssetAtPath<AvailableSoType>(AvailableSOPath);
            }
            return available;
        }

        private void SaveAsset(GenericSoType asset, string saveName)
        {
            RicUtilities.CreateAssetFolder(SavePath);
            if (!AssetDatabase.Contains(asset))
                AssetDatabase.CreateAsset(asset, $"{SavePath}/{saveName}.asset");
        }

        private void LoadScriptableObjectInternal(GenericSoType so)
        {

            bool isNull = so == null;

            if (isNull)
            {
                spawnableId.Value = "";
            }
            else
            {
                spawnableId.Value = so.id;
            }

            LoadScriptableObject(so, isNull);

            serializedObject.Update();

            OnLoadInternal();
        }

        protected abstract void LoadScriptableObject(GenericSoType so, bool isNull);

        protected abstract void CreateAsset(ref GenericSoType asset);

        private IEnumerable<CompleteCriteria> GetInbuiltCompleteCriteria()
        {
            yield return new CompleteCriteria(!string.IsNullOrWhiteSpace(spawnableId), "Empty ID");
        }

        protected virtual IEnumerable<CompleteCriteria> GetCompleteCriteria()
        {
            yield return new CompleteCriteria(true, "");
        }

        private void OnLoadInternal()
        {
            soObjectField.value = scriptableObject;

            idTextField.SetEnabled(scriptableObject.IsNull());

            idTextField.value = spawnableId;

            deleteButton.SetEnabled(!scriptableObject.IsNull());

            onLoad?.Invoke();

            CheckCompletion();

            OnLoad();
        }

        protected virtual void OnLoad()
        {

        }

        private void CheckCompletion()
        {
            List<CompleteCriteria> criteria = new List<CompleteCriteria>(GetInbuiltCompleteCriteria());
            criteria.AddRange(GetCompleteCriteria());
            string tooltip = "";
            bool complete = true;
            foreach (var c in criteria)
            {
                if (!c.isComplete)
                {
                    complete = false;
                    tooltip += $"{c.tooltip}\n";
                }
            }
            tooltip = tooltip.Trim();

            saveButton.SetEnabled(complete);
            saveButton.tooltip = tooltip;
        }

        protected void RegisterCheckCompletion<TValueType>(INotifyValueChanged<TValueType> control)
        {
            control.RegisterValueChangedCallback(callback =>
            {
                CheckCompletion();
            });
        }

        protected void RegisterLoadChange<TValueType>(BaseField<TValueType> element, EditorContainer<TValueType> editorContainer)
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }

        protected void RegisterLoadChange<TValueType>(BaseField<System.Enum> element, EditorContainer<TValueType> editorContainer) where TValueType : System.Enum
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }

        protected void RegisterLoadChange<TValueType>(ObjectField element, EditorContainer<TValueType> editorContainer) where TValueType : Object
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }
    }

    public class CompleteCriteria
    {
        public readonly bool isComplete;
        public readonly string tooltip;

        public CompleteCriteria(bool isComplete, string tooltip)
        {
            this.isComplete = isComplete;
            this.tooltip = tooltip;
        }
    }
}
