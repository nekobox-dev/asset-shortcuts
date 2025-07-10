#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
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
}

#endif // UNITY_EDITOR
