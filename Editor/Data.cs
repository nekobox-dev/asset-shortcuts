#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    public interface IListItem
    {
        public string Label { get; set; }
        public string Icon { get; set; }
    }

    [System.Serializable]
    public class Shortcut : IListItem
    {
        [SerializeReference] private Object asset;
        [SerializeField] private string label;
        [SerializeField] private string icon;

        public Object Asset { get => asset; set => asset = value; }
        public string Label { get => label; set => label = value; }
        public string Icon { get => icon; set => icon = value; }
    }

    [System.Serializable]
    public class Folder : IListItem
    {
        [SerializeReference] private List<IListItem> items;
        [SerializeField] private string label;
        [SerializeField] private string icon;

        public List<IListItem> Items { get => items; set => items = value; }
        public string Label { get => label; set => label = value; }
        public string Icon { get => icon; set => icon = value; }
    }

    [FilePath(Defines.SAVE_DATA_PATH, FilePathAttribute.Location.ProjectFolder)]
    public class Data : ScriptableSingleton<Data>
    {
        public static event System.Action<string> OnDataChanged;
        public static void NotifyChanges(string name)
        {
            //instance.Save(true);
            OnDataChanged?.Invoke(name);
        }

        [SerializeField] private Folder root = null;
        [SerializeField] private int counter = 0;
        
        public static Folder Root { get => instance.root; set => instance.root = value; }
        public static int Counter { get => instance.counter; set => instance.counter = value; }

        public void OnEnable()
        {
            if (root == null)
            {
                root = new Folder();
                root.Items = new List<IListItem>();
            }
        }

        public void OnDisable()
        {
            Save(true);
        }
    }
}

#endif // UNITY_EDITOR
