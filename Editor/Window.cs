#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    [System.Serializable]
    public class Window : EditorWindow
    {
        [SerializeField] private FolderPane folderPane;
        [SerializeField] private NarrowFolderPane narrowFolderPane;
        [SerializeField] private ShortcutPane shortcutPane;
        [SerializeField] private Folder selectedFolder;
        [SerializeField] private Shortcut selectedShortcut;
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

            var folderSelected = (folder) => 
            {
                selectedFolder = folder;
                selectedShortcut = null;
                shortcutPane?.Initialize(folder);
            };
            folderPane.OnFolderSelected += folderSelected;
            narrowFolderPane.OnFolderSelected += folderSelected;

            shortcutPane.OnShortcutSelected += (shortcut) =>
            {
                selectedShortcut = shortcut;
            };

            expansionChanged = (isExpanded) =>
            {
                this.isExpanded = isExpanded;
                leftPane = isExpanded ? folderPane : narrowFolderPane;
            };
            folderPane.OnExpansionChanged += expansionChanged;
            narrowFolderPane.OnExpansionChanged += expansionChanged;

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
                
                if (scale != UI.Scale)
                {
                    UI.Scale = Mathf.Round(scale * 10) / 10;
                    Data.NotifyChanges("UI scale changed");
                }
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (selectedFolder == null) return;

                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (!AssetDatabase.Contains(obj)) return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;

                case EventType.DragPerform:
                    if (selectedFolder == null) return;

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
    }
}

#endif // UNITY_EDITOR
