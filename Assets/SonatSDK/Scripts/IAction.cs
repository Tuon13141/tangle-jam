using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAction
{
    void Destroy();
}

public abstract class BaseAction<T> : IAction
{
    public Action<T> Action { get; set; }
    public virtual void Destroy()
    {
        Action = null;
    }
}

public class BaseAction : IAction
{
    public Action Action { get; set; }

    public virtual void Destroy()
    {
        Action = null;
    }
}


public class BooleanAction : BaseAction<bool>
{
    public BooleanAction()
    {
        Action = x => Console.WriteLine($"I just executed Event 6 with {x}");
    }
}

public class IntAction : BaseAction<int>
{
    public IntAction()
    {
        Action = x => Console.WriteLine($"I just executed Event 6 with {x}");
    }
}
