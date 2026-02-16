#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nekobox.AssetShortcuts
{
    [Serializable]
    public class Window : EditorWindow, IHasCustomMenu
    {
        [SerializeField] private FolderPane folderPane;
        [SerializeField] private NarrowFolderPane narrowFolderPane;
        [SerializeField] private ShortcutPane shortcutPane;
        [SerializeField] private Folder selectedFolder;
        [SerializeField] private Shortcut[] selectedShortcuts;
        [SerializeField] private IPane leftPane;
        [SerializeField] private IPane rightPane;
        [SerializeField] private IPane focusedPane;
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

            selectedFolder = Data.Root.Items[0] as Folder;
            selectedShortcuts = null;

            folderPane = new FolderPane();
            narrowFolderPane = new NarrowFolderPane();
            shortcutPane = new ShortcutPane();

            folderPane.Initialize();
            narrowFolderPane.Initialize();
            shortcutPane.Initialize(selectedFolder);

            leftPane = isExpanded ? folderPane : narrowFolderPane;
            rightPane = shortcutPane;
            focusedPane = shortcutPane;

            folderPane.OnFolderSelected += (folder) => 
            {
                selectedFolder = folder;
                selectedShortcuts = null;
                shortcutPane?.Initialize(folder);
                //focusedPane = folderPane;
            };
            narrowFolderPane.OnFolderSelected += (folder) => 
            {
                selectedFolder = folder;
                selectedShortcuts = null;
                shortcutPane?.Initialize(folder);
                //focusedPane = narrowFolderPane;
            };

            shortcutPane.OnShortcutsSelected += (shortcuts) =>
            {
                selectedShortcuts = shortcuts;
                //focusedPane = shortcutPane;
            };

            folderPane.OnExpansionChanged += (isExpanded) =>
            {
                this.isExpanded = isExpanded;
                leftPane = isExpanded ? folderPane : narrowFolderPane;
                //focusedPane = leftPane;
            };
            narrowFolderPane.OnExpansionChanged += (isExpanded) =>
            {
                this.isExpanded = isExpanded;
                leftPane = isExpanded ? folderPane : narrowFolderPane;
                //focusedPane = leftPane;
            };

            Data.OnDataChanged += (_) => this.Repaint();
            Undo.undoRedoPerformed += () => Data.NotifyChanges("UndoRedo Performed");
        }

        public void OnDisable()
        {
            Data.OnDataChanged -= (_) => this.Repaint();
            Undo.undoRedoPerformed -= () => Data.NotifyChanges("UndoRedo Performed");
        }

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                var index = 0;
                switch (Event.current.keyCode)
                {
                    case KeyCode.LeftArrow:
                        index = leftPane.GetIndex();
                        leftPane.Focus();
                        leftPane.Select(index != -1 ? index : 0);
                        Event.current.Use();
                        break;
                    case KeyCode.RightArrow:
                        index = rightPane.GetIndex();
                        rightPane.Focus();
                        rightPane.Select(index != -1 ? index : 0);
                        Event.current.Use();
                        break;
                }
            }

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
                    Event.current.Use();
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

                    Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Shortcut added");
                    Undo.FlushUndoRecordObjects();
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        var shortcut = new Shortcut();
                        shortcut.Asset = obj;
                        selectedFolder.Items.Add(shortcut);
                    }
                    Data.NotifyChanges("Shortcut added");
                    Event.current.Use();
                    break;
                
                case EventType.MouseDown:
                    Selection.activeObject = null;
                    Event.current.Use();
                    break;
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("UI Settings"), false, () => UISettingsWindow.Open());
        }

        [Shortcut("Toggle Lock UI / Local", typeof(Window), KeyCode.T, ShortcutModifiers.Action)]
        public static void ToggleLockUI()
        {
            UI.IsLocked = !UI.IsLocked;
            UI.NotifyChanges("UI lock toggled");
            EditorWindow.focusedWindow?.Repaint();
        }

        [Shortcut("Toggle Expansion / Local", typeof(Window), KeyCode.E, ShortcutModifiers.Action)]
        public static void ToggleExpansion()
        {
            var window = EditorWindow.focusedWindow as Window;
            if (window == null) return;

            window.isExpanded = !window.isExpanded;
            window.leftPane = window.isExpanded ? window.folderPane : window.narrowFolderPane;
            window.Repaint();
        }

        [Shortcut("Select All Shortcuts / Local", typeof(Window), KeyCode.A, ShortcutModifiers.Action)]
        public static void SelectAllShortcuts()
        {
            var window = EditorWindow.focusedWindow as Window;
            if (window == null) return;
            if (window.shortcutPane == null) return;
            window.shortcutPane.SelectAll();
            window.Repaint();
        }

        [Shortcut("Add New / Local", typeof(Window), KeyCode.N, ShortcutModifiers.Action)]
        public static void AddNew()
        {
            var window = EditorWindow.focusedWindow as Window;
            if (window == null) return;

            window.focusedPane?.Add();
            window.Repaint();
        }

        [Shortcut("Remove Selected / Local", typeof(Window), KeyCode.D, ShortcutModifiers.Action)]
        public static void RemoveSelected()
        {
            var window = EditorWindow.focusedWindow as Window;
            if (window == null) return;

            window.focusedPane?.Remove();
            window.Repaint();
        }
    }
}

#endif // UNITY_EDITOR
