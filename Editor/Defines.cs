#if UNITY_EDITOR

namespace Nekobox.AssetShortcuts
{
    public static class Defines
    {
        public const string PACKAGE_PATH = "Packages/com.github.nekobox-dev.asset-shortcuts/";
        public const string PACKAGE_NAME = "Asset Shortcuts";
        public const string MENU_PATH = "Tools/" + PACKAGE_NAME;
        public const string SAVE_PATH = PACKAGE_PATH + "Editor/SaveData.dat";
        public const string LOG_PREFIX = "["+ PACKAGE_NAME +"]: ";
    }
}

#endif // UNITY_EDITOR
