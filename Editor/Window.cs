#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    [System.Serializable]
    public class Window : EditorWindow, IHasCustomMenu
    {
        [SerializeField] private FolderPane folderPane;
        [SerializeField] private NarrowFolderPane narrowFolderPane;
        [SerializeField] private ShortcutPane shortcutPane;
        [SerializeField] private Folder selectedFolder;
        [SerializeField] private Shortcut[] selectedShortcuts;
        [SerializeField] private IPane leftPane;
        [SerializeField] private IPane rightPane;
        [SerializeField] private bool isExpanded;

        [MenuItem(Defines.MENU_PATH)]
        public static void Open()
        {
            var window = CreateInstance<Window>();
            window.titleContent = new GUIContent(Defines.PACKAGE_NAME);
            window.Show();
        }

        public void OnEnable()
        {
            if (Data.Root.Items.Count == 0) 
            {
                var folder = new Folder();
                folder.Label = "Defaults";
                folder.Icon = "Folder Icon";

                Data.Root.Items.Add(folder);
            }

            if (selectedFolder == null && Data.Root.Items.Count > 0)
            {
                selectedFolder = Data.Root.Items[0] as Folder;
            }

            folderPane = new FolderPane();
            narrowFolderPane = new NarrowFolderPane();
            shortcutPane = new ShortcutPane();

            folderPane.Initialize();
            narrowFolderPane.Initialize();
            shortcutPane.Initialize(selectedFolder);

            leftPane = isExpanded ? folderPane : narrowFolderPane;
            rightPane = shortcutPane;


            folderPane.OnFolderSelected += (folder) => 
            {
                selectedFolder = folder;
                selectedShortcuts = null;
                shortcutPane?.Initialize(folder);
            };
            narrowFolderPane.OnFolderSelected += (folder) => 
            {
                selectedFolder = folder;
                selectedShortcuts = null;
                shortcutPane?.Initialize(folder);
            };

            shortcutPane.OnShortcutsSelected += (shortcuts) =>
            {
                selectedShortcuts = shortcuts;
            };

            folderPane.OnExpansionChanged += (isExpanded) =>
            {
                this.isExpanded = isExpanded;
                leftPane = isExpanded ? folderPane : narrowFolderPane;
            };
            narrowFolderPane.OnExpansionChanged += (isExpanded) =>
            {
                this.isExpanded = isExpanded;
                leftPane = isExpanded ? folderPane : narrowFolderPane;
            };

            Data.OnDataChanged += (_) => this.Repaint();
            Undo.willFlushUndoRecord += () => Data.NotifyChanges("UndoRedo");
        }

        public void OnDisable()
        {
            Data.OnDataChanged -= (_) => this.Repaint();
            Undo.willFlushUndoRecord -= () => Data.NotifyChanges("UndoRedo");
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                leftPane.Draw();
                rightPane.Draw();
            }

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                EditorGUILayout.Space();
                var scale = EditorGUILayout.Slider(UI.Scale, 0.0f, 2.0f, GUILayout.Width(120));
                scale = Mathf.Round(scale * 10) / 10;
                
                if (UI.Scale != scale)
                {
                    UI.Scale = scale;
                    UI.NotifyChanges("UI scale changed");
                }
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (selectedFolder == null) break;
                    if (DragAndDrop.objectReferences.Length == 0) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;

                case EventType.DragPerform:
                    if (selectedFolder == null) break;
                    if (DragAndDrop.objectReferences.Length == 0) break;

                    var targetWindowId = this.GetInstanceID();

                    object generic = DragAndDrop.GetGenericData("SourceWindow");
                    if (generic is int sourceWindowId)
                    {
                        if (sourceWindowId == targetWindowId)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.None;
                            Event.current.Use();
                            break;
                        }
                    }

                    DragAndDrop.AcceptDrag();

                    Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Shortcut added");
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        var shortcut = new Shortcut();
                        shortcut.Asset = obj;
                        selectedFolder.Items.Add(shortcut);
                        Data.NotifyChanges("Shortcut added");
                    }
                    break;
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("UI Settings"), false, () => UISettingsWindow.Open());
        }
    }
}

#endif // UNITY_EDITOR
