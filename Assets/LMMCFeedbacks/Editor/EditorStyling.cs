using UnityEditor;
using UnityEngine;

namespace LMMCFeedbacks.Editor
{
    public static class EditorStyling
    {
        public static readonly GUIStyle smallTickbox;
        public static readonly GUIStyle headerLabel;

        static readonly Color headerBackgroundDark;
        static readonly Color headerBackgroundLight;
        static readonly Color headerBackgroundWhenActiveDark;
        static readonly Color headerBackgroundWhenActiveLight;
        static readonly Color headerBackgroundLineDark;
        static readonly Color headerBackgroundLineLight;
        
        public static Color HeaderBackground => EditorGUIUtility.isProSkin ? headerBackgroundDark : headerBackgroundLight;
        public static Color HeaderBackgroundWhenActive => EditorGUIUtility.isProSkin ? headerBackgroundWhenActiveDark : headerBackgroundWhenActiveLight;
        public static Color HeaderBackgroundLine => EditorGUIUtility.isProSkin ? headerBackgroundLineDark : headerBackgroundLineLight;


        static EditorStyling()
        {
            smallTickbox = new GUIStyle("ShurikenToggle");
            headerLabel = new GUIStyle("BoldLabel")
            {
                fontSize = 13
            };

            headerBackgroundDark = new Color(0.2f, 0.2f, 0.2f);
            headerBackgroundLight = new Color(1f, 1f, 1f);
            headerBackgroundWhenActiveDark = new Color(0.3f, 0.3f, 0.3f);
            headerBackgroundWhenActiveLight = new Color(0.8f, 0.8f, 0.8f);
            headerBackgroundLineDark = new Color(.12f, .12f, .12f);
            headerBackgroundLineLight = new Color(.4f, .4f, .4f);
        }
    }
}