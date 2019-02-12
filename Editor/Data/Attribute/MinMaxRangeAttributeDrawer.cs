﻿namespace Zinnia.Data.Attribute
{
    using UnityEngine;
    using UnityEditor;
    using System.Globalization;
    using Supyrb;
    using Zinnia.Data.Type;
    using Zinnia.Utility;

    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    class MinMaxRangeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = EditorHelper.GetTooltipAttribute(fieldInfo)?.tooltip ?? string.Empty;
            using (new EditorGUI.PropertyScope(position, GUIContent.none, property))
            {
                bool foundGeneric = false;
                bool valid;
                try
                {
                    Vector2 input = property.GetValue<FloatRange>().ToVector2();
                    Vector2 output = BuildSlider(position, label, input, out valid);
                    if (valid)
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, property.displayName);
                        property.SetValue(new FloatRange(output));
                    }

                    foundGeneric = true;
                }
                catch
                {
                    Error(position, label);
                }

                if (!foundGeneric)
                {
                    switch (property.propertyType)
                    {
                        case SerializedPropertyType.Vector2:
                            Vector2 input = property.vector2Value;
                            Vector2 output = BuildSlider(position, label, input, out valid);
                            if (valid)
                            {
                                Undo.RecordObject(property.serializedObject.targetObject, property.displayName);
                                property.vector2Value = output;
                            }

                            break;
                        default:
                            Error(position, label);
                            break;
                    }
                }
            }
        }

        private Vector2 BuildSlider(Rect position, GUIContent label, Vector2 range, out bool valid)
        {
            float fieldWidth = GUI.skin.textField.CalcSize(new GUIContent(1.23456f.ToString(CultureInfo.InvariantCulture))).x; ;
            float fieldPadding = 5f;
            float min = range.x;
            float max = range.y;

            MinMaxRangeAttribute attr = attribute as MinMaxRangeAttribute;
            EditorGUI.BeginChangeCheck();
            Rect updatedPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            min = EditorGUI.FloatField(new Rect(updatedPosition.x, updatedPosition.y, fieldWidth, updatedPosition.height), Mathf.Clamp(min, attr.min, attr.max));
            EditorGUI.MinMaxSlider(new Rect(updatedPosition.x + (fieldWidth + fieldPadding), updatedPosition.y, updatedPosition.width - ((fieldWidth + fieldPadding) * 2f), updatedPosition.height), ref min, ref max, attr.min, attr.max);
            max = EditorGUI.FloatField(new Rect(updatedPosition.x + (updatedPosition.width - fieldWidth), updatedPosition.y, fieldWidth, updatedPosition.height), Mathf.Clamp(max, attr.min, attr.max));

            if (EditorGUI.EndChangeCheck())
            {
                range.x = min;
                range.y = max;
                valid = true;
                return range;
            }
            valid = false;
            return Vector2.zero;
        }

        private void Error(Rect position, GUIContent label)
        {
            EditorGUI.LabelField(position, label, new GUIContent("Use only with Vector2 or FloatRange"));
        }
    }
}