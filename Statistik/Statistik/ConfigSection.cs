using System;
using System.Collections.Generic;
using System.Text;

namespace CMaurer.Common
{
    public class ConfigSection
    {
        string _section;
        Dictionary<string, string> _data = new Dictionary<string, string>();

        public ConfigSection(string section)
        {
            _section = section;
        }

        public Dictionary<string, string> GetData()
        {
            return _data;
        }

        public void AddKeyValue(string key, string value)
        {
            _data.Add(key, value);
        }

        public bool TryGetValue(string key, out string value)
        {
            bool success = false;
            value = "";

            try
            {
                value = _data[key];
                success = true;
            }
            catch
            {
            }

            return success;
        }

        public string this[string key]
        {
            get
            {
                return _data[key];
            }
        }
    }
}

