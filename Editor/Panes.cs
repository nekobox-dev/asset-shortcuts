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
        public void Add();
        public void Remove();
        public void Select(int index);
        public void SelectAll();
        public void Focus();
        public int GetIndex();
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
                try
                {
                    var folder = Data.Root.Items[index] as Folder;

                    if (GUI.Button(iconRect, EditorGUIUtility.TrIconContent(folder.Icon)))
                    {
                        var popupRect = new Rect(rect.x, rect.y + rect.height, 1, 1);
                        var content = new PopupIconSelector();

                        content.OnIconSelected += (icon) =>
                        {
                            Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Folder icon changed");
                            Undo.FlushUndoRecordObjects();
                            folder.Icon = icon;
                            Data.NotifyChanges("Folder icon changed");
                        };
                        PopupWindow.Show(popupRect, content);
                    }
                    var text = EditorGUI.DelayedTextField(labelRect, folder.Label);
                    if (text != folder.Label)
                    {
                        Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Folder label changed");
                        Undo.FlushUndoRecordObjects();
                        folder.Label = text;
                        Data.NotifyChanges("Folder label changed");
                    }
                }
                catch (System.Exception)
                {
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    EditorGUI.LabelField(labelRect, "Error Folder");
                }
            };

            reorderableList.onSelectCallback = (list) => 
            {
                if (list.index < 0 || Data.Root.Items.Count < list.index) return;

                FolderSelected(Data.Root.Items[list.index] as Folder);
            };
            
            reorderableList.onAddCallback = (_) => Add();
            reorderableList.onRemoveCallback = (_) => Remove();
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

        public void Add()
        {
            try
            {
                var newFolder = new Folder();
                newFolder.Label = $"Folder {Data.Counter}";
                newFolder.Icon = $"sv_icon_dot{Data.Counter % 16}_pix16_gizmo";
                newFolder.Items = new List<IListItem>();
                
                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Add Folder");
                Undo.FlushUndoRecordObjects();
                Data.Root.Items.Add(newFolder);
                Data.Counter++;
                Data.NotifyChanges("Folder added");

                reorderableList.Select(reorderableList.count - 1);
                FolderSelected(Data.Root.Items[reorderableList.count - 1] as Folder);
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Remove()
        {
            try
            {
                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Remove Folder");
                Undo.FlushUndoRecordObjects();
                Data.Root.Items.RemoveAt(reorderableList.index);
                Data.NotifyChanges("Folder removed");
                FolderSelected(null);
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Select(int index)
        {
            try
            {
                reorderableList.index = index;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void SelectAll()
        {
            try
            {
                
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Focus()
        {
            try
            {
                reorderableList?.GrabKeyboardFocus();
            }
            catch (System.Exception)
            {

            }
        }

        public int GetIndex()
        {
            return reorderableList.selectedIndices.Count > 0 ? reorderableList.selectedIndices[0] : -1;
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
                try
                {
                    var folder = Data.Root.Items[index] as Folder;

                    EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent(folder.Icon));
                }
                catch (System.Exception)
                {
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                }
            };

            reorderableList.onSelectCallback = (list) =>
            {
                if (list.index < 0 || Data.Root.Items.Count <= list.index) return;

                FolderSelected(Data.Root.Items[list.index] as Folder);
            };
            
            reorderableList.onAddCallback = (_) => Add();
            reorderableList.onRemoveCallback = (_) => Remove();
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

        public void Add()
        {
            try
            {
                var newFolder = new Folder();
                newFolder.Label = $"Folder {Data.Counter}";
                newFolder.Icon = $"sv_icon_dot{Data.Counter % 16}_pix16_gizmo";
                newFolder.Items = new List<IListItem>();
                
                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Add Folder");
                Undo.FlushUndoRecordObjects();
                Data.Root.Items.Add(newFolder);
                Data.Counter++;
                Data.NotifyChanges("Folder added");

                reorderableList.Select(reorderableList.count - 1);
                FolderSelected(Data.Root.Items[reorderableList.count - 1] as Folder);
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Remove()
        {
            try
            {
                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Remove Folder");
                Undo.FlushUndoRecordObjects();
                Data.Root.Items.RemoveAt(reorderableList.index);
                Data.NotifyChanges("Folder removed");
                FolderSelected(null);
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Select(int index)
        {
            try
            {
                reorderableList.index = index;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void SelectAll()
        {
            try
            {
                
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Focus()
        {
            try
            {
                reorderableList?.GrabKeyboardFocus();
            }
            catch (System.Exception)
            {

            }
        }

        public int GetIndex()
        {
            return reorderableList.selectedIndices.Count > 0 ? reorderableList.selectedIndices[0] : -1;
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
                try
                {
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent(folder.Icon));
                    EditorGUI.LabelField(labelRect, folder.Label);

                    if (GUI.Button(lockRect, EditorGUIUtility.TrIconContent(UI.IsLocked ? "LockIcon-On" : "LockIcon"), EditorStyles.toolbarButton))
                    {
                        UI.IsLocked = !UI.IsLocked;
                        UI.NotifyChanges("UI Lock State Changed");
                    }
                }
                catch (System.Exception)
                {
                    EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    EditorGUI.LabelField(labelRect, "Error Folder");
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
                catch (System.Exception)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Unknown Error");
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

                if (UI.IsLocked) return;

                if (shortcuts != null)
                {
                    NotifyShortcutsSelected(shortcuts);
                }

                if (assets != null)
                {
                    EditorGUIUtility.PingObject(assets[0]);
                    Selection.objects = assets;
                }
            };

            reorderableList.onAddCallback = (_) => Add();
            reorderableList.onRemoveCallback = (_) => Remove();
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
            try
            {
                var shortcuts = reorderableList.selectedIndices.Count == 0 ? new Shortcut[] { reorderableList.list[reorderableList.index] as Shortcut } :
                    reorderableList.selectedIndices
                    .Select(i => reorderableList.list[i] as Shortcut)
                    .Where(s => s != null && s.Asset != null)
                    .ToArray();

                return shortcuts;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public Object[] GetShortcutAssets(Shortcut[] shortcuts)
        {
            try
            {
                var assets = new Object[shortcuts.Length];
                for (int i = 0; i < shortcuts.Length; i++)
                {
                    assets[i] = shortcuts[i].Asset;
                }
                return assets;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public void Add()
        {
            try
            {
                if (Selection.objects == null || Selection.objects.Length == 0) return;

                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Add Shortcut");
                Undo.FlushUndoRecordObjects();
                foreach (var obj in Selection.objects)
                {
                    var shortcut = new Shortcut();
                    shortcut.Label = "";
                    shortcut.Icon = "";
                    shortcut.Asset = obj;
                    
                    reorderableList.list.Add(shortcut);
                }
                Data.NotifyChanges("Shortcut added");

                if (Selection.objects.Length == 1)
                {
                    reorderableList.index = reorderableList.count - 1;
                }
                else
                {
                    reorderableList.SelectRange(reorderableList.count - Selection.objects.Length, reorderableList.count - 1);
                }
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void Remove()
        {
            try
            {
                Undo.RegisterCompleteObjectUndo(Data.instance, Defines.LOG_PREFIX + "Remove Shortcut");
                Undo.FlushUndoRecordObjects();
                int[] deleteIndexes = reorderableList.selectedIndices.Count > 0 ? reorderableList.selectedIndices.Reverse<int>().ToArray() : new[] { reorderableList.index };
                int lastdeletedIndex = -1;
                foreach (var index in deleteIndexes)
                {
                    if (index >= reorderableList.count) continue;

                    reorderableList.list.RemoveAt(index);
                    lastdeletedIndex = index;
                }
                Data.NotifyChanges("Shortcut removed");
            }
            catch (System.Exception)
            {
                return;
            }
        }
        
        public void Select(int index)
        {
            try
            {
                //if (reorderableList == null) return;
                //if (index < 0 || index >= reorderableList.count) return;

                reorderableList.index = index;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        public void SelectAll()
        {
            if (reorderableList == null || reorderableList.count == 0) return;

            reorderableList.SelectRange(0, reorderableList.count - 1);

            var shortcuts = GetSelectedShortcuts();
            var assets = GetShortcutAssets(shortcuts);

            if (shortcuts == null || assets == null) return;
            
            NotifyShortcutsSelected(shortcuts);
            Selection.objects = assets;
        }

        public void Focus()
        {
            try
            {
                reorderableList?.GrabKeyboardFocus();
            }
            catch (System.Exception)
            {

            }
        }

        public int GetIndex()
        {
            return reorderableList.selectedIndices.Count > 0 ? reorderableList.selectedIndices[0] : -1;
        }
    }
}

#endif // UNITY_EDITOR
