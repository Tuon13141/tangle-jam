using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInspectorList
{
    public string[] MethodNames { get; set; }
    public string[] ButtonNames { get; set; }

    public ButtonInspectorList(string[] methodNames, string[] buttonNames)
    {
        MethodNames = methodNames;
        ButtonNames = buttonNames;
    }

    public ButtonInspectorList(string[] methodNames)
    {
        MethodNames = methodNames;
        ButtonNames = methodNames;
    }

    public ButtonInspectorList(string methodNames)
    {
        MethodNames = new[] {methodNames};
        ButtonNames = new[] {methodNames};
    }

    public ButtonInspectorList(string methodNames1, string methodNames2)
    {
        MethodNames = new[] {methodNames1, methodNames2};
        ButtonNames = new[] {methodNames1, methodNames2};
    }

    public ButtonInspectorList(string methodNames1, string methodNames2, string methodNames3)
    {
        MethodNames = new[] {methodNames1, methodNames2, methodNames3};
        ButtonNames = new[] {methodNames1, methodNames2, methodNames3};
    }
}