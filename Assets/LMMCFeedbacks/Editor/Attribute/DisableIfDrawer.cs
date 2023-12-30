using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GetCondFunc = System.Func<UnityEditor.SerializedProperty, LMMCFeedbacks.DisableIfAttribute, bool>;

namespace LMMCFeedbacks.Editor
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfDrawer : PropertyDrawer
    {
        private readonly Dictionary<Type, GetCondFunc> _disableCondFuncMap = new()
        {
            { typeof(bool), (prop, attr) => attr.TrueThenShow ? !prop.boolValue : prop.boolValue },
            {
                typeof(string),
                (prop, attr) => attr.TrueThenShow
                    ? prop.stringValue == attr.ComparedStr
                    : prop.stringValue != attr.ComparedStr
            },
            {
                typeof(int),
                (prop, attr) => attr.TrueThenShow
                    ? prop.intValue != attr.ComparedInt
                    : prop.intValue == attr.ComparedInt
            },
            {
                typeof(float),
                (prop, attr) => attr.TrueThenShow
                    ? prop.floatValue <= attr.ComparedFloat
                    : prop.floatValue > attr.ComparedFloat
            }
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not DisableIfAttribute attr) return;
            var prop = FindPathProperty(property, attr);
            if (prop == null)
            {
                Debug.LogError($"Not found '{attr.VariableName}' property");
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginDisabledGroup(IsDisable(attr, prop));
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }

        private SerializedProperty FindPathProperty(SerializedProperty property, DisableIfAttribute attr)
        {
            var path = property.propertyPath.Split('[', ']');
            if (path.Length == 1) return property.serializedObject.FindProperty(attr.VariableName);

            var targetPath = attr.VariableName;
            var rootPath = property.propertyPath;
            var splitIndex = rootPath.LastIndexOf(".", StringComparison.Ordinal);
            if (splitIndex >= 0)
                targetPath = $"{rootPath[..splitIndex]}.{targetPath}";
            var targetProperty = property.serializedObject.FindProperty(targetPath);
            return targetProperty;
        }

        private bool IsDisable(DisableIfAttribute attr, SerializedProperty prop)
        {
            if (!_disableCondFuncMap.TryGetValue(attr.VariableType, out var disableCondFunc))
            {
                Debug.LogError($"{attr.VariableType} type is not supported");
                return false;
            }

            //Debug.Log(prop);
            return disableCondFunc(prop, attr);
        }
    }
}