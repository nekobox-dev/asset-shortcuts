#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    public interface IPane
    {
        public void Draw();
    }

    [System.Serializable]
    public class FolderPane : IPane
    {
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
        }
    }

    [System.Serializable]
    public class NarrowFolderPane : IPane
    {
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
        }
    }

    [System.Serializable]
    public class ShortcutPane : IPane
    {
        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private ReorderableList reorderableList;
        
        public event System.Action<Shortcut> OnShortcutSelected;
        public void ShortcutSelected(Shortcut shortcut)
        {
            OnShortcutSelected?.Invoke(shortcut);
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

            reorderableList = new ReorderableList(folder.Items, typeof(Shortcut), draggable, true, true, true);

            reorderableList.drawHeaderCallback = (rect) =>
            {
                var (iconRect, labelRect) = UI.GetRectListItem(rect);
                EditorGUI.LabelField(iconRect, EditorGUIUtility.TrIconContent(folder.Icon));
                EditorGUI.LabelField(labelRect, folder.Label);
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
                    if (index < 0 || index >= folder.Items.Count) return;
                    
                    var shortcut = folder.Items[index] as Shortcut;

                    var thumbnail = AssetPreview.GetMiniThumbnail(shortcut.Asset);
                    var label = string.IsNullOrEmpty(shortcut.Label) ? shortcut.Asset.name : shortcut.Label;

                    GUI.Label(iconRect, thumbnail);
                    GUI.Label(labelRect, label);
                }
                catch (MissingReferenceException)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Missing Asset");
                }
                catch (System.NullReferenceException)
                {
                    GUI.Label(iconRect, EditorGUIUtility.TrIconContent("Error@2x"));
                    GUI.Label(labelRect, "Null Reference");
                }
            };

            reorderableList.onSelectCallback = (reorderableList) =>
            {
                if (reorderableList.index < 0 || folder.Items.Count <= reorderableList.index) return;
                
                var shortcut = folder.Items[reorderableList.index] as Shortcut;
                if (shortcut == null || shortcut.Asset == null) return;
                
                ShortcutSelected(shortcut);
                Utils.PingAndSelectionObject(shortcut.Asset);
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
        }
    }
}

#endif // UNITY_EDITOR
