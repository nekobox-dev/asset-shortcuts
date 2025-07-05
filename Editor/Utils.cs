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
                var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Defines.SAVE_FOLDER_PATH, directory, name));
                AssetDatabase.CreateAsset(obj, path + name);
            }

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
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
