#if UNITY_EDITOR

using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    public static class Utils
    {
        public static void SaveAsset(Object obj, string directory = "")
        {
            if (!AssetDatabase.Contains(obj))
            {
                var name = obj.name + ".asset";
                var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Defines.SAVE_PATH, directory, name));
                AssetDatabase.CreateAsset(obj, path + name);
            }

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }

        public static (Rect iconRect, Rect labelRect) GetRectListItem(Rect rect, bool isAdjust = true)
        {
            var size = isAdjust ? rect.height : Mathf.Max(Data.IconSize, Data.TextSize);

            var iconRect = new Rect(rect.x, rect.y, size, rect.height);
            var labelRect = new Rect(rect.x + size, rect.y, rect.width - size, rect.height);

            return (iconRect, labelRect);
        }

        public static void PingAndSelectionObject(Object obj)
        {
            if (obj == null) return;

            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}

#endif // UNITY_EDITOR
