#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Nekobox.AssetShortcuts
{
    public class PopupIconSelector : PopupWindowContent
    {
        public event System.Action<string> OnIconSelected;
        public void IconSelected(string icon)
        {
            OnIconSelected?.Invoke(icon);
        }

        public static float IconSize { get => UI.IconSize + 8; }
        public static readonly float Gap = 2;
        public static readonly float Margin = 4;
        
        public static readonly int Column = 8;
        public static int Row { get => (UI.Icons.Count - 1) / Column + 1; }

        public static float Width { get => Column * (IconSize + Gap * 2) + Margin; }
        public static float Height { get => Row * (IconSize + Gap * 2) + Margin; }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(Width, Height);
        }

        public override void OnGUI(Rect rect)
        {
            for (int i = 0; i < Row; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = 0; j < Column; j++)
                    {
                        var index = i * Column + j;
                        if (index >= UI.Icons.Count) break;

                        var icon = UI.Icons[index];
                        var iconRect = new Rect(
                            rect.x + Margin + j * (IconSize + Gap * 2),
                            rect.y + Margin + i * (IconSize + Gap * 2),
                            IconSize,
                            IconSize
                        );

                        if (GUI.Button(iconRect, EditorGUIUtility.IconContent(icon)))
                        {
                            IconSelected(icon);
                            editorWindow.Close();
                        }
                    }
                }
            }
        }

        public override void OnOpen()
        {

        }

        public override void OnClose()
        {

        }
    }
}

#endif // UNITY_EDITOR
