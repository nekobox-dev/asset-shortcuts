#if UNITY_EDITOR

using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    /*
    [FilePath(Defines.SAVE_FOLDER_PATH + "Import.dat", FilePathAttribute.Location.ProjectFolder)]
    public class ImportSettings : ScriptableSingleton<ImportSettings>
    {
        public static event System.Action<string> OnDataChanged;
        public static void NotifyChanges(string info)
        {
            OnDataChanged?.Invoke(info);
        }

        public void OnEnable()
        {
            
        }

        public void OnDisable()
        {
            Save(true);
        }
    }

    public class Import : AssetPostprocessor
    {
        public static void OnPreprocessAsset()
        {
            
        }

        public static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {

        }
    }
    */
}

#endif // UNITY_EDITOR
