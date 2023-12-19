using RicTools.Editor.UIElements;
using RicTools.Editor.Utilities;
using RicTools.EditorAttributes;
using RicTools.ScriptableObjects;
using RicTools.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RicTools.Editor.Windows
{
    public abstract class GenericEditorWindow<SoType> : EditorWindow where SoType : GenericScriptableObject, new()
    {
        [SerializeField]
        private string guid;

        public string SavePath => RicUtilities.GetScriptableObjectPath(typeof(SoType));

        private SoType scriptableObject;

        private string currentSearch = "";

        private List<AssetData<SoType>> loadedAssets = new List<AssetData<SoType>>();

        private VisualElement windowContainer;
        private ScrollView assetListContainer;
        private ScrollView editorContainer;

        public new VisualElement rootVisualElement => editorContainer;

        private List<Button> assetButtonList = new List<Button>();

        private event System.Action onLoad;

        [SerializeField]
        private bool debugMode;

        private List<VisualElement> debugElements = new List<VisualElement>();

        public static GenericEditorWindow<SoType> instance;

        private bool saved, createdNew, reloaded, deleted;

        private List<VisualElement> variableVisualElements = new List<VisualElement>();

        [SerializeField]
        private List<EditorVariableData> variableDatas = new List<EditorVariableData>();

        protected virtual string GetAssetName(SoType asset)
        {
            var attribute = System.Attribute.GetCustomAttribute(typeof(SoType), typeof(DefaultScriptableObjectName)) as DefaultScriptableObjectName;
            if (attribute != null)
            {
                var fieldName = attribute.FieldName;
                var field = typeof(SoType).GetField(fieldName);
                if (field != null && field.FieldType == typeof(string))
                {
                    return (string)field.GetValue(asset);
                }
                Debug.LogError($"No field by the name {fieldName} in type '{typeof(SoType)}' or it is not a string");
            }
            return null;
        }

        protected abstract void CreateEditorGUI();

        protected abstract void SaveAsset(ref SoType asset);

        protected abstract void LoadAsset(SoType asset, bool isNull);

        protected virtual void PostSaveAsset(SoType asset)
        {

        }

        protected virtual void OnDeleteAsset(SoType asset)
        {

        }

        protected void CreateDefaultGUI()
        {
            var fields = typeof(SoType).GetFields();
            var temp = CreateInstance<SoType>();
            foreach (var field in fields)
            {
                var attribute = System.Attribute.GetCustomAttribute(field, typeof(EditorVariableAttribute)) as EditorVariableAttribute;
                if (attribute == null) continue;
                if (!EditorWindowTypes.HasTypeToVisualElement(field.FieldType))
                {
                    Debug.LogError($"Type '{field.FieldType}' does not have a visual element type");
                    continue;
                }
                var visualElementType = EditorWindowTypes.GetVisualElementType(field.FieldType);
                var defaultValue = attribute.DefaultValue;
                if (defaultValue == null)
                {
                    defaultValue = field.GetValue(temp);
                    if (defaultValue == null)
                    {
                        if (field.FieldType.IsEnum)
                        {
                            defaultValue = field.FieldType.GetEnumValues().GetValue(0);
                        }
                        else if (field.FieldType.IsValueType)
                        {
                            defaultValue = System.Activator.CreateInstance(field.FieldType);
                        }
                    }
                }
                var visualElement = visualElementType.CreateVisualElement(attribute.Label, defaultValue, field.FieldType, attribute.ExtraData);
                var variableData = new EditorVariableData();
                variableData.defaultValue = defaultValue;
                variableData.fieldName = field.Name;
                variableData.visualElementIndex = variableVisualElements.Count;

                variableDatas.Add(variableData);
                variableVisualElements.Add(visualElement);
                editorContainer.Add(visualElement);
            }
            DestroyImmediate(temp);
        }

        protected virtual void OnEnable()
        {
            instance = this;
            ReloadAssetList();
        }

        private void CreateGUI()
        {
            LoadAllAssets();
            onLoad = null;
            base.rootVisualElement.AddStylesheet("RicTools/Common.uss", "RicTools/GenericEditorWindow.uss");

            windowContainer = new VisualElement()
            {
                name = "WindowRoot"
            };

            CreateToolbar();

            CreateAssetList();

            CreateEditor();

            windowContainer.StretchToParentSize();

            base.rootVisualElement.Add(windowContainer);

            UpdateAssetList();

            DoSearch(currentSearch);
            if (!string.IsNullOrEmpty(guid))
            {
                LoadGUID(guid);
            }
            else
            {
                NewAsset();
            }
            UpdateDebugElements();

            base.rootVisualElement.panel.visualTree.RegisterCallback<KeyDownEvent>(HandleKeyDown, TrickleDown.TrickleDown);
            base.rootVisualElement.panel.visualTree.RegisterCallback<KeyUpEvent>(HandleUpDown, TrickleDown.TrickleDown);
        }

        private void CreateToolbar()
        {
            var toolbar = new Toolbar()
            {
                name = "EditorToolbar"
            };

            {
                var button = new ToolbarButton(NewAsset)
                {
                    text = "New (Ctrl + N)",
                    focusable = false
                };

                toolbar.Add(button);
            }

            {
                var button = new ToolbarButton(SaveAsset)
                {
                    text = "Save (Ctrl + S)",
                    focusable = false
                };

                toolbar.Add(button);
            }

            {
                var button = new ToolbarButton(() => DeleteAsset(guid))
                {
                    text = "Delete (Del)",
                    focusable = false
                };

                toolbar.Add(button);
            }

            toolbar.Add(new ToolbarSpacer());

            {
                debugMode = EditorPrefs.GetBool($"Editor_{GetType().Name}", false);

                var button = new ToolbarToggle()
                {
                    text = "Debug Data",
                    focusable = false,
                    value = debugMode
                };

                button.RegisterValueChangedCallback(callback =>
                {
                    debugMode = callback.newValue;
                    EditorPrefs.SetBool($"Editor_{GetType().Name}", debugMode);
                    UpdateDebugElements();
                });

                toolbar.Add(button);
            }

            toolbar.Add(new ToolbarSpacer());


            {
                var searchBox = new ToolbarSearchField()
                {
                    name = "EditorSearchBox"
                };

                searchBox.RegisterValueChangedCallback<string>(callback =>
                {
                    var value = callback.newValue.Trim().ToLower();

                    DoSearch(value);
                });

                toolbar.Add(searchBox);

                searchBox.AddToClassList("searchBox");
            }

            base.rootVisualElement.Add(toolbar);
        }

        private void CreateAssetList()
        {
            assetListContainer = new ScrollView()
            {
                name = "AssetList"
            };

            windowContainer.Add(assetListContainer);
        }

        private void CreateEditor()
        {
            editorContainer = new ScrollView()
            {
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                name = "EditorContainer"
            };
            editorContainer.contentContainer.name = "EditorContentContainer";

            windowContainer.Add(editorContainer);

            {
                var objectField = new ObjectField()
                {
                    value = scriptableObject,
                    objectType = typeof(SoType),
                    label = "Scriptable Object"
                };
                objectField.SetEnabled(false);
                editorContainer.Add(objectField);

                onLoad += () =>
                {
                    objectField.value = scriptableObject;
                };

                AddElementToDebugView(objectField);
            }

            {
                var textField = new TextField()
                {
                    value = guid,
                    label = "GUID"
                };
                textField.SetEnabled(false);
                editorContainer.Add(textField);

                onLoad += () =>
                {
                    textField.value = guid;
                };

                AddElementToDebugView(textField);
            }

            CreateEditorGUI();
        }

        private void DoSearch(string search)
        {
            currentSearch = search.ToLower();

            foreach (var button in assetButtonList)
            {
                var index = (int)button.userData;
                var identifier = GetAssetIdentifier(loadedAssets[index].asset).ToLower();

                if (identifier.Contains(currentSearch) || string.IsNullOrWhiteSpace(currentSearch))
                {
                    button.RemoveFromClassList("hidden");
                }
                else
                {
                    button.AddToClassList("hidden");
                }
            }
        }

        private void LoadAllAssets()
        {
            loadedAssets = ToolUtilities.FindAssetsByType<SoType>().FindAll(a => !a.asset.setForDeletion);
        }

        private void UpdateAssetList()
        {
            if (assetListContainer == null) return;
            assetListContainer.Clear();
            assetButtonList.Clear();

            foreach (var asset in loadedAssets)
            {
                AddAssetButton(asset.asset);
            }
        }

        private void AddAssetButton(SoType asset)
        {
            var button = new Button(() => LoadGUID(asset.guid))
            {
                text = GetAssetIdentifier(asset),
                userData = assetButtonList.Count
            };
            button.AddToClassList("itemButton");
            button.focusable = false;

            button.AddManipulator(new ContextualMenuManipulator((@event) =>
            {
                @event.menu.AppendAction("Delete", (menuItem) =>
                {
                    DeleteAsset(loadedAssets[(int)button.userData].asset.guid);
                });
            }));

            assetButtonList.Add(button);
            assetListContainer.Add(button);

            DoSearch(currentSearch);
        }

        private string GetAssetIdentifier(SoType asset)
        {
            var identifier = asset.guid;
            identifier = identifier.Substring(0, Mathf.Min(identifier.Length, 8));

            var name = GetAssetName(asset);

            if (name != null)
                identifier = $"{name} - {identifier}";

            return identifier;
        }

        private void NewAsset()
        {
            string newGuid;
            do
            {
                newGuid = GUID.Generate().ToString();
            } while (loadedAssets.Find(asset => asset.asset.guid == newGuid) != null);

            LoadGUID(newGuid);
        }

        private void SaveAsset()
        {
            List<CompletionCriteria> criteria = new List<CompletionCriteria>(GetCompletionCriteria());

            bool notSaveable = false;
            string notSaveText = "";

            foreach (var c in criteria)
            {
                if (!c.isComplete)
                {
                    notSaveable = true;
                    notSaveText += c.completeCriteria + "\n";
                }
            }

            if (notSaveable)
            {
                EditorUtility.DisplayDialog("Error", "Cannot save asset because:\n" + notSaveText, "Ok");
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<SoType>($"{SavePath}/{guid}.asset");

            if (!asset)
            {
                asset = CreateInstance<SoType>();
            }

            foreach (var editorVariableData in this.variableDatas)
            {
                if (variableVisualElements.Count < editorVariableData.visualElementIndex) continue;
                var visualElement = variableVisualElements[editorVariableData.visualElementIndex];
                var ValueProperty = visualElement.GetType().GetProperty("value");
                var value = ValueProperty.GetValue(visualElement);
                asset.GetType().GetField(editorVariableData.fieldName).SetValue(asset, value);
            }

            SaveAsset(ref asset);
            asset.guid = guid;

            SaveAsset(asset);

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();

            PostSaveAsset(asset);

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();

            ReloadAssetList();
        }

        private void SaveAsset(SoType asset)
        {
            RicUtilities.CreateAssetFolder(SavePath);
            if (!AssetDatabase.Contains(asset))
                AssetDatabase.CreateAsset(asset, $"{SavePath}/{guid}.asset");
        }

        private void DeleteAsset(string guid)
        {
            var asset = AssetDatabase.LoadAssetAtPath<SoType>($"{SavePath}/{guid}.asset");

            if (!asset) return;

            if (!EditorUtility.DisplayDialog("Warning", $"You sure you want to delete this asset ({GetAssetIdentifier(asset)})?", "Continue", "Cancel"))
                return;

            OnDeleteAsset(asset);

            AssetDatabase.DeleteAsset($"{SavePath}/{guid}.asset");
            ReloadAssetList();
        }

        protected virtual IEnumerable<CompletionCriteria> GetCompletionCriteria()
        {
            yield return new CompletionCriteria(true, "");
        }

        private void LoadGUID(string guid)
        {
            this.guid = guid;

            foreach (var button in assetButtonList)
            {
                var index = (int)button.userData;

                if (loadedAssets[index].asset.guid == guid)
                {
                    button.AddToClassList("pressed");
                }
                else
                {
                    button.RemoveFromClassList("pressed");
                }
            }

            LoadAssetInternal(loadedAssets.Find(asset => asset.asset.guid == guid)?.asset);
        }

        private void LoadAssetInternal(SoType asset)
        {
            scriptableObject = asset;

            bool isNull = asset == null;

            foreach (var editorVariableData in this.variableDatas)
            {
                if (variableVisualElements.Count <= editorVariableData.visualElementIndex) continue;
                var visualElement = variableVisualElements[editorVariableData.visualElementIndex];
                var type = visualElement.GetType().GetProperty("value");
                if (isNull)
                {
                    type.SetValue(visualElement, editorVariableData.defaultValue);
                }
                else
                {
                    var value = asset.GetType().GetField(editorVariableData.fieldName).GetValue(asset);
                    type.SetValue(visualElement, value);
                }
            }

            LoadAsset(asset, isNull);

            onLoad?.Invoke();
        }

        public void ReloadAssetList()
        {
            LoadAllAssets();
            UpdateAssetList();
            LoadGUID(guid);
        }

        private void HandleKeyDown(KeyDownEvent keydown)
        {
            if (keydown.modifiers == EventModifiers.Control)
            {
                if (keydown.keyCode == KeyCode.S)
                {
                    if (!saved)
                    {
                        SaveAsset();
                        saved = true;
                    }
                    keydown.StopImmediatePropagation();
                }
                else if (keydown.keyCode == KeyCode.N)
                {
                    if (!createdNew)
                    {
                        NewAsset();
                        createdNew = true;
                    }
                    keydown.StopImmediatePropagation();
                }
                else if (keydown.keyCode == KeyCode.R)
                {
                    if (!reloaded)
                    {
                        ReloadAssetList();
                        reloaded = true;
                    }
                    keydown.StopImmediatePropagation();
                }
            }
            else if (keydown.modifiers == EventModifiers.FunctionKey)
            {
                if (keydown.keyCode == KeyCode.Delete)
                {
                    if (!deleted)
                    {
                        DeleteAsset(guid);
                        deleted = true;
                    }
                    keydown.StopImmediatePropagation();
                }
            }
        }

        private void HandleUpDown(KeyUpEvent keyup)
        {
            if (keyup.modifiers == EventModifiers.Control)
            {
                if (keyup.keyCode == KeyCode.S)
                {
                    saved = false;
                    keyup.StopImmediatePropagation();
                }
                else if (keyup.keyCode == KeyCode.N)
                {
                    createdNew = false;
                    keyup.StopImmediatePropagation();
                }
                else if (keyup.keyCode == KeyCode.R)
                {
                    reloaded = false;
                    keyup.StopImmediatePropagation();
                }
            }
            else if (keyup.modifiers == EventModifiers.FunctionKey)
            {
                if (keyup.keyCode == KeyCode.Delete)
                {
                    deleted = false;
                    keyup.StopImmediatePropagation();
                }
            }
        }

        #region Debug Mode
        protected void AddElementToDebugView(VisualElement element)
        {
            debugElements.Add(element);
            UpdateDebugElement(element);
        }

        protected void RemoveElementFromDebug(VisualElement element)
        {
            debugElements.Remove(element);
            element.RemoveFromClassList("hidden");
        }

        private void UpdateDebugElements()
        {
            foreach (var element in debugElements)
            {
                UpdateDebugElement(element);
            }
        }

        private void UpdateDebugElement(VisualElement element)
        {
            if (debugMode)
            {
                element.RemoveFromClassList("hidden");
            }
            else
            {
                element.AddToClassList("hidden");
            }
        }
        #endregion

        #region RegisterLoadChange
        public void RegisterLoadChange<TValueType>(BaseField<TValueType> element, EditorContainer<TValueType> editorContainer)
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }

        public void RegisterLoadChange<TValueType>(RicToolsListView<TValueType> listView, EditorContainerList<TValueType> editorContainerList)
        {
            onLoad += () =>
            {
                listView.UpdateItemSource(editorContainerList);
            };
        }

        public void RegisterLoadChange<TValueType>(BaseField<System.Enum> element, EditorContainer<TValueType> editorContainer) where TValueType : System.Enum
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }

        public void RegisterLoadChange<TValueType>(ObjectField element, EditorContainer<TValueType> editorContainer) where TValueType : Object
        {
            onLoad += () =>
            {
                element.value = editorContainer.Value;
            };
        }

        public void RegisterLoadChange(System.Action onLoad)
        {
            this.onLoad += onLoad;
        }
        #endregion
    }

    public class CompletionCriteria
    {
        public readonly bool isComplete;
        public readonly string completeCriteria;

        public CompletionCriteria(bool isComplete, string completeCriteria)
        {
            this.isComplete = isComplete;
            this.completeCriteria = completeCriteria;
        }
    }

    [System.Serializable]
    internal class EditorVariableData
    {
        public string fieldName;
        //public EditorContainer<object> value;
        public object defaultValue;
        public int visualElementIndex;
    }
}
