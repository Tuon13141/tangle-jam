using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


public static class DuongSerializationExtensions
{
    public static string RemoveWhiteSpace(this string source)
    {
        return Regex.Replace(source, @"s", "");
    }

    public static Dictionary<int, float> DictFloatFromString(string str)
    {
        var dict = new Dictionary<int, float>();
        var splits = str.Split(',');
        foreach (var split in splits)
        {
            var splits2 = split.Split(':');
            dict.Add(int.Parse(splits2[0]), float.Parse(splits2[1]));
        }

        return dict;
    }

    public static Dictionary<int, int> DictIntFromString(string str)
    {
        var dict = new Dictionary<int, int>();
        var splits = str.Split(',');
        foreach (var split in splits)
        {
            var splits2 = split.Split(':');
            dict.Add(int.Parse(splits2[0]), int.Parse(splits2[1]));
        }

        return dict;
    }


    public static string ToString(IEnumerable<int> list)
    {
        return string.Join(",", list);
    }

    public static IEnumerable<int> ListIntFromString(string str)
    {
        if (string.IsNullOrEmpty(str)) return new List<int>();
        try
        {
            var splits = str.Split(',');
            return splits.Select(int.Parse);
        }
        catch (Exception e)
        {
            Debug.LogError("Parse err");
            return new List<int>();
        }

    }

    public static string ToString(IEnumerable<float> list)
    {
        return string.Join(",", list);
    }

    public static IEnumerable<float> ListFloatFromString(string str)
    {
        try
        {
            var splits = str.Split(',');
            return splits.Select(float.Parse).ToList();
        }
        catch (Exception e)
        {
            Debug.LogError("Parse err");
            return new List<float>() { };
        }
    }
}