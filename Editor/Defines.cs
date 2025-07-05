#if UNITY_EDITOR

namespace Nekobox.AssetShortcuts
{
    public static class Defines
    {
        public const string PACKAGE_PATH = "Packages/com.github.nekobox-dev.asset-shortcuts/";
        public const string PACKAGE_NAME = "Asset Shortcuts";
        public const string MENU_PATH = "Tools/" + PACKAGE_NAME;
        public const string SAVE_FOLDER_PATH = PACKAGE_PATH + "Editor/SaveData/";
        public const string SAVE_DATA_PATH = SAVE_FOLDER_PATH + "Data.dat";
        public const string SAVE_UI_PATH = SAVE_FOLDER_PATH + "UI.dat";
        public const string LOG_PREFIX = "["+ PACKAGE_NAME +"]: ";

        public static readonly string[] DEFAULT_ICONS = 
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
    }
}

#endif // UNITY_EDITOR
