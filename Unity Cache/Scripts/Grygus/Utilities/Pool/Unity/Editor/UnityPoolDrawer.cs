using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Grygus.Utilities.Pool.Unity
{
    [CustomPropertyDrawer(typeof(UnityPool))]
    public class UnityPoolDrawer : PropertyDrawer
    {
        private float lineHeight { get { return EditorGUIUtility.singleLineHeight; } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prefabProperty = property.FindPropertyRelative("Prefab");
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty sizeProperty = property.FindPropertyRelative("Size");
            SerializedProperty allowExpandProperty = property.FindPropertyRelative("AllowExpand");
            SerializedProperty allowRecycleProperty = property.FindPropertyRelative("AllowRecycle");

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
            var headerPosition = new Rect(position.x, position.y, position.width, 20);
            GUI.Box(headerPosition, "", EditorStyles.toolbar);

            var layoutNavigator = new RectPositionWrapper(position);


            GUI.Label(layoutNavigator.ReserveDrawSpace(0.25f), string.Empty);

            allowExpandProperty.boolValue = GUI.Toggle(layoutNavigator.ReserveDrawSpace(0.25f), allowExpandProperty.boolValue, "Expand", buttonStyle);
            allowRecycleProperty.boolValue = GUI.Toggle(layoutNavigator.ReserveDrawSpace(0.25f), allowRecycleProperty.boolValue, "Recycle",
                buttonStyle);
            if (GUI.Button(layoutNavigator.ReserveDrawSpace(0.25f), "X",buttonStyle))
            {
                Delete(property);
            }
            property.isExpanded = EditorGUI.Foldout(headerPosition, property.isExpanded, nameProperty.stringValue);

            if (property.isExpanded)
            {

                GUI.Box(position, "", GUI.skin.box);
                layoutNavigator.MoveNextLine();
                layoutNavigator.Span();
                layoutNavigator.Span();
                EditorGUI.PropertyField(layoutNavigator.ReserveDrawSpace(1f), sizeProperty);
                layoutNavigator.MoveNextLine();
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(layoutNavigator.ReserveDrawSpace(1f), prefabProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    if (prefabProperty.objectReferenceValue)
                        nameProperty.stringValue = prefabProperty.objectReferenceValue.name;
                }
            }
        }


        private void DrawHeader(Rect position, string poolName, SerializedProperty property)
        {

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? lineHeight * 3 + 4 : lineHeight;
        }

        private UnityStorage GetPoolObject(SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            var filedName = fieldInfo.Name;
            var type = obj.GetType();

            return (UnityStorage)type.GetField(filedName).GetValue(obj);
        }

        private void Delete(SerializedProperty property)
        {
            int lIndex = property.propertyPath.IndexOf("[");
            var propPath = property.propertyPath.Remove(0, lIndex + 1);
            propPath = propPath.Replace("]", "");
            int index = int.Parse(propPath);

            var parent = PropertyDrawerHelper.GetParent(property);



            var fi = fieldInfo.GetValue(parent) as List<UnityPool>;
            var val = fi[index];
            fi.Remove(val);
        }
    }


    public class RectPositionWrapper
    {
        private Rect _startPosition;
        private Rect _currentPosition;
        public RectPositionWrapper(Rect position)
        {
            _startPosition = position;
            _currentPosition = _startPosition;
            _currentPosition.width = 0;
            _currentPosition.height = EditorGUIUtility.singleLineHeight;
        }

        public Rect MoveNextLine()
        {
            _currentPosition.y += EditorGUIUtility.singleLineHeight;
            _currentPosition.width = 0;
            _currentPosition.x = _startPosition.x;
            return _currentPosition;
        }

        public Rect ReserveDrawSpace(float widthRatio)
        {
            _currentPosition.x = Mathf.Clamp(_currentPosition.x + _currentPosition.width, _startPosition.x,
                _startPosition.x + _startPosition.width);
            _currentPosition.width = _startPosition.width * widthRatio;

            return _currentPosition;
        }

        public Rect GetLeftSpace()
        {
            _currentPosition.x = _startPosition.x;
            _currentPosition.height = _startPosition.height - (_currentPosition.y - _startPosition.y);
            _currentPosition.width = _startPosition.width;
            return _currentPosition;
        }

        public Rect Span()
        {
            _currentPosition.y += 2f;
            _currentPosition.width = 0;
            _currentPosition.x = _startPosition.x;
            return _currentPosition;
        }
    }
    public static class PropertyDrawerHelper
    {

        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}
