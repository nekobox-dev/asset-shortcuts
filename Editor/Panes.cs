#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    public interface IPane
    {
        public void Draw();
        public Rect GetRect();
    }

    [System.Serializable]
    public class FolderPane : IPane
    {
        [SerializeField] private Rect rect;
        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private ReorderableList reorderableList;
        
        public event System.Action<Folder> OnFolderSelected;
        public void FolderSelected(Folder folder)
        {
            OnFolderSelected?.Invoke(folder);
        }

        public event System.Action<bool> OnExpansionChanged;
        public void ExpansionChanged(bool isExpanded)
        {
            OnExpansionChanged?.Invoke(isExpanded);
        }

        public void Initialize()
        {
            //if (reorderableList != null) return;

            reorderableList = new ReorderableList(Data.Root.Items, typeof(Folder), true, true, true, true);

            reorderableList.drawHeaderCallback = (rect) =>
            {
                if (GUI.Button(rect, "<"))
                {
                    ExpansionChanged(false);
                }
            };

            reorderableList.elementHeightCallback = (_) => 
            {
                return Mathf.Max(UI.IconSize, UI.TextSize);
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var (iconRect, labelRect) = UI.GetRectListItem(rect);
                var folder = Data.Root.Items[index] as Folder;

                if (GUI.Button(iconRect, EditorGUIUtility.TrIconContent(folder.Icon)))
                {
                    var popupRect = new Rect(rect.x, rect.y + rect.height, 1, 1);
                    var content = new PopupIconSelector();

                    content.OnIconSelected += (icon) =>
                    {
                        Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Folder icon changed");
                        folder.Icon = icon;
                        Data.NotifyChanges("Folder icon changed");
                    };
                    PopupWindow.Show(popupRect, content);
                }
                var text = EditorGUI.DelayedTextField(labelRect, folder.Label);
                if (text != folder.Label)
                {
                    Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Folder label changed");
                    folder.Label = text;
                    Data.NotifyChanges("Folder label changed");
                }
            };

            reorderableList.onSelectCallback = (list) => 
            {
                if (list.index < 0 || Data.Root.Items.Count < list.index) return;

                FolderSelected(Data.Root.Items[list.index] as Folder);
            };
            
            reorderableList.onAddCallback = (list) =>
            {
                var newFolder = new Folder();
                newFolder.Label = $"Folder {Data.Counter}";
                newFolder.Icon = $"sv_icon_dot{Data.Counter % 16}_pix16_gizmo";
                newFolder.Items = new List<IListItem>();
                
                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Add Folder");
                Data.Root.Items.Add(newFolder);
                Data.Counter++;
                Data.NotifyChanges("Folder added");
            };

            reorderableList.onRemoveCallback = (list) =>
            {
                if (list.index < 0 || Data.Root.Items.Count <= list.index) return;

                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Remove Folder");
                Data.Root.Items.RemoveAt(list.index);
                Data.NotifyChanges("Folder removed");
                FolderSelected(null);
            };

            reorderableList.onReorderCallback = (list) =>
            {
                Data.NotifyChanges("Folder reordered");
            };

            reorderableList.onCanRemoveCallback = (list) =>
            {
                return Data.Root.Items.Count > 1;
            };
        }

        public void Draw()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(UI.IconSize + 150)))
            {
                scrollPosition = scrollView.scrollPosition;
                reorderableList?.DoLayoutList();
            }

            rect = GUILayoutUtility.GetLastRect();
        }

        public Rect GetRect()
        {
            return rect;
        }
    }

    [System.Serializable]
    public class NarrowFolderPane : IPane
    {
        [SerializeField] private Rect rect;
        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private ReorderableList reorderableList;

        public event System.Action<Folder> OnFolderSelected;
        public void FolderSelected(Folder folder)
        {
            OnFolderSelected?.Invoke(folder);
        }

        public event System.Action<bool> OnExpansionChanged;
        public void ExpansionChanged(bool isExpanded)
        {
            OnExpansionChanged?.Invoke(isExpanded);
        }

        public void Initialize()
        {
            //if (reorderableList != null) return;

            reorderableList = new ReorderableList(Data.Root.Items, typeof(Folder), true, true, true, true);

            reorderableList.drawHeaderCallback = (rect) =>
            {
                if (GUI.Button(rect, ">"))
                {
                    ExpansionChanged(true);
                }
            };

            reorderableList.elementHeightCallback = (_) => 
            {
                return Mathf.Max(UI.IconSize, UI.TextSize);
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var (iconRect, labelRect) = UI.GetRectListItem(rect);
                var folder = Data.Root.Items[index] as Folder;

                EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent(folder.Icon));
            };

            reorderableList.onSelectCallback = (list) =>
            {
                if (list.index < 0 || Data.Root.Items.Count <= list.index) return;

                FolderSelected(Data.Root.Items[list.index] as Folder);
            };
            
            reorderableList.onAddCallback = (list) =>
            {
                var newFolder = new Folder();
                newFolder.Label = $"Folder {Data.Counter}";
                newFolder.Icon = $"sv_icon_dot{Data.Counter % 16}_pix16_gizmo";
                newFolder.Items = new List<IListItem>();
                
                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Add Folder");
                Data.Root.Items.Add(newFolder);
                Data.Counter++;
                Data.NotifyChanges("Folder added");
            };

            reorderableList.onRemoveCallback = (list) =>
            {
                if (list.index < 0 || Data.Root.Items.Count <= list.index) return;

                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Remove Folder");
                Data.Root.Items.RemoveAt(list.index);
                Data.NotifyChanges("Folder removed");
                FolderSelected(null);
            };

            reorderableList.onReorderCallback = (list) =>
            {
                Data.NotifyChanges("Folder reordered");
            };

            reorderableList.onCanRemoveCallback = (list) =>
            {
                return Data.Root.Items.Count > 1;
            };
        }

        public void Draw()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(UI.IconSize + 12 + 16 + 8)))
            {
                scrollPosition = scrollView.scrollPosition;
                reorderableList?.DoLayoutList();
            }

            rect = GUILayoutUtility.GetLastRect();
        }

        public Rect GetRect()
        {
            return rect;
        }
    }

    [System.Serializable]
    public class ShortcutPane : IPane
    {
        [SerializeField] private Rect rect;
        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private ReorderableList reorderableList;
        
        public event System.Action<Shortcut[]> OnShortcutsSelected;
        public void NotifyShortcutsSelected(Shortcut[] shortcuts)
        {
            OnShortcutsSelected?.Invoke(shortcuts);
        }

        public void Initialize(Folder folder, bool draggable = true)
        {
            //if (reorderableList != null) return;

            if (folder == null && Data.Root.Items.Count > 0)
            {
                folder = Data.Root.Items[0] as Folder;
            }
            
            if (folder.Items == null)
            {
                folder.Items = new List<IListItem>();
            }

            reorderableList = new ReorderableList(folder.Items, typeof(Shortcut), UI.IsLocked ? false : draggable, true, true, true);
            reorderableList.multiSelect = true;

            UI.OnDataChanged += (_) =>
            {
                reorderableList.draggable = !UI.IsLocked;
            };

            reorderableList.drawHeaderCallback = (rect) =>
            {
                var (iconRect, labelRect) = UI.GetRectListItem(rect);

                var lockRect = new Rect(rect.x + rect.width - rect.height, rect.y, rect.height, rect.height);
                labelRect.width -= rect.height;
                
                EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent(folder.Icon));
                EditorGUI.LabelField(labelRect, folder.Label);

                if (GUI.Button(lockRect, EditorGUIUtility.TrIconContent(UI.IsLocked ? "LockIcon-On" : "LockIcon"), EditorStyles.toolbarButton))
                {
                    UI.IsLocked = !UI.IsLocked;

                    Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "UI Lock State Changed");
                    UI.NotifyChanges("UI Lock State Changed");
                }
            };

            reorderableList.elementHeightCallback = (_) => 
            {
                return Mathf.Max(UI.IconSize, UI.TextSize);
            };
            
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var (iconRect, labelRect) = UI.GetRectListItem(rect);
                try
                {
                    //if (index < 0 || index >= folder.Items.Count) return;
                    
                    var shortcut = folder.Items[index] as Shortcut;

                    var icon = AssetPreview.GetAssetPreview(shortcut.Asset);
                    if (icon == null) icon = AssetPreview.GetMiniThumbnail(shortcut.Asset);
                    var label = string.IsNullOrEmpty(shortcut.Label) ? shortcut.Asset.name : shortcut.Label;

                    GUI.Label(iconRect, icon);
                    GUI.Label(labelRect, label);
                }
                catch (UnityEngine.MissingReferenceException)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Missing Asset");
                }
                catch (System.NullReferenceException)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Null Reference");
                }
                catch (System.IndexOutOfRangeException)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Index Out of Range");
                }

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (!UI.IsLocked) break;
                        if (!rect.Contains(Event.current.mousePosition)) break;

                        DragAndDrop.PrepareStartDrag();
                        var shortcuts = GetSelectedShortcuts();
                        var assets = GetShortcutAssets(shortcuts);
                        DragAndDrop.objectReferences = assets;
                        DragAndDrop.SetGenericData("SourceWindow", EditorWindow.mouseOverWindow.GetInstanceID());
                        DragAndDrop.StartDrag(Defines.LOG_PREFIX + "Dragging Start");
                        //Event.current.Use();
                        break;
                }
            };

            reorderableList.onSelectCallback = (reorderableList) =>
            {
                if (reorderableList.index < 0 || folder.Items.Count <= reorderableList.index) return;
                
                //var shortcut = folder.Items[reorderableList.index] as Shortcut;
                //if (shortcut == null || shortcut.Asset == null) return;
                
                var shortcuts = GetSelectedShortcuts();
                var assets = GetShortcutAssets(shortcuts);
                
                NotifyShortcutsSelected(shortcuts);
                EditorGUIUtility.PingObject(assets[0]);
                Selection.objects = assets;
            };

            reorderableList.onAddCallback = (reorderableList) =>
            {
                var shortcut = new Shortcut();
                shortcut.Label = "";
                shortcut.Icon = "";
                shortcut.Asset = Selection.activeObject;

                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Add Shortcut");
                folder.Items.Add(shortcut);
                Data.NotifyChanges("Shortcut added");
            };

            reorderableList.onRemoveCallback = (reorderableList) =>
            {
                if (reorderableList.index < 0 || folder.Items.Count <= reorderableList.index) return;

                Undo.RecordObject(Data.instance, Defines.LOG_PREFIX + "Remove Shortcut");
                folder.Items.RemoveAt(reorderableList.index);
                Data.NotifyChanges("Shortcut removed");
            };

            reorderableList.onReorderCallback = (list) =>
            {
                Data.NotifyChanges("Shortcut reordered");
            };

            reorderableList.onCanAddCallback = (list) =>
            {
                return Selection.activeObject != null;
            };
        }

        public void Draw()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                reorderableList?.DoLayoutList();
            }

            rect = GUILayoutUtility.GetLastRect();
        }

        public Rect GetRect()
        {
            return rect;
        }

        public Shortcut[] GetSelectedShortcuts()
        {
            var shortcuts = reorderableList.selectedIndices
                .Select(i => reorderableList.list[i] as Shortcut)
                .Where(s => s != null && s.Asset != null)
                .ToArray();
            
            if (shortcuts.Length == 0)
            {
                shortcuts = new Shortcut[] { reorderableList.list[reorderableList.index] as Shortcut };
            }

            return shortcuts;
        }

        public Object[] GetShortcutAssets(Shortcut[] shortcuts)
        {
            var assets = new Object[shortcuts.Length];
            for (int i = 0; i < shortcuts.Length; i++)
            {
                assets[i] = shortcuts[i].Asset;
            }
            return assets;
        }

        public void SelectAll()
        {
            if (reorderableList == null || reorderableList.count == 0) return;

            reorderableList.SelectRange(0, reorderableList.count - 1);

            var shortcuts = GetSelectedShortcuts();
            var assets = GetShortcutAssets(shortcuts);
            
            NotifyShortcutsSelected(shortcuts);
            Selection.objects = assets;
        }
    }
}

#endif // UNITY_EDITOR
