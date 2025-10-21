using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public static class EditorHelper2
{
    public static bool IsArrayElement(this SerializedProperty property)
    {
        var path = property.propertyPath.Split('.');
        return (path.Length > 1) && (path[path.Length - 2] == "Array");
    }
    
    public static Rect[] GetSplitRect(this Rect position, int n, float gap = 0)
    {
        var rects = new Rect[n];
        var d = position.width / rects.Length;
        for (var i = 0; i < rects.Length; i++)
        {
            rects[i] = position;
            rects[i].width = d - (i == rects.Length - 1 ? 0 :gap);
            rects[i].x += i * d;
        }
        return rects;
    }
    
    public static Rect[] GetSplitRect2(this Rect position, int n, float gap = 0)
    {
        var rects = new Rect[n];
        var d = (position.width * 1 / 2) / (rects.Length - 1);
        var d0 = position.width / 2;
        for (var i = 0; i < rects.Length; i++)
        {
            rects[i] = position;
            if (i == 0)
            {
                rects[i].width = d0 - gap;
                rects[i].x += 0;
            }
            else
            {
                rects[i].width = d - (i == rects.Length - 1 ? 0 : gap);
                rects[i].x += d0 + (i - 1) * d;
            }
        }
        
        
        return rects;
    }
    
    public static SerializedProperty DuongFindChildProperty (this SerializedProperty property, string propertyName) 
    {
        return property.serializedObject.FindProperty(property.propertyPath + "."+propertyName);
    }

    public static SerializedProperty DuongFindPropertyRelative(this SerializedProperty property, string relativeProperty)
    {
        if (property.depth == 0) return property.serializedObject.FindProperty(relativeProperty);

        var path = property.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');
        SerializedProperty parent = null;
        for (int i = 0; i < elements.Length - 1; i++)
        {
            var element = elements[i];
            int index = -1;
            if (element.Contains("["))
            {
                index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                    .Replace("[", "").Replace("]", ""));
                element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
            }

            parent = i == 0 ? property.serializedObject.FindProperty(element) : parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }

        return parent.FindPropertyRelative(relativeProperty);
    }
    
    public static T DuongGetParentSerializedValue<T>(this SerializedProperty property)
    {
        return (T) DuongGetParentSerializedValueRaw(property);
    }

    public static object DuongGetParentSerializedValueRaw(this SerializedProperty property)
    {
        var properties = property.propertyPath.Split('.').ToList();
        if (properties.Count > 1)
        {
            properties.RemoveAt(properties.Count-1);
            var path = string.Join(".", properties);
//                    var find = property.serializedObject.FindProperty(path);
                 
            object @object = property.serializedObject.targetObject;
            @object = @object.GetType()
                .GetField(path,
                    BindingFlags.NonPublic | BindingFlags.Public |  BindingFlags.Instance )
                ?.GetValue(@object);
            return @object;
        }

        return null;
    }
    
    public static T DuongGetSerializedValue<T>(this SerializedProperty property)
    {
        return (T) DuongGetSerializedValueRaw(property);
    }

    public static object DuongGetSerializedValueRaw(this SerializedProperty property)
    {
        object @object = property.serializedObject.targetObject;
        string[] propertyNames = property.propertyPath.Split('.');

        List<string> propertyNamesClean = new List<String>();

        for (int i = 0; i < propertyNames.Count(); i++)
        {
            if (propertyNames[i] == "Array")
            {
                if (i != (propertyNames.Count() - 1) && propertyNames[i + 1].StartsWith("data"))
                {
                    int pos = int.Parse(propertyNames[i + 1].Split('[', ']')[1]);
                    propertyNamesClean.Add($"-GetArray_{pos}");
                    i++;
                }
                else
                    propertyNamesClean.Add(propertyNames[i]);
            }
            else
                propertyNamesClean.Add(propertyNames[i]);
        }

        // Get the last object of the property path.
        foreach (string path in propertyNamesClean)
        {
            if (path.StartsWith("-GetArray"))
            {
                string[] split = path.Split('_');
                int index = int.Parse(split[split.Count() - 1]);
                IList l = (IList) @object;
                @object = l[index];
            }
            else
            {
                if (@object != null)
                    @object = @object.GetType()
                        .GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        ?.GetValue(@object);
            }
        }

        return @object;
    }
    
    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }

    public static object GetTargetObjectOfProperty(SerializedProperty prop, object targetObj)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                targetObj = GetValue_Imp(targetObj, elementName, index);
            }
            else
            {
                targetObj = GetValue_Imp(targetObj, element);
            }
        }
        return targetObj;
    }
    
    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }
}