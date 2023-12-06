using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.DisableIf.Editor
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute), true)]
    public class DisableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginDisabledGroup(MeetsConditions(property));
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }

        private bool MeetsConditions(SerializedProperty property) {
            DisableIfAttribute hideAttribute = attribute as DisableIfAttribute;
            if (hideAttribute == null) {
                return false;
            }

            object obj = GetParent(property);
            FieldInfo field = obj.GetType().GetField(hideAttribute.FieldName);
            if (field == null) {
                return false;
            }

            if (field.Name != hideAttribute.FieldName) {
                return false;
            }
            
            if ((bool) field.GetValue(obj)) {
                return hideAttribute.Cond is DisableIfAttribute.Condition.IS_TRUE;
            }
            
            return hideAttribute.Cond is DisableIfAttribute.Condition.IS_FALSE;
        }
        
        private object GetParent(SerializedProperty prop) {
            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            
            string[] elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1)) {
                if (element.Contains("[")) {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
                    
                    obj = GetValue(obj, elementName, index);
                } else {
                    obj = GetValue(obj, element);
                }
            }
            
            return obj;
        }

        private object GetValue(object source, string name) {
            if (source == null) {
                return null;
            }
            
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null) {
                return f.GetValue(source);
            }
            
            PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) {
                return null;                
            }
                
            return p.GetValue(source, null);
        }

        private object GetValue(object source, string name, int index) {
            IEnumerable<object> enumerable = GetValue(source, name) as IEnumerable<object>;
            using IEnumerator<object> enm = enumerable.GetEnumerator();

            while (index-- >= 0) {
                enm.MoveNext();
            }
            
            return enm.Current;
        }
    }
}
