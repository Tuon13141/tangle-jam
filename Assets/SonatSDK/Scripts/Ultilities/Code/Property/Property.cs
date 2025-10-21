using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonatSdkUltilities
{
    [Serializable]
    public class ReactiveInt
    {
        public void Subscribe(Action<int> onNext)
        {
            onChanged += onNext;
        }

        public Action<int> onChanged;

        public virtual int Value
        {
            get => _value;
            set
            {
                _value = value;
                onChanged?.Invoke(value);
            }
        }


        private int _value;

        public virtual void Init()
        {
        }
    }

    [Serializable]
    public class ReactiveBool
    {
        public void Subscribe(Action<bool> onNext)
        {
            onChanged += onNext;
        }

        public Action<bool> onChanged;

        public virtual bool Value
        {
            get => _value;
            set
            {
                this._value = value;
                onChanged?.Invoke(value);
            }
        }


        private bool _value;

        public virtual void Init()
        {
        }
    }
}
