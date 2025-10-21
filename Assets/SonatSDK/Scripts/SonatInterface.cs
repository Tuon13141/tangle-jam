using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionScript
{
    void StartAction();
}

public interface ISetIndex
{
    void SetIndex(int index);
}

public interface ISetArrayInt
{
    void SetArrayInt(int[] arr);
}

public interface IDelayScript
{
    float GetDelay(int i = 0);
}