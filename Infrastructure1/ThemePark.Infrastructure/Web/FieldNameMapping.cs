using System.Collections.Generic;

namespace ThemePark.Infrastructure.Web
{
    public class FieldNameMapping : Dictionary<string, string>
    {
        public string Get(string key)
        {
            if (!ContainsKey(key))
            {
                return null;
            }

            return this[key];
        }

        public void Set(string key, string value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                this.Add(key, value);
            }
        }
    }
}
