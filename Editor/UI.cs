#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    [FilePath(Defines.SAVE_FOLDER_PATH + "UI.dat", FilePathAttribute.Location.ProjectFolder)]
    public class UI : ScriptableSingleton<UI>
    {
        [SerializeField] private List<string> icons = new List<string>(Defines.DEFAULT_ICONS);
        [SerializeField] private float scale = 1.0f;
        [SerializeField] private float iconSizeMin = 20;
        [SerializeField] private float iconSizeMax = 32;
        [SerializeField] private float textSizeMin = 10;
        [SerializeField] private float textSizeMax = 16;
        [SerializeField] private bool isLocked = false;
        
        public static event System.Action<string> OnDataChanged;
        public static void NotifyChanges(string name)
        {
            OnDataChanged?.Invoke(name);
        }
        
        public static List<string> Icons { get => instance.icons; }
        public static float Scale { get => instance.scale; set => instance.scale = value; }
        public static float IconSizeMin { get => instance.iconSizeMin; set => instance.iconSizeMin = value; }
        public static float IconSizeMax { get => instance.iconSizeMax; set => instance.iconSizeMax = value; }
        public static float TextSizeMin { get => instance.textSizeMin; set => instance.textSizeMin = value; }
        public static float TextSizeMax { get => instance.textSizeMax; set => instance.textSizeMax = value; }

        public static float IconSize { get => IconSizeMin + (IconSizeMax - IconSizeMin) * Scale; }
        public static float TextSize { get => TextSizeMin + (TextSizeMax - TextSizeMin) * Scale; }

        public static bool IsLocked { get => instance.isLocked; set => instance.isLocked = value; }

        public void OnEnable()
        {
            if (icons == null || icons.Count == 0)
            {
                icons = new List<string>(Defines.DEFAULT_ICONS);
            }
        }

        public void OnDisable()
        {
            Save(true);
        }

        public static (Rect iconRect, Rect labelRect) GetRectListItem(Rect rect, bool isAdjust = true)
        {
            var size = isAdjust ? rect.height : Mathf.Max(IconSize, TextSize);

            var iconRect = new Rect(rect.x, rect.y, size, rect.height);
            var labelRect = new Rect(rect.x + size, rect.y, rect.width - size, rect.height);

            return (iconRect, labelRect);
        }
    }

    public class UISettingsWindow : EditorWindow
    {
        [SerializeField] private Vector2 scrollPosition;
        [SerializeField] private ReorderableList reorderableList;

        //[MenuItem("Window/Asset Shortcuts/UI Settings")]
        public static void Open()
        {
            var window = GetWindow<UISettingsWindow>();

            window.titleContent = new GUIContent("Asset Shortcuts UI Settings");
            window.Show();
        }

        private void OnEnable()
        {
            reorderableList = new ReorderableList(UI.Icons, typeof(string), true, true, true, true);
            
            reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Icons", EditorStyles.boldLabel);
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                if (index < 0 || index >= UI.Icons.Count) return;

                var (iconRect, labelRect) = UI.GetRectListItem(rect, true);

                var icon = UI.Icons[index];
                var iconContent = EditorGUIUtility.TrIconContent(icon);
                EditorGUI.LabelField(iconRect, iconContent);
                
                var edited = EditorGUI.DelayedTextField(labelRect, icon);

                if (string.IsNullOrEmpty(edited))
                {
                    edited = UI.Icons[index];
                }

                if (icon != edited)
                {
                    Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "Icon Changed");
                    UI.Icons[index] = edited;
                    UI.NotifyChanges("Icon list changed");
                }
            };

            reorderableList.onChangedCallback = (list) =>
            {
                Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "Icons Changed");
                UI.NotifyChanges("Icons list changed");
            };

            //reorderableList.onReorderCallback = (list) =>
            //{
            //    Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "Icons Reordered");
            //    UI.NotifyChanges("Icons reordered");
            //};
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;

                reorderableList?.DoLayoutList();
                if (GUILayout.Button("Preview Icons"))
                {
                    var popupRect = new Rect(Event.current.mousePosition, Vector2.one);
                    var content = new PopupIconSelector();
                    PopupWindow.Show(popupRect, content);
                }

                EditorGUILayout.Space(10);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.LabelField("Icon Size", EditorStyles.boldLabel);
                    var iconSizeMin = UI.IconSizeMin = EditorGUILayout.FloatField("Icon Size Min", UI.IconSizeMin);
                    var iconSizeMax = UI.IconSizeMax = EditorGUILayout.FloatField("Icon Size Max", UI.IconSizeMax);

                    EditorGUILayout.Space(10);

                    //EditorGUILayout.LabelField("Text Size (Unimplemented)");
                    //EditorGUI.BeginDisabledGroup(true);
                    //var textSizeMin = EditorGUILayout.FloatField("Text Size Min", UI.TextSizeMin);
                    //var textSizeMax = EditorGUILayout.FloatField("Text Size Max", UI.TextSizeMax);
                    //EditorGUI.EndDisabledGroup();

                    if (check.changed)
                    {
                        Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "UI Settings Changed");

                        UI.IconSizeMin = Mathf.Max(0, Mathf.Round(iconSizeMin * 10) / 10);
                        UI.IconSizeMax = Mathf.Max(UI.IconSizeMin, Mathf.Round(iconSizeMax * 10) / 10);
                        //UI.TextSizeMin = Mathf.Max(0, Mathf.Round(textSizeMin * 10) / 10);
                        //UI.TextSizeMax = Mathf.Max(UI.TextSizeMin, Mathf.Round(textSizeMax * 10) / 10);

                        UI.NotifyChanges("UI settings changed");
                    }
                }

                EditorGUILayout.Space(10);

                if (GUILayout.Button("Reset"))
                {
                    Undo.RecordObject(UI.instance, Defines.LOG_PREFIX + "UI Settings Reset");

                    UI.Icons.Clear();
                    UI.Icons.AddRange(Defines.DEFAULT_ICONS);
                    UI.Scale = 1.0f;
                    UI.IconSizeMin = 20;
                    UI.IconSizeMax = 32;
                    UI.TextSizeMin = 10;
                    UI.TextSizeMax = 16;
                    UI.IsLocked = false;

                    UI.NotifyChanges("UI settings reset to defaults");
                }
            }
        }
    }
}

#endif // UNITY_EDITOR
