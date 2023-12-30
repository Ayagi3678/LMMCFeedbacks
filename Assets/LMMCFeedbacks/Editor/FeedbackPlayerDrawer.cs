using System;
using System.Collections.Generic;
using LitMotion;
using LMMCFeedbacks.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace LMMCFeedbacks.Editor
{
    [CustomEditor(typeof(FeedbackPlayer))] public class FeedbackPlayerDrawer : UnityEditor.Editor
    {
        private SerializedProperty _feedbackListProperty;
        private List<IFeedback> _feedbacks;

        private bool _isRequiresConstantRepaint;
        private bool _isSceneViewRepaint;
        private ReorderableList _reorderableList;
        private FeedbackPlayer FeedbackPlayer => target as FeedbackPlayer;

        private void Initialize()
        {
            _feedbacks = FeedbackPlayer.Feedbacks;
            _feedbackListProperty = serializedObject.FindProperty("Feedbacks");

            _reorderableList = new ReorderableList(_feedbacks, typeof(IFeedback), true, true,
                false, false)
            {
                drawElementCallback = DrawElement,
                drawElementBackgroundCallback = (rect, index, active, focused) => { },
                elementHeightCallback = GetElementHeight,
                headerHeight = 0f,
                footerHeight = 10f,
                showDefaultBackground = false,
                drawHeaderCallback = rect => { },
                drawNoneElementCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, rect.height), "--- No Feedbacks ---",
                        EditorStyles.boldLabel);
                }
            };
        }


        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index == 0)
            {
                _isRequiresConstantRepaint = false;
                _isSceneViewRepaint = false;
            }

            if (index >= _feedbacks.Count) return;
            var element = _feedbackListProperty.GetArrayElementAtIndex(index);
            var feedback = _feedbacks[index];
            var tagColor = Color.clear;
            var label = feedback.Name.Split('/')[^1];
            if (feedback is IFeedbackTagColor feedbackColor) tagColor = feedbackColor.TagColor;
            var expandToggle = element.isExpanded;
            var deleteButton = false;
            var menuButton = false;
            feedback.IsActive = FeedbackEditorUtility.Foldout(rect, label, feedback.IsActive,
                feedback.Handle.IsActive(), tagColor, ref deleteButton, ref menuButton, ref expandToggle);
            element.isExpanded = expandToggle;

            var propertyRect = rect;
            propertyRect.y += 25;
            propertyRect.height = EditorGUIUtility.singleLineHeight;

            if (feedback.Handle.IsActive()) _isRequiresConstantRepaint = true;
            if (feedback is IFeedbackSceneRepaint) _isSceneViewRepaint = true;
            if (feedback.Handle.IsActive() && feedback is IFeedbackForceMeshUpdate forceMeshUpdate)
                forceMeshUpdate.Target.ForceMeshUpdate(false, true);
            if (deleteButton) _feedbacks.RemoveAt(index);
            if (menuButton)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Duplicate"), false, () => { _feedbacks.Add(feedback.CopyFeedback()); });
                menu.AddItem(new GUIContent("Reset"), false, () =>
                {
                    _feedbacks.RemoveAt(index);
                    _feedbacks.Insert(index, Activator.CreateInstance(feedback.GetType()) as IFeedback);
                });
                menu.ShowAsContext();
            }

            EditorGUI.BeginDisabledGroup(!feedback.IsActive);
            if (expandToggle)
            {
                if (feedback is IFeedbackWaringMessageBox waringMessageBox)
                {
                    propertyRect.height = 20;
                    EditorGUI.HelpBox(propertyRect, waringMessageBox.WarningMessage, MessageType.Warning);
                    propertyRect.y += 20;
                }

                var depth = -1;
                while (element.NextVisible(true) || depth == -1)
                {
                    if (depth != -1 && element.depth < depth) break;
                    if (depth != -1 && element.depth > depth) continue;
                    depth = element.depth;
                    propertyRect.height = EditorGUI.GetPropertyHeight(element);
                    EditorGUI.PropertyField(propertyRect, element, true);
                    propertyRect.y += EditorGUI.GetPropertyHeight(element) + 2;
                }

                propertyRect.y += 5;
                propertyRect.height = EditorGUIUtility.singleLineHeight;
                var playButtonRect = propertyRect;
                playButtonRect.width = rect.width * .5f;
                var stopButtonRect = playButtonRect;
                stopButtonRect.x += rect.width * .5f;
                if (GUI.Button(playButtonRect, "Play", new GUIStyle("minibuttonmid"))) feedback.Create();
                if (GUI.Button(stopButtonRect, "Stop", new GUIStyle("minibuttonmid"))) feedback.Cancel();
            }

            EditorGUI.EndDisabledGroup();
        }

        private float GetElementHeight(int index)
        {
            var feedback = _feedbackListProperty.GetArrayElementAtIndex(index);
            var elementHeight = EditorGUI.GetPropertyHeight(feedback);
            if (feedback.isExpanded)
            {
                elementHeight += 35;
                if (_feedbacks[index] is IFeedbackWaringMessageBox) elementHeight += 20;
            }

            return elementHeight;
        }

        public override void OnInspectorGUI()
        {
            if (_reorderableList == null) Initialize();

            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 0);

            FeedbackPlayer.playMode = (FeedbackPlayMode)EditorGUILayout.EnumPopup("Play Mode", FeedbackPlayer.playMode);

            FeedbackPlayer.playOnAwake = EditorGUILayout.Toggle("Play On Awake", FeedbackPlayer.playOnAwake);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("options"), true);
            _reorderableList?.DoLayoutList();
            rect.y += _reorderableList.GetHeight();
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Feedback", new GUIStyle("MiniPullDown"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * .7f)))
            {
                var dropdown = new FeedbackDropDown(new AdvancedDropdownState());
                rect.y += EditorGUIUtility.singleLineHeight * 4 + 10;
                rect.width = EditorGUIUtility.currentViewWidth * .6f;
                dropdown.OnSelect += AddFeedback;
                dropdown.Show(rect);
            }

            if (GUILayout.Button("Clear", new GUIStyle("minibutton"))) _feedbacks.Clear();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play", new GUIStyle("minibuttonmid"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * .25f))) FeedbackPlayer.Play();
            if (GUILayout.Button("Stop", new GUIStyle("minibuttonmid"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * .25f))) FeedbackPlayer.Stop();
            if (GUILayout.Button("Initialize", new GUIStyle("minibuttonmid"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * .25f))) FeedbackPlayer.Initialize();
            if (GUILayout.Button("Initial Set", new GUIStyle("minibuttonmid"),
                    GUILayout.Width(EditorGUIUtility.currentViewWidth * .2f))) FeedbackPlayer.InitialSetup();
            EditorGUILayout.EndHorizontal();
        }

        public override bool RequiresConstantRepaint()
        {
            if (_isRequiresConstantRepaint)
                if (_isSceneViewRepaint)
                    InternalEditorUtility.RepaintAllViews();
            //SceneView.lastActiveSceneView.Repaint();
            return _isRequiresConstantRepaint;
        }

        private void AddFeedback(Type type)
        {
            var instance = Activator.CreateInstance(type);
            _feedbacks.Add(instance as IFeedback);
        }
    }
}