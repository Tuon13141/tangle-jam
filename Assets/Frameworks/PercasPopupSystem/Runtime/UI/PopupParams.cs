using System.Collections.Generic;

namespace Percas
{
    public class PopupParams
    {
        private Dictionary<string, object> _data = new();

        public PopupParams With(string key, object value)
        {
            _data[key] = value;
            return this;
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var value) && value is T t)
                return t;
            return defaultValue;
        }

        public object GetRaw(string key) => _data.TryGetValue(key, out var value) ? value : null;
    }
}
