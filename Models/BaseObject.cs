using System;
using System.Collections.Generic;

namespace SupernoteDesktopClient.Models
{
    public abstract class BaseObject
    {
        private IDictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        internal T GetValue<T>(string key)
        {
            var value = GetValue(key);

            return (value is T) ? (T)value : default(T);
        }

        private object GetValue(string key)
        {
            if (String.IsNullOrEmpty(key) == true)
                return null;

            return _values.ContainsKey(key) ? _values[key] : null;
        }

        public void SetValue(string key, object value)
        {
            if (_values.ContainsKey(key) == false)
                _values.Add(key, value);
            else
                _values[key] = value;
        }
    }
}
