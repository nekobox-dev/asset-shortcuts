#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string GetCommonDirectory(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
                return string.Empty;

            var parts1 = path1.Replace(@"\", "/").Split("/");
            var parts2 = path2.Replace(@"\", "/").Split("/");

            var commonParts = parts1.Zip(parts2, (a, b) => a == b ? a : null)
                .TakeWhile(part => part != null)
                .ToArray();

            return string.Join("/", commonParts);
        }

        public static string[] UniformSample(string[] paths, int samplingRate)
        {
            var result = new string[samplingRate];
            for (int i = 0; i < samplingRate; i++)
            {
                int index = Mathf.RoundToInt((float)i / (samplingRate - 1) * (paths.Length - 1));
                result[i] = paths[Mathf.Clamp(index, 0, paths.Length - 1)];
            }
            return result;
        }

        public static void PingAndSelectionObject(Object obj)
        {
            if (obj == null) return;

            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public static void PingAndSelectionObjects(Object[] objects)
        {
            if (objects == null || objects.Length == 0) return;

            Selection.objects = objects;
            EditorGUIUtility.PingObject(objects[0]);
        }
    }
}

#endif // UNITY_EDITOR
