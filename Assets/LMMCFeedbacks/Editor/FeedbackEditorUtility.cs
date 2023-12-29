using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LMMCFeedbacks.Editor
{
    public static class FeedbackEditorUtility
    {
        internal static bool Foldout(Rect rect,string label,bool isActive,bool isPlaying,Color tagColor,ref bool deleteButton,ref bool menuButton,ref bool expandToggle)
        {
            var labelRect = rect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;
            labelRect.y += 4f;
            labelRect.height = 13f;

            var foldoutRect = rect;
            foldoutRect.y += 1f;
            foldoutRect.height = 18f;

            var toggleRect = rect;
            toggleRect.x += 16f;
            toggleRect.y += 4f;
            toggleRect.width = 20f;
            toggleRect.height = 20f;
            
            var holdMarkRect = rect;
            holdMarkRect.x -=15f;
            holdMarkRect.y += 6f;
            holdMarkRect.width = 12f;
            holdMarkRect.height = 1f;

            var menuRect = rect;
            menuRect.y += 1;
            menuRect.xMin += rect.width-40f;
            menuRect.height = 18f;
            menuRect.width = 18f;

            var deleteRect = menuRect;
            deleteRect.x += 18f;

            var backgroundRect = rect;
            backgroundRect.xMin -= 50f;
            backgroundRect.xMax += 20f;
            backgroundRect.height = 20;

            var tagRect = rect;
            tagRect.xMin -= 38f;
            tagRect.width = 2f;
            tagRect.height = 20f;

            var tagBackgroundRect = rect;
            tagBackgroundRect.xMin -= 38f;
            tagBackgroundRect.width = 2f;

            var topLineRect = rect;
            topLineRect.xMin -= 50f;
            topLineRect.xMax += 20f;
            topLineRect.height = 1f;

            var bottomLineRect = rect;
            bottomLineRect.xMin -= 50f;
            bottomLineRect.xMax += 20f;
            bottomLineRect.y += rect.height;
            bottomLineRect.height = 1f;
            
            // Background
            EditorGUI.DrawRect(backgroundRect,
                isPlaying ? EditorStyling.HeaderBackgroundWhenActive : EditorStyling.HeaderBackground);

            /*EditorGUI.DrawRect(backgroundRect, tagColor*.3f);*/
            EditorGUI.DrawRect(topLineRect,EditorStyling.HeaderBackgroundLine);
            EditorGUI.DrawRect(bottomLineRect,EditorStyling.HeaderBackgroundLine);
            EditorGUI.DrawRect(tagBackgroundRect,tagColor*.5f);
            EditorGUI.DrawRect(tagRect,tagColor);
            // Title
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            
            EditorGUI.DrawRect(holdMarkRect,Color.gray);
            holdMarkRect.y += 4f;
            EditorGUI.DrawRect(holdMarkRect,Color.gray);
            holdMarkRect.y += 4f;
            EditorGUI.DrawRect(holdMarkRect,Color.gray);
            
            // Active checkbox
            isActive = GUI.Toggle(toggleRect, isActive, GUIContent.none, EditorStyling.smallTickbox);
            // menu
            deleteButton = GUI.Button(deleteRect, "X");
            menuButton = GUI.Button(menuRect, "≡");
            
            // foldout
            expandToggle = GUI.Toggle(foldoutRect, expandToggle, GUIContent.none, EditorStyles.foldout);
            return isActive;
        }
        
    }
}