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

        public static readonly string[] Icons = 
        {
            "sv_icon_dot0_pix16_gizmo",
            "sv_icon_dot1_pix16_gizmo",
            "sv_icon_dot2_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot5_pix16_gizmo",
            "sv_icon_dot6_pix16_gizmo",
            "sv_icon_dot7_pix16_gizmo",

            "sv_icon_dot8_pix16_gizmo",
            "sv_icon_dot9_pix16_gizmo",
            "sv_icon_dot10_pix16_gizmo",
            "sv_icon_dot11_pix16_gizmo",
            "sv_icon_dot12_pix16_gizmo",
            "sv_icon_dot13_pix16_gizmo",
            "sv_icon_dot14_pix16_gizmo",
            "sv_icon_dot15_pix16_gizmo",

            "d_Folder Icon",
            "d_FolderFavorite Icon",
            "d_Prefab Icon",
            "d_PrefabModel Icon",
            "d_PrefabVariant Icon",
            "d_Material Icon",
            "d_Shader Icon",
            "d_cs Script Icon",
            
            "d_SceneAsset Icon",
            "d_Avatar Icon",
            "d_Cloth Icon",
            "d_Texture Icon",
            "d_AnimatorController Icon",
            "d_AnimationClip Icon",
            "d_TimelineAsset Icon",
            "d_AudioClip Icon"
        };

        public static float IconSize { get => Data.IconSize + 8; }
        public static readonly float Gap = 2;
        public static readonly float Margin = 4;
        
        public static readonly int Column = 8;
        public static readonly int Row = (Icons.Length - 1) / Column + 1;

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
                        if (index >= Icons.Length) break;

                        var icon = Icons[index];
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
